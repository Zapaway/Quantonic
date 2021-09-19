using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Quantum.Operators {
    /// <summary>
    /// All types of quantum operators derive from this.
    /// </summary>
    public abstract class QuantumOperator
    {
        // coefficient that is applied to the matrix
        public abstract double Coefficient {get;}

        // matrix of the operator (coefficient is applied onto the matrix)
        public abstract Matrix<Complex> Matrix {get;}  
    }

    public abstract class UnaryOperator : QuantumOperator {}  // single-qubit gates   
    public abstract class BinaryOperator : QuantumOperator {}  // two-qubit gates
    public abstract class MultiOperator : QuantumOperator {}  // multi-qubit gates

    public interface IControlledOperator<T> where T : QuantumOperator {
        T ControlledOpRef {get;}

        /// <summary>
        /// The unary operator to apply if the control qubit is not grounded.
        /// </summary>
        UnaryOperator TargetUnaryOperator {get;}
    }
}