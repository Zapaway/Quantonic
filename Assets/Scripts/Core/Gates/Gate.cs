using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;
using Quantum.Operators;

/// <summary>
/// All gate GameObjects inherit from this.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Gate<T> : MonoBehaviour where T : QuantumOperator {
    private float _offsetWhenExit = 2.55f;  // used to calculate end pos of lerp animation 
    private float _lerpSpeed = 5;  // how fast should the animation be
    private Controllable _occupiedControllable;
    protected Controllable OccupiedControllable => _occupiedControllable;
    protected T _operator;  

    /// <summary>
    /// Execute its gate action.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision) {
        var currCtrlable = StageControlManager.Instance.CurrentControllable;

        if (
            collision.gameObject.CompareTag("Controllable") && 
            _occupiedControllable == null &&
            currCtrlable.QubitCount >= Capacity  // ensure that there are enough qubits to be used 
        ) {
            _occupiedControllable = currCtrlable;
            GateCollisionAction(collision).Forget();
        }
    }

    /// <summary>
    /// Stop the gate action.
    /// </summary>
    private void OnCollisionExit2D(Collision2D other) {
        if (_occupiedControllable != null) {
            // if the controllable exits without going onto the other side, they had definitely canceled the prompt
            if (!_occupiedControllable.reachedOtherSideOfGate) { 
                _occupiedControllable.CancelForNotBeingNearGate();
            }

            // always reset the parameters
            _occupiedControllable.reachedOtherSideOfGate = false;
            _occupiedControllable = null;
        }
    }

    /// <summary>
    /// When a controllable enters the gate, all input for it should be disabled until they get out.
    /// It should also play the simple lerp animation.
    /// Any gates deriving from this should focus on executing their actions.
    /// </summary>
    protected async virtual UniTaskVoid GateCollisionAction(Collision2D collision) {
        if (collision.gameObject.CompareTag("Controllable")) {
            StageControlManager.Instance.SetPlayingControlsActive(false);

            await _playAnimation(collision.gameObject);

            StageControlManager.Instance.SetPlayingControlsActive(true);
            
            await StageUIManager.Instance.RefreshAllQubitRepresentationsUnsafe();
        }
    }
    
    #region Abstract Properties and Methods
    /// <value>
    /// How many qubits needed to use this gate. 
    /// </value>
    public abstract int Capacity {get;} 

    /// <summary>
    /// Apply the operator onto the QuantumState(s) via controllable methods.
    /// Indices in the array represent the specific qubits on the controllable.
    /// </summary>
    protected abstract void _apply(Controllable controllable, int[] qsIndices);

    // protected abstract UniTaskVoid GateCollisionAction(Collision2D collision);
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