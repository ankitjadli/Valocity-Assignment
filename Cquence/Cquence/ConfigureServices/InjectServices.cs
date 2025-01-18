using valocity.assignment.cquence.Application.Services;
using valocity.assignment.cquence.core.gameengine.IServices;
using valocity.assignment.cquence.infrastructure.contacts.gamerepository;
using valocity.assignment.cquence.Infrastructure.Persistence;

namespace valocity.assignment.cquence.ConfigureServices
{
    public static class InjectServices
    {
        // Method to configure services and register them in DI container
        public static void ConfigureServices(IServiceCollection services)
        {
            // Register the repository (data access layer) with DI container as Scoped
            services.AddScoped<IGameRepository, GameRepository>();

            // Register the service layer (business logic) with DI container as Scoped
            services.AddScoped<IGameService, GameService>();
        }
    }
}
