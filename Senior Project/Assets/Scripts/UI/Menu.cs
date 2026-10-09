using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI screen navigation manager. State machine + stack.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
[DisallowMultipleComponent]
public class Menu : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public CanvasGroup Group { get; private set; }
        
    /// <summary>
    /// Whether this panel will block inputs on the previous screen. Doesn't replace the previous panel.
    /// Think of popup UI.
    /// </summary>
    [field: SerializeField] public bool IsAdditive { get; private set; } = false;
        
    // [field: SerializeField] public bool StopGameplayInputs { get; private set; } = true;
    public static Selectable TargetSelectedObject { get; private set; }
    /// <summary>
    /// The first object to select when opening this panel.
    /// For controller navigation.
    /// </summary>
    [field: SerializeField] public Selectable DefaultSelected { get; private set; }

    [field: Header("Events")]
    [field: SerializeField] public UnityEvent OnFocused { get; private set; } = new();
    [field: SerializeField] public UnityEvent OnUnfocused { get; private set; } = new();
        
    public Menu PreviousPanel { get; private set; }
    /// <summary>
    /// Globally accessible reference to the current active panel.
    /// </summary>
    public static Menu CurrentActiveMenu { get; private set; }
    public static event Action<Menu> OnPanelChanged = delegate { };

    private void OnValidate()
    {
        Group = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Helper to hide the active panel.
    /// Good for temporarily hiding and showing again later.
    /// </summary>
    public static void HideActive()
    {
        if (CurrentActiveMenu != null)
            CurrentActiveMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// Helper to show the active panel.
    /// Good for temporarily hiding and showing again later.
    /// </summary>
    public static void ShowActive()
    {
        if (CurrentActiveMenu != null)
            CurrentActiveMenu.gameObject.SetActive(true);
    }

    public static void Focus(Menu panel)
    {
        if (CurrentActiveMenu != null)
            CurrentActiveMenu.FocusPanel(panel);
        else
            panel.Focus();
    }

    /// <summary>
    /// Repeatedly goes back until there is no previous panel.
    /// Returns back to the "root" UI.
    /// </summary>
    public static void GoBackToInitial()
    {
        while(CurrentActiveMenu?.PreviousPanel)
            CurrentActiveMenu.Back();
    }

    /// <summary>
    /// Closes everything and clears the stack.
    /// </summary>
    public static void CloseAll()
    {
        if (CurrentActiveMenu == null)
        {
            OnPanelChanged?.Invoke(null);
            return;
        }

        while(CurrentActiveMenu)
            CurrentActiveMenu.BackOrClose();
    }

    private void OnDestroy()
    {
        // Safely cleans up the ActivePanel static variable
        if (CurrentActiveMenu == this)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) 
                return;
#endif
            if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().isLoaded)
            {
                CurrentActiveMenu = null;
                OnPanelChanged?.Invoke(null);
                return;
            }
            Debug.LogWarning($"Active UIScreen {this} is being destroyed");
            Back();
        }
    }

    private void OnApplicationQuit()
    {
        OnPanelChanged = delegate { };
    }

    public void FocusPanel(Menu panel)
    {
        if (panel == this)
        {
            Focus();
            return;
        }

        panel.SetPreviousPanel(this);
        panel.Focus();
        if(!panel.IsAdditive)
            Unfocus();
        else
            Group.interactable = false;
    }

    public void Focus()
    {
        Group.interactable = true;
        gameObject.SetActive(true);
        CurrentActiveMenu = this;
        
        ChangeCurrentSelectedObject(DefaultSelected);

        OnFocused?.Invoke();
        OnPanelChanged?.Invoke(this);
    }

    public void Unfocus()
    {
        gameObject.SetActive(false);
        OnUnfocused?.Invoke();
    }

    /// <summary>
    /// Closes the current panel, but does not enable the previous screen.
    /// Useful for additive panels or other special cases.
    /// </summary>
    public void Close()
    {
        Unfocus();
        if (PreviousPanel)
        {
            CurrentActiveMenu = PreviousPanel;
            SetPreviousPanel(null);
        }
        else
        {
            CurrentActiveMenu = null;
        }
        OnPanelChanged?.Invoke(null);
    }

    /// <summary>
    /// Returns to the previous panel if it exists.
    /// </summary>
    public void Back()
    {
        if (PreviousPanel)
        {
            Unfocus();
            PreviousPanel.Focus();
            PreviousPanel = null;
        }
    }

    /// <summary>
    /// Safe exit method for the current panel.
    /// </summary>
    public void BackOrClose()
    {
        Unfocus();
        if (PreviousPanel)
        {
            Back();
        }
        else
        {
            CurrentActiveMenu = null;
            OnPanelChanged?.Invoke(null);
        }
    }

    /// <summary>
    /// Jumps directly to a panel, clearing navigation history.
    /// Useful for return to main menu type actions.
    /// </summary>
    public void GoBackTo(Menu panel)
    {
        FocusPanel(panel);
        SetPreviousPanel(null);
    }

    public void SetPreviousPanel(Menu previous) => PreviousPanel = previous;

    /// <summary>
    /// Changes the currently selected object in the UI.
    /// KeyboardMouse control scheme will always set the selected object to null.
    /// </summary>
    /// <param name="selectedObject"></param>
    public static void ChangeCurrentSelectedObject(Selectable selectedObject)
    {
        TargetSelectedObject = selectedObject;
        SetCurrentSelectedObject();
    }

    /// <summary>
    /// Helper method to set the currently selected object in the EventSystem based on the current control scheme.
    /// </summary>
    public static void SetCurrentSelectedObject()
    {
        if (InputManager.Instance.CurrentControlScheme == InputManager.ControlScheme.KBM)
            EventSystem.current.SetSelectedGameObject(null);
        else
        {
            if(TargetSelectedObject != null)
                EventSystem.current.SetSelectedGameObject(TargetSelectedObject.gameObject);
            else
                EventSystem.current.SetSelectedGameObject(null);
        }
    }

    /// <summary>
    /// Utility method to check if a UI object is interactable by raycasting against it to see if any other UI elements block it.
    /// Needed for gamepad interactions.
    /// </summary>
    public static bool IsUIObjectInteractable(EventSystem eventSystem, GameObject target)
    {
        if (target == null || !target.activeInHierarchy)
            return false;

        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null)
            return false;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
        var pointerData = new PointerEventData(eventSystem) { position = screenPoint };

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == target || result.gameObject.transform.IsChildOf(target.transform))
                return true;

            // Hit something else first
            return false;
        }

        return false;
    }
}