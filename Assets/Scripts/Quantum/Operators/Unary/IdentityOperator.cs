using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Quantum.Operators {
    /// <summary>
    /// Does nothing.
    /// </summary>
    public class IdentityOperator : UnaryOperator
    {
        private readonly Matrix<Complex> _matrixWithoutCoeff = Matrix<Complex>.Build.DenseIdentity(2);

        public override double Coefficient => 1;
        public override Matrix<Complex> Matrix => Coefficient * _matrixWithoutCoeff; 
    }
}