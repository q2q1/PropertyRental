using PropertyRental.Models;

namespace PropertyRental.ViewModels;

public class HomeIndexViewModel
{
    public IReadOnlyList<Apartment> FeaturedApartments { get; init; } = Array.Empty<Apartment>();
}
