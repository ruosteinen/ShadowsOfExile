using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Conditions/CanHide")]
public class CanHideCondition : Condition
{
    [SerializeField] private bool negation;

    public override bool Test(FiniteStateMachine fsm)
    {
        if (!fsm.GetNavMeshAgent().CheckEnemyType(FSMNavMeshAgent.EnemyType.Ranged))
        {
            return negation;
        }
        else if (fsm.GetNavMeshAgent().CanHide())
        {
            fsm.GetNavMeshAgent().ResetShootCounter();
            return !negation;
        }
        else
        {
            return negation;
        }
    }
}