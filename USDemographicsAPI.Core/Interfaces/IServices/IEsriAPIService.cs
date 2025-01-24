using System.Net.Http;
using System.Text.Json;
using System.Text;
using USDemographicsAPI.Core.DomainModels;

namespace USDemographicsAPI.Core.Interfaces.IServices;

public interface IEsriAPIService
{
    public Task<string> GetExternalDataAsync(Dictionary<string, string> queryParameters);
    public List<County> ProcessExternalData(string json);
    public Task TryUpdateOrAddCountiesAndStates(List<County> currentCounties);
}
