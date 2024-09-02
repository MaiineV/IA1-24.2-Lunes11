using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    public List<Transform> waypoints;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public Transform target;


    public FSM<IAStates, EnemyIA> fsm;

    public enum IAStates
    {
        WALK,
        SEARCH,
        CHASE,
        ATTACK,
        DEATH
    }


    public float speed;

    void Awake()
    {
        fsm = new FSM<IAStates, EnemyIA>()
            .AddState(IAStates.WALK, new Walk())
            .AddState(IAStates.SEARCH, new Search())
            .AddState(IAStates.CHASE, new Chase())
            .AddState(IAStates.ATTACK, new Attack())
            .AddState(IAStates.DEATH, new Death());

        fsm.ChangeState(IAStates.WALK);
    }

    void Update()
    {
       fsm.OnUpdate();
    }

   
}