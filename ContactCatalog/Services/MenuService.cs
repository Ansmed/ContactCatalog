using ContactCatalog.Exceptions;
using Microsoft.Extensions.Logging;

namespace ContactCatalog.Services;

    public class MenuService
    {
        private readonly ContactService _service;
        private readonly ILogger<MenuService> _logger;
        private bool _running = true;

        public MenuService(ContactService service, ILogger<MenuService> logger)
        {
            _service = service;
            _logger = logger;
        }

        public void PrintMenu()
        {
            _logger.LogInformation("Application started");
            while (_running)
            {
                ShowMenu();
                HandleInput(Console.ReadLine());
            }
            _logger.LogInformation("Application exited");
        }

        private void ShowMenu()
        {
            Console.WriteLine("\n--- Contact Catalog ---");
            Console.WriteLine("1. Add Contact");
            Console.WriteLine("2. List Contacts");
            Console.WriteLine("3. Search by Name or Email");
            Console.WriteLine("4. Filter by Tag");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
        }

        private void HandleInput(string? input)
        {
            _logger.LogDebug("User selected option: {Input}", input);
            
            switch (input)
            {
                case "1":
                    AddContact();
                    break;
                case "2":
                    ListContacts();
                    break;
                case "3":
                    SearchContacts();
                    break;
                case "4":
                    FilterByTag();
                    break;
                case "5":
                    _running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    _logger.LogWarning("Invalid menu input: {Input}", input);
                    break;
            }
        }

        private void AddContact()
        {
            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Enter Email: ");
            string email = Console.ReadLine() ?? "";

            Console.Write("Enter Tags (comma separated): ");
            string tags = Console.ReadLine() ?? "";

            try
            {
                _service.AddContact(name, email, tags);
                Console.WriteLine("Contact added successfully!");
            }
            catch (DuplicateEmailException ex)
            {
                _logger.LogWarning(ex, "Duplicate email detected");
                Console.WriteLine($"{ex.Message}");
            }
            catch (InvalidEmailException ex)
            {
                _logger.LogWarning(ex, "Invalid email input");
                Console.WriteLine($"{ex.Message}");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed");
                Console.WriteLine($"{ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding contact");
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        private void ListContacts()
        {
            var contacts = _service.ListContacts();
            Console.WriteLine("\n--- All Contacts ---");

            if (!contacts.Any())
            {
                Console.WriteLine("No contacts found.");
                _logger.LogInformation("ListContacts: no contacts found");
                return;
            }

            foreach (var c in contacts)
                Console.WriteLine(c);
        }

        private void SearchContacts()
        {
            Console.Write("Enter search term: ");
            string term = Console.ReadLine() ?? "";
            _logger.LogInformation("User searching for: {Term}", term);

            var results = _service.Search(term);
            Console.WriteLine("\n--- Search Results ---");

            if (!results.Any())
            {
                Console.WriteLine("No matching contacts found.");
                return;
            }

            foreach (var c in results)
                Console.WriteLine(c);
        }

        private void FilterByTag()
        {
            Console.Write("Enter tag: ");
            string tag = Console.ReadLine() ?? "";
            _logger.LogInformation("User filtering by tag: {Tag}", tag);

            var results = _service.FilterByTag(tag);
            Console.WriteLine("\n--- Filtered by Tag ---");

            if (!results.Any())
            {
                Console.WriteLine("No contacts found with that tag.");
                return;
            }

            foreach (var c in results)
                Console.WriteLine(c);
        }
    }
