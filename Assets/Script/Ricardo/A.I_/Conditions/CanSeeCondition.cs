using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Conditions/CanSeeCondition")]
public class CanSeeCondition : Condition
{
    [SerializeField] private bool negation;
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewDistance;

    public override bool Test(FiniteStateMachine fsm)
    {
        if (fsm.GetCurrentState().name == "Patrol State" || fsm.GetCurrentState().name == "Chase State" || fsm.GetCurrentState().name == "Attack State")
        {
            return CanSee(fsm);
        }

        return negation;
    }

    private bool CanSee(FiniteStateMachine fsm)
    {
        float vd = viewDistance;
        if (!fsm.GetNavMeshAgent().CheckEnemyType(FSMNavMeshAgent.EnemyType.Melee) && (fsm.GetCurrentState().name == "Attack State" || fsm.GetCurrentState().name == "Chase State"))
        {
            vd = 5 * viewDistance;
        }

        Vector3 direction = fsm.GetNavMeshAgent().target.position - fsm.GetNavMeshAgent().transform.position;
        if (direction.magnitude < vd)
        {
            float angle = Vector3.Angle(direction.normalized, fsm.GetNavMeshAgent().transform.forward);
            if (angle < viewAngle)
            {
                if (fsm.GetNavMeshAgent().DirectContactWithPlayer())
                {
                    return !negation;
                }
            }
        }
        return negation;
    }
}
