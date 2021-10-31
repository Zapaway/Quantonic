using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UIScripts.QQV {
    /// <summary>
    /// Script for the arrow buttons in the QQV panel. 
    /// </summary>
    [RequireComponent(typeof(Button), typeof(AddOns.CustomButton))]
    internal sealed class QQVArrowButtonScript : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private QQVMoveOptions _qqvDirection; 
        [SerializeField] private Button _button;

        private void Awake() {
            _button = GetComponent<Button>();
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Left && _button.interactable) {
                QQVEvents.InvokeMoveExecuted(this, new QQVMoveExecEventArgs{Arrow = _qqvDirection});
            }
        }
    }
}