using System;

namespace AliYil.Mandelbrot
{
    class Mandelbrot
    {
        public int TotalIterations = 300;
        public int Limit = 30000000;
        public double Calculate(double x, double y)
        {
            var a = x;
            var b = y;

            double z0 = 0;
            double z1 = 0;
            //double z2 = 0;
            double i = 0;
            while (i < TotalIterations)
            {
                var bb = 2 * a * b;
                a = a * a - b * b + x;
                b = bb + y;

                //a = Math.Tan(a);
                //a++;
                z1 = z0;
                z0 = Math.Abs(a + b);
                //if (Math.Abs(z0 - z2) <= 0)
                //    return TotalIterations;
                if (z0 == z1)
                    return 1;
                if (z1 > Limit) return i / TotalIterations;
                i++;
            }
            return i / TotalIterations;
        }

        private double Z(double a, double b, int iteration = 0, double? previous = null)
        {
            throw new Exception();
        }
    }
}
