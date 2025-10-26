
using ContactCatalog.Exceptions;
using ContactCatalog.Models;
using ContactCatalog.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContactCatalog.Tests.Repositories;

public class ContactRepositoryTests
{
    private readonly Mock<ILogger<ContactRepository>> _mockLogger;
    private readonly ContactRepository _repository;

    public ContactRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<ContactRepository>>();
        _repository = new ContactRepository(_mockLogger.Object);
    }

    [Fact]
    public void Add_ShouldAddContactSuccessfully()
    {
        // Arrange
        var contact = new Contact { Name = "John Doe", Email = "john@example.com", Tags = new List<string>() };

        // Act
        _repository.Add(contact);

        // Assert
        Assert.Equal(1, contact.Id);
        var allContacts = _repository.GetAll().ToList();
        Assert.Single(allContacts);
        Assert.Equal(contact, allContacts[0]);
    }

    [Fact]
    public void Add_ShouldThrowDuplicateEmailException_WhenEmailExists()
    {
        // Arrange
        var contact1 = new Contact { Name = "John Doe", Email = "john@example.com", Tags = new List<string>() };
        var contact2 = new Contact { Name = "Jane Doe", Email = "john@example.com", Tags = new List<string>() };
        _repository.Add(contact1);

        // Act & Assert
        Assert.Throws<DuplicateEmailException>(() => _repository.Add(contact2));
    }

    [Fact]
    public void GetAll_ShouldReturnEmptyList_WhenNoContactsAdded()
    {
        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAll_ShouldReturnAllContacts()
    {
        // Arrange
        var contact1 = new Contact { Name = "John Doe", Email = "john@example.com", Tags = new List<string>() };
        var contact2 = new Contact { Name = "Jane Smith", Email = "jane@example.com", Tags = new List<string>() };
        _repository.Add(contact1);
        _repository.Add(contact2);

        // Act
        var result = _repository.GetAll().ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Search_ShouldReturnMatchingContacts()
    {
        // Arrange
        var contact1 = new Contact { Name = "John Doe", Email = "john@example.com", Tags = new List<string>() };
        var contact2 = new Contact { Name = "Jane Smith", Email = "jane@example.com", Tags = new List<string>() };
        _repository.Add(contact1);
        _repository.Add(contact2);

        // Act
        var result = _repository.Search(c => c.Name.Contains("John")).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("John Doe", result[0].Name);
    }
}