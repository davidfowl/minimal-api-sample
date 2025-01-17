using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Tests.IntegrationTests;

public class ApiIntegrationTestBase : IDisposable
{
    protected readonly ITestOutputHelper TestOutputHelper;
    private WebApplicationFactory<Program> _host = null!;

    protected ApiIntegrationTestBase(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
        InitializeAsync();
    }

    protected HttpClient Client { get; private set; } = null!;
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _host?.Dispose();
        Client?.Dispose();
    }

    private void InitializeAsync()
    {
        _host = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder
                    .UseContentRoot(Path.GetDirectoryName(GetType().Assembly.Location)!)
                    .ConfigureAppConfiguration(
                        configurationBuilder =>
                            configurationBuilder.AddJsonFile("appsettings.json", false, true))
                    .ConfigureServices(services =>
                    {
                        services.AddLogging(logging =>
                            logging.AddXUnit(TestOutputHelper));
                    });
            });

        Client = _host.CreateClient();
    }
}