using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private static Bootstrapper instance;

    private const string BootstrapPrefabFileName = "Bootstrap";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoSpawn()
    {
        if (instance != null) return;

        GameObject prefab = Resources.Load<GameObject>(BootstrapPrefabFileName);
        Instantiate(prefab);
    }
        
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}