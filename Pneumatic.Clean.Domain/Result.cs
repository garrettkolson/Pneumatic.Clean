namespace Pneumatic.Clean.Domain;

public class Result<T, TException> 
    where TException : Exception
{
    public T? Value { get; private init; }
    public TException? Error { get; private init; }
    
    public bool IsOk => Value != null;
    public bool IsError => Error != null;
    
    private Result() { }

    public static Result<T, TException> Ok(T ok)
    {
        return new Result<T, TException> { Value = ok };
    }

    public static Result<T, TException> Err(TException err)
    {
        return new Result<T, TException> { Error = err };
    }
}