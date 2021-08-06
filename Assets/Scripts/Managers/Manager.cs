using UnityEngine;

namespace Managers {
    /// <summary>
    /// Base class that all managers derive from, using CRTP design.
    /// As a warning, please use the same subclass type when deriving.
    /// Otherwise, you may find different types of managers deleting each other.
    /// </summary>
    public abstract class Manager<T> : MonoBehaviour where T : Manager<T>, new()
    {
        #region Fields/Properties
        public static T Instance {get; private set;}

        protected bool _wontDestroyOnLoad;
        public bool WontDestroyOnLoad => _wontDestroyOnLoad;
        #endregion Fields/Properties
        
        protected virtual void Awake() {
            // ensure that this is a singleton
            if (Instance == null) {
                Instance = (T)this;

                var ddolScript = GetComponent<AddOns.DontDestroyOnLoad>();
                _wontDestroyOnLoad = ddolScript != null;
            }
            else {
                Destroy(gameObject);
            }
        }
    }
}
