using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using USDemographicsAPI.Core.DomainModels;
using USDemographicsAPI.Core.Dtos;

namespace USDemographicsAPI.Core.Interfaces.IServices
{
    public interface ICountyService
    {
        County? GetCountyFromReadDto(ReadCountyDto readCountyDto);

        bool ValidateCountyReadDto(ReadCountyDto readCountyDto);

        public Task<County?> GetCountyAsync(Expression<Func<County, bool>> filter, string? includeProperties = null);

        public Task<IEnumerable<County>> GetCountiesAsync(Expression<Func<County, bool>> filter, string? includeProperties = null);

        public Task<IEnumerable<County>> GetAllCountiesAsync(string? includeProperties = null);

        public Task<bool> AnyCountyAsync(Expression<Func<County, bool>> filter);

        public void AddCounty(County county);

        public void AddCounties(IEnumerable<County> counties);

        public void RemoveCounty(County county);

        public void RemoveCounties(IEnumerable<County> counties);

        public void UpdateCounty(County county);
        public void UpdateCounties(IEnumerable<County> counties);

    }
}
