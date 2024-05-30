using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Actions/Recover")]
public class RecoverAction : Action
{
    public override void Act(FiniteStateMachine fsm)
    {
        fsm.GetNavMeshAgent().Recover();
    }
}