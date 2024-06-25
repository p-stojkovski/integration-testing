using Customers.Api.Database;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Testcontainers.PostgreSql;

namespace Customers.Api.Tests.Integration;

public class CustomerApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    public const string ValidGitHubUser = "validUser";
    public const string ThrottledUser = "throttledUser"; 

    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
        .WithDatabase("mydb")
        .WithUsername("workshop")
        .WithPassword("changeme")
        .Build();

    private readonly GitHubApiServer _gitHubApiServer = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureTestServices(services =>
        {
            //For removing backgound tasks that could interfere the tests if you don't need them
            services.RemoveAll(typeof(IHostedService));

            services.RemoveAll(typeof(IDbConnectionFactory));
            services.AddSingleton<IDbConnectionFactory>(_ => 
                new NpgsqlConnectionFactory(_dbContainer.GetConnectionString()));

            services.AddHttpClient("GitHub", httpClient =>
            {
                httpClient.BaseAddress = new Uri(_gitHubApiServer.Url);
                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.Accept, "application/vnd.github.v3+json");
                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.UserAgent, $"Course-{Environment.MachineName}");
            });

            //Example for inital setup of EF Core
            services.RemoveAll(typeof(DbContext));
            services.AddDbContext<AppDbContext>(optionsBuilder 
                => optionsBuilder.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        _gitHubApiServer.Start();
        _gitHubApiServer.SetupUser(ValidGitHubUser);
        _gitHubApiServer.SetupThrottledUser(ThrottledUser);

        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();

        _gitHubApiServer.Dispose();
    }
}

public class AppDbContext : DbContext
{

}

//private readonly PostgreSqlContainer _dbContainer =
//    new PostgreSqlBuilder()
//    .WithImage("postgres:latest")
//    .WithEnvironment("POSTGRES_USER", "course")
//    .WithEnvironment("POSTGRES_PASSWORD", "changeme")
//    .WithEnvironment("POSTGRES_DB", "mydb")
//    .WithPortBinding(5555, 5432)
//    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
//    .Build();