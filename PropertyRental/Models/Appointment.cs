using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyRental.Models;

public enum AppointmentStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    Completed = 3
}

public class Appointment
{
    public int Id { get; set; }

    [Required]
    public string TenantId { get; set; } = string.Empty;

    [ForeignKey(nameof(TenantId))]
    public ApplicationUser? Tenant { get; set; }

    [Required]
    public string PropertyManagerId { get; set; } = string.Empty;

    [ForeignKey(nameof(PropertyManagerId))]
    public ApplicationUser? PropertyManager { get; set; }

    public int ApartmentId { get; set; }

    [ForeignKey(nameof(ApartmentId))]
    public Apartment? Apartment { get; set; }

    public DateTime StartUtc { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    [StringLength(1000)]
    public string? Notes { get; set; }
}
