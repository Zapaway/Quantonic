using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

using Managers;
using Quantum;
using Quantum.Operators;

/// <summary>
/// Represents a quantum state on a Bloch sphere.
/// </summary>
public sealed class Qubit : MonoBehaviour
{
    private static RenderTextureDescriptor _renderTextureDesc = new RenderTextureDescriptor(
        256,
        256,
        GraphicsFormat.R8G8B8A8_UNorm,
        24
    );  // use this for all instances of qubit render textures
    private RenderTexture _renderTexture;
    public RenderTexture RenderTexture => _renderTexture;

    [SerializeField] private BasisQuantumState _initialState;  // default is the ground state
    [SerializeField] private GameObject _quantumStateIndicator;  
    [SerializeField] private GameObject _blochSphere; 
    [SerializeField] private Camera _camera;

    // cache coords for determining bloch sphere pos -> unity pos 
    // will always be in the form (x, 0, 0)
    private Vector3 _coords;  
    public Camera Camera => _camera;

    // temp; uses position to determine representation on bloch sphere
    // todo; use rotation to determine representation on bloch sphere
    private QuantumState _quantumState;
    public QuantumStateDescription Description {get; private set;}

    private void Awake() {
        // initalize render texture
        _renderTexture = new RenderTexture(_renderTextureDesc);
        _camera.targetTexture = _renderTexture;
        
        // initalize rest of qubit information
        (_quantumState, Description) = QuantumFactory.MakeQuantumState(_initialState);
        _coords = Vector3.right * _blochSphere.transform.position.x;  
    }

    /// <summary>
    /// After applying the unary operator, update its position. This does not notify Controllable of any changes.
    /// </summary>
    public void ApplyUnaryOperator(UnaryOperator unaryOperator) {
        Description = _quantumState.ApplyUnaryOperator(unaryOperator);
        Vector3 unityPos = QuantumFactory.GetUnityPosition(_quantumState);
        _quantumStateIndicator.transform.position = unityPos + _coords;
    }
}
