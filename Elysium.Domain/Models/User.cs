using Elysium.Domain.Primitives;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace Elysium.Domain.Models;

public class User
{
    public int Id { get; private set; }
    public string Username { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public DateOnly BirthDate { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public User() { }

    private User(string username , string passwordHash , string firstName , string lastName , DateOnly birthDate , UserRole role )
    {
        Username = username;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Role = role;
    }

    public static Result<User> Create(string username, string passwordHash, string firstName, string lastName, DateOnly birthDate, UserRole role)
    {
        var result = ValidateUsername(username);
        result.AddResult(ValidatePasswordHash(passwordHash));
        result.AddResult(ValidateFirstName(firstName));
        result.AddResult(ValidateLastName(lastName));
        result.AddResult(ValidateBirthdate(birthDate));

        if (!result.IsSuccess)
            return Result<User>.Failure(result);

        var user = new User(username, passwordHash, firstName, lastName, birthDate, role);
        user.CreatedAt = user.UpdatedAt = DateTime.UtcNow;

        return user;

    }
    
    public Result ChangeUsername(string newUsername )
    {
        var result = ValidateUsername(newUsername);

        if (!result.IsSuccess)
            return result;

        Username = newUsername;
        Touch();
        return result;

    }

    public Result ChangePasswordHash(string hashPassword)
    {
        var result = ValidatePasswordHash(hashPassword);

        if (!result.IsSuccess)
            return result;

        PasswordHash = hashPassword;
        Touch();
        return result;
    }

    public Result ChangeBirthDate(DateOnly birthDate )
    {
        var result = ValidateBirthdate(birthDate);

        if (!result.IsSuccess)
            return result;

        BirthDate = birthDate;
        Touch();
        return result;
    }

    public Result ChangeFirstName(string firstName)
    {
        var result = ValidateFirstName(firstName);

        if (!result.IsSuccess)
            return result;

        FirstName = firstName;
        Touch();
        return result;
    }
    
    public Result ChangeLastName (string lastName)
    {
        var result = ValidateLastName(lastName);

        if (!result.IsSuccess)
            return result;

        LastName = lastName;
        Touch();
        return result;
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    private static Result ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Result.Failure("username is empty");

        if ( username.Length > 64)
            return Result.Failure("username maximum length is 64 characters");

        return Result.Success();
            

    }

    private static Result ValidatePasswordHash(string passwordHash )
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result.Failure("password is empty");

        if ( passwordHash.Length > 256)
            return Result.Failure("hashed password maximum length is 256 characters");

        return Result.Success();
    }

    private static Result ValidateFirstName(string firstname )
    {
        if (string.IsNullOrWhiteSpace(firstname))
            return Result.Failure("firstname is empty");

        if (firstname.Length > 64)
            return Result.Failure("first maximum length is 64 characters");

        return Result.Success();

    }

    private static Result ValidateLastName(string lastname)
    {
        if (string.IsNullOrWhiteSpace(lastname))
            return Result.Failure("lastname is empty");

        if (lastname.Length > 64)
            return Result.Failure("lastname maximum length is 64 characters");

        return Result.Success();

    }

    private static Result ValidateBirthdate(DateOnly birthDate)
    {
        if (birthDate >= DateOnly.FromDateTime(DateTime.Now))
            return Result.Failure("date is invalid");
        
        return Result.Success();
    }

    

    
}
