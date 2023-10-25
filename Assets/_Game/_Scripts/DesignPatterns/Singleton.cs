using UnityEngine;

namespace _Game.DesignPattern
{
    public class Singleton<T> : HMonoBehaviour where T : HMonoBehaviour
    {
        private static T _instance;

        public static T Ins { 
            get
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<T>();
                }
                return _instance; 
            } 
        }
    }
}
