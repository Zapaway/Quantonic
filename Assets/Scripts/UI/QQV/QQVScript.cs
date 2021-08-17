using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

namespace UIScripts.QQV {
    internal class QubitRepresentation {
        internal readonly RawImage rawImage;
        internal readonly Button rawImageButton;
        internal readonly int representationIndex;
        internal int qubitIndex; 
        internal readonly TextMeshProUGUI qubitIndexText;

        public QubitRepresentation(RawImage rawImage, Button rawImageButton, int repIndex, int qubitIndex, TextMeshProUGUI qubitIndexText) {
            this.rawImage = rawImage;
            this.rawImageButton = rawImageButton;
            this.representationIndex = repIndex;
            this.qubitIndex = qubitIndex;
            this.qubitIndexText = qubitIndexText;
        }
    }

    /// <summary>
    /// The script for the entire QQV panel.
    /// </summary>
    public sealed class QQVScript : MonoBehaviour
    {
        // buttons
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;

        // qubit representations
        [SerializeField] private GameObject[] _qubitRepresentations;  
        private Dictionary<int, QubitRepresentation> _qubitRepresentationsDict = new Dictionary<int, QubitRepresentation>();
        public int RawImageCapacity => _qubitRepresentations.Length;

        // delegate type
        public delegate UniTask QubitRepresentationHandler((int repIndex, int qubitIndex) qubitRepInfo);

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
                    rawImage, rawImageScript.Button, i, i, indexText
                );
                rawImageScript.qubitRepresentation = qubitRepData;
                _qubitRepresentationsDict[qubitRepID] = qubitRepData;
            }

            // for every move that is executed, invoke a delegate that responds to this
            QQVEvents.OnMoveExecuted += async (object sender, QQVMoveExecEventArgs e) => {
                if (_moveExecAsyncFunc != null && !_moveExecAsyncFunc.Equals(null)) {
                    await _moveExecAsyncFunc(e.Arrow);
                }
            };
            
            QQVEvents.OnRepresentationSelected += async (object sender, QQVRepresentationEventArgs e) => {
                if (_repSelectedAsyncFunc != null && !_repSelectedAsyncFunc.Equals(null)) {
                    await _repSelectedAsyncFunc((
                        repIndex: e.RepresentationIndex,
                        qubitIndex: e.QubitIndex
                    ));
                }
            };
            
            QQVEvents.OnRepresentationSubmitted += async (object sender, QQVRepresentationEventArgs e) => {
                if (_repSubmittedAsyncFunc != null && !_repSubmittedAsyncFunc.Equals(null)) {
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
            return Tuple.Create(qubitRep.rawImage.texture, qubitRep.qubitIndex);
        }

        /// <summary>
        /// Set the texture and index string of a qubit representation. 
        /// Do caution that it does not check if the representation index is out of bounds.
        /// </summary>
        public void SetQubitRepresentation(
            int representationIndex, 
            int qubitIndex,
            Texture rawImageTexture 
        ) {
            QubitRepresentation qubitRep = _getQubitRepresentation(representationIndex);
            qubitRep.rawImage.texture = rawImageTexture;
            qubitRep.qubitIndex = qubitIndex;
            qubitRep.qubitIndexText.SetText($"[{qubitIndex}]");
        }

        /// <summary>
        /// Set the panel unactive or active.
        /// </summary>
        public void SetPanelActive(bool isActive) {
            gameObject.SetActive(isActive);
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
            qubitRep.rawImageButton.Select();
            qubitRep.rawImageButton.OnSelect(null);  // forces the highlight
        }

        /// <summary>
        /// Select a qubit representation.
        /// </summary>
        private QubitRepresentation _getQubitRepresentation(int representationIndex) {
            int representationID = _qubitRepresentations[representationIndex].GetInstanceID();
            return _qubitRepresentationsDict[representationID];
        }
        #endregion Getters and Setters
    }
}
