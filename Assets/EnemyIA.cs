using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    public List<Transform> waypoints;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public Transform target;

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

    public float speed;
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
    protected LayerMask _playerMask;

    //public BaseState(EnemyIA enemy)
    //{
    //    _enemy = enemy; 
    //}

    public void SetUp(EnemyIA enemy)
    {
        _enemy = enemy;
        _playerMask = _enemy.playerMask;
    }

    public abstract void OnUpdate();
    public abstract void OnEnter();
    public abstract void OnExit();
}

public class Player { }

public class Walk : BaseState
{
    int wayPointCounter = 0;
    bool isWaiting;
    float waitingTime;
    float viewAngle = 50;
    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        Debug.Log("Caminando");
        #region Comentado Player cercano
        //var playerInRange = Physics.OverlapSphere(_enemy.transform.position, 5f, _playerMask)
        //    /*.OrderBy(x => Vector3.Distance(_enemy.transform.position, x.transform.position))
        //    .FirstOrDefault().gameObject.GetComponent<Player>()*/;
        //if (target == null && playerInRange.Any())
        //{
        //    target = playerInRange[0].transform;
        //    for (int i = playerInRange.Length > 1 ? 1 : 0; i < playerInRange.Length; i++)
        //    {
        //        target = i != 0 && Vector3.Distance(playerInRange[i].transform.position, _enemy.transform.position) <
        //             Vector3.Distance(target.transform.position, _enemy.transform.position) ?
        //             playerInRange[i].transform : target;
        //    }

        //    _enemy.ActualState = _enemy._posibleStates[EnemyIA.IAStates.CHASE];
        //}
        #endregion

        var posiblePlayer = Physics.OverlapSphere(_enemy.transform.position, 5, _playerMask);

        if (posiblePlayer.Length > 0)
        {
            var playerDir = (posiblePlayer[0].transform.position - _enemy.transform.position);
            if (Vector3.Angle(_enemy.transform.forward, playerDir) < viewAngle / 2 &&
                !Physics.Raycast(_enemy.transform.position,
                playerDir, playerDir.magnitude, _enemy.obstacleMask))
            {
                _enemy.target = posiblePlayer[0].transform;
                _enemy.ChangeState(EnemyIA.IAStates.CHASE);
                Debug.Log("Encontre");
                return;
            }
        }

        #region Patrol
        var actualDir = (_enemy.waypoints[wayPointCounter].position - _enemy.transform.position);
        actualDir.y = 0;

        _enemy.transform.forward =(_enemy.transform.forward * 0.99f + actualDir.normalized * 0.01f) ;

        if (isWaiting)
        {
            waitingTime -= Time.deltaTime;
            if(waitingTime<0)
                isWaiting = false;
            return;
        }

        _enemy.transform.position +=
            actualDir.normalized
            * Time.deltaTime * _enemy.speed;

        if (actualDir.magnitude < .2f)
        {
            wayPointCounter++;

            if (wayPointCounter >= _enemy.waypoints.Count)
            {
                wayPointCounter = 0;
            }
            isWaiting = true;
            waitingTime = 1;
        }
        #endregion
    }
}

public class Chase : BaseState
{
    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        Debug.Log("Persiguiendo");
        var dir = _enemy.target.transform.position - _enemy.transform.position;
        dir.y = 0;
        _enemy.transform.forward = (_enemy.transform.forward * 0.9f + dir * 0.1f);

        if (dir.magnitude < 1) return;

        _enemy.transform.position += _enemy.transform.forward * _enemy.speed * Time.deltaTime;
    }
}

public class Attack : BaseState
{
    public override void OnEnter()
    {;
    }

    public override void OnExit()
    {
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
    }

    public override void OnExit()
    {
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
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        Debug.Log("Muelto");
    }
}