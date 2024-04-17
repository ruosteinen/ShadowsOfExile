using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Actions/Hide")]
public class HideAction : Action
{
    public override void Act(FiniteStateMachine fsm)
    {
        if (fsm.GetNavMeshAgent().IsAtDestination() && fsm.GetNavMeshAgent().DirectContactWithPlayer())
        {
            fsm.GetNavMeshAgent().RunAway();
        }
    }
}