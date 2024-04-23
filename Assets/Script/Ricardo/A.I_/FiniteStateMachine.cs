using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
            Debug.LogError("FSMNavMeshAgent component not found!");
            // Handle the absence of the FSMNavMeshAgent component
        }
    }

    private void Update()
    {
        Transition triggeredTrasition = null;
        foreach(Transition t in currentState.getTransitions())
        {
            if (t.IsTriggered(this))
            {
                triggeredTrasition = t;
                break;
            }
        }
        List<Action> actions = new List<Action>();
        if(triggeredTrasition != null)
        {
            actions.Add(currentState.getExitAction());
            actions.Add(triggeredTrasition.GetAction());
            actions.Add(triggeredTrasition.GetTargetState().getEntryAction());
            currentState = triggeredTrasition.GetTargetState();
        }
        else
        {
            foreach(Action action in currentState.getStateActions())
            { 
                actions.Add(action); 
            }
        }
        DoActions(actions);
    }

    void DoActions(List<Action> actions)
    {
        foreach(Action action in actions)
        {
            if(action != null)
            {
                action.Act(this);
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
