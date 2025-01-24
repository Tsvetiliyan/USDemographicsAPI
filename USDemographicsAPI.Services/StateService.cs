using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using USDemographicsAPI.Core.DomainModels;
using USDemographicsAPI.Core.Interfaces.IRepos;
using USDemographicsAPI.Core.Interfaces.IServices;

namespace USDemographicsAPI.Services;

public class StateService : IStateService
{
    private readonly IRepository<State> _stateRepository;
    private readonly IRepository<County> _countyRepository;

    public StateService(IRepository<State> stateRepository, IRepository<County> countyRepository)
    {
        _stateRepository = stateRepository;
        _countyRepository = countyRepository;
    }

    public async Task<Dictionary<State, int>> GetSelectedStatePop(string stateName)
    {
        State? state = await _stateRepository.GetAsync(x => x.StateName.ToLower() == stateName.ToLower());
        if (state == null) 
        {
            return new Dictionary<State, int>();
        }
        IEnumerable<County>? counties = await _countyRepository.GetRangeAsync(x => x.StateId == state.Id);
        if (counties == null)
        {
            return new Dictionary<State, int>();
        }

        Dictionary<State, int> statesPop = GoThroughCountiesPopAndAddToState(counties);
        return statesPop;
    }

    public async Task<Dictionary<State, int>> GetAllStatesPop()
    {
        IEnumerable<County> counties = await _countyRepository.GetAllAsync("State");

        Dictionary<State, int> statesPop = GoThroughCountiesPopAndAddToState(counties);

        return statesPop;
    }

    private static Dictionary<State, int> GoThroughCountiesPopAndAddToState(IEnumerable<County> counties)
    {
        Dictionary<State, int> statesPop = [];
        foreach (var county in counties)
        {
            if (!statesPop.ContainsKey(county.State))
            {
                statesPop.Add(county.State, county.Population);
                continue;
            }
            statesPop[county.State] += county.Population;
        }
        return statesPop;
    }

    public async Task<State?> GetStateAsync(Expression<Func<State, bool>> filter, string? includeProperties = null)
    {
        if (filter == null)
        {
            return null;
        }
        return await _stateRepository.GetAsync(filter, includeProperties);
    }

    public async Task<IEnumerable<State>> GetStatesAsync(Expression<Func<State, bool>> filter, string? includeProperties = null)
    {
        if (filter == null)
        {
            return await GetAllStatesAsync(includeProperties);
        }
        return await _stateRepository.GetRangeAsync(filter, includeProperties);
    }

    public async Task<IEnumerable<State>> GetAllStatesAsync(string? includeProperties = null)
    {
        return await _stateRepository.GetAllAsync(includeProperties);
    }

    public async Task<bool> AnyStateAsync(Expression<Func<State, bool>> filter)
    {
        if (filter == null)
        {
            return false;
        }
        return await _stateRepository.AnyAsync(filter);
    }

    public void AddState(State state)
    {
        if (state == null)
        {
            return;
        }
        _stateRepository.Add(state);
        _stateRepository.SaveChanges();
    }

    public void AddStates(IEnumerable<State> states)
    {
        if (states.IsNullOrEmpty())
        {
            return;
        }
        _stateRepository.AddRange(states);
        _stateRepository.SaveChanges();
    }

    public void RemoveState(State state)
    {
        if (state == null)
        {
            return;
        }
        _stateRepository.Remove(state);
        _stateRepository.SaveChanges();
    }

    public void RemoveStates(IEnumerable<State> states)
    {
        if (states.IsNullOrEmpty())
        {
            return;
        }
        _stateRepository.RemoveRange(states);
        _stateRepository.SaveChanges();
    }

    public void UpdateState(State state)
    {
        if (state == null)
        {
            return;
        }
        _stateRepository.Update(state);
        _stateRepository.SaveChanges();
    }

    public void UpdateStates(IEnumerable<State> states)
    {
        if (states.IsNullOrEmpty())
        {
            return;
        }
        _stateRepository.UpdateRange(states);
        _stateRepository.SaveChanges();
    }
}
