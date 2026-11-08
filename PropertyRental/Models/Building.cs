using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyRental.Models;

public class Building
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string StreetAddress { get; set; } = string.Empty;

    [StringLength(80)]
    public string? City { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    /// <summary>Property owner (landlord) who owns this building.</summary>
    [Required]
    public string PropertyOwnerId { get; set; } = string.Empty;

    [ForeignKey(nameof(PropertyOwnerId))]
    public ApplicationUser? PropertyOwner { get; set; }

    /// <summary>Property manager responsible for this building.</summary>
    [Required]
    public string PropertyManagerId { get; set; } = string.Empty;

    [ForeignKey(nameof(PropertyManagerId))]
    public ApplicationUser? PropertyManager { get; set; }

    public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
}
