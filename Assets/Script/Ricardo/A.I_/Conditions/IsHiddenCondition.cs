using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Conditions/IsHidden")]
public class IsHiddenCondition : Condition
{
    [SerializeField] private bool negation;
    public override bool Test(FiniteStateMachine fsm)
    {
        if (fsm.GetNavMeshAgent().IsAtDestination())
        {
            if (!fsm.GetNavMeshAgent().DirectContactWithPlayer())
            {
                return !negation;
            }
        }
        return negation;
    }
}