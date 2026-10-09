using UnityEngine;

public class GameManagerActions : MonoBehaviour
{
    public void StartGameFromMainMenu()
    {
        GameManager.Instance.StartGameFromMainMenu();
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    public void QuitGame()
    {
        GameManager.Quit();
    }
}