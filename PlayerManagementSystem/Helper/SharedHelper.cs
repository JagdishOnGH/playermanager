using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Helper;

public static class SharedHelper
{
    public static ApiResponse<string> CreateErrorResponse(string message)
    {
        return new ApiResponse<string> { Error = message };
    }

    public static string? ModelValidationCheck(ModelStateDictionary modelState)
    {
        if(!modelState.IsValid)
        {
            return   modelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToString();
        }
        return null;
        
    }

    public static async Task ValidateTeamRolesAsync(Guid teamId, Role role, EfDbContext context)
    {
        // Count the number of players, coaches, and managers in the team
        var roleCounts = await context
            .PersonTeams.Where(pt => pt.TeamId == teamId)
            .GroupBy(pt => pt.Person.Role)
            .Select(g => new { Role = g.Key, Count = g.Count() })
            .ToListAsync();

        // Get current counts for each role
        var playerCount = roleCounts.FirstOrDefault(r => r.Role == Role.Player)?.Count ?? 0;
        var coachCount = roleCounts.FirstOrDefault(r => r.Role == Role.Coach)?.Count ?? 0;
        var managerCount = roleCounts.FirstOrDefault(r => r.Role == Role.Manager)?.Count ?? 0;

        // Validate based on role
        switch (role)
        {
            case Role.Player: // Player
                if (playerCount >= 12)
                {
                    throw new Exception("Max player reached");
                }
                break;

            case Role.Coach: // Coach
                if (coachCount >= 1)
                {
                    throw new Exception("Team already has 1 coach");
                }
                break;

            case Role.Manager: // Manager
                if (managerCount >= 1)
                {
                    throw new Exception("Team already has 1 manager");
                }
                break;
        }
    }
}
