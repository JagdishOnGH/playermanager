using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Repositories;

public class PlayerRepository(EfDbContext ctx)
{
    private readonly EfDbContext context = ctx;

  public  Task<List<Person>> GetAllPlayers()
    {
        
        return context.Persons.ToListAsync();

    }
}