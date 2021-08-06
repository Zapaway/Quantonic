using System;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Quantum.Operators {
    /// <summary>
    /// Rotate on the X-axis 180 degrees.
    /// </summary>
    public class PauliXOperator : UnaryOperator
    {
        private readonly Matrix<Complex> _matrixWithoutCoeff = Matrix<Complex>.Build.Dense(
            2, 2, (i, j) => new Complex(1f * Math.Abs(i - j), 0f)
        );

        public override double Coefficient => 1;
        public override Matrix<Complex> Matrix => Coefficient * _matrixWithoutCoeff; 
    }
}