using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Actions/Search")]
public class SearchAction : Action
{
    public override void Act(FiniteStateMachine fsm)
    {
        fsm.GetNavMeshAgent().Search();
    }
}