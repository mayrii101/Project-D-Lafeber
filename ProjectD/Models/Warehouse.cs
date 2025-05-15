using System.ComponentModel.DataAnnotations;

public class Warehouse
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string ContactPerson { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}