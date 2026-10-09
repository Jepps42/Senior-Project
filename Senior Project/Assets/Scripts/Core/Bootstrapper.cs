using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private static Bootstrapper _instance;

    private const string BootstrapPrefabFileName = "Bootstrap";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoSpawn()
    {
        if (_instance != null) return;

        GameObject prefab = Resources.Load<GameObject>(BootstrapPrefabFileName);
        Instantiate(prefab);
    }
        
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}