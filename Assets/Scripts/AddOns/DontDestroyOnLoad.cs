using UnityEngine;

namespace AddOns {
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake() { 
            DontDestroyOnLoad(this);
        }
    }
}