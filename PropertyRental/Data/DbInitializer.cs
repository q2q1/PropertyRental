using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Models;

namespace PropertyRental.Data;

public static class DbInitializer
{
    public const string RolePropertyOwner = "PropertyOwner";
    public const string RolePropertyManager = "PropertyManager";
    public const string RoleTenant = "Tenant";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var context = provider.GetRequiredService<ApplicationDbContext>();
        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        // Schema is managed via EF Core migrations (dotnet ef database update).

        string[] roles = { RolePropertyOwner, RolePropertyManager, RoleTenant };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        await EnsureUserAsync(userManager, "owner@propertyrental.local", "Owner123!", "Alex", "Owner",
            RolePropertyOwner);

        var manager = await EnsureUserAsync(userManager, "manager@propertyrental.local", "Manager123!", "Jamie", "Manager",
            RolePropertyManager);

        await EnsureUserAsync(userManager, "tenant@propertyrental.local", "Tenant123!", "Taylor", "Tenant",
            RoleTenant);

        if (manager != null)
        {
            var owner = await userManager.FindByEmailAsync("owner@propertyrental.local");
            if (owner == null)
                throw new InvalidOperationException("Seed owner user was not created.");
            var existingBuildings = await context.Buildings
                .AsNoTracking()
                .Select(b => new Building { Id = b.Id, Name = b.Name })
                .ToListAsync();
            var existingBuildingsByName = existingBuildings
                .ToDictionary(b => b.Name, StringComparer.OrdinalIgnoreCase);

            var buildingsToAdd = new List<Building>();

            Building EnsureBuilding(
                string name,
                string streetAddress,
                string? city,
                string? postalCode)
            {
                if (existingBuildingsByName.TryGetValue(name, out var existing))
                    return existing;

                var b = new Building
                {
                    Name = name,
                    StreetAddress = streetAddress,
                    City = city,
                    PostalCode = postalCode,
                    PropertyOwnerId = owner.Id,
                    PropertyManagerId = manager.Id
                };
                buildingsToAdd.Add(b);
                context.Buildings.Add(b);
                existingBuildingsByName[name] = b;
                return b;
            }

            var riverside = EnsureBuilding("Riverside Tower", "100 Main St", "Montreal", "H1A1A1");
            var oldPort = EnsureBuilding("Old Port Lofts", "22 Rue de la Commune O", "Montreal", "H2Y2C5");
            var plateau = EnsureBuilding("Plateau Garden Flats", "4110 Av. du Parc", "Montreal", "H2W1S8");
            var downtown = EnsureBuilding("Downtown Micro Suites", "880 René-Lévesque Blvd W", "Montreal", "H3B1P1");

            if (buildingsToAdd.Count > 0)
                await context.SaveChangesAsync();

            var existingUnits = await context.Apartments
                .Join(context.Buildings, a => a.BuildingId, b => b.Id, (a, b) => new { b.Name, a.UnitNumber })
                .ToListAsync();
            var existingUnitSet = existingUnits
                .Select(x => $"{x.Name}::{x.UnitNumber}")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            void AddApartmentIfMissing(Building b, Apartment a)
            {
                var key = $"{b.Name}::{a.UnitNumber}";
                if (existingUnitSet.Contains(key))
                    return;
                a.BuildingId = b.Id;
                context.Apartments.Add(a);
                existingUnitSet.Add(key);
            }

            // Riverside Tower
            AddApartmentIfMissing(riverside, new Apartment
            {
                UnitNumber = "101",
                Bedrooms = 2,
                Bathrooms = 1,
                MonthlyRent = 1450,
                Status = ApartmentStatus.Available,
                Description = "Sunny corner unit with balcony.",
                ImageUrl = "/images/apartment-sample.jpg"
            });
            AddApartmentIfMissing(riverside, new Apartment
            {
                UnitNumber = "205",
                Bedrooms = 1,
                Bathrooms = 1,
                MonthlyRent = 1100,
                Status = ApartmentStatus.Available,
                Description = "Compact downtown studio layout.",
                ImageUrl = "/images/apartment-sample.jpg"
            });
            AddApartmentIfMissing(riverside, new Apartment
            {
                UnitNumber = "1204",
                Bedrooms = 3,
                Bathrooms = 2,
                MonthlyRent = 2250,
                Status = ApartmentStatus.Available,
                Description = "High-floor unit with river view and in-suite laundry.",
                ImageUrl = "/images/apartment-sample.jpg"
            });

            // Old Port Lofts
            AddApartmentIfMissing(oldPort, new Apartment
            {
                UnitNumber = "2A",
                Bedrooms = 1,
                Bathrooms = 1,
                MonthlyRent = 1650,
                Status = ApartmentStatus.Available,
                Description = "Exposed brick loft near the Old Port.",
                ImageUrl = "/images/apartment-sample.jpg"
            });
            AddApartmentIfMissing(oldPort, new Apartment
            {
                UnitNumber = "3C",
                Bedrooms = 2,
                Bathrooms = 1,
                MonthlyRent = 1950,
                Status = ApartmentStatus.Available,
                Description = "Loft-style layout with large windows and open kitchen.",
                ImageUrl = "/images/apartment-sample.jpg"
            });

            // Plateau Garden Flats
            AddApartmentIfMissing(plateau, new Apartment
            {
                UnitNumber = "1",
                Bedrooms = 2,
                Bathrooms = 1,
                MonthlyRent = 1750,
                Status = ApartmentStatus.Available,
                Description = "Quiet ground-floor unit with shared garden access.",
                ImageUrl = "/images/apartment-sample.jpg"
            });
            AddApartmentIfMissing(plateau, new Apartment
            {
                UnitNumber = "3",
                Bedrooms = 3,
                Bathrooms = 1,
                MonthlyRent = 2150,
                Status = ApartmentStatus.Available,
                Description = "Family-friendly flat steps from cafés and parks.",
                ImageUrl = "/images/apartment-sample.jpg"
            });

            // Downtown Micro Suites
            AddApartmentIfMissing(downtown, new Apartment
            {
                UnitNumber = "508",
                Bedrooms = 0,
                Bathrooms = 1,
                MonthlyRent = 1250,
                Status = ApartmentStatus.Available,
                Description = "Modern micro-suite with built-in storage.",
                ImageUrl = "/images/apartment-sample.jpg"
            });
            AddApartmentIfMissing(downtown, new Apartment
            {
                UnitNumber = "914",
                Bedrooms = 0,
                Bathrooms = 1,
                MonthlyRent = 1350,
                Status = ApartmentStatus.Available,
                Description = "Bright studio with city view; ideal for commuters.",
                ImageUrl = "/images/apartment-sample.jpg"
            });

            if (owner != null)
            {
                context.OwnerEventReports.Add(new OwnerEventReport
                {
                    PropertyManagerId = manager.Id,
                    PropertyOwnerId = owner.Id,
                    Title = "Welcome sample report",
                    Description = "This is seeded data demonstrating manager-to-owner reporting.",
                    CreatedUtc = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync();
        }
    }

    private static async Task<ApplicationUser?> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string firstName,
        string lastName,
        params string[] roles)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
            return user;

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to create {email}: " +
                string.Join("; ", result.Errors.Select(e => e.Description)));

        foreach (var role in roles)
            await userManager.AddToRoleAsync(user, role);

        return user;
    }
}
