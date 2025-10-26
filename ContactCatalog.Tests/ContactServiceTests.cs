using ContactCatalog.Exceptions;
using ContactCatalog.Models;
using ContactCatalog.Repositories;
using ContactCatalog.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContactCatalog.Tests.Services;

public class ContactServiceTests
{
    private readonly Mock<IContactRepository> _mockRepository;
    private readonly Mock<ILogger<ContactService>> _mockLogger;
    private readonly ContactService _contactService;

    public ContactServiceTests()
    {
        _mockRepository = new Mock<IContactRepository>();
        _mockLogger = new Mock<ILogger<ContactService>>();
        _contactService = new ContactService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void AddContact_ShouldCallRepositoryAdd_WithValidInput()
    {
        // Arrange
        string name = "John Doe";
        string email = "john@example.com";
        string tags = "friend,colleague";

        // Act
        _contactService.AddContact(name, email, tags);

        // Assert
        _mockRepository.Verify(r => r.Add(It.Is<Contact>(c => 
            c.Name == name.Trim() && 
            c.Email == email.Trim().ToLower() && 
            c.Tags.Count == 2 &&
            c.Tags.Contains("friend") &&
            c.Tags.Contains("colleague")
        )), Times.Once);
    }

    [Fact]
    public void AddContact_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        string name = "";
        string email = "john@example.com";
        string tags = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _contactService.AddContact(name, email, tags));
        _mockRepository.Verify(r => r.Add(It.IsAny<Contact>()), Times.Never);
    }

    [Fact]
    public void AddContact_ShouldThrowInvalidEmailException_WhenEmailIsInvalid()
    {
        // Arrange
        string name = "John Doe";
        string email = "invalid-email";
        string tags = "";

        // Act & Assert
        Assert.Throws<InvalidEmailException>(() => _contactService.AddContact(name, email, tags));
        _mockRepository.Verify(r => r.Add(It.IsAny<Contact>()), Times.Never);
    }

    [Fact]
    public void ListContacts_ShouldReturnContactsFromRepository()
    {
        // Arrange
        var expectedContacts = new List<Contact>
        {
            new Contact { Id = 1, Name = "John Doe", Email = "john@example.com", Tags = new List<string>() },
            new Contact { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Tags = new List<string>() }
        };
        _mockRepository.Setup(r => r.GetAll()).Returns(expectedContacts);

        // Act
        var result = _contactService.ListContacts();

        // Assert
        Assert.Equal(expectedContacts, result);
        _mockRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void Search_ShouldCallRepositorySearch_WithCorrectPredicate()
    {
        // Arrange
        string searchTerm = "John";
        var expectedContacts = new List<Contact>
        {
            new Contact { Id = 1, Name = "John Doe", Email = "john@example.com", Tags = new List<string>() }
        };
        _mockRepository.Setup(r => r.Search(It.IsAny<Func<Contact, bool>>()))
            .Returns(expectedContacts);

        // Act
        var result = _contactService.Search(searchTerm);

        // Assert
        Assert.Equal(expectedContacts, result);
        _mockRepository.Verify(r => r.Search(It.IsAny<Func<Contact, bool>>()), Times.Once);
    }

    [Fact]
    public void FilterByTag_ShouldCallRepositorySearch_WithCorrectPredicate()
    {
        // Arrange
        string tag = "friend";
        var expectedContacts = new List<Contact>
        {
            new Contact 
            { 
                Id = 1, 
                Name = "John Doe", 
                Email = "john@example.com",
                Tags = new List<string> { "friend", "colleague" }
            }
        };
        _mockRepository.Setup(r => r.Search(It.IsAny<Func<Contact, bool>>()))
            .Returns(expectedContacts);

        // Act
        var result = _contactService.FilterByTag(tag);

        // Assert
        Assert.Equal(expectedContacts, result);
        _mockRepository.Verify(r => r.Search(It.IsAny<Func<Contact, bool>>()), Times.Once);
    }

    [Theory]
    [InlineData("john@example.com")]
    [InlineData("jane.smith@company.co.uk")]
    [InlineData("test123@test-domain.com")]
    public void AddContact_ShouldAcceptValidEmails(string validEmail)
    {
        // Arrange
        string name = "Test User";
        string tags = "";

        // Act
        _contactService.AddContact(name, validEmail, tags);

        // Assert
        _mockRepository.Verify(r => r.Add(It.IsAny<Contact>()), Times.Once);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test @example.com")]
    public void AddContact_ShouldRejectInvalidEmails(string invalidEmail)
    {
        // Arrange
        string name = "Test User";
        string tags = "";

        // Act & Assert
        Assert.Throws<InvalidEmailException>(() => _contactService.AddContact(name, invalidEmail, tags));
    }
}