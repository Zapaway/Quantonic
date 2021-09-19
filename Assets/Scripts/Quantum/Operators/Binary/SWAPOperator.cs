using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Quantum.Operators {
    /// <summary>
    /// Swap quantum states.
    /// </summary>
    public class SWAPOperator : BinaryOperator
    {        
        /*
        [1 0 0 0]
        [0 0 1 0]
        [0 1 0 0]
        [0 0 0 1]
        */
        private readonly Matrix<Complex> _matrixWithoutCoeff = Matrix<Complex>.Build.DenseOfArray(
            new Complex[,] {
                {new Complex(1, 0), new Complex(0, 0), new Complex(0, 0), new Complex(0, 0)}, 
                {new Complex(0, 0), new Complex(0, 0), new Complex(1, 0), new Complex(0, 0)},
                {new Complex(0, 0), new Complex(1, 0), new Complex(0, 0), new Complex(0, 0)},
                {new Complex(0, 0), new Complex(0, 0), new Complex(0, 0), new Complex(1, 0)}
            }
        );

        public override double Coefficient => 1;
        public override Matrix<Complex> Matrix => Coefficient * _matrixWithoutCoeff; 
    }
}