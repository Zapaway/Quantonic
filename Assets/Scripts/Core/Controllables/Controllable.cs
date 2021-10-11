using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;
using Quantum;
using Quantum.Operators;
using StateMachines.QSM;

/*
TODO: 
    - add qubitstatemachine 
*/

/// <summary>
/// Any object that can be controlled with input derives from this base class.
/// </summary>
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public abstract class Controllable : MonoBehaviour
{
    protected IQubitSubcircuit _subcirc;   
    public int QubitCount => _subcirc.Count;

    private CancellationTokenSource _notNearGateCancellationSource = new CancellationTokenSource();
    private bool _listenForNotNearGateCancellation = false;  // makes sure cancellation of a token doesn't happen twice
    public bool reachedOtherSideOfGate = false;

    private QSM _qsm = new QSM();
    public QSM QSM => _qsm;
    private QSMState _qsmState;
    public QSMState QSMState => _qsmState;
    private MultipleState _multiState;
    public MultipleState MultiState => _multiState;


    #region Unity Events
    protected virtual void Awake() {
        _subcirc = StageControlManager.Instance.circ.CreateQubitSubcircuit(this);

        _qsmState = new QSMState(StageControlManager.Instance, SpawnManager.Instance, this, _qsm);
        _multiState = new MultipleState(StageControlManager.Instance, SpawnManager.Instance, this, _qsm);
    }

    protected virtual void Update() {
        if (StageControlManager.Instance.IsJumpTriggered()) {
            CancelForNotBeingNearGate();
        }
    }

    protected virtual void OnEnable() {
        if (_notNearGateCancellationSource != null) _notNearGateCancellationSource.Dispose();
        _notNearGateCancellationSource = new CancellationTokenSource();
    }

    protected virtual void OnDisable() {
        _notNearGateCancellationSource.Cancel();
    }

    protected virtual void OnDestroy() {
        _notNearGateCancellationSource.Cancel();
        _notNearGateCancellationSource.Dispose();
    }
    #endregion Unity Events

    #region Subcircuit Manipulation
    /// <summary>
    /// Checks if the GameObject is a qubit and returns it. If true, it adds it onto the list.
    /// </summary>
    protected bool _addQubitSafe(GameObject possibleQubit) {
        bool isQubit = possibleQubit.CompareTag("Qubit");
        if (isQubit) {
            var qubit = possibleQubit.GetComponent<Qubit>();

            _subcirc.Add(qubit);
        }
        return isQubit;
    }
    /// <summary>
    /// Just adds the GameObject into the list without checking. Can result in null values being put.
    /// </summary>
    protected void _addQubitUnsafe(GameObject possibleQubit) {
        var qubit = possibleQubit.GetComponent<Qubit>();

        _subcirc.Add(qubit);
    }
    /// <summary>
    /// Creates a qubit from prefab and adds it onto the list.
    /// </summary>
    protected Qubit _addQubitFromPrefab(float xOffset) {
        Qubit qubit = SpawnManager.Instance.MakeQubit(xOffset);

        _subcirc.Add(qubit);
        return qubit;
    }
    protected Qubit _addQubitFromPrefab(Vector3 position) {
        Qubit qubit = SpawnManager.Instance.MakeQubit(position);

        _subcirc.Add(qubit);
        return qubit;
    }

    /// <summary>
    /// Get a render texture of a qubit. Do note that it will not check if the index is out of bounds.
    /// </summary>
    public RenderTexture GetRenderTextureUnsafe(int qsIndex) {
        return _subcirc.GetRenderTexture(qsIndex, isQCIndex: false);
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
    /// Ask for one qubit.
    /// </summary>
    /// <param name="autoSuccess">
    /// Automatically execute success case if the operation wasn't cancelled.
    /// If put to false, user is responsible of executing success case.
    /// </param>
    public async UniTask<int> AskForSingleQubitIndex(bool autoSuccess=true) {
        _listenForNotNearGateCancellation = true;

        var (isCancelled, res) = await StageUIManager.Instance.WaitForSubmitResults(
            StageUIManager.QQVSubmitMode.Single, 
            _notNearGateCancellationSource.Token        
        );
        if (!isCancelled && autoSuccess) _gateOperationSuccess();

        _notNearGateCancellationSource.Dispose();
        _notNearGateCancellationSource = new CancellationTokenSource();

        return isCancelled ? -1 : res[0];
    }
    
    /// <summary>
    /// Ask for one qubit at a time. Goes until n times.
    /// </summary>
    public async UniTask<List<int>> AskForMultipleSingleQubitIndices(int n) {
        await UniTask.Yield();
        
        if (n > _subcirc.Count) return null;

        List<int> res = new List<int>(n);  // qubit indices

        for (int _ = 0; _ < n; ++_) {
            int qubitIndex = await AskForSingleQubitIndex(autoSuccess: false);
            if (qubitIndex == -1) break; // operation was cancelled
            
            StageUIManager.Instance.DisableQubitRepInteract(qubitIndex).Forget();
            res.Add(qubitIndex);
        }

        // reset
        UniTask.WhenAll(from i in res select StageUIManager.Instance.EnableQubitRepInteract(i)).Forget();

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
    /// TODO: Update the QSM of controllable.
    /// </summary>
    public async UniTask ApplyBinaryOperator(
        BinaryOperator binaryOperator, 
        int[] qsPair
    ) {
        await _subcirc.ApplyBinaryOperator(binaryOperator, qsPair, isQCPair: false);
    }
    #endregion Applying Methods
}