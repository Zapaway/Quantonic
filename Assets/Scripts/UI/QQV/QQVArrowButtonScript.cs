using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIScripts.QQV {
    /// <summary>
    /// The arrows available in the QQV panel.
    /// </summary>
    public enum QQVArrowButtons {
        Left = MoveDirection.Left,
        Right = MoveDirection.Right
    }
    /// <summary>
    /// Event args for the event in QQVArrowButtonScript.
    /// </summary>
    public sealed class QQVArrowButtonEventArgs : EventArgs {
        public QQVArrowButtons Arrow {get; set;}
    }

    /// <summary>
    /// Script for the arrow buttons in the QQV panel. 
    /// </summary>
    [RequireComponent(typeof(AddOns.CustomButton))]
    public sealed class QQVArrowButtonScript : MonoBehaviour, IMoveHandler
    {
        [SerializeField] private MoveDirection _moveDirection; 
        public static event EventHandler<QQVArrowButtonEventArgs> ArrowExecuted; 

        public void OnMove(AxisEventData eventData) {
            MoveDirection eventDir = eventData.moveDir;
            
            // only invokes the event IF on the arrow button a certain key is pressed
            // e.g. if on LEFT arrow button, only invoke the event that the LEFT key is pressed
            if (eventDir == _moveDirection) {
                switch (eventDir) {
                    case MoveDirection.Left:
                    case MoveDirection.Right:
                        var eventArgs = new QQVArrowButtonEventArgs{Arrow = (QQVArrowButtons)eventDir};
                        ArrowExecuted(this, eventArgs);
                        break;
                }
            }
        }
    }
}