using ContactCatalog.Models;

namespace ContactCatalog.Repositories;

public interface IContactRepository
{
    void Add(Contact contact);
    IEnumerable<Contact> GetAll();
    IEnumerable<Contact> Search(Func<Contact, bool> predicate);
}