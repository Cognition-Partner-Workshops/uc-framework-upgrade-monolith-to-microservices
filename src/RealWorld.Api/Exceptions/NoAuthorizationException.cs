namespace RealWorld.Api.Exceptions;

public class NoAuthorizationException : Exception
{
    public NoAuthorizationException() : base("No authorization") { }
    public NoAuthorizationException(string message) : base(message) { }
}
