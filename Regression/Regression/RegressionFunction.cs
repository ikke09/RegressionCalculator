using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RuntimeFunctionParser;
using Math_Collection;
using Math_Collection.LinearAlgebra.Vectors;
using Math_Collection.LinearAlgebra.Matrices;
using Math_Collection.LGS;

namespace Regression
{
    static class RegressionFunction
    {
        /// <summary>
        /// Berechnet die Regression für die gegebenen Punkte
        /// </summary>
        /// <param name="?"></param>
        /// <param name="degreeOfPolynominal"></param>
        /// <returns></returns>
        public static Function CalculateRegression(List<Point> points, int degreeOfPolynominal = 1)
        {
            if (points == null || points.Count < 2)
                return null;

            Matrix inputMatrix = new Matrix(new double[degreeOfPolynominal + 1, degreeOfPolynominal + 1]);
            Math_Collection.LinearAlgebra.Vectors.Vector outputVector =
                new Math_Collection.LinearAlgebra.Vectors.Vector(new double[degreeOfPolynominal + 1]);

            int maxPower = degreeOfPolynominal * 2;
            int actualPower = maxPower;
            // Input Matrix aufbauen
            for (int i = 0; i < inputMatrix.RowCount; i++)
            {
                for (int k = 0; k < inputMatrix.ColumnCount; k++)
                {
                    actualPower = maxPower - (i + k);
                    inputMatrix[i, k] = SumXValues(points, actualPower);

                }
            }

            maxPower = degreeOfPolynominal;
            // Output Vector aufbauen
            for (int m = 0; m < outputVector.Size; m++)
            {
                outputVector[m] = SumXYValues(points, maxPower);
                maxPower--;
            }

            // LGS Lösen
            LGS lgs = new LGS(inputMatrix, outputVector);
            Math_Collection.LinearAlgebra.Vectors.Vector resultVector = lgs.Solve();

            // Function aufbauen
            // ax^1 + b; ax^2+bx^1+c
            string functionString = "";
            maxPower = degreeOfPolynominal;
            for (int o = 0; o <= degreeOfPolynominal; o++)
            {
                if (o != degreeOfPolynominal)
                {
                    functionString += string.Format(resultVector[o] + "*x^{0}+", maxPower);
                }
                else
                {
                    functionString += resultVector[o];
                }

                maxPower--;
            }
            functionString = functionString.Replace(",", ".");

            Parser parser = new Parser();
            return parser.ParseFunction(functionString);
        }

        private static double SumXValues(List<Point> points, int power)
        {
            double rtnValue = 0;
            foreach (Point p in points)
            {
                rtnValue += Math.Pow(p.X, power);
            }
            return rtnValue;
        }

        private static double SumYValues(List<Point> points, int power)
        {
            double rtnValue = 0;
            foreach (Point p in points)
            {
                rtnValue += Math.Pow(p.Y, power);
            }
            return rtnValue;
        }

        private static double SumXYValues(List<Point> points, int power)
        {
            double rtnValue = 0;
            foreach (Point p in points)
            {
                rtnValue += Math.Pow(p.X, power) * p.Y;
            }
            return rtnValue;
        }
    }
}
