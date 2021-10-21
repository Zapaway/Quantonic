using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIScripts {
    public class Panel : MonoBehaviour
    {
        /// <summary>
        /// Get the activity of the panel.
        /// </summary>
        public bool GetPanelActive() {
            return gameObject.activeSelf;
        }

        /// <summary>
        /// Set the panel unactive or active.
        /// </summary>
        public void SetPanelActive(bool isActive) {
            gameObject.SetActive(isActive);
        }
    }
}

