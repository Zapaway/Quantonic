using sys = System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Cysharp.Threading.Tasks;
using sysnum = System.Numerics;
using mathnetl = MathNet.Numerics.LinearAlgebra;

using Managers;
using Quantum;
using Quantum.Operators;

/*
TODO:
    // temp; uses position to determine representation on bloch sphere
    // todo; use rotation to determine representation on bloch sphere
*/

/// <summary>
/// Represents a single quantum state on a Bloch sphere.
/// </summary>
public sealed class Qubit : MonoBehaviour
{
    // gameobjects & unity data
    [SerializeField] private BasisQuantumState _initialState;  // default is the ground state
    [SerializeField] private GameObject _quantumStateIndicator;  
    [SerializeField] private GameObject _blochSphere; 
    [SerializeField] private Camera _camera;
    /* cache coords for determining bloch sphere pos -> unity pos 
     will always be in the form (x, 0, 0) */
    private Vector3 _blochSphereCoords;  
    public Camera Camera => _camera;

    // render texture
    private static RenderTextureDescriptor _renderTextureDesc = new RenderTextureDescriptor(
        256,
        256,
        GraphicsFormat.R8G8B8A8_UNorm,
        24
    );  // use this for all instances of qubit render textures
    private RenderTexture _renderTexture;
    public RenderTexture RenderTexture => _renderTexture;

    // quantum state data
    private QuantumState _quantumState;
    public mathnetl.Vector<sysnum.Complex> QuantumStateVector => _quantumState.State;
    public QuantumStateDescription Description => _quantumState.Description;
    public string DescriptionString => _quantumState.DescriptionToString();
    public (double ground, double excited) Probabilities => (_quantumState.ProbsZero, _quantumState.ProbsOne);

    // entangled pair data
    private (Qubit control, Qubit target)? _entangledPair = null;
    public (Qubit control, Qubit target) EntangledPair => (_entangledPair?.control, _entangledPair?.target);

    private void Awake() {
        // initalize render texture
        _renderTexture = new RenderTexture(_renderTextureDesc);
        _camera.targetTexture = _renderTexture;
        
        // initalize rest of qubit information
        _blochSphereCoords = Vector3.right * _blochSphere.transform.position.x;
        ResetQuantumState();
    }

    #region Quantum State Operations
    /// <summary>
    /// After applying the unary operator, update its position. This method by itself 
    /// does not notify Controllable of any changes.
    /// </summary>
    public async UniTask ApplyUnaryOperator(UnaryOperator unaryOperator) {
        await UniTask.Yield();

        _quantumState.ApplyUnaryOperator(unaryOperator);
        _updatePos();
    }

    /// <summary>
    /// Transform this qubit into an entangled control and the other qubit to an entangled target.
    /// </summary>
    /// <returns> Successfully turned both qubits into an entangled pair or not. (If they are already transformed, return false.) </returns>
    public bool AttemptTransformToControl(Qubit attemptTarget) {
        if (_entangledPair != null) return false;

        _quantumState.CheckIfEntangledCtrl();
        bool res = attemptTarget._quantumState.CheckIfEntangledTar(_quantumState);
        if (res) {
            attemptTarget._entangledPair = _entangledPair = (this, attemptTarget);
        }
        
        return res;
    }

    /// <summary>
    /// Revert this qubit back to its state and kill the entangled pair. Note that this will not revert the target qubit's quantum state desc.
    /// </summary>
    /// <returns> Successfully removed the entangled pair. (If they are already removed, return false.) </returns>
    public bool AttemptRevertToRegular() {
        if (_entangledPair == null) return false;

        bool res = _quantumState.Description == QuantumStateDescription.EntangledControl;
        if (res) {
            _quantumState.UpdateProbabilities();
            _setBothQubitsEntangledPairsNull();
        }

        return res;
    }

    /// <summary>
    /// Set a quantum state for the qubit.
    /// </summary>
    public Qubit SetQuantumState(QuantumState state) {
        _quantumState = state;
        _updatePos();

        return this;
    }

    /// <summary>
    /// Set the quantum state to the default one.
    /// </summary>
    public Qubit ResetQuantumState() {
        _setBothQubitsEntangledPairsNull();
        return SetQuantumState(QuantumFactory.MakeQuantumState(_initialState));
    }
    #endregion Quantum State Operations

    private void _updatePos() {
        Vector3 unityPos = QuantumFactory.GetUnityPosition(_quantumState);
        _quantumStateIndicator.transform.position = unityPos + _blochSphereCoords;
        _quantumState.ToUnityRotation();
    }

    private void _setBothQubitsEntangledPairsNull() {
        _entangledPair?.target._setEntangledPairNull();
        _entangledPair = null;
    }
    private void _setEntangledPairNull() {
        _entangledPair = null;
    }
}
