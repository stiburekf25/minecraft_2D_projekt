using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minecraft
{
    public enum TypMoba { Prase, Krava, Kure, Ovce, Zombie, Pavouk }

    // Zvíře nebo nepřítel. Pozice = střed moba ve světě.
    internal class Mob
    {
        const float DOHLED = 7 * Biome.DLAZDICE;    // na jakou vzdálenost nepřítel hráče vidí

        static Random nahoda = new Random();

        public TypMoba Typ;
        public Vector2 Pozice;
        public float Zdravi;
        public float CasZraneni;    // > 0 = mob bliká červeně

        Vector2 smer;               // kam se zrovna toulá
        float casSmeru;
        float casUteku;             // zraněné zvíře chvíli utíká
        float casUtoku;             // pauza mezi útoky nepřítele
        Vector2 odhoz;              // odhození po zásahu
        float uhel;                 // natočení obrázku podle směru pohybu
        bool hori;

        public Mob(TypMoba typ, Vector2 pozice)
        {
            Typ = typ;
            Pozice = pozice;
            Zdravi = MaxZdravi;
            uhel = (float)(nahoda.NextDouble() * Math.PI * 2);
        }

        public bool JeNepritel => Typ == TypMoba.Zombie || Typ == TypMoba.Pavouk;

        public float MaxZdravi
        {
            get
            {
                switch (Typ)
                {
                    case TypMoba.Kure: return 4;
                    case TypMoba.Zombie: return 20;
                    case TypMoba.Pavouk: return 16;
                    default: return 10;
                }
            }
        }

        public int Velikost => Typ == TypMoba.Kure ? 34 : Typ == TypMoba.Pavouk ? 50 : 46;

        // pixely za snímek (hráč chodí 3)
        float Rychlost
        {
            get
            {
                switch (Typ)
                {
                    case TypMoba.Zombie: return 1.7f;
                    case TypMoba.Pavouk: return 2.4f;
                    case TypMoba.Kure: return 0.9f;
                    default: return 0.7f;
                }
            }
        }

        public Rectangle Rect => new Rectangle((int)(Pozice.X - Velikost / 2), (int)(Pozice.Y - Velikost / 2), Velikost, Velikost);

        // vrátí, kolik poškození mob tento snímek dal hráči (0 = nic)
        public float Aktualizuj(float dt, Vector2 hrac, bool den)
        {
            CasZraneni -= dt;
            casSmeru -= dt;
            casUteku -= dt;
            casUtoku -= dt;

            Vector2 kHraci = hrac - Pozice;
            float vzdalenost = kHraci.Length();
            Vector2 pohyb;

            if (JeNepritel && vzdalenost < DOHLED && vzdalenost > 1)
            {
                pohyb = kHraci / vzdalenost * Rychlost;             // jde po hráči
            }
            else if (casUteku > 0 && vzdalenost > 1)
            {
                pohyb = -kHraci / vzdalenost * Rychlost * 2.2f;     // utíká od hráče
            }
            else
            {
                if (casSmeru <= 0) NovySmer();
                pohyb = smer * Rychlost;                            // toulá se
            }

            Pozice += pohyb + odhoz;
            odhoz *= 0.8f;

            if (pohyb.LengthSquared() > 0.01f)
                uhel = (float)Math.Atan2(pohyb.X, -pohyb.Y);       // obrázky koukají nahoru

            // nepřátelé na slunci hoří
            hori = JeNepritel && den;
            if (hori) Zdravi -= dt * 2;

            // útok na blízko
            if (JeNepritel && vzdalenost < Velikost / 2 + 22 && casUtoku <= 0)
            {
                casUtoku = Typ == TypMoba.Zombie ? 1f : 0.8f;
                return Typ == TypMoba.Zombie ? 3 : 2;
            }
            return 0;
        }

        void NovySmer()
        {
            casSmeru = 1.5f + (float)nahoda.NextDouble() * 3f;
            if (nahoda.NextDouble() < 0.4)
            {
                smer = Vector2.Zero;    // chvíli stojí
            }
            else
            {
                double u = nahoda.NextDouble() * Math.PI * 2;
                smer = new Vector2((float)Math.Cos(u), (float)Math.Sin(u));
            }
        }

        public void Zasah(float poskozeni, Vector2 odkud)
        {
            Zdravi -= poskozeni;
            CasZraneni = 0.25f;

            Vector2 smerOdhozu = Pozice - odkud;
            if (smerOdhozu != Vector2.Zero) smerOdhozu.Normalize();
            odhoz = smerOdhozu * 7;

            if (!JeNepritel) casUteku = 4;
        }

        // co padne, když ho hráč zabije
        public (Predmet Predmet, int Pocet) Drop()
        {
            switch (Typ)
            {
                case TypMoba.Prase: return (Predmet.Veprove, nahoda.Next(1, 4));
                case TypMoba.Krava: return (Predmet.Hovezi, nahoda.Next(1, 4));
                case TypMoba.Kure: return (Predmet.Kureci, 1);
                case TypMoba.Ovce: return (Predmet.Vlna, nahoda.Next(1, 3));
                case TypMoba.Zombie: return (Predmet.ShnileMaso, nahoda.Next(0, 3));
                case TypMoba.Pavouk: return (Predmet.Provazek, nahoda.Next(0, 3));
                default: return (Predmet.Nic, 0);
            }
        }

        public void Kresli(SpriteBatch sb, Vector2 kamera, float cas)
        {
            Texture2D textura = Textury.Moby[Typ];

            Color barva = Color.White;
            if (CasZraneni > 0) barva = new Color(255, 90, 90);
            else if (hori && (int)(cas * 8) % 2 == 0) barva = new Color(255, 170, 80);   // blikání ohně

            sb.Draw(textura, Pozice - kamera, null, barva, uhel,
                    new Vector2(textura.Width / 2f, textura.Height / 2f),
                    (float)Velikost / textura.Width, SpriteEffects.None, 0);
        }
    }
}
