using ContactCatalog.Exceptions;
using ContactCatalog.Models;
using Microsoft.Extensions.Logging;

namespace ContactCatalog.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly Dictionary<int, Contact> _contacts = new();
    private readonly HashSet<string> _emails = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<ContactRepository> _logger;
    private int _nextId = 1;

    public ContactRepository(ILogger<ContactRepository> logger)
    {
        _logger = logger;
    }

    public void Add(Contact contact)
    {
        _logger.LogInformation("Attempting to add contact: {Name}, {Email}", contact.Name, contact.Email);

        if (_emails.Contains(contact.Email))
        {
            _logger.LogWarning("Duplicate email detected: {Email}", contact.Email);
            throw new DuplicateEmailException(contact.Email);
        }

        contact.Id = _nextId++;
        _contacts[contact.Id] = contact;
        _emails.Add(contact.Email);

        _logger.LogInformation("Contact added successfully with ID {Id}", contact.Id);
    }

    public IEnumerable<Contact> GetAll()
    {
        _logger.LogInformation("Retrieving all contacts ({Count})", _contacts.Count);
        return _contacts.Values;
    }

    public IEnumerable<Contact> Search(Func<Contact, bool> predicate)
    {
        _logger.LogDebug("Performing a search across {Count} contacts", _contacts.Count);

        foreach (var c in _contacts.Values)
            if (predicate(c))
                yield return c;
    }
}