using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PlayerManagementSystem.Models;


namespace PlayerManagementSystem.EfContext;

public class EfDbContext(DbContextOptions<EfDbContext> options) : DbContext(options)
{
   DbSet<Province> Provinces { get; set; }
   DbSet<District> Districts { get; set; }
   DbSet<Municipality> Municipalities { get; set; }
   DbSet<Ward> Wards { get; set; }
   //teams
   DbSet<Team> Teams { get; set; }
   DbSet<Person> Persons { get; set; }
   
   
    

   
    
    
    
}