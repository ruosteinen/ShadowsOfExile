using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Conditions/CanSeeCondition")]
public class CanSeeCondition : Condition
{
    [SerializeField] private bool negation;
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewDistance;

    public override bool Test(FiniteStateMachine fsm)
    {
        // Check if the current state is one where visibility matters
        if (fsm.GetCurrentState().name == "Patrol State" || fsm.GetCurrentState().name == "Chase State" || fsm.GetCurrentState().name == "Attack State")
        {
            // Check if the enemy can see the player
            return CanSee(fsm);
        }

        // If not in a relevant state, return the negation value
        return negation;
    }

    private bool CanSee(FiniteStateMachine fsm)
    {
        // Adjust view distance for different enemy types or states if needed
        float vd = viewDistance;

        // If the enemy is not of type "Melee", adjust the view distance
        if (!fsm.GetNavMeshAgent().CheckEnemyType(FSMNavMeshAgent.EnemyType.Melee) && (fsm.GetCurrentState().name == "Attack State" || fsm.GetCurrentState().name == "Chase State"))
        {
            vd = 5 * viewDistance;
        }

        // Calculate direction to the player
        Vector3 direction = fsm.GetNavMeshAgent().target.position - fsm.GetNavMeshAgent().transform.position;

        // Check if the player is within the adjusted view distance and angle
        if (direction.magnitude < vd)
        {
            float angle = Vector3.Angle(direction.normalized, fsm.GetNavMeshAgent().transform.forward);
            if (angle < viewAngle)
            {
                // Check if there is direct contact with the player
                if (fsm.GetNavMeshAgent().DirectContactWithPlayer())
                {
                    return !negation;
                }
            }
        }

        // Return the negation value if the player is not visible
        return negation;
    }
}
