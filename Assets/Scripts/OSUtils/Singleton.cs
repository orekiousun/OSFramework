using System;
using UnityEngine;

/// <summary>
/// 单例模式
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> where T : class, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Activator.CreateInstance<T>();
                if (_instance != null)
                {
                    (_instance as Singleton<T>).Init();
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
