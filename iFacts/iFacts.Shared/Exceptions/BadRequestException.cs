namespace iFacts.Shared.Exceptions;

public class BadRequestException : HttpResponseException
{
    public BadRequestException(string error) : base(400, error)
    {

    }

    public BadRequestException() : base(400, "Conflict")
    {

    }
}
