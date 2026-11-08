using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyRental.Models;

public class OwnerEventReport
{
    public int Id { get; set; }

    [Required]
    public string PropertyManagerId { get; set; } = string.Empty;

    [ForeignKey(nameof(PropertyManagerId))]
    public ApplicationUser? PropertyManager { get; set; }

    /// <summary>Owner or administrator who should receive this report.</summary>
    [Required]
    public string PropertyOwnerId { get; set; } = string.Empty;

    [ForeignKey(nameof(PropertyOwnerId))]
    public ApplicationUser? PropertyOwner { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public bool IsAcknowledged { get; set; }
}
