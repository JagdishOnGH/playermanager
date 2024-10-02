using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.EfContext;

public class EfDbContext: DbContext
{
    public EfDbContext(DbContextOptions<EfDbContext> options): base(options)
    {
        
    }
    
    public DbSet<Address> Addresses { get; set; }
    
    public DbSet<Ward> Wards { get; set; }
    public DbSet<Teams> Teams { get; set; }
    public DbSet<PersonalDetails> PersonalDetails { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Coach> Coaches { get; set; }
    public DbSet<Palika> Palikas { get; set; }

   protected override void OnModelCreating(ModelBuilder mb)
   {
       mb.Entity<Teams>().HasOne<Manager>().WithOne(m => m.ManagingTeam).HasForeignKey<Teams>(t=>t.ManagerId);
         mb.Entity<Teams>().HasOne<Coach>().WithOne(c => c.CoachingTeam).HasForeignKey<Teams>(t=>t.CoachId);
         
   }
    

   
    
    
    
}