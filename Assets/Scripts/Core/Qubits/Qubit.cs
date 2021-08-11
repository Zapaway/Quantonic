using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Managers;
using Quantum;
using Quantum.Operators;

/// <summary>
/// Represents a quantum state on a Bloch sphere.
/// </summary>
public sealed class Qubit : MonoBehaviour
{
    [SerializeField] private BasisQuantumState _initialState;  // default is the ground state
    [SerializeField] private GameObject _quantumStateIndicator;  
    [SerializeField] private GameObject _blochSphere; 
    private Vector3 _blochSphereCoords;  // cache coords for determining bloch sphere pos -> unity pos 
    [SerializeField] private Camera _camera;
    [SerializeField] private RenderTexture _renderTexture;
    // temp; uses position to determine representation on bloch sphere
    // todo; use rotation to determine representation on bloch sphere
    private QuantumState _quantumState;


    private void Awake() {
        _quantumState = QuantumFactory.MakeQuantumState(_initialState);
        _camera.targetTexture = _renderTexture;
        _blochSphereCoords = Vector3.right * _blochSphere.transform.position.x;
    }

    /// <summary>
    /// After applying the unary operator, update its position. This does not notify Controllable of any changes.
    /// </summary>
    public void ApplyUnaryOperator(UnaryOperator unaryOperator) {
        _quantumState.ApplyUnaryOperator(unaryOperator);
        Vector3 unityPos = QuantumFactory.GetUnityPosition(_quantumState);
        _quantumStateIndicator.transform.position = unityPos + _blochSphereCoords;
    }
}
