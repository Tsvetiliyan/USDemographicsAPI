using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Text.Json;
using USDemographicsAPI.Core.DomainModels;
using USDemographicsAPI.Core.Dtos;
using USDemographicsAPI.Core.Interfaces.IRepos;
using USDemographicsAPI.Core.Interfaces.IServices;

namespace USDemographicsAPI.Services;

public class CountyService : ICountyService
{
    private readonly ILogger<CountyService> _logger;
    private readonly IRepository<County> _countyRepository;
    public CountyService(ILogger<CountyService> logger, IRepository<County> countyRepository)
    {
        _logger = logger;
        _countyRepository = countyRepository;
    }

    public County? GetCountyFromReadDto(ReadCountyDto readCountyDto)
    {
        if (!ValidateCountyReadDto(readCountyDto))
        {
            _logger.LogWarning("Validation failed for ReadCountyDto: {@ReadCountyDto}", readCountyDto);
            return null;
        }
        return new County
        {
            CountyName = readCountyDto.NAME,
            CountyFips = readCountyDto.COUNTY_FIPS,
            Population = (readCountyDto.POPULATION as int?) ?? 0,
            PopulationPerSquareMile = (readCountyDto.POPULATION as int?) ?? 0,
            SquareMiles = readCountyDto.SQMI,
            ShapeArea = readCountyDto.Shape__Area,
            ShapeLength = readCountyDto.Shape__Length,
            State = new State
            {
                StateName = readCountyDto.STATE_NAME,
                StateFips = readCountyDto.STATE_FIPS,
                StateAbbreviation = readCountyDto.STATE_ABBR
            }
        };
    }
    public bool ValidateCountyReadDto(ReadCountyDto readCountyDto)
    {
        if (readCountyDto.OBJECTID <= 0)
        {
            return false;
        }
        else if (string.IsNullOrWhiteSpace(readCountyDto.NAME))
        {
            return false;
        }
        else if (string.IsNullOrWhiteSpace(readCountyDto.STATE_NAME))
        {
            return false;
        }
        else if (string.IsNullOrWhiteSpace(readCountyDto.STATE_ABBR) || readCountyDto.STATE_ABBR.Length != 2)
        {
            return false;
        }
        else if (string.IsNullOrWhiteSpace(readCountyDto.STATE_FIPS) || readCountyDto.STATE_FIPS.Length != 2)
        {
            return false;
        }
        else if (string.IsNullOrWhiteSpace(readCountyDto.COUNTY_FIPS) || readCountyDto.COUNTY_FIPS.Length != 3)
        {
            return false;
        }
        else if (string.IsNullOrWhiteSpace(readCountyDto.FIPS) || readCountyDto.FIPS.Length != 5)
        {
            return false;
        }
        else if (readCountyDto.POPULATION < 0)
        {
            return false;
        }
        else if (readCountyDto.POP_SQMI < 0)
        {
            return false;
        }
        else if (readCountyDto.SQMI < 0)
        {
            return false;
        }
        else if (readCountyDto.Shape__Area < 0)
        {
            return false;
        }
        else if (readCountyDto.Shape__Length < 0)
        {
            return false;
        }
        return true;
    }

    public async Task<County?> GetCountyAsync(Expression<Func<County, bool>> filter, string? includeProperties = null)
    {
        if (filter == null)
        {
            return null;
        }
        return await _countyRepository.GetAsync(filter, includeProperties);
    }

    public async Task<IEnumerable<County>> GetCountiesAsync(Expression<Func<County, bool>> filter, string? includeProperties = null)
    {
        if (filter == null)
        {
            return await GetAllCountiesAsync();
        }
        return await _countyRepository.GetRangeAsync(filter, includeProperties);
    }

    public async Task<IEnumerable<County>> GetAllCountiesAsync(string? includeProperties = null)
    {
        return await _countyRepository.GetAllAsync(includeProperties);
    }

    public async Task<bool> AnyCountyAsync(Expression<Func<County, bool>> filter)
    {
        if (filter == null)
        {
            return false;
        }
        return await _countyRepository.AnyAsync(filter);
    }

    public void AddCounty(County county)
    {
        if (county == null)
        {
            return;
        }
        _countyRepository.Add(county);
        _countyRepository.SaveChanges();
    }

    public void AddCounties(IEnumerable<County> counties)
    {
        if (counties.IsNullOrEmpty())
        {
            return;
        }
        _countyRepository.AddRange(counties);
        _countyRepository.SaveChanges();
    }

    public void RemoveCounty(County county)
    {
        if (county == null)
        {
            return;
        }
        _countyRepository.Remove(county);
        _countyRepository.SaveChanges();
    }

    public void RemoveCounties(IEnumerable<County> counties)
    {
        if (counties.IsNullOrEmpty())
        {
            return;
        }
        _countyRepository.RemoveRange(counties);
        _countyRepository.SaveChanges();
    }
    public void UpdateCounty(County county)
    {
        if (county == null)
        {
            return;
        }
        _countyRepository.Update(county);
        _countyRepository.SaveChanges();
    }
    public void UpdateCounties(IEnumerable<County> counties)
    {
        if (counties.IsNullOrEmpty())
        {
            return;
        }
        _countyRepository.UpdateRange(counties);
        _countyRepository.SaveChanges();
    }
}
