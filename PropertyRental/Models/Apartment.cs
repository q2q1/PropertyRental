using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyRental.Models;

public class Apartment
{
    public int Id { get; set; }

    [Required]
    public int BuildingId { get; set; }

    [ForeignKey(nameof(BuildingId))]
    public Building? Building { get; set; }

    [Required, StringLength(40)]
    public string UnitNumber { get; set; } = string.Empty;

    [Range(0, 20)]
    public int Bedrooms { get; set; }

    [Range(0, 20)]
    public int Bathrooms { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    [Range(0, 1000000)]
    public decimal MonthlyRent { get; set; }

    public ApartmentStatus Status { get; set; } = ApartmentStatus.Available;

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }
}
