using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

namespace UIScripts.QQV {
    public delegate UniTask QubitRepresentationHandler((int repIndex, int qubitIndex) qubitRepInfo);
    
    /// <summary>
    /// Holds information about a qubit representation.
    /// </summary>
    internal class QubitRepresentation {
        internal readonly QQVQubitRawImageScript rawImageScript;
        internal readonly TextMeshProUGUI qubitIndexText;
        internal readonly int representationIndex;
        internal int qubitIndex; 

        public QubitRepresentation(
            QQVQubitRawImageScript rawImageScript,
            TextMeshProUGUI qubitIndexText,
            int repIndex, 
            int qubitIndex
        ) {
            this.rawImageScript = rawImageScript;
            this.qubitIndexText = qubitIndexText;
            this.representationIndex = repIndex;
            this.qubitIndex = qubitIndex;

            this.qubitIndexText.SetText("");  // ensure that there is no text in the beginning
        }
    }

    /// <summary>
    /// The script for the entire QQV panel.
    /// </summary>
    public sealed class QQVScript : Panel
    {
        // buttons
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;

        // qubit representations
        [SerializeField] private GameObject[] _qubitRepresentations;  
        private Dictionary<int, QubitRepresentation> _qubitRepresentationsDict = new Dictionary<int, QubitRepresentation>();
        public int RawImageCapacity => _qubitRepresentations.Length;

        // any disabled qubit representations to be aware of 
        // using dictionary for faster key-based access times - do not use their values
        private Dictionary<int, Nullable<byte>> _disabledQubitIndices = new Dictionary<int, Nullable<byte>>();

        // all delegates that can be used for custom behavior on the QQV child componenets 
        private Func<QQVMoveOptions, UniTask> _moveExecAsyncFunc; 
        public Func<QQVMoveOptions, UniTask> MoveExecAsyncFunc {
            set => _moveExecAsyncFunc = value;
        }
        private QubitRepresentationHandler _repSelectedAsyncFunc;
        public QubitRepresentationHandler RepSelectedAsyncFunc {
            set => _repSelectedAsyncFunc = value;
        }
        private QubitRepresentationHandler _repSubmittedAsyncFunc;
        public QubitRepresentationHandler RepSubmittedAsyncFunc {
            set => _repSubmittedAsyncFunc = value;
        }

        private void Awake() {
            /*
            for every qubit representation, get its children components (default starts with 0-1-2 indices)
            index 0 of getchild: raw image
            index 1 of getchild: textmeshpro
            */
            for (int i = 0; i < RawImageCapacity; ++i) {
                GameObject qubitRep = _qubitRepresentations[i];
                int qubitRepID = qubitRep.GetInstanceID();

                var rawImage = qubitRep.transform.GetChild(0).GetComponent<RawImage>();
                var rawImageScript = rawImage.GetComponent<QQVQubitRawImageScript>();
                var indexText = qubitRep.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

                QubitRepresentation qubitRepData = new QubitRepresentation(
                    rawImageScript, indexText, i, i
                );
                rawImageScript.qubitRepresentation = qubitRepData;
                _qubitRepresentationsDict[qubitRepID] = qubitRepData;
            }

            // for every move that is executed, invoke a delegate that responds to this
            QQVEvents.OnMoveExecuted += async (object sender, QQVMoveExecEventArgs e) => {
                if (_moveExecAsyncFunc != null) {
                    await _moveExecAsyncFunc(e.Arrow);
                }
            };
            
            QQVEvents.OnRepresentationSelected += async (object sender, QQVRepresentationEventArgs e) => {
                if (_repSelectedAsyncFunc != null) {
                    await _repSelectedAsyncFunc((
                        repIndex: e.RepresentationIndex,
                        qubitIndex: e.QubitIndex
                    ));
                }
            };
            
            QQVEvents.OnRepresentationSubmitted += async (object sender, QQVRepresentationEventArgs e) => {
                if (_repSubmittedAsyncFunc != null) {
                    await _repSubmittedAsyncFunc((
                        repIndex: e.RepresentationIndex,
                        qubitIndex: e.QubitIndex
                    ));
                }
            };
        }

        #region Getters and Setters
        /// <summary>
        /// Get the texture and qubit (not raw image) index of the qubit representation. 
        /// </summary>
        public Tuple<Texture, int> GetQubitRepresentation(int representationIndex) {
            QubitRepresentation qubitRep = _getQubitRepresentation(representationIndex);
            return Tuple.Create(qubitRep.rawImageScript.Texture, qubitRep.qubitIndex);
        }

        /// <summary>
        /// Set the texture and index string of a qubit representation. 
        /// If the render texture is null, then erase the text.
        /// Do caution that it does not check if the representation index is out of bounds.
        /// </summary>
        public void SetQubitRepresentation(
            int representationIndex, 
            int qubitIndex,
            Texture rawImageTexture = null
        ) {
            QubitRepresentation qubitRep = _getQubitRepresentation(representationIndex);
            qubitRep.rawImageScript.Texture = rawImageTexture;
            qubitRep.qubitIndex = qubitIndex;

            Button btn = qubitRep.rawImageScript.Button;
            // this is used to maintain a disabled qubit rep when the qubit reps move  
            if (_disabledQubitIndices.ContainsKey(qubitIndex)) btn.interactable = false;
            else btn.interactable = true;

            string text = rawImageTexture != null ? $"[{qubitIndex}]" : "";
            qubitRep.qubitIndexText.SetText(text);
        }

        public void ResetQubitRepresentation() {
            for (int i = 0; i < _qubitRepresentations.Count(); ++i) {
                SetQubitRepresentation(i, 0, rawImageTexture: null);
            }

            SetArrowButtonActive(QQVMoveOptions.Left, false);
            SetArrowButtonActive(QQVMoveOptions.Right, false);
        }

        /// <summary>
        /// Set an arrow button unactive or active.
        /// <summary>
        public void SetArrowButtonActive(QQVMoveOptions moveAction, bool isActive) {
            Button button;
            if (moveAction == QQVMoveOptions.Left) {
                button = _leftButton;
            }
            else {
                button = _rightButton;
            }
            button.gameObject.SetActive(isActive);
        }

        /// <summary>
        /// Set a qubit representation selected.
        /// Do caution that it does not check if the representation index is out of bounds.
        /// </summary>
        public void SelectQubitRepresentation(int representationIndex) {
            QubitRepresentation qubitRep = _getQubitRepresentation(representationIndex);
            qubitRep.rawImageScript.Button.Select();
            qubitRep.rawImageScript.Button.OnSelect(null);  // forces the highlight
        }

        /// <summary>
        /// Adds or removes the qubit rep indices depending on its interactable state.
        /// Caution that visual effects do not take effect right away.
        /// </summary>
        public void SetQubitRepresentationInteractable(int qubitIndex, bool isInteractable) {
            if (!isInteractable) _addDisabled(qubitIndex);  
            else _removeDisabled(qubitIndex);
        }

        /// <summary>
        /// Allow the specified arrow to be seen, but change its interactability.
        /// </summary>
        public void SetArrowButtonInteractable(QQVMoveOptions dir, bool isInteractable) {
            Button button = dir == QQVMoveOptions.Left ? _leftButton : _rightButton;

            if (isInteractable) button.interactable = true;
            else button.interactable = false;
        }

        /// <summary>
        /// Select a qubit representation.
        /// </summary>
        private QubitRepresentation _getQubitRepresentation(int representationIndex) {
            int representationID = _qubitRepresentations[representationIndex].GetInstanceID();
            return _qubitRepresentationsDict[representationID];
        }
        #endregion Getters and Setters

        private void _addDisabled(int qubitIndex) {
            _disabledQubitIndices.Add(qubitIndex, null);
        }
        private void _removeDisabled(int qubitIndex) {
            _disabledQubitIndices.Remove(qubitIndex);
        }
    }
}
