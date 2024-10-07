using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.EfContext;

public class EfDbContext(DbContextOptions<EfDbContext> options) : DbContext(options)
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Teams> Teams { get; set; }
    public DbSet<PersonalDetails> PersonalDetails { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<Ward> Wards { get; set; }
    public DbSet<Palika> Palikas { get; set; }

    public DbSet<User> Users { get; set; }
   
   protected override void OnModelCreating(ModelBuilder mb)
   {
      // base.OnModelCreating(mb);
            
       mb.Entity<PersonalDetails>()
           .Property(p => p.Gender)
           .HasConversion<int>(); //
     //  mb.Entity<PersonalDetails>().HasOne<Teams>(p=>p.Team).WithMany(t=>t.PersonalDetails).HasForeignKey(p=>p.TeamId).OnDelete(DeleteBehavior.Cascade);
       
       mb.Entity<User>()
           .Property(u => u.Role)
           .HasConversion<int>();
     
       
       base.OnModelCreating(mb);
       
       
         
   }
    

   
    
    
    
}