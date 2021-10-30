using sys = System;
using sysnum = System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

using Quantum.Operators;

namespace Quantum {
    public enum QuantumStateDescription {
        Ground, 
        Excited,
        Superposition,
        EntangledControl,
        EntangledTarget
    }

    /// <summary>
    /// A single quantum state stored in a 2D complex vector. 
    /// </summary> 
    public sealed class QuantumState
    {  
        #region Fields/Properties
        // used to describe its quantum state by itself
        private QuantumStateDescription _desc;
        public QuantumStateDescription Description => _desc;
        
        // used to describe its quantum state in correlation to other quantum states in a quantum system
        public bool IsEntangled {get; private set;}

        private Vector<sysnum.Complex> _state;
        public Vector<sysnum.Complex> State => _state;

        private double _probsZero;
        public double ProbsZero => _probsZero;

        private double _probsOne;
        public double ProbsOne => _probsOne;

        public static sys.Func<double, string> ProbabilityToStringFunc => x => x.ToString("#0.##%");
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
        public void ApplyUnaryOperator(UnaryOperator unaryOperator) {
            _state *= unaryOperator.Matrix;
            UpdateProbabilities();
        }

        /// <summary>
        /// Check if this quantum state qualifies for an entangled control, and if so, change it to that.
        /// </summary>
        public bool CheckIfEntangledCtrl() {
            bool res = _desc == QuantumStateDescription.Superposition;
            if (res) _desc = QuantumStateDescription.EntangledControl;

            return res;
        }
        /// <summary>
        /// Check if this quantum state is an entangled target, and if so, reflect it on its desc.
        /// </summary>
        public bool CheckIfEntangledTar(QuantumState qsControl) {
            bool res = qsControl._desc == QuantumStateDescription.EntangledControl;
            if (res && (_desc == QuantumStateDescription.Ground || _desc == QuantumStateDescription.Excited)) {
                _desc = QuantumStateDescription.EntangledTarget;
            }

            return res;
        }

        public void UpdateProbabilities() {
            _probsZero = ComplexExtensions.MagnitudeSquared(_state[0]).Round(2);
            _probsOne = ComplexExtensions.MagnitudeSquared(_state[1]).Round(2);

            if (_probsZero == 1) {
                _desc = QuantumStateDescription.Ground;
            }
            else if (_probsOne == 1) {
                _desc = QuantumStateDescription.Excited;
            }
            else {
                _desc = QuantumStateDescription.Superposition;
            }
        }
        #endregion Operations
        
        #region To Unity
        /// <summary>
        /// Turn the quantum state's position on the Bloch sphere to a Unity position.
        /// The y-axis on the Bloch sphere represents the z-axis in Unity.
        /// The z-axis on the Bloch sphere represents the y-axis in Unity.
        /// </summary>
        internal Vector3 ToUnityPosition(PauliXOperator xOperator, PauliYOperator yOperator, PauliZOperator zOperator) {
            var densityMatrix = MakeDensityMatrix();
            return new Vector3(
                (float)(xOperator.Matrix * densityMatrix).Trace().Real,
                (float)(zOperator.Matrix * densityMatrix).Trace().Real,
                (float)(yOperator.Matrix * densityMatrix).Trace().Real
            );
        }
        #endregion To Unity

        #region String Representations
        public (string, string) ProbabilitiesToString() {
            return (
                ProbabilityToStringFunc(_probsZero),
                ProbabilityToStringFunc(_probsOne)
            );
        }

        public string DescriptionToString() {
            switch (_desc) {
                case QuantumStateDescription.Ground:
                    return "Ground";
                case QuantumStateDescription.Excited:
                    return "Excited";
                case QuantumStateDescription.Superposition:
                    return _probsZero == 0.5 ? "Max Superposition" : "Superposition";
                case QuantumStateDescription.EntangledControl:
                    return "Entangled (Control)";
                case QuantumStateDescription.EntangledTarget:
                    return "Entangled (Target)";
                default:
                    return "";
            }
        }
        #endregion String Representations

        internal Matrix<sysnum.Complex> MakeDensityMatrix() {
            return _state.OuterProduct(_state);
        }

        private double _fixedArcCosine(double radians) {
            if (radians > 1) radians = 1;
            else if (radians < -1) radians = -1;

            return Trig.Acos(radians);
        }
    }
}