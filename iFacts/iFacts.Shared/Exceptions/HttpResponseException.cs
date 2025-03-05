namespace iFacts.Shared.Exceptions;

public class HttpResponseException : Exception
{
    public int StatusCode { get; }

    public HttpResponseException() : this(400)
    {
    }

    public HttpResponseException(int statusCode, string error = null) : base(error) => StatusCode = statusCode;

    public HttpResponseException(string error, int statusCode) : this(statusCode, error)
    {
    }
}
