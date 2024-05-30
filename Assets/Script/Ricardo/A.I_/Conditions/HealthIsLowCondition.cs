using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Conditions/HealthIsLow")]
public class HealthIsLowCondition : Condition
{
    [SerializeField] private bool hasHealth;
    [SerializeField][Range(0, 100)] private float healthToCheck;

    public override bool Test(FiniteStateMachine fsm)
    {
        if (!fsm.GetNavMeshAgent().CheckEnemyType(FSMNavMeshAgent.EnemyType.Ranged) && fsm.GetCurrentState().name != "Recover State")
        {
            return false;
        }
        else if(fsm.GetNavMeshAgent().GetHealth() < ((healthToCheck / 100) * fsm.GetNavMeshAgent().GetMaxHealth()) && hasHealth == false)
        {
            return true;
        }
        else if(fsm.GetNavMeshAgent().GetHealth() > ((healthToCheck / 100) * fsm.GetNavMeshAgent().GetMaxHealth()) && hasHealth)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}