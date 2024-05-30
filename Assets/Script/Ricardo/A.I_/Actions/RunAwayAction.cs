using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Actions/RunAway")]
public class RunAwayAction : Action
{
    public override void Act(FiniteStateMachine fsm)
    {
        fsm.GetNavMeshAgent().RunAway();
    }
}