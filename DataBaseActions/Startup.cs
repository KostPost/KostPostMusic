using DataBaseActions.Music;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataBaseActions;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<MusicFileDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("Host=localhost;Database=KostPostMusic;Username=postgres;Password=2025;")));
        
        // Add other services
    }

}