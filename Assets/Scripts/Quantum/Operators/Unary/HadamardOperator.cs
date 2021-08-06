using System.Numerics;
using mathnet = MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Quantum.Operators {
    /// <summary>
    /// Rotate on the Y-axis 90 degrees. Turns Z-axis basis states into X-axis basis states.
    /// </summary>
    public class HadamardOperator : UnaryOperator
    {
        private readonly Matrix<Complex> _matrixWithoutCoeff = Matrix<Complex>.Build.DenseOfArray(
            new Complex[,] {
                {new Complex(1, 0), new Complex(1, 0)}, 
                {new Complex(1, 0), new Complex(-1, 0)}
            }
        );

        public override double Coefficient => mathnet.Constants.Sqrt1Over2;
        public override Matrix<Complex> Matrix => Coefficient * _matrixWithoutCoeff; 
    }
}