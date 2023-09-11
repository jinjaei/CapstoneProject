using UnityEngine;

/*
 * Base abstract class for managers.
 * A manager instance can be accessed from a static context by a child class with the instance property.
 * 
 */
namespace IV_Demo
{
    public class AManager<T> : MonoBehaviour where T: AManager<T>
    {
        // manager pattern
        public static bool exist => instance != null;

        static T _instance = null;
        protected static T instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();
                return _instance;
            }
        }
    }
}