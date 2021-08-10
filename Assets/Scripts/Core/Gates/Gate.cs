using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;
using Quantum.Operators;

/// <summary>
/// All gate GameObjects inherit from this.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Gate<T> : MonoBehaviour where T : QuantumOperator {
    private float _offsetWhenExit = 1;  // used to calculate end pos of lerp animation 
    private float _lerpSpeed = 5;  // how fast should the animation be
    protected T _operator;  

    /// <summary>
    /// When a controllable enters the gate, all input for it should be disabled until they get out.
    /// It should also play the simple lerp animation.
    /// Any gates deriving from this should focus on executing 
    /// </summary>
    protected async virtual UniTaskVoid OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Controllable")) {
            GameManager.Instance.StageInputs.Controllable.Disable();
            await _playAnimation(other.gameObject);
            GameManager.Instance.StageInputs.Controllable.Enable();
        }
    }

    #region Abstract Properties and Methods
    /// <value>
    /// How many qubits needed to use this gate. 
    /// </value>
    public abstract int Capacity {get;} 

    /// <summary>
    /// Apply the operator onto the QuantumState(s) via controllable methods.
    /// Indices represent the specific qubits on the controllable.
    /// </summary>
    protected abstract void _apply(Controllable controllable, int[] indices);
    #endregion Abstract Properties and Methods

    /// <summary>
    /// Play the lerp animation.
    /// </summary>
    private async UniTask _playAnimation(GameObject controllable) {
        Vector3 startPos = controllable.transform.position;
        (Vector3 endPos, float journeyLength) = _calculateEndPosition(startPos);
        float startTime = Time.time;

        float fractionOfJourney;
        do {
            float distanceCovered = (Time.time - startTime) * _lerpSpeed;
            fractionOfJourney = distanceCovered / journeyLength;
            controllable.transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);
            await UniTask.Yield();
        } while (fractionOfJourney < 1);
    } 

    private Tuple<Vector3, float> _calculateEndPosition(Vector3 startPos) {
        bool enteringFromLeft = startPos.x < transform.position.x;
        float distance = transform.localScale.x + _offsetWhenExit;

        return new Tuple<Vector3, float>(
            new Vector3(
            startPos.x + (enteringFromLeft ? distance : -distance),
            startPos.y,
            startPos.z
            ),  
            distance
        );
    }
}