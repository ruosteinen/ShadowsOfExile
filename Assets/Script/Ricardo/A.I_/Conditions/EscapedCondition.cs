using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Conditions/EscapedCondition")]
public class EscapedCondition : Condition
{
    [SerializeField] private bool negation;
    [SerializeField] private float viewDistance;
    public override bool Test(FiniteStateMachine fsm)
    {
        float distance = Vector3.Distance(fsm.GetNavMeshAgent().target.position, fsm.GetNavMeshAgent().transform.position);
        if (distance > viewDistance)
        {
            if (fsm.GetNavMeshAgent().IsAtDestination())
            {
                return !negation;
            }
        }

        return negation;
    }
}