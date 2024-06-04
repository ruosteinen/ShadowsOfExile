using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Conditions/HealthCondition")]
public class HealthCondition : Condition
{
    [SerializeField] private float healthThreshold = 0.5f; 

    public override bool Test(FiniteStateMachine fsm)
    {
        FSMNavMeshAgent agent = fsm.GetNavMeshAgent();
        return agent.GetHealth() <= agent.GetMaxHealth() * healthThreshold;
    }
}