using PuppeteerSharp;

namespace StreamScraperTest;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


public sealed class StreamscraperHostedService : IHostedService
{
    private readonly ILogger _logger;

    public StreamscraperHostedService(
        ILogger<StreamscraperHostedService> logger)
    {
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("4. StopAsync has been called.");

        return Task.CompletedTask;
    }
}