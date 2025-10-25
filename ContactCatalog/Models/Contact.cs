namespace ContactCatalog.Models;

public class Contact
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> Tags { get; set; }
    
    public override string ToString()
    {
        string tagList = Tags.Any() ? string.Join(", ", Tags) : "No tags";
        return $"ID: {Id}, Name: {Name}, Email: {Email}, Tags: {tagList}";  // Makes the program properly print the list
    }
}