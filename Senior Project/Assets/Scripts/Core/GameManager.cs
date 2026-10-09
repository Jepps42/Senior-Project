using System;
using NaughtyAttributes;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum State
    {
        None,
        MainMenu,
        Gameplay,
        Pause
    }

    [field: SerializeField, ReadOnly] public State CurrentState { get; private set; }
    [field: SerializeField, ReadOnly] public State PreviousState { get; private set; } = State.None;
    public event Action<State> OnStateChanged = delegate { };

    protected override void Awake()
    {
        base.Awake();
        
        ChangeState(State.MainMenu, true);
    }

    public void ChangeState(State newState, bool forceChange = false)
    {
        if (CurrentState == newState && !forceChange)
            return;

        PreviousState = CurrentState;
        OnStateExit(CurrentState);
        
        CurrentState = newState;
        
        OnStateEnter(CurrentState);
        
        OnStateChanged.Invoke(newState);
    }

    private void OnStateEnter(State state)
    {
        switch (state)
        {
            case State.MainMenu:
                break;
        }
    }

    private void OnStateExit(State state)
    {
        switch (state)
        {
            case State.MainMenu:
                break;
        }
    }
}