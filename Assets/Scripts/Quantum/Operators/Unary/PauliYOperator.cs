  using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Quantum.Operators {
    /// <summary>
    /// Rotate on the Y-axis 180 degrees.
    /// </summary>
    public class PauliYOperator : UnaryOperator
    {
        private readonly Matrix<Complex> _matrixWithoutCoeff = Matrix<Complex>.Build.Dense(
            2, 2, (i, j) => new Complex(0, (i - j))
        );

        public override double Coefficient => 1;
        public override Matrix<Complex> Matrix => Coefficient * _matrixWithoutCoeff; 
    }
}