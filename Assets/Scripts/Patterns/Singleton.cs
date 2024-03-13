using UnityEngine;


namespace BOW.Patterns
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T instance;

        public bool isPersistant;

        private void Awake()
        {
            if (isPersistant)
            {
                if (instance)
                {
                    Destroy(gameObject);
                }
                else
                {
                    instance = this as T;
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                instance = this as T;
            }
        }
    }
}