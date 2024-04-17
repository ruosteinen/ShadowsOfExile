using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Actions/Attack")]
public class AttackAction : Action
{
    public override void Act(FiniteStateMachine fsm)
    {
        if (fsm.GetNavMeshAgent().CheckEnemyType(FSMNavMeshAgent.EnemyType.Ranged))
        {
            fsm.GetNavMeshAgent().RangedAttack();
        }
        else if (fsm.GetNavMeshAgent().CheckEnemyType(FSMNavMeshAgent.EnemyType.Melee))
        {
            fsm.GetNavMeshAgent().MeleeAttack();
        }
        else if (fsm.GetNavMeshAgent().CheckEnemyType(FSMNavMeshAgent.EnemyType.Striker))
        {
            fsm.GetNavMeshAgent().StrikerAttack();
        }
    }
}
