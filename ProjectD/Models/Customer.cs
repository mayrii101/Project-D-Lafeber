using System.ComponentModel.DataAnnotations;

public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string BedrijfsNaam { get; set; } = string.Empty;

    [Required]
    public string ContactPersoon { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string TelefoonNummer { get; set; } = string.Empty;

    public string Adres { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}