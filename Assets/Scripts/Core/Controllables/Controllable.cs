using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using sysnum = System.Numerics;
using System.Threading;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using Cysharp.Threading.Tasks;

using Managers;
using Quantum;
using Quantum.Operators;
using StateMachines.QSM;

/// <summary>
/// Any object that can be controlled with input derives from this base class.
/// </summary>
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public abstract class Controllable : MonoBehaviour
{
    // components of controllable
    private BoxCollider2D _boxCollider2D;
    public BoxCollider2D BoxCollider2D => _boxCollider2D;
    private Rigidbody2D _rigidbody2D;
    public Rigidbody2D Rigidbody2D => _rigidbody2D;


    // qubits of controllable
    private IQubitSubcircuit _subcirc;   
    public int QubitCount => _subcirc.Count;
    public Vector<sysnum.Complex>[] SubcircVectors => _subcirc.Vectors;
    public (int controlQSIndex, int targetQSIndex)[] SubcircEntanglementInfo => _subcirc.EntanglementInfo;


    // determines what abilities can be used with the qubits
    private QSM _qsm = new QSM();
    public QSM QSM => _qsm;
    private QSMState _qsmState;
    public QSMState QSMState => _qsmState;
    private MultipleState _multiState;
    public MultipleState MultiState => _multiState;


    // used for checking if the controllable is busy
    private CancellationTokenSource _notNearGateCancellationSource = new CancellationTokenSource();
    private bool _listenForNotNearGateCancellation = false;  // makes sure cancellation of a token doesn't happen twice
    public bool IsBusy => _listenForNotNearGateCancellation;
    [HideInInspector] public bool reachedOtherSideOfGate = false;


    // special cases gates can use if they want to check for any entangled qubits (returning true means prevents, and false is opposite)
    public Func<int, bool> PreventEntangledQubits => (qsIndex) => CheckIfQubitEntangled(qsIndex);
    public Func<int, bool> PreventTargetedQubits => (qsIndex) => CheckIfQubitTarget(qsIndex);

    private List<int> _allowedQSIndices = new List<int>(1);  // if empty, it is assumed that all qs indices are allowed
    public Func<int, bool> PreventUnallowedQubits => (qsIndex) => CheckIfQubitIsNotAllowed(qsIndex);

    #region Unity Events
    protected virtual void Awake() {
        _subcirc = StageControlManager.Instance.circ.GetOrCreateQubitSubcircuit(this);

        _boxCollider2D = GetComponent<BoxCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _qsmState = new QSMState(StageControlManager.Instance, StageUIManager.Instance, SpawnManager.Instance, this, _qsm);
        _multiState = new MultipleState(StageControlManager.Instance, StageUIManager.Instance, SpawnManager.Instance, this, _qsm);
    }

    protected virtual async UniTask Start() {
        await _qsm.InitializeState(_qsmState);
    }

    private async UniTaskVoid Update() {
        if (StageControlManager.Instance.IsJumpTriggered()) {
            CancelForNotBeingNearGate();
        }

        await _qsm.CurrentState.HandleInput();
        await _qsm.CurrentState.LogicUpdate();
    }

    private async UniTaskVoid FixedUpdate() {
        await _qsm.CurrentState.PhysicsUpdate();
    }

    protected virtual void OnEnable() {
        if (_notNearGateCancellationSource != null) _notNearGateCancellationSource.Dispose();
        _notNearGateCancellationSource = new CancellationTokenSource();
        _freezeControllable(this, false);
    }

    protected virtual void OnDisable() {
        _notNearGateCancellationSource.Cancel();
        _freezeControllable(this, true);
    }

    protected virtual void OnDestroy() {
        _notNearGateCancellationSource.Cancel();
        _notNearGateCancellationSource.Dispose();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Contains("Qubit")) {
            _addQubit();
            other.gameObject.SetActive(false);
            SpawnManager.Instance.AddUnactivatedQubitCollectable(other.gameObject);
        }
    }

    private void _freezeControllable(Controllable controllable, bool isFreeze) {
        if (isFreeze) controllable.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        else controllable.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    #endregion Unity Events

    #region Subcircuit Manipulation
    /// <summary>
    /// Get an available qubit and give it to the controllable.
    /// </summary>
    protected Qubit _addQubit() {
        var (_, qubit) = _subcirc.Add();
        return qubit;
    }
    /// <summary>
    /// Get an available qubit, set its quantum state, and give it to the controllable.
    /// </summary>
    protected Qubit _addQubit(QuantumState state) {
        return _addQubit().SetQuantumState(state);
    }

    /// <summary>
    /// Clear subcirc.
    /// </summary>
    protected void _clear() {
        _subcirc.Clear();
    }

    /// <summary>
    /// Get a render texture of a qubit. Do note that it will not check if the index is out of bounds.
    /// </summary>
    public RenderTexture GetRenderTextureUnsafe(int qsIndex) {
        return _subcirc.GetRenderTexture(qsIndex, isQCIndex: false);
    }
    /// <summary>
    /// Get the qubit's state along with its ground and excited state probabilites. 
    /// Do note that it will not check if the index is out of bounds. 
    /// </summary>
    public (string descString, double ground, double excited) GetQubitInfoUnsafe(int qsIndex) {
        return _subcirc.GetQubitInfoUnsafe(qsIndex, isQCIndex: false);
    } 

    /// <summary>
    /// Check if the selected qubit via qsIndex is a target qubit.
    /// </summary>
    public bool CheckIfQubitTarget(int qsIndex) {
        return _subcirc.IsQubitTarget(qsIndex, isQCIndex: false);
    }
    /// <summary>
    /// Check if the selected qubit via qsIndex is a control qubit.
    /// </summary>
    public bool CheckIfQubitControl(int qsIndex) {
        return _subcirc.IsQubitControl(qsIndex, isQCIndex: false);
    }
    /// <summary>
    /// Check if the selected qubit via qsIndex is entangled.
    /// </summary>
    public bool CheckIfQubitEntangled(int qsIndex) {
        return _subcirc.IsQubitEntangled(qsIndex, isQCIndex: false);
    }
    /// <summary>
    /// Check if the selected qubit is the qubit that can only be selected.
    /// </summary>
    public bool CheckIfQubitIsNotAllowed(int qsIndex) {
        return _allowedQSIndices.Count == 0 ? false : !_allowedQSIndices.Contains(qsIndex);
    }

    public void SubscribeToSubcircuitCollection(NotifyCollectionChangedEventHandler eventHandler) {
        _subcirc.Subscribe(eventHandler);
    }
    public void UnsubscribeToSubcircuitCollection(NotifyCollectionChangedEventHandler eventHandler) {
        _subcirc.Unsubscribe(eventHandler);
    }
    #endregion Subcircuit Manipulation
    
    #region Select Qubits
    /// <summary>
    /// If the qubit is not an entangled control type, then allow any qubits to be submitted.
    /// If the qubit is of entangled control type, then only its targetted qubit can be submitted.
    /// </summary>
    /// <returns> If the inputted qsIndex is entangled or not. </returns>
    public bool SetAllowedQubitsForSubmissionBasedOnType(int qsIndex) {
        _allowedQSIndices = new List<int>(1);

        var res = _subcirc.GetEntangledQSIndexPair(qsIndex, isQCIndex: false);
        if (res.targetQSIndex != -1) _allowedQSIndices = new List<int>(1) { res.targetQSIndex };

        return _allowedQSIndices.Count > 0;
    }

    /// <summary>
    /// Ask for one qubit.
    /// </summary>
    /// <param name="autoSuccess">
    /// Automatically execute success case if the operation wasn't cancelled.
    /// If put to false, user is responsible of executing success case.
    /// </param>
    public async UniTask<int> AskForSingleQubitIndex(bool autoSuccess=true, Func<int, bool> specialCase=null) {
        _listenForNotNearGateCancellation = true;

        var (isCancelled, res) = await StageUIManager.Instance.WaitForSubmitResults(
            StageUIManager.QQVSubmitMode.Single, 
            _notNearGateCancellationSource.Token,
            specialCase       
        );
        if (!isCancelled && autoSuccess) _gateOperationSuccess();

        _notNearGateCancellationSource.Dispose();
        _notNearGateCancellationSource = new CancellationTokenSource();

        return isCancelled ? -1 : res[0];
    }
    
    /// <summary>
    /// Ask for one qubit at a time. Goes until n times.
    /// </summary>
    /// <param name="firstSpecialCase">
    /// If not null, only the first selected index will execute this case.
    /// </param>
    /// <param name="transitionalFirstCase">
    /// If not null, the input of the first special case (if not null) will be used to set up for 
    /// the executing of the future special cases. Do note that the bool must return if it is entangled or not.
    /// </param>
    /// <param name="considerEntangledCase">
    /// If true and "transitionalFirstCase" is not null,
    /// it will prevent non-entangled qubits to pick currently entangled qubits. It will also only allow an
    /// entangled qubit to choose from a certain selection of qubits.
    /// </param>
    public async UniTask<List<int>> AskForMultipleSingleQubitIndices(
        int n, 
        Func<int, bool> firstSpecialCase=null,
        Func<int, bool> transitionalFirstCase=null,
        bool considerEntangledCase=false
    ) {
        await UniTask.Yield();
        
        if (n > _subcirc.Count) return null;

        List<int> res = new List<int>(n);  // qubit indices
        bool isFirstQubitEntangled = false;

        for (int i = 0; i < n; ++i) {
            // determine the special case
            Func<int, bool> specialCase = null;
            bool isOnFirstSpecialCase = firstSpecialCase != null && i == 0;
            if (isOnFirstSpecialCase) {
                specialCase = firstSpecialCase;
            }
            else if (considerEntangledCase) {
                specialCase = isFirstQubitEntangled ? PreventUnallowedQubits : PreventEntangledQubits;
            }
            
            // to prevent any duplicates
            Func<int, bool> preventDuplicateCase = (int qsIndex) => {
                return res.Contains(qsIndex) || (specialCase?.Invoke(qsIndex) ?? false);
            };
            
            // wait fr indices
            int qubitIndex = await AskForSingleQubitIndex(
                autoSuccess: false, 
                preventDuplicateCase
            );
            if (qubitIndex == -1) break; // operation was cancelled
            
            // add into the results
            StageUIManager.Instance.DisableQubitRepInteract(qubitIndex).Forget();
            res.Add(qubitIndex);

            // execute the transitional case if applicable
            if (isOnFirstSpecialCase && transitionalFirstCase != null) {
                isFirstQubitEntangled = transitionalFirstCase(qubitIndex);
            }
        }

        // reset
        UniTask.WhenAll(from i in res select StageUIManager.Instance.EnableQubitRepInteract(i)).Forget();
        if (_allowedQSIndices.Count > 0) _allowedQSIndices = new List<int>(1);

        if (res.Count == n) {
            _gateOperationSuccess();
            return res;
        } else return null;
    }

    /// <summary>
    /// Sucess case of the gate operation.
    /// </summary>
    private void _gateOperationSuccess() {
        reachedOtherSideOfGate = true;
        _listenForNotNearGateCancellation = false; 
    }
    #endregion Select Qubits

    #region Cancellation Methods
    public void CancelForNotBeingNearGate() {
        if (_listenForNotNearGateCancellation) { 
            _listenForNotNearGateCancellation = false;
            _notNearGateCancellationSource.Cancel();
        }
    }
    #endregion Cancellation Methods

    // using these methods will automatically notify the controllable to check its qubit state
    #region Applying Methods
    /// <summary>
    /// Apply a unary operator on qubit(s) within the subcircuit.
    /// </summary>
    public async UniTask ApplyUnaryOperator(UnaryOperator unaryOperator, int[] qsIndices) {
        await _subcirc.ApplyUnaryOperator(unaryOperator, qsIndices, isQCIndices: false);
    }

    /// <summary>
    /// Apply a binary operator on qubit pair(s) within the subcircuit.
    /// </summary>
    public async UniTask ApplyBinaryOperator(
        BinaryOperator binaryOperator, 
        int[] qsPair
    ) {
        await _subcirc.ApplyBinaryOperator(binaryOperator, qsPair, isQCPair: false);
    }

    /// <summary>
    /// Force entanglement upon two qubits. Not recommended to use unless if you are loading in quantum states.
    /// </summary>
    public void ApplyForcedEntanglement(int controlQSIndex, int targetQSIndex) {
        _subcirc.ForceEntanglement(controlQSIndex, targetQSIndex);
    }
    #endregion Applying Methods
}