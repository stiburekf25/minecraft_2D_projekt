using System;

namespace Minecraft
{
    // Šum (value noise): pro každý bod vrátí číslo 0..1, které se mění plynule.
    // Sousední místa mají podobnou hodnotu -> vznikají lesy, skupiny biomů atd.
    // "kanal" = různé nezávislé šumy (lesy, pole, teplota...).
    internal static class Sum
    {
        public static int Semeno = new Random().Next();   // každá hra má jiný svět

        // pseudonáhodné číslo 0..1 pro celočíselný bod (pořád stejné pro stejný bod)
        static double Hash(int x, int y, int kanal)
        {
            unchecked
            {
                uint h = (uint)(x * 374761393 + y * 668265263 + Semeno * 1442695041 + kanal * 1274126177);
                h = (h ^ (h >> 13)) * 1274126177;
                h ^= h >> 16;
                return h / (double)uint.MaxValue;
            }
        }

        static double Lerp(double a, double b, double t) => a + (b - a) * t;

        // meritko = jak velké jsou "skvrny" (v jednotkách souřadnic)
        public static double Hodnota(double x, double y, double meritko, int kanal)
        {
            x /= meritko;
            y /= meritko;

            int x0 = (int)Math.Floor(x);
            int y0 = (int)Math.Floor(y);
            double fx = x - x0;
            double fy = y - y0;

            // vyhlazení, ať nejsou vidět hrany mřížky
            fx = fx * fx * (3 - 2 * fx);
            fy = fy * fy * (3 - 2 * fy);

            double a = Hash(x0, y0, kanal);
            double b = Hash(x0 + 1, y0, kanal);
            double c = Hash(x0, y0 + 1, kanal);
            double d = Hash(x0 + 1, y0 + 1, kanal);

            return Lerp(Lerp(a, b, fx), Lerp(c, d, fx), fy);
        }
    }
}
