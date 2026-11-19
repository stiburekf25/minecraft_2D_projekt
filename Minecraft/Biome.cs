using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Minecraft
{
    internal class Biome
    {
        public const int POCET = 10;                     
        public const int DLAZDICE = 64;                 
        public const int VELIKOST = POCET * DLAZDICE;   

        static Random nahoda = new Random();

        // pořadí odpovídá enumům: Louka = Trava, Drevo, Pole | Doly = Kamen, Zlato, Diamanty
        static readonly int[][] vahy = new int[][]
        {
        new int[] { 85, 10, 5 },   // Louka: 70 % tráva, 20 % dřevo, 10 % pole
        new int[] { 85, 10, 5 },     // Doly:  80 % kámen, 15 % zlato, 5 % diamanty
        new int[] { 85, 10, 5 }     // Doly:  80 % kámen, 15 % zlato, 5 % diamanty
        };

        int[,] kusy_biomu;
        Katalog.TypBiomu MujTyp;
        Texture2D[] MojeObrazky;
        Vector2 pozice;                                 

        public Biome(Texture2D[][] obrazky, Katalog.TypBiomu typ, Vector2 pozice)
        {
            MujTyp = typ;
            MojeObrazky = obrazky[(int)typ];
            this.pozice = pozice;

            kusy_biomu = new int[POCET, POCET];
            for (int r = 0; r < POCET; r++)
                for (int s = 0; s < POCET; s++)
                    kusy_biomu[r, s] = VyberTexturu(vahy[(int)typ]);
        }

        public void Vykreslit(SpriteBatch davka, Vector2 kamera)
        {
            for (int r = 0; r < POCET; r++)
                for (int s = 0; s < POCET; s++)
                    davka.Draw(
                        MojeObrazky[kusy_biomu[r, s]],
                        new Rectangle(
                            (int)(pozice.X + s * DLAZDICE - kamera.X),
                            (int)(pozice.Y + r * DLAZDICE - kamera.Y),
                            DLAZDICE, DLAZDICE),
                        Color.White);
        }

        static int VyberTexturu(int[] vahy_biomu)
        {
            int soucet = 0;
            foreach (int v in vahy_biomu)
                soucet += v;

            int hod = nahoda.Next(soucet);   

            for (int i = 0; i < vahy_biomu.Length; i++)
            {
                if (hod < vahy_biomu[i])
                    return i;
                hod -= vahy_biomu[i];
            }

            return 0; 
        }
    }
}