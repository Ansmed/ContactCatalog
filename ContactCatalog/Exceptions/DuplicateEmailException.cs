namespace ContactCatalog.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string email)
        : base($"A contact with the email '{email}' already exists.")
    {
    }
}