using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Minecraft
{
    internal class Biome
    {
        int[,] kusy_biomu;

        Katalog.TypBiomu MujTyp;

        Texture2D[] MojeObrazky;

        public Biome(Texture2D[][] obrazky, Katalog.TypBiomu typ)
        {
            MujTyp = typ;
            MojeObrazky = obrazky[(int)typ];

            Random nahoda = new Random();

            kusy_biomu = new int[5, 5];
            for (int r = 0; r < 5; r++)
                for (int s = 0; s < 5; s++)
                    kusy_biomu[r, s] = nahoda.Next(obrazky.Length);
        }
            
        public void Vykreslit(SpriteBatch davka)
        {
            for (int r = 0; r < 5; r++)
                for (int s = 0; s < 5; s++)
                    davka.Draw(MojeObrazky[kusy_biomu[r, s]],);
        }
    }
}
