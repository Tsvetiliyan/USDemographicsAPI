using Microsoft.Extensions.Hosting;
using System.Text.Json;
using USDemographicsAPI.Core.Interfaces.IServices;
using USDemographicsAPI.Core;
using USDemographicsAPI.Core.DomainModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace USDemographicsAPI.Services;

public class BackgroundPopulationDataFetcherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundPopulationDataFetcherService> _logger;

    public BackgroundPopulationDataFetcherService(IServiceProvider serviceProvider, ILogger<BackgroundPopulationDataFetcherService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Fetching External API's data...");

                await UpdateCountiesFromExternalSourceAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to get the External API's data!");
            }

            await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
        }
    }

    private async Task UpdateCountiesFromExternalSourceAsync()
    {
        //I have left the queryParameters as a parameter because in the future there could be a need in changing what data we need to get -> like we may want to change the outfield or get a different type of format like not pjson or change the where clause
        using (var scope = _serviceProvider.CreateScope())
        {
            IEsriAPIService _esriAPIService = scope.ServiceProvider.GetRequiredService<IEsriAPIService>();

            Dictionary<string, string> queryParameters = new()
            {
                ["where"] = "1=1",
                ["outFields"] = "*",
                ["returnGeometry"] = "false",
                ["f"] = "pjson"
            };
            string countiesJson = await _esriAPIService.GetExternalDataAsync(queryParameters);
            List<County> result = _esriAPIService.ProcessExternalData(countiesJson);
            await _esriAPIService.TryUpdateOrAddCountiesAndStates(result);
        }
    }
}
