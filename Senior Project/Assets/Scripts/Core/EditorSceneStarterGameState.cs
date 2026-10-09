using UnityEngine;

/// <summary>
/// So that we can start in any scene and our game manager knows what state to use
/// </summary>
public class EditorSceneStarterGameState : MonoBehaviour
{
    [SerializeField] private GameManager.State _startingState;

    private void Awake()
    {
#if UNITY_EDITOR
        GameManager.Instance.ChangeState(_startingState);
#endif
    }
}