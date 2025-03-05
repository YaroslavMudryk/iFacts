namespace iFacts.Shared.Exceptions;

public class NotFoundException : HttpResponseException
{
    public NotFoundException(string error) : base(404, error)
    {

    }
}
