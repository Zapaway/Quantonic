using UnityEngine;
using UnityEngine.UI;

namespace AddOns {
    /// <summary>
    /// Add this to define boundaries for buttons 
    /// (e.g. triangle buttons should only be able to be clickable within a triangle boundary)
    /// </summary>
    public class CustomButton : MonoBehaviour
    {
        private void Awake() {
            GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }
    }
}