public class ChaseAction : Action
{
    public float meleeStoppingDistance = 2f; // Adjust as needed
    public float rangedStoppingDistance = 10f; // Adjust as needed

    public override void Act(FiniteStateMachine fsm)
    {
        FSMNavMeshAgent navMeshAgent = fsm.GetNavMeshAgent();
        if (navMeshAgent.CheckEnemyType(FSMNavMeshAgent.EnemyType.Melee))
        {
            navMeshAgent.GoToTarget(meleeStoppingDistance);
        }
        else if (navMeshAgent.CheckEnemyType(FSMNavMeshAgent.EnemyType.Ranged))
        {
            navMeshAgent.GoToTarget(rangedStoppingDistance);
        }
    }
}
