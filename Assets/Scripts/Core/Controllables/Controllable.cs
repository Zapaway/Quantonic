using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;
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
    protected readonly ObservableCollection<Qubit> _qubits = new ObservableCollection<Qubit>();   

    private CancellationTokenSource _notNearGateCancellationSource = new CancellationTokenSource();
    private bool _listenForNotNearGateCancellation = false;  // makes sure cancellation of a token doesn't happen twice
    public bool reachedOtherSideOfGate = false;

    protected virtual void Awake() {
    }

    protected virtual void Update() {
        if (ControlManager.Instance.JumpTriggered()) {
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

    #region Qubit Collection Manipulation
    /// <summary>
    /// Checks if the GameObject is a qubit and returns it. If true, it adds it onto the list.
    /// </summary>
    protected bool _addQubitSafe(GameObject possibleQubit) {
        bool isQubit = possibleQubit.CompareTag("Qubit");
        if (isQubit) {
            var qubit = possibleQubit.GetComponent<Qubit>();

            _qubits.Add(qubit);
        }
        return isQubit;
    }
    /// <summary>
    /// Just adds the GameObject into the list without checking. Can result in null values being put.
    /// </summary>
    protected void _addQubitUnsafe(GameObject possibleQubit) {
        var qubit = possibleQubit.GetComponent<Qubit>();

        _qubits.Add(qubit);
    }
    /// <summary>
    /// Creates a qubit from prefab and adds it onto the list.
    /// </summary>
    protected Qubit _addQubitFromPrefab(float xOffset) {
        Qubit qubit = SpawnManager.Instance.MakeQubit(xOffset);

        _qubits.Add(qubit);
        return qubit;
    }
    protected Qubit _addQubitFromPrefab(Vector3 position) {
        Qubit qubit = SpawnManager.Instance.MakeQubit(position);

        _qubits.Add(qubit);
        return qubit;
    }

    /// <summary>
    /// Get a render texture of a qubit. Do note that it will not check if the index is out of bounds.
    /// </summary>
    public RenderTexture GetRenderTextureUnsafe(int index) {
        return _qubits[index].RenderTexture;
    }

    public int GetQubitCount() {
        return _qubits.Count;
    }
    /// <summary>
    /// Add an event subscriber to the observable qubit collection.
    /// </summary>
    public void SubscribeToQubitCollection(NotifyCollectionChangedEventHandler eventHandler) {
        _qubits.CollectionChanged += eventHandler;
    }
    /// <summary>
    /// Remove an event subscriber from the observable qubit collection.
    /// </summary>
    public void UnsubscribeToQubitCollection(NotifyCollectionChangedEventHandler eventHandler) {
        _qubits.CollectionChanged -= eventHandler;
    }
    #endregion Qubit Collection Manipulation
    
    #region Select Qubits
    /// <summary>
    /// Ask the controllable what single qubit to use.
    /// (For now, use the first qubit in the controllable)
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

    /// <summary>
    /// Cancels the not-being-near-gate source ONLY if it is being listened to.
    /// </summary>
    public void CancelForNotBeingNearGate() {
        if (_listenForNotNearGateCancellation) { 
            _listenForNotNearGateCancellation = false;
            _notNearGateCancellationSource.Cancel();
        }
    }
    #endregion Select Qubits

    // using these methods will automatically notify the controllable to check its qubit state
    #region Applying Methods
    public void ApplyUnaryOperator(UnaryOperator unaryOperator, int index) {
        _qubits[index].ApplyUnaryOperator(unaryOperator);

        // TODO: update qubitstatemachine
    }
    #endregion Applying Methods
}