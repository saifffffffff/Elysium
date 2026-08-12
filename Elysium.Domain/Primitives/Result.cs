using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Elysium.Domain.Primitives;

public class Result
{

    public List<Error?> Errors { get; } = new List<Error>()!;

    public bool IsSuccess { get; set; }
    
    Result(bool isSuccess , Error? error = null )
    {
        IsSuccess = isSuccess;
        Errors.Add(error);
    }

    Result(List<Error> errors)
    {
        IsSuccess = false;
        Errors = errors!;
    }
    
    public static Result Success()
    {
        return new Result (true );
    }
    
    public static Result Failure(string message) 
    {
        return new Result(false,new(message));
    }

    public static Result Failure(List<Error> errors) => new Result(errors);

    public void AddResult(Result result)
    {
        if (!result.IsSuccess)
        {
            IsSuccess = false;
            Errors.AddRange(result.Errors);
        }
    }

    


}


public class Result<T>   
{
    public T? Value { get; }

    public bool IsSuccess { get; private set; }

    public List<Error> Errors { get; } = new List<Error>();

    Result( T? value , bool isSuccess, Error? error = null) 
    {
        Value = value;
        IsSuccess = isSuccess;
        
        if ( error is not null )
            Errors.Add(error);
    }

    Result(List<Error> errors)
    {
        Errors = errors!;
        IsSuccess = false;
    }


    public static Result<T> Success(T value )
    {
        return new Result<T>(value , true );
    }

    public static Result<T> Failure(Result result ) => new Result<T>(result.Errors!);

    public static Result<T> Failure(string message) => new Result<T>(default, false, new(message));

    public static Result<T> Failure(List<Error> errors) => new Result<T>(errors);
    public void AddResult ( Result result )
    {
        if ( !result .IsSuccess)
        {
            IsSuccess = false;
            Errors.AddRange(result.Errors!);
        }
    }



}