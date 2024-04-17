using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Conditions/TakeDamage")]
public class TakeDamageCondition : Condition
{
    [SerializeField] private bool negation;

    public override bool Test(FiniteStateMachine fsm)
    {
        if (fsm.GetNavMeshAgent().CheckEnemyType(FSMNavMeshAgent.EnemyType.Striker))
        {
            return false;
        }
        else if (fsm.GetNavMeshAgent().tdcHealthHolder > fsm.GetNavMeshAgent().GetHealth())
        {
            return !negation;
        }
        else
        {
            return negation;
        }
    }
}