using System.Text;
using System.Text.Json;
using USDemographicsAPI.Core.DomainModels;
using USDemographicsAPI.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using USDemographicsAPI.Core.Dtos;

namespace USDemographicsAPI.Services;
/// <summary>
/// This class is used for getting counties info
/// </summary>
public class EsriAPIService : IEsriAPIService
{
    private readonly ILogger<EsriAPIService> _logger;
    private readonly string _baseUrl;
    private readonly ICountyService _countyService;
    private readonly IStateService _stateService;

    private DateTime _lastModifiedDate;

    public EsriAPIService(ICountyService countyService, IStateService stateService, ILogger<EsriAPIService> logger)
    {
        _baseUrl = "https://services.arcgis.com/P3ePLMYs2RVChkJx/ArcGIS/rest/services/USA_Census_Counties/FeatureServer/0/query";
        _countyService = countyService;
        _stateService = stateService;
        _logger = logger;
    }
    public async Task<string> GetExternalDataAsync(Dictionary<string, string> queryParameters)
    {
        string queryParametersString = BuildQueryParameterString(queryParameters);

        StringBuilder urlAddress = new(_baseUrl);
        if (queryParametersString.Length > 0)
        {
            urlAddress.Append("?" + queryParametersString);
        }

        string urlAdressToConnect = urlAddress.ToString();

        string jsonCountyInfo = await ConnectToApiAndGetResponse(urlAdressToConnect);

        return jsonCountyInfo;
    }

    private static string BuildQueryParameterString(Dictionary<string, string> queryParameters)
    {
        EnsureDefaultQueryParameters(queryParameters);
        StringBuilder queryParametersString = new();
        int parameterCounter = 0;
        foreach (var parameter in queryParameters)
        {
            string encodedKey = Uri.EscapeDataString(parameter.Key);
            string encodedValue = Uri.EscapeDataString(parameter.Value);
            queryParametersString.Append($"{encodedKey}={encodedValue}");

            if (parameterCounter < queryParameters.Count - 1)
            {
                queryParametersString.Append('&');
            }

            parameterCounter++;
        }
        return queryParametersString.ToString();
    }
    private static void EnsureDefaultQueryParameters(Dictionary<string, string> queryParameters)
    {
        if (!queryParameters.ContainsKey("where"))
        {
            queryParameters["where"] = "1=1";
        }
        if (!queryParameters.ContainsKey("outFields"))
        {
            queryParameters["outFields"] = "*";
        }
        if (!queryParameters.ContainsKey("returnGeometry"))
        {
            queryParameters["returnGeometry"] = "false";
        }
        if (!queryParameters.ContainsKey("f"))
        {
            queryParameters["f"] = "pjson";
        }
    }
    private async Task<string> ConnectToApiAndGetResponse(string urlAddressToConnect)
    {
        using (HttpClient client = new())
        {
            using (HttpResponseMessage response = await client.GetAsync(urlAddressToConnect))
            {
                if (response.IsSuccessStatusCode)
                {
                    if (response.Content.Headers.LastModified.HasValue)
                    {
                        _lastModifiedDate = response.Content.Headers.LastModified.Value.DateTime;
                    }
                    string content = await response.Content.ReadAsStringAsync();
                    return content;
                }
                else
                {
                    _logger.LogWarning("Failed to fetch data from external API");
                    return string.Empty;
                }
            }
        }
    }
    
    public List<County> ProcessExternalData(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            _logger.LogWarning("The json was invalid during the execution of ProcessExternaldata");
            return new List<County>();
        }
        List<County?>counties = new List<County?>(3500);
        JsonDocument jsonObject = JsonDocument.Parse(json);
        JsonElement featuresArray = jsonObject.RootElement.GetProperty("features");
        counties = featuresArray.EnumerateArray()
            .Select(ExtractAndProcessCountiesJson)
            .Where(county => county != null)
            .OrderBy(county => county?.State.StateName)
            .ThenBy(county => county?.CountyName)
            .ToList();

        return counties!;
    }
    private County? ExtractAndProcessCountiesJson(JsonElement feature)
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        JsonElement attributes = feature.GetProperty("attributes");
        ReadCountyDto? readCountyDto = attributes.Deserialize<ReadCountyDto>(options);

        if (readCountyDto == null)
        {
            return (County?)null;
        }

        readCountyDto.POPULATION ??= 0;
        readCountyDto.POP_SQMI ??= 0;

        County? county = _countyService.GetCountyFromReadDto(readCountyDto);
        if (county != null)
        {
            county.LastUpdated = _lastModifiedDate;
        }
        return county;
    }

    public async Task TryUpdateOrAddCountiesAndStates(List<County> currentCounties)
    {
        //This is used so we dont look up in the data base again and again for the same state -> like 50 counties which have alabama as a state for example
        HashSet<State> StatesWeHaveComeAcrossed = new HashSet<State>();

        HashSet<State> statesToAdd = [];
        HashSet<State> statesToUpdate = [];

        List<County> countiesToAdd = [];
        List<County> countiesToUpdate = [];
        State? state;
        foreach (County county in currentCounties)
        {
            //We have the fist if so we do not go through the database again and again -> like keeping cache or something like that? -> Now should be a lot faster because there are only 35 states and like 2000 counties given by the API as of date (24.01.2024)
            if (StatesWeHaveComeAcrossed.Any(st => st.StateName == county.State.StateName))
            {
                state = StatesWeHaveComeAcrossed.FirstOrDefault(st => st.StateName == county?.State?.StateName)!;
            }
            //Now we go through the normal path, where we have not come across the state
            else
            {
                state = await _stateService.GetStateAsync(s => s.StateName == county.State.StateName);

                if (state == null)
                {
                    state = new State
                    {
                        StateName = county.State.StateName,
                        StateFips = county.State.StateFips,
                        StateAbbreviation = county.State.StateAbbreviation,
                        LastUpdated = county.LastUpdated,
                    };
                    if (!statesToAdd.Any(state => state.StateName == county.State.StateName))
                    {
                        statesToAdd.Add(state);
                    }
                    else
                    {
                        state = statesToAdd.First(st => st.StateName == state.StateName);
                    }
                }
                else if (state.LastUpdated != county.LastUpdated)
                {
                    state.LastUpdated = county.LastUpdated;
                    statesToUpdate.Add(state);
                }
                StatesWeHaveComeAcrossed.Add(state);
            }

            county.State = state;

            County? countyFromDb = await _countyService.GetCountyAsync(c => c.CountyName == county.CountyName);

            if (countyFromDb == null)
            {
                countiesToAdd.Add(county);
            }
            else if (countyFromDb.LastUpdated != county.LastUpdated)
            {
                countyFromDb.Population = county.Population;
                countyFromDb.LastUpdated = county.LastUpdated;
                countiesToUpdate.Add(county);
            }
        }

        _stateService.AddStates(statesToAdd);
        _stateService.UpdateStates(statesToUpdate);

        _countyService.AddCounties(countiesToAdd);
        _countyService.UpdateCounties(countiesToUpdate);

    }
}
