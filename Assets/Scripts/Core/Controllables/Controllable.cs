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

    protected virtual void Awake() {
        _subcirc = ControlManager.Instance.circ.CreateQubitSubcircuit(this);
    }

    protected virtual void Update() {
        if (ControlManager.Instance.IsJumpTriggered()) {
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
    /// Ask the controllable qubit(s) to use.
    /// </summary>
    public async UniTask<int> AskForSingleQubitIndex() {
        StageUIManager ui = StageUIManager.Instance;

        _listenForNotNearGateCancellation = true;
        var (isCancelled, res) = await ui.WaitForSubmitResults(
            StageUIManager.QQVSubmitMode.Single, 
            _notNearGateCancellationSource.Token
        );
        if (!isCancelled) {
            reachedOtherSideOfGate = true;
            _listenForNotNearGateCancellation = false;  
        }

        _notNearGateCancellationSource.Dispose();
        _notNearGateCancellationSource = new CancellationTokenSource();

        return isCancelled ? -1 : res[0];
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
    #endregion Applying Methods
}