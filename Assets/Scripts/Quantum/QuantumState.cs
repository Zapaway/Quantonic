using sys = System;
using sysnum = System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

using Quantum.Operators;

namespace Quantum {
    public enum UnaryQuantumStateDescription {
        Ground, 
        Excited,
        Superposition,
        MaxSuperposition
    }

    /// <summary>
    /// A single quantum state stored in a 2D complex vector. 
    /// </summary> 
    public sealed class QuantumState
    {  
        #region Fields/Properties
        // used to describe its quantum state by itself
        private UnaryQuantumStateDescription _desc;
        public UnaryQuantumStateDescription Description => _desc;
        
        // used to describe its quantum state in correlation to other quantum states in a quantum system
        public bool IsEntangled {get; private set;}

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
            IsEntangled = false;
            UpdateProbabilities();
        }
        #endregion Constructors
        
        #region Operations
        public UnaryQuantumStateDescription ApplyUnaryOperator(UnaryOperator unaryOperator) {
            _state *= unaryOperator.Matrix;
            UpdateProbabilities();

            return _desc;
        }

        public void UpdateProbabilities() {
            _probsZero = ComplexExtensions.MagnitudeSquared(_state[0]); 
            _probsOne = ComplexExtensions.MagnitudeSquared(_state[1]);

            if (_probsZero == 1) {
                _desc = UnaryQuantumStateDescription.Ground;
            }
            else if (_probsOne == 1) {
                _desc = UnaryQuantumStateDescription.Excited;
            }
            else if (_probsZero == 0.5) {
                _desc = UnaryQuantumStateDescription.MaxSuperposition;
            }
            else {
                _desc = UnaryQuantumStateDescription.Superposition;
            }
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

        public string DescriptionToString() {
            switch (_desc) {
                case UnaryQuantumStateDescription.Ground:
                    return "Ground";
                case UnaryQuantumStateDescription.Excited:
                    return "Excited";
                case UnaryQuantumStateDescription.Superposition:
                    return "Superposition";
                case UnaryQuantumStateDescription.MaxSuperposition:
                    return "Max Superposition";
                default:
                    return "";
            }
        }
        #endregion String Representations

        private Matrix<sysnum.Complex> _densityMatrix() {
            return _state.OuterProduct(_state);
        }
    }
}