using System.Numerics;
using static MathNet.Numerics.Constants;
using unity = UnityEngine;

using Quantum.Operators;

namespace Quantum {
    /// <summary>
    /// Use these to quickly make a basis state via factory.
    /// </summary>
    public enum BasisQuantumState : ushort {
        Ground,
        Excited,
        Plus,
        Minus,
        PlusImaginary,
        MinusImaginary
    }

    /// <summary>
    /// A factory to easily create quantum states and use quantum operators.
    /// </summary>
    public static class QuantumFactory {
        #region Operators
        public static readonly PauliXOperator pauliXOperator = new PauliXOperator();
        public static readonly PauliYOperator pauliYOperator = new PauliYOperator();
        public static readonly PauliZOperator pauliZOperator = new PauliZOperator();
        public static readonly HadamardOperator hadamardOperator = new HadamardOperator();
        public static readonly CNOTOperator cnotOperator = new CNOTOperator();
        public static readonly SWAPOperator swapOperator = new SWAPOperator();
        #endregion Operators
        
        #region State
        public static (QuantumState, UnaryQuantumStateDescription) MakeQuantumState(
            Complex ampliZero, 
            Complex ampliOne, 
            double scalarCoeff = 1
        ) 
        {
            QuantumState qa = new QuantumState(ampliZero, ampliOne, scalarCoeff);
            return (qa, qa.Description);
        }
        public static (QuantumState, UnaryQuantumStateDescription) MakeQuantumState(BasisQuantumState basis) {
            // set up amplitudes
            Complex ampliZero = (
                basis == BasisQuantumState.Excited ? 
                new Complex(0, 0) : 
                new Complex(1, 0)
            );
            Complex ampliOne = new Complex(0, 0); 
            switch (basis) {
                case BasisQuantumState.Ground: // already covered in expression of ampliZero
                    break; 
                case BasisQuantumState.Excited:
                case BasisQuantumState.Plus: 
                    ampliOne = new Complex(1, 0);
                    break;
                case BasisQuantumState.Minus:
                    ampliOne = new Complex(-1, 0);
                    break;
                case BasisQuantumState.PlusImaginary:
                    ampliOne = new Complex(0, 1);
                    break;
                case BasisQuantumState.MinusImaginary:
                    ampliOne = new Complex(0, -1);
                    break;
            }

            // get the scalar coefficient
            double scalarCoeff = (
                basis == BasisQuantumState.Ground ||
                basis == BasisQuantumState.Excited
            ) ? 1 : Sqrt1Over2;

            QuantumState qa = new QuantumState(ampliZero, ampliOne, scalarCoeff);
            return (qa, qa.Description);
        }

        /// <summary>
        /// Easily get Unity position of a quantum state.
        /// </summary>
        public static unity.Vector3 GetUnityPosition(QuantumState state) {
            return state.ToUnityPosition(
                pauliXOperator,
                pauliYOperator,
                pauliZOperator
            );
        }
        #endregion State
    }
}