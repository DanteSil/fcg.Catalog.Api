namespace Fcg.Application.Common;

public class UnauthorizedAppException : Exception
{
    public UnauthorizedAppException(string message) : base(message)
    {
    }
}
