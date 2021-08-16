using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

namespace UIScripts.QQV {
    /// <summary>
    /// The script for the entire QQV panel.
    /// </summary>
    public sealed class QQVScript : MonoBehaviour
    {
        // buttons
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;

        // delegate for executing actions based on arrow 
        private Func<QQVArrowButtons, UniTask> _buttonAction; 
        public Func<QQVArrowButtons, UniTask> ButtonAction {
            set => _buttonAction = value;
        }

        // raw images
        [SerializeField] private GameObject[] _qubitRepresentations;  
        public int RawImageCapacity => _qubitRepresentations.Length;
        private Dictionary<int, QubitRepresentation> _qubitRepresentationsDict;
        private class QubitRepresentation {
            public RawImage rawImage;
            public int index; 
            public TextMeshProUGUI indexText;
        }

        private void Awake() {
            /*
            for every qubit representation, get its children components
            index 0: raw image
            index 1: textmeshpro
            */
            _qubitRepresentationsDict = new Dictionary<int, QubitRepresentation>();
            int qubitIndex = 0;
            foreach (GameObject qubitRep in _qubitRepresentations) {
                _qubitRepresentationsDict[qubitRep.GetInstanceID()] = (
                    new QubitRepresentation { 
                        rawImage = qubitRep.transform.GetChild(0).GetComponent<RawImage>(),
                        index = ++qubitIndex,
                        indexText = qubitRep.transform.GetChild(1).GetComponent<TextMeshProUGUI>()
                    }
                );
            }

            // for every arrow that is executed, invoke an action that uses these arrows
            QQVArrowButtonScript.ArrowExecuted += async (object sender, QQVArrowButtonEventArgs e) => {
                if (_buttonAction != null && !_buttonAction.Equals(null)) {
                    await _buttonAction(e.Arrow);
                }
            };
        }

        #region Getters and Setters
        /// <summary>
        /// Get the texture and qubit (not raw image) index of the qubit representation. 
        /// </summary>
        public Tuple<Texture, int> GetQubitRepresentation(int representationIndex) {
            QubitRepresentation qubitRep = _getQubitRepresentation(representationIndex);
            return Tuple.Create(qubitRep.rawImage.texture, qubitRep.index);
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
            qubitRep.index = qubitIndex;
            qubitRep.indexText.SetText($"[{qubitIndex}]");
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
        public void SetArrowButtonActive(QQVArrowButtons arrowButton, bool isActive) {
            Button button;
            if (arrowButton == QQVArrowButtons.Left) {
                button = _leftButton;
            }
            else {
                button = _rightButton;
            }
            button.gameObject.SetActive(isActive);
        }

        /// <summary>
        /// Select a qubit representation.

        private QubitRepresentation _getQubitRepresentation(int representationIndex) {
            int representationID = _qubitRepresentations[representationIndex].GetInstanceID();
            return _qubitRepresentationsDict[representationID];
        }
        #endregion Getters and Setters
    }
}
