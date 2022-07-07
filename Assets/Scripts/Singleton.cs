using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool m_isShuttingDown = false;
    private static object m_lock = new object();
    private static T m_instance;

    private void Awake()
    {
        m_isShuttingDown = false;
    }

    public static T Instance
    {
        get
        {
            if (m_isShuttingDown)
            {
                return null;
            }

            lock (m_lock)
            {
                if (m_instance == null)
                {
                    m_instance = (T)FindObjectOfType(typeof(T));

                    if (m_instance == null)
                    {
                        GameObject _singletonObject = new GameObject();
                        _singletonObject.name = typeof(T).Name;
                        m_instance = _singletonObject.AddComponent<T>();

                        DontDestroyOnLoad(_singletonObject);
                    }
                }

                return m_instance;
            }
        }
    }

    private void OnApplicationQuit()
    {
        m_isShuttingDown = true;
    }

    private void OnDestroy()
    {
        m_isShuttingDown = true;
    }
}
