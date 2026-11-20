using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minecraft
{
    // Ender drak. Žije v End biomu (3x3 biomy).
    //  - když je hráč venku, jen krouží nad Endem a neútočí
    //  - když je hráč v Endu, krouží kolem něj a občas se na něj vrhne
    //  - trefit ho jde hlavně, když se vrhá dolů k hráči
    internal class Drak
    {
        public const float MAX_ZDRAVI = 200;    // jako v MC
        const int VELIKOST = 180;
        const float POLOMER_KRUHU = 320;

        enum Stav { Krouzi, Utoci }

        static Random nahoda = new Random();

        public Vector2 Pozice;
        public float Zdravi = MAX_ZDRAVI;
        public float CasZraneni;
        public bool Zivy => Zdravi > 0;

        Rectangle oblast;           // End ve světových souřadnicích
        Stav stav = Stav.Krouzi;
        float casStavu = 4;
        float uhelKruhu;
        float casUtoku;             // pauza mezi zásahy hráče
        Vector2 cilUtoku;
        float uhel;                 // natočení obrázku

        public Drak(Rectangle oblast)
        {
            this.oblast = oblast;
            Pozice = oblast.Center.ToVector2();
        }

        // menší než obrázek, ať se počítají jen zásahy do těla
        public Rectangle Rect => new Rectangle((int)Pozice.X - VELIKOST / 3, (int)Pozice.Y - VELIKOST / 3, VELIKOST * 2 / 3, VELIKOST * 2 / 3);

        // vrátí poškození, které drak tento snímek dal hráči
        public float Aktualizuj(float dt, Vector2 hrac, bool hracVEndu)
        {
            CasZraneni -= dt;
            casStavu -= dt;
            casUtoku -= dt;

            if (!hracVEndu) stav = Stav.Krouzi;

            Vector2 cil;
            float rychlost;

            if (stav == Stav.Krouzi)
            {
                Vector2 stred = hracVEndu ? hrac : oblast.Center.ToVector2();
                uhelKruhu += dt * 0.8f;
                cil = stred + new Vector2((float)Math.Cos(uhelKruhu), (float)Math.Sin(uhelKruhu)) * POLOMER_KRUHU;
                rychlost = 4f;

                if (hracVEndu && casStavu <= 0)
                {
                    // vrhne se tam, kde hráč právě stojí (dá se uhnout)
                    stav = Stav.Utoci;
                    casStavu = 1.8f;
                    cilUtoku = hrac;
                }
            }
            else
            {
                cil = cilUtoku;
                rychlost = 7.5f;
                if (casStavu <= 0 || Vector2.Distance(Pozice, cilUtoku) < 10)
                {
                    stav = Stav.Krouzi;
                    casStavu = 3 + (float)nahoda.NextDouble() * 3;
                    uhelKruhu = (float)Math.Atan2(Pozice.Y - hrac.Y, Pozice.X - hrac.X);   // naváže na kruh tam, kde je
                }
            }

            Vector2 k = cil - Pozice;
            float d = k.Length();
            if (d > 1)
            {
                Vector2 krok = k / d * Math.Min(rychlost, d);
                Pozice += krok;
                uhel = (float)Math.Atan2(krok.X, -krok.Y);
            }

            // drak neopustí End
            Pozice.X = MathHelper.Clamp(Pozice.X, oblast.Left, oblast.Right);
            Pozice.Y = MathHelper.Clamp(Pozice.Y, oblast.Top, oblast.Bottom);

            // náraz do hráče
            if (hracVEndu && Vector2.Distance(Pozice, hrac) < VELIKOST / 3 + 25 && casUtoku <= 0)
            {
                casUtoku = 1.2f;
                return 6;
            }
            return 0;
        }

        public void Zasah(float poskozeni)
        {
            Zdravi -= poskozeni;
            CasZraneni = 0.2f;
        }

        public void Kresli(SpriteBatch sb, Vector2 kamera)
        {
            Texture2D t = Textury.Drak;
            Color barva = CasZraneni > 0 ? new Color(255, 100, 100) : Color.White;

            // stín pod drakem
            sb.Draw(t, Pozice - kamera + new Vector2(25, 35), null, Color.Black * 0.3f, uhel,
                    new Vector2(t.Width / 2f, t.Height / 2f), (float)VELIKOST / t.Width * 0.9f, SpriteEffects.None, 0);

            sb.Draw(t, Pozice - kamera, null, barva, uhel,
                    new Vector2(t.Width / 2f, t.Height / 2f), (float)VELIKOST / t.Width, SpriteEffects.None, 0);
        }
    }
}
