using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Actions/SetSeachDir")]
public class SetSeachDirAction : Action
{
    public override void Act(FiniteStateMachine fsm)
    {
        fsm.GetNavMeshAgent().SetSearchDir();
        fsm.GetNavMeshAgent().rotationComplete = false;
        fsm.GetNavMeshAgent().UpdateHealthHolder();
    }
}