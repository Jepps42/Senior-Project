using System;
using Eflatun.SceneReference;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    [field: Header("Scenes")]
    [field: SerializeField] public SceneReference MainMenuScene { get; private set; }
    [field: SerializeField] public SceneReference GameplayScene { get; private set; }

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

    public void RevertToPreviousState()
    {
        if (PreviousState == State.None)
            return;
        
        ChangeState(PreviousState);
    }

    public void StartGameFromMainMenu()
    {
        if (GetCurrentScene().name != MainMenuScene.Name)
            return;
        
        SceneManager.LoadScene(GameplayScene.Name);
        ChangeState(State.Gameplay);
    }

    public void ReturnToMainMenu()
    {
        if (GetCurrentScene().name == MainMenuScene.Name)
            return;
        
        SceneManager.LoadScene(MainMenuScene.Name);
        ChangeState(State.MainMenu);
    }

    public static Scene GetCurrentScene()
    {
        return SceneManager.GetActiveScene();
    }

    public static void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}