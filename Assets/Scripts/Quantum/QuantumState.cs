using sys = System;
using sysnum = System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

using Quantum.Operators;

namespace Quantum {
    /// <summary>
    /// A single quantum state stored in a 2D complex vector. 
    /// </summary> 
    public sealed class QuantumState
    {  
        #region Fields/Properties
        private Vector<sysnum.Complex> _state;
        public Vector<sysnum.Complex> State => _state;

        private double _probsZero;
        public double ProbsZero => _probsZero;

        private double _probsOne;
        public double ProbsOne => _probsOne;
        #endregion Fields/Properties
        
        #region Constructors
        /// <summary>
        /// Construct a quantum state based on two probability amplitudes.
        /// </summary>
        public QuantumState(sysnum.Complex ampliZero, sysnum.Complex ampliOne, double scalarCoeff = 1) {
            _state = scalarCoeff * Vector<sysnum.Complex>.Build.Dense(new[] {ampliZero, ampliOne});
            UpdateProbabilities();
        }
        #endregion Constructors
        
        #region Operations
        public void ApplyUnaryOperator(UnaryOperator unaryOperator) {
            _state *= unaryOperator.Matrix;
            UpdateProbabilities();
        }

        public void UpdateProbabilities() {
            var ampliZero = _state[0].Real; var ampliOne = _state[1].Real;
            _probsZero = sys.Math.Pow(ampliZero, 2); _probsOne = sys.Math.Pow(ampliOne, 2);
        }
        #endregion Operations
        
        // #TODO Add toUnityRotation method
        #region To Unity
        /// <summary>
        /// Turn the quantum state's position on the Bloch sphere to a Unity position.
        /// The y-axis on the Bloch sphere represents the z-axis in Unity.
        /// The z-axis on the Bloch sphere represents the y-axis in Unity.
        /// </summary>
        internal Vector3 ToUnityPosition(PauliXOperator xOperator, PauliYOperator yOperator, PauliZOperator zOperator) {
            var densityMatrix = _densityMatrix();
            return new Vector3(
                (float)(xOperator.Matrix * densityMatrix).Trace().Real,
                (float)(zOperator.Matrix * densityMatrix).Trace().Real,
                (float)(yOperator.Matrix * densityMatrix).Trace().Real
            );
        }
        /// <summary> 
        /// Get Unity rotations based on the quantum state's position on the Bloch sphere.
        /// This is assuming that the GameObject (representing the quantum state) shows its
        /// positive x-, y-, and z-axis towards the camera.
        /// </summary>
        public void ToUnityRotation() {
            // return type should be Vector3; void is placeholder
        }
        #endregion To Unity

        #region String Representations
        public (string, string) ProbabilitiesToString() {
            sys.Func<double, string> toString = x => x.ToString("#0.##%");
            return (
                toString(_probsZero),
                toString(_probsOne)
            );
        }
        #endregion String Representations

        private Matrix<sysnum.Complex> _densityMatrix() {
            return _state.OuterProduct(_state);
        }
    }
}