using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyIA;

public class FSM<T, J> where J : MonoBehaviour
{
    public Dictionary<T, BaseState<T, J>> _posibleStates { private set; get; }

    private BaseState<T, J> _actualState;

    public FSM()
    {
        _posibleStates = new Dictionary<T, BaseState<T, J>>();
    }

    public FSM<T, J> AddState(T stateKey, BaseState<T, J> newState)
    {
        _posibleStates.Add(stateKey, newState);
        return this;
    }

    public void Done(J avatarParam)
    {
        foreach (var state in _posibleStates.Values)
        {
            state.SetUp(avatarParam, this);
        }
    }

    public void OnUpdate()
    {
        _actualState?.OnUpdate();
    }

    public void ChangeState(T targetState)
    {
        _actualState?.OnExit();
        _actualState = _posibleStates[targetState];
        _actualState.OnEnter();
    }
}
