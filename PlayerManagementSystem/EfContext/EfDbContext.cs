using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.EfContext;

public class EfDbContext: DbContext
{
    public EfDbContext(DbContextOptions<EfDbContext> options): base(options)
    {
        
    }
    
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Teams> Teams { get; set; }
    public DbSet<PersonalDetails> PersonalDetails { get; set; }
    public DbSet<Roles> Roles { get; set; }  
    
}