using NaughtyAttributes;
using UnityEngine;

public class GameStateChanger : MonoBehaviour
{
    [SerializeField] private bool _specifyState = true;
    [SerializeField, ShowIf("_specifyState")] private GameManager.State _targetState;

    public void ChangeState()
    {
        GameManager.Instance.ChangeState(_targetState);
    }

    public void ReturnToPreviousState()
    {
        GameManager.Instance.RevertToPreviousState();
    }
}