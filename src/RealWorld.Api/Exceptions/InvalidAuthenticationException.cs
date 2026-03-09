namespace RealWorld.Api.Exceptions;

public class InvalidAuthenticationException : Exception
{
    public InvalidAuthenticationException() : base("Invalid authentication") { }
    public InvalidAuthenticationException(string message) : base(message) { }
}
