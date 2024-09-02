using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : BaseState<EnemyIA.IAStates, EnemyIA>
{
    public override void OnEnter()
    {
        ;
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        Debug.Log("Atacando");
    }
}