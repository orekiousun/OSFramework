using UnityEngine;

/// <summary>
/// Mono单例模式
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static bool isNotDestory = true;
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).ToString());
                    _instance = go.AddComponent<T>();
                    if (_instance != null)
                    {
                        (_instance as MonoSingleton<T>).Init();
                    }
                }

                if (Application.isPlaying && isNotDestory)
                {
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            
            return _instance;
        }
    }
    
    public virtual void Init()
    {

    }
    
    public static void Release()
    {
        if (_instance != null)
        {
            _instance = (T)((object)null);
        }
    }

    public virtual void Dispose()
    {
        
    }
}

