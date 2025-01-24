using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using USDemographicsAPI.Core.DomainModels;

namespace USDemographicsAPI.Core.Interfaces.IServices;

public interface IStateService
{
    public Task<Dictionary<State, int>> GetSelectedStatePop(string stateName);

    public Task<Dictionary<State, int>> GetAllStatesPop();

    public Task<State?> GetStateAsync(Expression<Func<State, bool>> filter, string? includeProperties = null);

    public Task<IEnumerable<State>> GetStatesAsync(Expression<Func<State, bool>> filter, string? includeProperties = null);

    public Task<IEnumerable<State>> GetAllStatesAsync(string? includeProperties = null);

    public Task<bool> AnyStateAsync(Expression<Func<State, bool>> filter);

    public void AddState(State state);

    public void AddStates(IEnumerable<State> states);

    public void RemoveState(State state);

    public void RemoveStates(IEnumerable<State> states);
    public void UpdateState(State state);
    public void UpdateStates(IEnumerable<State> states);

}
