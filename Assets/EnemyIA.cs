using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    public enum IAStates
    {
        WALK,
        SEARCH,
        CHASE,
        ATTACK,
        DEATH
    }

    public Dictionary<IAStates, BaseState> _posibleStates { private set; get; }

    private BaseState _actualState;
    public BaseState ActualState
    {
        get => _actualState;
        set
        {
            if (value != _actualState) _actualState = value;
        }
    }

    void Awake()
    {
        _posibleStates = new Dictionary<IAStates, BaseState>();

        _posibleStates.Add(IAStates.WALK, new Walk());
        _posibleStates.Add(IAStates.SEARCH, new Search());
        _posibleStates.Add(IAStates.CHASE, new Chase());
        _posibleStates.Add(IAStates.ATTACK, new Attack());
        _posibleStates.Add(IAStates.DEATH, new Death());

        foreach (var state in _posibleStates.Values) { state.SetUp(this); }

        _actualState = _posibleStates[IAStates.WALK];
    }

    void Update()
    {
        _actualState.OnUpdate();
    }

    public void ChangeState(IAStates targetState)
    {
        _actualState?.OnExit();
        _actualState = _posibleStates[targetState];
        _actualState.OnEnter();
    }
}


public abstract class BaseState
{
    protected EnemyIA _enemy;
    protected LayerMask _playerMask = 1 << 0 & 1 << 4;

    protected Transform target;

    //public BaseState(EnemyIA enemy)
    //{
    //    _enemy = enemy; 
    //}

    public void SetUp(EnemyIA enemy)
    {
        _enemy = enemy;
    }

    public abstract void OnUpdate();
    public abstract void OnEnter();
    public abstract void OnExit();
}

public class Player { }

public class Walk : BaseState
{
    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    //public Walk(EnemyIA enemy) : base(enemy)
    //{
    //}

    public override void OnUpdate()
    {
        Debug.Log("Caminando");
        var playerInRange = Physics.OverlapSphere(_enemy.transform.position, 5f, _playerMask)
            /*.OrderBy(x => Vector3.Distance(_enemy.transform.position, x.transform.position))
            .FirstOrDefault().gameObject.GetComponent<Player>()*/;
        if (target == null && playerInRange.Any())
        {
            target = playerInRange[0].transform;
            for (int i = playerInRange.Length > 1 ? 1 : 0; i < playerInRange.Length; i++)
            {
                target = i != 0 && Vector3.Distance(playerInRange[i].transform.position, _enemy.transform.position) <
                     Vector3.Distance(target.transform.position, _enemy.transform.position) ?
                     playerInRange[i].transform : target;
            }

            _enemy.ActualState = _enemy._posibleStates[EnemyIA.IAStates.CHASE];
        }
    }
}

public class Chase : BaseState
{
    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        Debug.Log("Persiguiendo");
        var dir = _enemy.transform.position - target.transform.position;
        dir.y = 0;
        _enemy.transform.forward = (_enemy.transform.forward * 0.9f + dir * 0.1f);
        _enemy.transform.position += _enemy.transform.forward * Time.deltaTime;
    }
}

public class Attack : BaseState
{
    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        Debug.Log("Atacando");
    }
}

public class Search : BaseState
{
    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        Debug.Log("Buscando");
    }
}

public class Death : BaseState
{
    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        Debug.Log("Muelto");
    }
}