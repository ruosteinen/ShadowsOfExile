using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
    private FSMNavMeshAgent navMeshAgent;
    public State initialState;
    private State currentState;

    private void Start()
    {
        currentState = initialState;
        navMeshAgent = GetComponent<FSMNavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogWarning("FSMNavMeshAgent component not found! Adding it now...");
            navMeshAgent = gameObject.AddComponent<FSMNavMeshAgent>();
        }
    }

    private void Update()
    {
        Transition triggeredTransition = null;
        foreach (Transition t in currentState.getTransitions())
        {
            if (t.IsTriggered(this))
            {
                triggeredTransition = t;
                break;
            }
        }
        List<Action> actions = new List<Action>();
        if (triggeredTransition != null)
        {
            actions.Add(currentState.getExitAction());
            actions.Add(triggeredTransition.GetAction());
            actions.Add(triggeredTransition.GetTargetState().getEntryAction());
            currentState = triggeredTransition.GetTargetState();
        }
        else
        {
            foreach (Action action in currentState.getStateActions())
            {
                actions.Add(action);
            }
        }

        DoActions(actions);
        //Debug.Log("Current State: " + currentState.name);
    }

    void DoActions(List<Action> actions)
    {
        foreach (Action action in actions)
        {
            if (action != null)
            {
                action.Act(this);
                //Debug.Log("Action: " + action.GetType().Name);
            }
        }
    }

    public FSMNavMeshAgent GetNavMeshAgent()
    {
        return navMeshAgent;
    }

    public State GetCurrentState()
    {
        return currentState;
    }
}
