using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Attach this component to the Menu that is the scene's default menu
/// </summary>
[RequireComponent(typeof(Menu))]
public class DefaultSceneMenuSetter : MonoBehaviour
{
    private Menu _panel;

    private void Awake()
    {
        _panel = GetComponent<Menu>();
        
        _panel.SetPreviousPanel(null);
        
        if (Menu.CurrentActiveMenu == null)
            _panel.Focus();
        else
            Menu.CurrentActiveMenu.SetPreviousPanel(_panel);
    }
}