using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

using UIScripts.QQV;

/*
TODO
- add more handle change case definitions
*/

namespace Managers
{
    /// <summary>
    /// All stage UI events and elements can be found here.
    /// </summary>
    public sealed class StageUIManager : Manager<StageUIManager>
    {   
        #region Quick Qubit Viewer (QQV) Data
        [SerializeField] private GameObject _QQV;
        private QQVScript _qqvScript;
        
        // keep track of qubit index in the left qubit representation
        private int _qubitLeftIndex = 0;

        // keep track of how many lefts and rights may be pressed
        private int _avalLeftPresses = 0;
        private bool _isLeftButtonActive = false;
        private int _avalRightPresses = 0;
        private bool _isRightButtonActive = false;


        // used to toggle the QQV on and off
        private bool _isQQVDisplayed = false;
        #endregion Quick Qubit Viewer (QQV) Data

        #region Event Methods
        protected override void Awake()
        {
            base.Awake();    
            _qqvScript = _QQV.GetComponent<QQVScript>();
            _qqvScript.ButtonAction = MoveQQVRenderTextures;
        }

        private void Start() {
            // the current controllable on every stage is the player, which will spawn one qubit at start
            Controllable controllable = ControlManager.Instance.CurrentControllable;
            SetQQVRenderTextures(controllable);
            
            _qqvScript.SetPanelActive(_isQQVDisplayed);
            _qqvScript.SetArrowButtonActive(QQVArrowButtons.Left, _isLeftButtonActive);
            _qqvScript.SetArrowButtonActive(QQVArrowButtons.Right, _isRightButtonActive);
        }

        private void OnEnable() {

        }
        #endregion Event Methods

        #region QQV Methods
        /// <summary>
        /// Toggle on/off the QQV.
        /// </summary>
        public void ToggleQQVPanel() {
            _isQQVDisplayed = !_isQQVDisplayed;
            _qqvScript.SetPanelActive(_isQQVDisplayed);
        }

        /// <summary>
        /// Move the render textures left or right by one.
        /// <summary>
        public async UniTask MoveQQVRenderTextures(QQVArrowButtons arrowButton) {
            if (arrowButton == QQVArrowButtons.Left) {
                _addQQVPressesAndActivate(QQVArrowButtons.Right);
                _qubitLeftIndex--;
                _subQQVPressesAndDeactivate(QQVArrowButtons.Left);
                await _updateAllQubitRepresentationsUnsafe();
            }
            else if (arrowButton == QQVArrowButtons.Right) {
                // allow the person to go back
                _addQQVPressesAndActivate(QQVArrowButtons.Left);

                // moving right increases the leftmost qubit index by 1 
                _qubitLeftIndex++;

                // do not let the person continue if we reached the end of the collection
                _subQQVPressesAndDeactivate(QQVArrowButtons.Right);

                // update render textures 
                await _updateAllQubitRepresentationsUnsafe();
            }

            await UniTask.Yield();
        }

        // render texture methods
        public void SetQQVRenderTextures(Controllable controllable) {
            controllable.SubscribeToQubitCollection(_qqvHandleChange);
        }
        public void RemoveQQVRenderTextures(Controllable controllable) {
            controllable.UnsubscribeToQubitCollection(_qqvHandleChange);
        }
        public void ChangeQQVRenderTextures(Controllable oldControllable, Controllable newControllable) {
            RemoveQQVRenderTextures(oldControllable);
            SetQQVRenderTextures(newControllable);
        }
        /// <summary>
        /// Observe any changes in the controllable's qubit collection and reflect it onto the
        /// qubit representations in the QQV.
        /// </summary>
        private void _qqvHandleChange(object sender, NotifyCollectionChangedEventArgs e) {
            Controllable ctrlable = ControlManager.Instance.CurrentControllable;

            switch (e.Action) {
                // newStartingIndex: index of added element on collection
                // newItems: added element
                case NotifyCollectionChangedAction.Add: 
                    bool wasInserted = (e.NewStartingIndex != ctrlable.GetQubitCount() - 1);
                    int currentQubitCount = ctrlable.GetQubitCount();

                    // if added
                    if (!wasInserted) {  
                        RenderTexture renderTexture = (e.NewItems[0] as Qubit).RenderTexture;

                        // if there is an available raw image
                        if (currentQubitCount <= _qqvScript.RawImageCapacity) {   
                            _qqvScript.SetQubitRepresentation(
                                currentQubitCount - 1,  // convert to index
                                e.NewStartingIndex,
                                renderTexture
                            );
                        }
                        else {  
                            _addQQVPressesAndActivate(QQVArrowButtons.Right);
                        }
                    } 
                    break;

                // oldStartingIndex: index of removed element on collection
                // oldItems: removed element
                case NotifyCollectionChangedAction.Remove:  

                // oldStartingIndex/newStartingIndex: index where replacement occured on collection
                // oldItems: old, replaced element
                // newItems: new element
                case NotifyCollectionChangedAction.Replace:  
                
                // oldStartingIndex: old index of element that was moved on collection
                // newStartingIndex: new index of moved element where its currently located on collection
                // oldItems/newItems: moved element
                case NotifyCollectionChangedAction.Move: 

                case NotifyCollectionChangedAction.Reset:  
                    break;
            }
        }

        /// <summary>
        /// Update all qubit representations.
        /// </summary>
        private async UniTask _updateAllQubitRepresentationsUnsafe() {
            IEnumerable<UniTask> renderingTasks = (
                from i in Enumerable.Range(0, _qqvScript.RawImageCapacity) 
                select _setQubitRepresentationUnsafe(i, _qubitLeftIndex + i)
            );
            await UniTask.WhenAll(renderingTasks);
        }

        /// <summary>
        /// Set qubit representation based on a new qubit.
        /// </summary>
        private async UniTask _setQubitRepresentationUnsafe(int representationIndex, int qubitIndex) {
            Controllable ctrlable = ControlManager.Instance.CurrentControllable;
            RenderTexture renderTexture = ctrlable.GetRenderTextureUnsafe(qubitIndex);
            _qqvScript.SetQubitRepresentation(representationIndex, qubitIndex, renderTexture);

            await UniTask.Yield();
        }

        /// <summary>
        /// Add one arrow press and activate the respective arrow button if it hasn't been already.
        /// </summary>
        private void _addQQVPressesAndActivate(QQVArrowButtons arrowButtons) {
            switch (arrowButtons) {
                case QQVArrowButtons.Left:
                    _avalLeftPresses++;
                    if (!_isLeftButtonActive) _updateQQVLeftButtonActive(true);
                    break;
                case QQVArrowButtons.Right:
                    _avalRightPresses++;
                    if (!_isRightButtonActive) _updateQQVRightButtonActive(true);
                    break;
            }
        }
        /// <summary>
        /// Subtract one arrow press and deactivate the respective arrow button if it hasn't been already.
        /// </summary>
        private void _subQQVPressesAndDeactivate(QQVArrowButtons arrowButtons) {
            switch (arrowButtons) {
                case QQVArrowButtons.Left:
                    if (--_avalLeftPresses == 0) _updateQQVLeftButtonActive(false);
                    break;
                case QQVArrowButtons.Right:
                    if (--_avalRightPresses == 0) _updateQQVRightButtonActive(false);
                    break;
            }
        }

        private void _updateQQVLeftButtonActive(bool isActive) {
            _isLeftButtonActive = isActive;
            _qqvScript.SetArrowButtonActive(QQVArrowButtons.Left, _isLeftButtonActive);
        }
        private void _updateQQVRightButtonActive(bool isActive) {
            _isRightButtonActive = isActive;
            _qqvScript.SetArrowButtonActive(QQVArrowButtons.Right, _isRightButtonActive);
        }
        #endregion QQV Methods

        // private void _test(IList ilist, string type, NotifyCollectionChangedAction action) {
        //     if (ilist != null) {
        //         Array items = Array.CreateInstance(typeof(Qubit), ilist.Count);
        //         ilist.CopyTo(items, 0);

        //         string debugString = $"{type} Items on {action}: ";
        //         foreach (Qubit qubit in items) {
        //             debugString += '\n' + qubit.name;
        //         }
        //         Debug.Log(debugString);
        //     }
        //     else {
        //         Debug.Log($"{type} Items on {action}: NONE");
        //     }
        // }
    }
}
