using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UIScripts.QQV {
    /// <summary>
    /// Events that are used to pass information between QQV scripts.
    /// </summary>
    internal static class QQVEvents
    {
        internal static event EventHandler<QQVMoveExecEventArgs> OnMoveExecuted;
        internal static void InvokeMoveExecuted(object sender, QQVMoveExecEventArgs eventArgs) {
            OnMoveExecuted?.Invoke(sender, eventArgs);
        }

        internal static event EventHandler<QQVRepresentationEventArgs> OnRepresentationSelected;
        internal static void InvokeRepresentationSelected(object sender, QQVRepresentationEventArgs eventArgs) {
            OnRepresentationSelected?.Invoke(sender, eventArgs);
        }

        internal static event EventHandler<QQVRepresentationEventArgs> OnRepresentationSubmitted;
        internal static void InvokeRepresentationSubmitted(object sender, QQVRepresentationEventArgs eventArgs) {
            OnRepresentationSubmitted?.Invoke(sender, eventArgs);
        }
    }

    #region Move Execution Data
    /// <summary>
    /// The movement available in the QQV panel.
    /// </summary>
    public enum QQVMoveOptions {
        Left = MoveDirection.Left,
        Right = MoveDirection.Right,
    }
    /// <summary>
    /// Event args for any moves executed.
    /// </summary>
    internal sealed class QQVMoveExecEventArgs : EventArgs {
        public QQVMoveOptions Arrow {get; set;}
    }
    #endregion Move Execution Data

    #region Representation Event Data
    internal sealed class QQVRepresentationEventArgs : EventArgs {
        public int RepresentationIndex {get; set;}
        public int QubitIndex {get; set;}
    }
    #endregion Representation Event Data
}