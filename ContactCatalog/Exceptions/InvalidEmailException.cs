namespace ContactCatalog.Exceptions;

public class InvalidEmailException : Exception
{
    public InvalidEmailException(string email)
        : base($"The email '{email}' is not a valid email address.")
    {
    }
}