using PropertyRental.Models;

namespace PropertyRental.ViewModels;

public class ApartmentSearchViewModel
{
    public string? City { get; set; }
    public int? MinBedrooms { get; set; }
    public decimal? MaxRent { get; set; }
    public ApartmentStatus? Status { get; set; }
    public IReadOnlyList<Apartment> Results { get; set; } = Array.Empty<Apartment>();
}
