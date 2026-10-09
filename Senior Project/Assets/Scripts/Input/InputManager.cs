using System;

public class InputManager : Singleton<InputManager>
{
    public PlayerInputActions Actions { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        Actions = new PlayerInputActions();
        Actions.Enable();
    }
}