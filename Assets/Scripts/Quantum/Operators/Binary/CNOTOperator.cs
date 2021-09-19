using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Quantum.Operators {
    /// <summary>
    /// Apply a PauliX (NOT) operator on the target quantum state if the control quantum state is excited.
    /// </summary>
    public class CNOTOperator : BinaryOperator, IControlledOperator<BinaryOperator>
    {
        public BinaryOperator ControlledOpRef => this;
        public UnaryOperator TargetUnaryOperator => QuantumFactory.pauliXOperator;

        /*
        [1 0 0 0]
        [0 0 0 1]
        [0 0 1 0]
        [0 1 0 0]
        */
        private readonly Matrix<Complex> _matrixWithoutCoeff = Matrix<Complex>.Build.DenseOfArray(
            new Complex[,] {
                {new Complex(1, 0), new Complex(0, 0), new Complex(0, 0), new Complex(0, 0)}, 
                {new Complex(0, 0), new Complex(0, 0), new Complex(0, 0), new Complex(1, 0)},
                {new Complex(0, 0), new Complex(0, 0), new Complex(1, 0), new Complex(0, 0)},
                {new Complex(0, 0), new Complex(1, 0), new Complex(0, 0), new Complex(0, 0)}
            }
        );

        public override double Coefficient => 1;
        public override Matrix<Complex> Matrix => Coefficient * _matrixWithoutCoeff; 
    }
}