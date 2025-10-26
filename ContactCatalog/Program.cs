using ContactCatalog.Repositories;
using ContactCatalog.Services;
using Microsoft.Extensions.Logging;

namespace ContactCatalog;

class Program
{
    static void Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Information);
        });

        var repoLogger = loggerFactory.CreateLogger<ContactRepository>();
        var serviceLogger = loggerFactory.CreateLogger<ContactService>();
        var menuLogger = loggerFactory.CreateLogger<MenuService>();
        
        IContactRepository repository = new ContactRepository(repoLogger);
        var service = new ContactService(repository, serviceLogger);
        var menu = new MenuService(service, menuLogger);
        menu.PrintMenu();
    }
}