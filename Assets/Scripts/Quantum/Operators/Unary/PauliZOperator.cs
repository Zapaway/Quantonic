using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Quantum.Operators {
    /// <summary>
    /// Rotate on the Z-axis 180 degrees.
    /// </summary>
    public class PauliZOperator : UnaryOperator
    {
        private readonly Matrix<Complex> _matrixWithoutCoeff = Matrix<Complex>.Build.DenseOfArray(
            new Complex[,] {
                {new Complex(1, 0), new Complex(0, 0)}, 
                {new Complex(0, 0), new Complex(-1, 0)}
            }
        );

        public override double Coefficient => 1;
        public override Matrix<Complex> Matrix => Coefficient * _matrixWithoutCoeff; 
    }
}