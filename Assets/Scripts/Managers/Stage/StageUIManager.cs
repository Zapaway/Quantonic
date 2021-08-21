using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
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
        /// <summary>
        /// Different submit modes that the QQV can be in.
        /// </summary>
        public enum QQVSubmitMode {
            Default, 
            Single,
            Multi
        }
        private int[] _submittedQubitIndex;  // used for knowing when to stop awaiting submission

        [SerializeField] private GameObject _QQV;
        private QQVScript _qqvScript;

        // current selected representation 
        private int _selectedRepresentationIndex = 0;
        
        // keep track of qubit index of the leftmost qubit representation
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

            // QQV
            _qqvScript = _QQV.GetComponent<QQVScript>();
            _qqvScript.MoveExecAsyncFunc = MoveQQVRenderTextures;
            _qqvScript.RepSelectedAsyncFunc = UpdateSelectedQubit;
            WaitForSubmitResults().Forget();  // set to default
        }

        private void Start() {
            // the current controllable on every stage is the player, which will spawn one qubit at start
            Controllable controllable = ControlManager.Instance.CurrentControllable;
            
            // QQV
            SetQQVRenderTextures(controllable);
            _qqvScript.SetPanelActive(_isQQVDisplayed);
            _qqvScript.SetArrowButtonActive(QQVMoveOptions.Left, _isLeftButtonActive);
            _qqvScript.SetArrowButtonActive(QQVMoveOptions.Right, _isRightButtonActive);
        }
        #endregion Event Methods

        #region QQV Methods
        /// <summary>
        /// Toggle on/off the QQV.
        /// </summary>
        public void ToggleQQVPanel() {
            _isQQVDisplayed = !_isQQVDisplayed;
            SetQQVPanelActive(_isQQVDisplayed);
        }
        public void SetQQVPanelActive(bool isActive) {
            if (isActive) {
                _qqvScript.SelectQubitRepresentation(_selectedRepresentationIndex);
            }
            _qqvScript.SetPanelActive(isActive);
        }

        // subscriber methods to QQV
        /// <summary>
        /// Move the render textures left or right by one.
        /// <summary>
        public async UniTask MoveQQVRenderTextures(QQVMoveOptions moveAction) {
            if (moveAction == QQVMoveOptions.Left) {
                _addQQVPressesAndActivate(QQVMoveOptions.Right);
                _qubitLeftIndex--;
                _subQQVPressesAndDeactivate(QQVMoveOptions.Left);
                await _updateAllQubitRepresentationsUnsafe();
            }
            else if (moveAction == QQVMoveOptions.Right) {
                // allow the person to go back
                _addQQVPressesAndActivate(QQVMoveOptions.Left);

                // moving right increases the leftmost qubit index by 1 
                _qubitLeftIndex++;

                // do not let the person continue if we reached the end of the collection
                _subQQVPressesAndDeactivate(QQVMoveOptions.Right);

                // update render textures 
                await _updateAllQubitRepresentationsUnsafe();
            }

            await UniTask.Yield();
        }
        /// <summary>
        /// Update the selected representation index.
        /// </summary>
        public async UniTask UpdateSelectedQubit((int repIndex, int qubitIndex) qubitRep) {
            _selectedRepresentationIndex = qubitRep.repIndex;
            await UniTask.Yield();
        }
        /// <summary>
        /// <para>
        /// When submitting, use a certain mode and automatically subscribe to the event handler 
        /// with the respective subscriber. 
        /// </para>
        /// <para>
        /// If it is in default mode, it will skip the waiting. Otherwise, it will
        /// automatically wait for a result if a cancellation token is passed.
        /// </para>
        /// </summary>
        public async UniTask<(bool isCanceled, int[] indices)> WaitForSubmitResults(
            QQVSubmitMode mode = QQVSubmitMode.Default, 
            CancellationToken token = default
        ) {
            // default results
            (bool isCanceled, int[] indices) results = (false, null);

            // Default subscriber to the representation submit event. All it does is turn off the panel.
            async UniTask SubmitDefaultMode((int repIndex, int qubitIndex) qubitRep) {
                ToggleQQVPanel();
                await UniTask.Yield();
            }  
            /* Single-mode subscriber to the representation submit event.
            Updates the submitted qubit index array with one qubit index. */
            async UniTask SubmitSingleMode((int repIndex, int qubitIndex) qubitRep) {
                _submittedQubitIndex = new int[1]{ qubitRep.qubitIndex };
                ToggleQQVPanel();
                await UniTask.Yield();
            }  
            /* Multi-mode subscriber to the representation submit event.
            Updates the submitted qubit index array with qubit indices. */
            async UniTask SubmitMultiMode((int repIndex, int qubitIndex) qubitRep) {
                await UniTask.Yield();
                throw new NotImplementedException();
            }  

            // get the submit subscriber respective to a mode
            QubitRepresentationHandler submitSubscriber = null;
            switch (mode) {
                case QQVSubmitMode.Default:
                    submitSubscriber = SubmitDefaultMode;
                    break;
                case QQVSubmitMode.Single:
                    submitSubscriber = SubmitSingleMode;
                    break;
                case QQVSubmitMode.Multi:
                    submitSubscriber = SubmitMultiMode;
                    break;
            }
            _qqvScript.RepSubmittedAsyncFunc = submitSubscriber;

            // if not default, wait for the appropiate results
            switch (mode) {
                case QQVSubmitMode.Single:
                case QQVSubmitMode.Multi:
                    if (token == default) {
                        throw new ArgumentException("Must use cancellation token if not using default subscriber");
                    }

                    results.isCanceled = await UniTask
                    .WaitUntil(() => _submittedQubitIndex != null, cancellationToken: token)
                    .SuppressCancellationThrow();

                    results.indices = _submittedQubitIndex?.ToArray();
                    _submittedQubitIndex = null;

                    _qqvScript.RepSubmittedAsyncFunc = SubmitDefaultMode;
                    break;
            }
            
            return results;
        }

        // render texture methods
        public void SetQQVRenderTextures(Controllable controllable) {
            controllable.SubscribeToSubcircuitCollection(_qqvHandleChange);
        }
        public void RemoveQQVRenderTextures(Controllable controllable) {
            controllable.UnsubscribeToSubcircuitCollection(_qqvHandleChange);
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
                            _addQQVPressesAndActivate(QQVMoveOptions.Right);
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
        private void _addQQVPressesAndActivate(QQVMoveOptions moveAction) {
            switch (moveAction) {
                case QQVMoveOptions.Left:
                    _avalLeftPresses++;
                    if (!_isLeftButtonActive) _updateQQVLeftButtonActive(true);
                    break;
                case QQVMoveOptions.Right:
                    _avalRightPresses++;
                    if (!_isRightButtonActive) _updateQQVRightButtonActive(true);
                    break;
            }
        }
        /// <summary>
        /// Subtract one arrow press and deactivate the respective arrow button if it hasn't been already.
        /// </summary>
        private void _subQQVPressesAndDeactivate(QQVMoveOptions moveAction) {
            switch (moveAction) {
                case QQVMoveOptions.Left:
                    if (--_avalLeftPresses == 0) _updateQQVLeftButtonActive(false);
                    break;
                case QQVMoveOptions.Right:
                    if (--_avalRightPresses == 0) _updateQQVRightButtonActive(false);
                    break;
            }
        }

        private void _updateQQVLeftButtonActive(bool isActive) {
            _isLeftButtonActive = isActive;
            _qqvScript.SetArrowButtonActive(QQVMoveOptions.Left, _isLeftButtonActive);
        }
        private void _updateQQVRightButtonActive(bool isActive) {
            _isRightButtonActive = isActive;
            _qqvScript.SetArrowButtonActive(QQVMoveOptions.Right, _isRightButtonActive);
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
