using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Models;

namespace PropertyRental.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<OwnerEventReport> OwnerEventReports => Set<OwnerEventReport>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Building>()
            .HasOne(b => b.PropertyManager)
            .WithMany()
            .HasForeignKey(b => b.PropertyManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Building>()
            .HasOne(b => b.PropertyOwner)
            .WithMany()
            .HasForeignKey(b => b.PropertyOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Apartment>()
            .HasOne(a => a.Building)
            .WithMany(b => b.Apartments)
            .HasForeignKey(a => a.BuildingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Appointment>()
            .HasOne(a => a.Tenant)
            .WithMany()
            .HasForeignKey(a => a.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.PropertyManager)
            .WithMany()
            .HasForeignKey(a => a.PropertyManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Apartment)
            .WithMany()
            .HasForeignKey(a => a.ApartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OwnerEventReport>()
            .HasOne(r => r.PropertyManager)
            .WithMany()
            .HasForeignKey(r => r.PropertyManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OwnerEventReport>()
            .HasOne(r => r.PropertyOwner)
            .WithMany()
            .HasForeignKey(r => r.PropertyOwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
