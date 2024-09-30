using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.EfContext;

public class EfDbContext: DbContext
{
    public EfDbContext(DbContextOptions<EfDbContext> options): base(options)
    {
        
    }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Palika> Palikas { get; set; }
    public DbSet<Ward> Wards { get; set; }
    public DbSet<Teams> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne(p => p.PermanentAddress)
            .WithMany(a => a.Persons)
            .HasForeignKey(p => p.PermanentAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Person>()
            .HasOne<Address>()
            .WithMany()
            .HasForeignKey( "TempAddressId")
            .OnDelete(DeleteBehavior.Restrict);
        
        

        modelBuilder.Entity<Person>()
            .HasMany(p => p.ManagedTeams)
            .WithOne(t => t.Manager)
            .HasForeignKey(t => t.ManagerId);

        modelBuilder.Entity<Person>()
            .HasMany(p => p.CoachedTeams)
            .WithOne(t => t.Coach)
            .HasForeignKey(t => t.CoachId);

        // Configure Team Relationships
        modelBuilder.Entity<Teams>()
            .HasOne(t => t.Ward)
            .WithMany(w => w.TeamList)
            .HasForeignKey(t => t.WardId);

        modelBuilder.Entity<Teams>()
            .HasOne(t => t.Palika)
            
            .WithMany(p => p.LocalTeamsList)
            .HasForeignKey(t => t.PalikaId);

        // Configure Ward Relationships
        modelBuilder.Entity<Ward>()
            .HasOne(w => w.RefPalika)
            .WithMany( p => p.WardsList)
            .HasForeignKey(w => w.PalikaId);
        
        base.OnModelCreating(modelBuilder);
        
        
    }
    
    
    
}