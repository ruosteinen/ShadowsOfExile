using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Conditions/SearchCompleted")]
public class SearchCompletedCondition : Condition
{
    public override bool Test(FiniteStateMachine fsm)
    {
        if (fsm.GetNavMeshAgent().rotationComplete)
        {
            fsm.GetNavMeshAgent().rotationComplete = false;
            fsm.GetNavMeshAgent().rotationNum = 0;
            return true;
        }
        else
        {
            return false;
        }
    }
}