using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UIScripts.QQV {
    [RequireComponent(typeof(RawImage), typeof(Button))]
    internal sealed class QQVQubitRawImageScript : MonoBehaviour, IMoveHandler, ISelectHandler, ISubmitHandler, IPointerClickHandler
    {
        [SerializeField] private MoveDirection _moveDirection;

        // used to know when its alright to move
        [SerializeField] private GameObject _indicator;

        // keep a reference to the qubit representation
        internal QubitRepresentation qubitRepresentation; 

        // button reference
        private Button _button;
        internal Button Button => _button;

        private void Awake() {
            _button = GetComponent<Button>();
        }

        public void OnMove(AxisEventData eventData) {
            MoveDirection eventDir = eventData.moveDir;
            
            /* 
            only invoke the move execution event IF the key pressed is the same as the targeted key press
            and the indicator is active
            */
            if (eventDir == _moveDirection) {
                switch (eventDir) {
                    case MoveDirection.Left:
                    case MoveDirection.Right:
                        if (_indicator.activeInHierarchy) {
                            var eventArgs = new QQVMoveExecEventArgs{Arrow = (QQVMoveOptions)eventDir};
                            QQVEvents.InvokeMoveExecuted(this, eventArgs);
                            
                        }
                        break;
                }
            }
        }

        public void OnSelect(BaseEventData eventData) {
            _invokeRepresentationSelected();
        }  

        public void OnSubmit(BaseEventData eventData) {
            _invokeRepresentationSubmitted();
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Left) {
                _invokeRepresentationSubmitted();
            }
        }

        private QQVRepresentationEventArgs _createQQVRepresentationEventArgs() {
            var eventArgs = new QQVRepresentationEventArgs {
                RepresentationIndex = qubitRepresentation.representationIndex,
                QubitIndex = qubitRepresentation.qubitIndex
            };
            return eventArgs;
        }
        private void _invokeRepresentationSelected() {
            QQVEvents.InvokeRepresentationSelected(this, _createQQVRepresentationEventArgs());
        }
        private void _invokeRepresentationSubmitted() {
            QQVEvents.InvokeRepresentationSubmitted(this, _createQQVRepresentationEventArgs());
        }
    }
}