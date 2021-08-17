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

        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Left) {
                QQVEvents.InvokeMoveExecuted(this, new QQVMoveExecEventArgs{Arrow = _qqvDirection});
            }
        }
    }
}