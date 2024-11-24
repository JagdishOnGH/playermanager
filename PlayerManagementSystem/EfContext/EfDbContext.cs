using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using PlayerManagementSystem.Models;
using PlayerManagementSystem.Models.AuthModel;

namespace PlayerManagementSystem.EfContext;

public class EfDbContext : IdentityDbContext<AppUser>
{
    public EfDbContext(DbContextOptions<EfDbContext> options)
        : base(options) { }

    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Municipality> Municipalities { get; set; }
    public DbSet<Ward> Wards { get; set; }
    public DbSet<PersonTeam> PersonTeams { get; set; }

    //teams
    public DbSet<Team> Teams { get; set; }
    public DbSet<Person> Persons { get; set; }

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
        modelBuilder.Entity<PersonTeam>().Property(pt => pt.PersonId).HasColumnType("uuid");
        modelBuilder.Entity<PersonTeam>().Property(pt => pt.TeamId).HasColumnType("uuid");

        //relationship for person and team and person team
        modelBuilder.Entity<PersonTeam>().HasKey(pt => new { pt.PersonId, pt.TeamId });

        modelBuilder
            .Entity<PersonTeam>()
            .HasOne<Person>(p => p.Person)
            .WithMany()
            .HasForeignKey(pt => pt.PersonId);

        modelBuilder
            .Entity<PersonTeam>()
            .HasOne<Team>(t => t.Team)
            .WithMany()
            .HasForeignKey(t => t.TeamId);

        modelBuilder
            .Entity<Province>()
            .HasData(
                new Province
                {
                    ProvinceId = Guid.Parse("d6c29e07-8824-4f31-bf07-9e0fbb39d9a8"),
                    Name = "Koshi",
                },
                new Province
                {
                    ProvinceId = Guid.Parse("8a2b99b3-c36f-4a4f-9e5b-2939efba2b30"),
                    Name = "Madhesh",
                },
                new Province
                {
                    ProvinceId = Guid.Parse("e5724aef-2d64-4fd4-9e1c-8f0d05e2e7b4"),
                    Name = "Bagmati",
                },
                new Province
                {
                    ProvinceId = Guid.Parse("f2123c8b-37df-4a41-97fc-584c1af4d07d"),
                    Name = "Gandaki",
                },
                new Province
                {
                    ProvinceId = Guid.Parse("aec679cb-958e-4cd7-8f41-cc9a76276a3e"),
                    Name = "Lumbini",
                },
                new Province
                {
                    ProvinceId = Guid.Parse("2b8d6ad7-83b6-4972-a5d7-d84b1c40d8c4"),
                    Name = "Karnali",
                },
                new Province
                {
                    ProvinceId = Guid.Parse("aa6b64b7-6551-45c3-a988-b1d9be35d8cd"),
                    Name = "Sudurpashchim",
                }
            );
        modelBuilder.Entity<Team>().HasData(new Team
            {
                TeamId = Guid.Parse("79b1d3f5-7f28-4f47-b3ed-7a36b721fb13"),
                Name = "Koshi Team",
                TerritoryType = TerritoryType.Province,
                TerritoryId = Guid.Parse("d6c29e07-8824-4f31-bf07-9e0fbb39d9a8")
            }
            , new Team
            {
                TeamId = Guid.Parse("a296f452-9ee0-4607-922e-39ae666a083a"),
                Name = "Madhesh Team",
                TerritoryType = TerritoryType.Province,
                TerritoryId = Guid.Parse("8a2b99b3-c36f-4a4f-9e5b-2939efba2b30")
            },
            new Team
            {
                TeamId = Guid.Parse("fb21a147-6546-4a17-9e68-549bdbcee16c"),
                Name = "Bagmati Team",
                TerritoryType = TerritoryType.Province,
                TerritoryId = Guid.Parse("e5724aef-2d64-4fd4-9e1c-8f0d05e2e7b4")
            },
            new Team
            {
                TeamId = Guid.Parse("e62a9871-51b5-4c2b-aca1-571bada67f59"),
                Name = "Gandaki Team",
                TerritoryType = TerritoryType.Province,
                TerritoryId = Guid.Parse("f2123c8b-37df-4a41-97fc-584c1af4d07d")
            },
            new Team
            {
                TeamId = Guid.Parse("f3b1d3f5-7f28-4f47-b3ed-7a36b721fb13"),
                Name = "Lumbini Team",
                TerritoryType = TerritoryType.Province,
                TerritoryId = Guid.Parse("aec679cb-958e-4cd7-8f41-cc9a76276a3e")
            },
            new Team
            {
                TeamId = Guid.Parse("c296f452-9ee0-4607-922e-39ae666a083a"),
                Name = "Karnali Team",
                TerritoryType = TerritoryType.Province,
                TerritoryId = Guid.Parse("2b8d6ad7-83b6-4972-a5d7-d84b1c40d8c4")
            },
            new Team
            {
                TeamId = Guid.Parse("eb21a147-6546-4a17-9e68-549bdbcee16c"),
                Name = "Sudurpashchim Team",
                TerritoryType = TerritoryType.Province,
                TerritoryId = Guid.Parse("aa6b64b7-6551-45c3-a988-b1d9be35d8cd")
            }
        );
        
            
        
        base.OnModelCreating(modelBuilder);
        // List<IdentityRole> roles =
        // [
        //     new IdentityRole { Name = "WardAdmin", NormalizedName = "WARD_ADMIN" },
        //     new IdentityRole { Name = "MunicipalityAdmin", NormalizedName = "MUNICIPALITY_ADMIN" },
        //     new IdentityRole { Name = "DistrictAdmin", NormalizedName = "DISTRICT_ADMIN" },
        //     new IdentityRole { Name = "ProvinceAdmin", NormalizedName = "PROVINCE_ADMIN" },
        //     new IdentityRole { Name = "SuperAdmin", NormalizedName = "SUPER_ADMIN" }
        // ];
        //
        // modelBuilder.Entity<IdentityRole>().HasData(roles);
        
    }
}
