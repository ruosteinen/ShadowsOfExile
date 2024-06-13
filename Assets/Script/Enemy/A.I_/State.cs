using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Finite State Machine/State")]
public class State : ScriptableObject
{
    [SerializeField] private Action entryAction;
    [SerializeField] private Action[] stateActions;
    [SerializeField] private Action exitAction;
    [SerializeField] private Transition[] transitions;

    public Action getEntryAction()
    {
        return entryAction;
    }

    public Action[] getStateActions()
    {
        return stateActions;
    }

    public Action getExitAction()
    {
        return exitAction;
    }

    public Transition[] getTransitions()
    {
        return transitions;
    }
}
