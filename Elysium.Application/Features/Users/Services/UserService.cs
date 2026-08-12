using Elysium.Application.Features.Users.DTOs;
using Elysium.Domain.Interfaces;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Elysium.Domain.Primitives;
using FluentValidation;
using Microsoft.AspNetCore.Identity;




namespace Elysium.Application.Features.Users.Services;

public class UserService(IUserRepository userRepository, ITeacherRepository teacherRepository , IStudentRepository studentRepository ,IUnitOfWork unitOfWork ,  IValidator<CreateUserRequest> userValidator, IValidator<SignInRequest> signInValidator, IValidator<UpdateProfileRequest> updateProfileValidator, IValidator<ChangeUsernameRequest> changeUsernameValidator, IValidator<ChangePasswordRequest> changePasswordValidator, IPasswordHasher<User> hasher) : IUserService
{
    public async Task<Result<int>> CreateAsync(CreateUserRequest request , CancellationToken cancellationToken = default)
    {

        var dataInputResult = await userValidator.ValidateAsync(request , cancellationToken);

        if (!dataInputResult.IsValid)
            return Result<int>.Failure(dataInputResult.Errors.Select(error => new Error(error.ErrorMessage)).ToList());


        if (await userRepository.ExistsAsync(u => u.Username == request.username))
            return Result<int>.Failure("Username already exists");

        var businessRulesResult = User.Create(
            request.username,
            hasher.HashPassword(new User(), request.password),
            request.firstname,
            request.lastname,
            request.birthDate,
            request.role
        );

        if (!businessRulesResult.IsSuccess)
            return Result<int>.Failure(businessRulesResult.Errors);

        User user = businessRulesResult.Value!;

        await unitOfWork.StartTransactionAsync(cancellationToken);
        try
        {
            
            await userRepository.AddAsync(user , cancellationToken);
            await unitOfWork.SaveChangesAsync();

            if ( user.Role is UserRole.Student)
            {
                var student = Student.Create(user.Id).Value;
                await studentRepository.AddAsync(student! , cancellationToken);
            }
            else if  (user.Role is UserRole.Teacher)
            {
                var teacher = Teacher.Create(user.Id).Value;
                await teacherRepository.AddAsync(teacher!, cancellationToken);   
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
        
        
        return Result<int>.Success(user.Id);
    
    }

    public async Task<Result> DeleteAsync(int id , CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null )
            Console.WriteLine("User is null ");

        if (user is null)
            return Result.Failure("User not found");

        userRepository.Delete(user);

        await userRepository.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async Task<Result<User>> GetByIdAsync(int id , CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        
        if (user is null)
            return Result<User>.Failure("User not found");

        return Result<User>.Success(user);

    }

    public async Task<Result<IEnumerable<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dtos = (await userRepository.GetAllAsync(cancellationToken)).Select(ToDto);
        
        return Result<IEnumerable<UserDto>>.Success(dtos);
    }

    private UserDto ToDto(User user) => new UserDto(user.Username, user.FirstName, user.LastName, user.BirthDate, user.Role);
    
    public async Task<Result<UserDto>> SignInAsync(SignInRequest request  , CancellationToken cancellationToken = default)
    {

        var validationResult = await signInValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Result<UserDto>.Failure(validationResult.Errors.Select(error => new Error(error.ErrorMessage)).ToList());

        var user = await userRepository.GetByUsernameAsync(request.username, cancellationToken);
        
        if ( user is null ) 
            return Result<UserDto>.Failure("Incorrect username/password");


        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.password );

        if (result == PasswordVerificationResult.Failed)
            return Result<UserDto>.Failure("Incorrect username/password");

        return Result<UserDto>.Success(ToDto(user));

    }

    public async Task<Result> UpdateProfileAsync( UpdateProfileRequest request , CancellationToken cancellationToken = default )
    {
        var validationResult = updateProfileValidator.Validate(request);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.Select(error => new Error(error.ErrorMessage)).ToList());

        var user = await userRepository.GetByIdAsync(request.id);

        if (user is null)
            return Result.Failure("User not found");

        var result = user.ChangeFirstName(request.firstname);
        result.AddResult(user.ChangeLastName(request.lastname));
        result.AddResult(user.ChangeBirthDate(request.birthDate));

        if (!result.IsSuccess)
            return result;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        return result;

        

    }

    public async Task<Result> ChangeUsernameAsync(ChangeUsernameRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await changeUsernameValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.Select(error => new Error(error.ErrorMessage)).ToList());

        var user = await userRepository.GetByIdAsync(request.id, cancellationToken);

        if (user is null)
            return Result.Failure("User not found");

        if (await userRepository.ExistsAsync(u => u.Username == request.username && u.Id != request.id, cancellationToken))
            return Result.Failure("Username already exists");

        var changeResult = user.ChangeUsername(request.username);

        if (!changeResult.IsSuccess)
            return changeResult;

        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await changePasswordValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.Select(error => new Error(error.ErrorMessage)).ToList());

        var user = await userRepository.GetByIdAsync(request.id, cancellationToken);

        if (user is null)
            return Result.Failure("User not found");

        var verifyResult = hasher.VerifyHashedPassword(user, user.PasswordHash, request.currentPassword);

        if (verifyResult == PasswordVerificationResult.Failed)
            return Result.Failure("Current password is incorrect");

        var hashPassword = hasher.HashPassword(user, request.newPassword);

        var changeResult = user.ChangePasswordHash(hashPassword);

        if (!changeResult.IsSuccess)
            return changeResult;

        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
