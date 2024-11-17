using Microsoft.EntityFrameworkCore;

using PlayerManagementSystem.Models;


namespace PlayerManagementSystem.EfContext;

public class EfDbContext(DbContextOptions<EfDbContext> options) : DbContext(options)
{
  public DbSet<Province> Provinces { get; set; }
 public  DbSet<District> Districts { get; set; }
 public  DbSet<Municipality> Municipalities { get; set; }
 public  DbSet<Ward> Wards { get; set; }
 public DbSet<PersonTeam> PersonTeams { get; set; }
   //teams
 public  DbSet<Team> Teams { get; set; }
 public  DbSet<Person> Persons { get; set; }
   
   //onModelCreating
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      //add type caste guid to string
      modelBuilder.Entity<Province>().Property(p => p.ProvinceId).HasColumnType("uuid");
      modelBuilder.Entity<District>().Property(d => d.DistrictId).HasColumnType("uuid");
      modelBuilder.Entity<Municipality>().Property(m => m.MunicipalityId).HasColumnType("uuid");
      modelBuilder.Entity<Ward>().Property(w => w.WardId).HasColumnType("uuid");
      modelBuilder.Entity<Team>().Property(t => t.TeamId).HasColumnType("uuid");
      modelBuilder.Entity<Person>().Property(p => p.PersonId).HasColumnType("uuid");
      modelBuilder.Entity<PersonTeam>().Property(pt=>pt.PersonId).HasColumnType("uuid");
        modelBuilder.Entity<PersonTeam>().Property(pt=>pt.TeamId).HasColumnType("uuid");
        
        
        //relationship for person and team and personteam
        modelBuilder.Entity<PersonTeam>()
          .HasKey(pt => new { pt.PersonId, pt.TeamId });

        modelBuilder.Entity<PersonTeam>()
          .HasOne<Person>()
          .WithMany()
          .HasForeignKey(pt => pt.PersonId);
          ;

        modelBuilder.Entity<PersonTeam>()
          .HasOne<Team>()
          .WithMany().HasForeignKey(t=>t.TeamId)
          ;
     
      
      modelBuilder.Entity<Province>().HasData(
          new Province { ProvinceId = Guid.Parse("d6c29e07-8824-4f31-bf07-9e0fbb39d9a8"), Name = "Koshi" },
          new Province { ProvinceId = Guid.Parse("8a2b99b3-c36f-4a4f-9e5b-2939efba2b30"), Name = "Madhesh" },
          new Province { ProvinceId = Guid.Parse("e5724aef-2d64-4fd4-9e1c-8f0d05e2e7b4"), Name = "Bagmati" },
          new Province { ProvinceId = Guid.Parse("f2123c8b-37df-4a41-97fc-584c1af4d07d"), Name = "Gandaki" },
          new Province { ProvinceId = Guid.Parse("aec679cb-958e-4cd7-8f41-cc9a76276a3e"), Name = "Lumbini" },
          new Province { ProvinceId = Guid.Parse("2b8d6ad7-83b6-4972-a5d7-d84b1c40d8c4"), Name = "Karnali" },
          new Province { ProvinceId = Guid.Parse("aa6b64b7-6551-45c3-a988-b1d9be35d8cd"), Name = "Sudurpashchim" }
      );


   }
    

   
    
    
    
}