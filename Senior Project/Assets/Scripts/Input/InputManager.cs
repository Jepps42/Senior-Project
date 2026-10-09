using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : Singleton<InputManager>
{
    public PlayerInputActions Actions { get; private set; }
    
    /// <summary>
    /// We are using C# generated inputs, so this is just a dummy playerInput to detect controlScheme changes
    /// </summary>
    [SerializeField, ReadOnly] private PlayerInput _playerInput;
        
    public enum ControlScheme
    {
        KBM,
        Gamepad
    }
    
    public ControlScheme CurrentControlScheme { get; private set; }
        
    /// <summary>
    /// Action that is invoked when the current control scheme is changed based on the input device used.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>ControlScheme controlScheme</c>: The new controlScheme that was changed to.</description></item>
    /// </list>
    /// </remarks>
    public event Action<ControlScheme> OnControlSchemeChanged = delegate { };
        
    /// <summary>
    /// The intended lock state.
    /// Controllers will always be locked, but when switching back to mouse, this will be the applied lock state
    /// </summary>
    private CursorLockMode _desiredCursorLockState;

    private void OnValidate()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    protected override void Awake()
    {
        base.Awake();
        
        _playerInput = GetComponent<PlayerInput>();
        
        Actions = new PlayerInputActions();
        _playerInput.onControlsChanged += PlayerInput_OnControlsChanged;
    }

    private void OnEnable()
    {
        if (Actions == null)
            return;

        Actions.Enable();
    }

    private void OnDisable()
    {
        if (Actions == null)
            return;
            
        Actions.Disable();
    }

    protected override void OnDestroy()
    {
        _playerInput.onControlsChanged -= PlayerInput_OnControlsChanged;
        Actions?.Dispose();
        
        base.OnDestroy();
    }
    
    /// <summary>
    /// Detects whether the control scheme has changed based on the input device used.
    /// </summary>
    /// <param name="input">The PlayerInput component that detected the control scheme change</param>
    private void PlayerInput_OnControlsChanged(PlayerInput input)
    {
        ControlScheme newScheme = ControlScheme.KBM;
        if (input.currentControlScheme == "Gamepad")
            newScheme = ControlScheme.Gamepad;

        if (newScheme != CurrentControlScheme)
        {
            CurrentControlScheme = newScheme;
            OnControlSchemeChanged.Invoke(CurrentControlScheme);
                
            ApplyCursorLockState();
        }
    }
        
    /// <summary>
    /// Locks or unlocks the cursor based on the user's preference.
    /// Keeps track of the desired cursor lock state, which is used when switching between control schemes.
    /// Gamepad always locks the cursor.
    /// </summary>
    /// <param name="locked"></param>
    public void LockCursor(bool locked)
    {
        // Always remember the user's preference
        _desiredCursorLockState = locked ? CursorLockMode.Locked : CursorLockMode.None;

        ApplyCursorLockState();
    }
        
    /// <summary>
    /// Helper method to apply the desired cursor lock state based on the current control scheme.
    /// </summary>
    private void ApplyCursorLockState()
    {
        if (CurrentControlScheme == ControlScheme.Gamepad)
        {
            // Gamepad always locks
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        Cursor.lockState = _desiredCursorLockState;
        Cursor.visible = _desiredCursorLockState != CursorLockMode.Locked;
    }
}