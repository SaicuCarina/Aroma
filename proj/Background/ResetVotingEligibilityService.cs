using proj.Controllers;
using proj.Data;

public class ResetVotingEligibilityService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ResetVotingEligibilityService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Run once every day at midnight or at your preferred time
            var currentHour = DateTime.Now.Hour;
            if (currentHour == 0)  // You can adjust this check
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var controller = new RecipeController(context, null, null);
                    controller.ResetEligibleForVoting();
                }
            }

            // Wait for a while before checking again
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Sleep for 24 hours
        }
    }
}
