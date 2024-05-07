using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/Transitions")]
public class Transition : ScriptableObject
{
    [SerializeField] private Condition decision;
    [SerializeField] private Action action;
    [SerializeField] private State targetState;

    public bool IsTriggered(FiniteStateMachine fsm)
    {
        // Debugging statement to log the name of the current state
        //Debug.Log("Current State: " + fsm.GetCurrentState().name);

        // Debugging statement to log the decision being tested
        //Debug.Log("Testing decision: " + decision.GetType().Name);

        // Debugging statement to log the result of the decision test
        bool result = decision.Test(fsm);
        //Debug.Log("Decision result: " + result);

        return result;
    }

    public Action GetAction() 
    {
        return action;
    }

    public State GetTargetState()
    {
        return targetState;
    }
}
