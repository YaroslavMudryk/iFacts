namespace iFacts.Shared.Exceptions;

public class ForbiddenException : HttpResponseException
{
    public ForbiddenException(string error) : base(403, error)
    {

    }

    public ForbiddenException() : base(403, "Forbidden request")
    {

    }
}