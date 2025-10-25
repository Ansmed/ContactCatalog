using System.Text.RegularExpressions;
using ContactCatalog.Exceptions;
using ContactCatalog.Models;
using ContactCatalog.Repositories;
using Microsoft.Extensions.Logging;

namespace ContactCatalog.Services;

    public class ContactService
    {
        private readonly ContactRepository _repo;
        private readonly ILogger<ContactService> _logger;

        public ContactService(ContactRepository repo, ILogger<ContactService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public void AddContact(string name, string email, string tagInput)
        {
            _logger.LogInformation("Adding contact with email {Email}", email);

            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("Invalid name input (empty)");
                throw new ArgumentException("Name cannot be empty.");
            }

            if (!IsValidEmail(email))
            {
                _logger.LogWarning("Invalid email format for {Email}", email);
                throw new InvalidEmailException(email);
            }

            var tags = tagInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(t => t.Trim())
                               .ToList();

            var contact = new Contact
            {
                Name = name.Trim(),
                Email = email.Trim().ToLower(),
                Tags = tags
            };

            _repo.Add(contact);

            _logger.LogInformation("Contact successfully added: {Name} ({Email})", name, email);
        }

        public IEnumerable<Contact> ListContacts()
        {
            _logger.LogInformation("Listing all contacts");
            return _repo.GetAll();
        }

        public IEnumerable<Contact> Search(string term)
        {
            _logger.LogInformation("Searching contacts for term: {Term}", term);
            term = term.ToLower();
            return _repo.Search(c => c.Name.ToLower().Contains(term) || c.Email.ToLower().Contains(term));
        }

        public IEnumerable<Contact> FilterByTag(string tag)
        {
            _logger.LogInformation("Filtering contacts by tag: {Tag}", tag);
            tag = tag.ToLower();
            return _repo.Search(c => c.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)));
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
    }
