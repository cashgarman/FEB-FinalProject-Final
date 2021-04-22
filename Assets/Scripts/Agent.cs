using System;
using TMPro;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public TMP_Text stateText;

    protected State currentState;

    public enum State
    {
        None,
        Idle,
        DetectingPlayer,
        ChasingPlayer,
        MoveToLastKnownPlayerPosition,
        LookingForPlayer,
        ReturningHome,
        GrabbingArtifact,
        ReturningArtifact,
    }

    protected void GotoState(State newState)
    {
        // If there is a current state
        if(currentState != State.None)
        {
            // Let the agent know the current state is being left
            OnStateLeft(currentState);
        }

        // Update the current state
        currentState = newState;

        // Let the new state know it was been entered
        OnStateEntered(currentState);
    }

    protected virtual void OnStateEntered(State state)
    {
        // Update the state text
        stateText.text = state.ToString();
    }

    protected virtual void OnStateLeft(State state)
    {
    }
}