using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Minecraft
{
    // co z bloku padne, jak dlouho se těží (v sekundách holou rukou) a jaký krumpáč je potřeba
    internal class InfoBloku
    {
        public Predmet Drop;
        public float Tvrdost;
        public int PotrebnaSila;

        public InfoBloku(Predmet drop, float tvrdost, int potrebnaSila)
        {
            Drop = drop;
            Tvrdost = tvrdost;
            PotrebnaSila = potrebnaSila;
        }
    }

    internal class Biome
    {
        public const int POCET = 10;
        public const int DLAZDICE = 64;
        public const int VELIKOST = POCET * DLAZDICE;

        // vytěžené políčko (nakreslí se jako ztmavený základní blok)
        public const int VYTEZENO = -1;

        static Random nahoda = new Random();

        // vesnice: dvě chatky, cesta a políčka
        //  L = dřevo (rohy), P = prkna (stěny), # = cesta, S = pracovní stůl, C = truhla,
        //  _ = volná podlaha uvnitř, F = pole, . = nechat, co tam je
        static readonly string[] VESNICE =
        {
            "LPPPL.FFF.",
            "P_S_P.FFF.",
            "P___P.FFF.",
            "LP#PL.....",
            "##########",
            ".....LP#PL",
            ".FFF.P___P",
            ".FFF.P__CP",
            ".....LPPPL",
            "..........",
        };

        const double SANCE_NA_VESNICI = 0.25;   // 25 % luk má vesnici

        int[,] kusy_biomu;
        Predmet[,] polozene;        // bloky, které tu položil hráč (Nic = nic tu neleží)
        Dictionary<Point, Slot[]> truhly = new Dictionary<Point, Slot[]>();   // obsah truhel, klíč = (sloupec, řádek)
        Katalog.TypBiomu MujTyp;
        Texture2D[] MojeObrazky;
        Vector2 pozice;
        int svetX, svetY;           // souřadnice levého horního políčka v dlaždicích celého světa

        public Biome(Texture2D[][] obrazky, Katalog.TypBiomu typ, Vector2 pozice)
        {
            MujTyp = typ;
            MojeObrazky = obrazky[(int)typ];
            this.pozice = pozice;
            svetX = (int)pozice.X / DLAZDICE;
            svetY = (int)pozice.Y / DLAZDICE;

            kusy_biomu = new int[POCET, POCET];
            polozene = new Predmet[POCET, POCET];

            switch (typ)
            {
                case Katalog.TypBiomu.Louka: GenerujLouku(); break;
                case Katalog.TypBiomu.Doly: GenerujDoly(); break;
                case Katalog.TypBiomu.Snezny: GenerujSnezny(); break;
                case Katalog.TypBiomu.Poust: GenerujPoust(); break;
                case Katalog.TypBiomu.End: GenerujEnd(); break;
            }
        }

        void GenerujEnd()
        {
            // všude end kámen (0), občas obsidiánový sloup 2x2
            if (nahoda.NextDouble() < 0.6)
            {
                int r = 1 + nahoda.Next(POCET - 3), s = 1 + nahoda.Next(POCET - 3);
                for (int dr = 0; dr < 2; dr++)
                    for (int ds = 0; ds < 2; ds++)
                        kusy_biomu[r + dr, s + ds] = (int)Katalog.End.Obsidian;
            }
        }

        // ---------- generování ----------

        void GenerujLouku()
        {
            for (int r = 0; r < POCET; r++)
                for (int s = 0; s < POCET; s++)
                {
                    int x = svetX + s, y = svetY + r;

                    // šum "les": kde je vysoký, je hustý les (lesy jdou i přes hranice biomů)
                    double les = Sum.Hodnota(x, y, 7, 1);
                    double sanceNaStrom = les > 0.65 ? 0.5 : les > 0.55 ? 0.12 : 0.02;

                    if (nahoda.NextDouble() < sanceNaStrom)
                        kusy_biomu[r, s] = (int)Katalog.Louka.Drevo;
                    else if (Sum.Hodnota(x, y, 3, 2) > 0.8)        // políčka pšenice
                        kusy_biomu[r, s] = (int)Katalog.Louka.Pole;
                    else
                        kusy_biomu[r, s] = (int)Katalog.Louka.Trava;
                }

            if (nahoda.NextDouble() < SANCE_NA_VESNICI)
                UdelejVesnici();
        }

        void GenerujDoly()
        {
            // všude kámen, rudy v žilách
            int pocetZlata = 2 + nahoda.Next(3);
            for (int i = 0; i < pocetZlata; i++)
                UdelejZilu((int)Katalog.Doly.Zlato, 3 + nahoda.Next(4));

            if (nahoda.NextDouble() < 0.6)       // diamanty jsou vzácné
                UdelejZilu((int)Katalog.Doly.Diamanty, 1 + nahoda.Next(3));
        }

        void GenerujSnezny()
        {
            // skupinky dýní
            int skupin = nahoda.Next(3);
            for (int i = 0; i < skupin; i++)
                UdelejZilu((int)Katalog.Snezny.Dyne, 2 + nahoda.Next(3));

            UdelejZamrzleJezero();
        }

        void GenerujPoust()
        {
            for (int r = 0; r < POCET; r++)
                for (int s = 0; s < POCET; s++)
                {
                    double hod = nahoda.NextDouble();
                    // kaktus nesmí mít souseda kaktus (jako v MC)
                    if (hod < 0.06 && !SousediS(r, s, (int)Katalog.Poust.Kaktus))
                        kusy_biomu[r, s] = (int)Katalog.Poust.Kaktus;
                    else if (hod > 0.96)
                        kusy_biomu[r, s] = (int)Katalog.Poust.Krovi;
                }
        }

        // náhodná procházka: vytvoří souvislou "žílu" bloků
        void UdelejZilu(int kus, int delka)
        {
            int r = nahoda.Next(POCET), s = nahoda.Next(POCET);
            for (int i = 0; i < delka; i++)
            {
                kusy_biomu[r, s] = kus;
                switch (nahoda.Next(4))
                {
                    case 0: r++; break;
                    case 1: r--; break;
                    case 2: s++; break;
                    case 3: s--; break;
                }
                r = Math.Clamp(r, 0, POCET - 1);
                s = Math.Clamp(s, 0, POCET - 1);
            }
        }

        bool SousediS(int r, int s, int kus)
        {
            for (int dr = -1; dr <= 1; dr++)
                for (int ds = -1; ds <= 1; ds++)
                {
                    int rr = r + dr, ss = s + ds;
                    if ((dr != 0 || ds != 0) && rr >= 0 && rr < POCET && ss >= 0 && ss < POCET && kusy_biomu[rr, ss] == kus)
                        return true;
                }
            return false;
        }

        // ---------- truhly ----------

        // obsah truhly ve vesnici: někdy prázdná, někdy něco dobrého
        static readonly (Predmet Predmet, int Min, int Max, double Sance)[] LOOT =
        {
            (Predmet.Chleba, 1, 3, 0.5),
            (Predmet.Psenice, 2, 6, 0.4),
            (Predmet.Prkna, 3, 8, 0.4),
            (Predmet.Drevo, 2, 5, 0.3),
            (Predmet.Tycka, 2, 6, 0.3),
            (Predmet.Dyne, 1, 3, 0.3),
            (Predmet.Zlato, 1, 4, 0.2),
            (Predmet.KamennyKrumpac, 1, 1, 0.12),
            (Predmet.ZlataHelma, 1, 1, 0.08),
            (Predmet.ZlateBoty, 1, 1, 0.08),
            (Predmet.Diamant, 1, 2, 0.07),
        };

        static Slot[] NovaTruhla()
        {
            Slot[] obsah = new Slot[Inventar.TRUHLA];
            for (int i = 0; i < obsah.Length; i++)
                obsah[i] = new Slot();
            return obsah;
        }

        static Slot[] TruhlaSLootem()
        {
            Slot[] obsah = NovaTruhla();
            if (nahoda.NextDouble() < 0.25) return obsah;     // čtvrtina truhel je prázdná

            foreach (var loot in LOOT)
            {
                if (nahoda.NextDouble() >= loot.Sance) continue;

                // na náhodné volné místo
                int index;
                do index = nahoda.Next(obsah.Length); while (!obsah[index].JePrazdny);
                obsah[index].Nastav(loot.Predmet, nahoda.Next(loot.Min, loot.Max + 1));
            }
            return obsah;
        }

        public Slot[] Truhla(int r, int s)
        {
            truhly.TryGetValue(new Point(s, r), out Slot[] obsah);
            return obsah;
        }

        public bool JeTruhla(int r, int s)
        {
            return polozene[r, s] == Predmet.Truhla;
        }

        void UdelejVesnici()
        {
            // náhodně otočíme, ať nejsou všechny vesnice stejné
            bool zrcadlitVodorovne = nahoda.Next(2) == 0;
            bool zrcadlitSvisle = nahoda.Next(2) == 0;

            for (int r = 0; r < POCET; r++)
                for (int s = 0; s < POCET; s++)
                {
                    char c = VESNICE[r][s];
                    if (c == '.') continue;

                    int rr = zrcadlitSvisle ? POCET - 1 - r : r;
                    int ss = zrcadlitVodorovne ? POCET - 1 - s : s;

                    if (c == 'F')
                    {
                        kusy_biomu[rr, ss] = (int)Katalog.Louka.Pole;
                        continue;
                    }

                    kusy_biomu[rr, ss] = (int)Katalog.Louka.Trava;   // pod stavbou je tráva (žádné stromy)
                    switch (c)
                    {
                        case 'L': polozene[rr, ss] = Predmet.Drevo; break;
                        case 'P': polozene[rr, ss] = Predmet.Prkna; break;
                        case '#': polozene[rr, ss] = Predmet.Cesta; break;
                        case 'S': polozene[rr, ss] = Predmet.PracovniStul; break;
                        case 'C':
                            polozene[rr, ss] = Predmet.Truhla;
                            truhly[new Point(ss, rr)] = TruhlaSLootem();
                            break;
                    }
                }
        }

        // led uprostřed biomu ve tvaru oválu s trochu nerovným okrajem
        void UdelejZamrzleJezero()
        {
            double stredX = 2.5 + nahoda.NextDouble() * (POCET - 5);
            double stredY = 2.5 + nahoda.NextDouble() * (POCET - 5);
            double polomerX = 2 + nahoda.NextDouble() * 1.5;
            double polomerY = 2 + nahoda.NextDouble() * 1.5;

            for (int r = 0; r < POCET; r++)
                for (int s = 0; s < POCET; s++)
                {
                    double dx = (s + 0.5 - stredX) / polomerX;
                    double dy = (r + 0.5 - stredY) / polomerY;
                    double okraj = 1 + (nahoda.NextDouble() - 0.5) * 0.5;   // nerovný břeh

                    if (dx * dx + dy * dy < okraj)
                        kusy_biomu[r, s] = (int)Katalog.Snezny.Led;
                }
        }

        public Vector2 Pozice => pozice;

        // obdélník políčka ve světových souřadnicích
        public Rectangle RectKusu(int r, int s)
        {
            return new Rectangle((int)pozice.X + s * DLAZDICE, (int)pozice.Y + r * DLAZDICE, DLAZDICE, DLAZDICE);
        }

        // null = tohle políčko se těžit nedá
        public InfoBloku InfoKusu(int r, int s)
        {
            // položený blok má přednost před tím, co je pod ním
            if (polozene[r, s] != Predmet.Nic)
                return InfoPolozeneho(polozene[r, s]);

            int kus = kusy_biomu[r, s];
            if (kus == VYTEZENO) return null;

            switch (MujTyp)
            {
                case Katalog.TypBiomu.Louka:
                    switch ((Katalog.Louka)kus)
                    {
                        case Katalog.Louka.Drevo: return new InfoBloku(Predmet.Drevo, 1.5f, 0);
                        case Katalog.Louka.Pole: return new InfoBloku(Predmet.Psenice, 0.4f, 0);
                        default: return null;   // tráva se netěží
                    }

                case Katalog.TypBiomu.Doly:
                    switch ((Katalog.Doly)kus)
                    {
                        case Katalog.Doly.Kamen: return new InfoBloku(Predmet.Kamen, 3f, 1);
                        case Katalog.Doly.Zlato: return new InfoBloku(Predmet.Zlato, 4f, 2);
                        case Katalog.Doly.Diamanty: return new InfoBloku(Predmet.Diamant, 5f, 2);
                    }
                    break;

                case Katalog.TypBiomu.Snezny:
                    switch ((Katalog.Snezny)kus)
                    {
                        case Katalog.Snezny.Snih: return new InfoBloku(Predmet.Snih, 0.8f, 0);
                        case Katalog.Snezny.Led: return new InfoBloku(Predmet.Led, 1.5f, 1);
                        case Katalog.Snezny.Dyne: return new InfoBloku(Predmet.Dyne, 1f, 0);
                    }
                    break;

                case Katalog.TypBiomu.Poust:
                    switch ((Katalog.Poust)kus)
                    {
                        case Katalog.Poust.Pisek: return new InfoBloku(Predmet.Pisek, 0.6f, 0);
                        case Katalog.Poust.Kaktus: return new InfoBloku(Predmet.Kaktus, 0.8f, 0);
                        case Katalog.Poust.Krovi: return new InfoBloku(Predmet.Tycka, 0.3f, 0);   // jako v MC: z křoví padají tyčky
                    }
                    break;

                case Katalog.TypBiomu.End:
                    switch ((Katalog.End)kus)
                    {
                        case Katalog.End.EndKamen: return new InfoBloku(Predmet.EndKamen, 3f, 1);
                        case Katalog.End.Obsidian: return new InfoBloku(Predmet.Obsidian, 8f, 3);   // jen diamantovým krumpáčem
                    }
                    break;
            }
            return null;
        }

        static InfoBloku InfoPolozeneho(Predmet p)
        {
            switch (p)
            {
                case Predmet.Kamen: return new InfoBloku(p, 3f, 1);
                case Predmet.Led: return new InfoBloku(p, 1.5f, 1);
                case Predmet.Snih: return new InfoBloku(p, 0.8f, 0);
                case Predmet.Dyne: return new InfoBloku(p, 1f, 0);
                case Predmet.Pisek: return new InfoBloku(p, 0.6f, 0);
                case Predmet.Kaktus: return new InfoBloku(p, 0.8f, 0);
                case Predmet.Cesta: return new InfoBloku(p, 0.6f, 0);
                case Predmet.EndKamen: return new InfoBloku(p, 3f, 1);
                case Predmet.Obsidian: return new InfoBloku(p, 8f, 3);
                case Predmet.Vlna: return new InfoBloku(p, 0.6f, 0);
                case Predmet.Postel: return new InfoBloku(p, 0.4f, 0);
                default: return new InfoBloku(p, 1.5f, 0);   // dřevo, prkna, pracovní stůl
            }
        }

        public void Vytez(int r, int s)
        {
            if (polozene[r, s] != Predmet.Nic)
            {
                polozene[r, s] = Predmet.Nic;      // pod položeným blokem zůstane, co tam bylo
                truhly.Remove(new Point(s, r));
            }
            else
                kusy_biomu[r, s] = VYTEZENO;
        }

        // pokládat jde jen na volnou zem (vytěžené políčko nebo základní blok biomu)
        public bool MuzePolozit(int r, int s)
        {
            int kus = kusy_biomu[r, s];
            return polozene[r, s] == Predmet.Nic && (kus == VYTEZENO || kus == 0);
        }

        public void Poloz(int r, int s, Predmet p)
        {
            polozene[r, s] = p;
            if (p == Predmet.Truhla)
                truhly[new Point(s, r)] = NovaTruhla();   // položená truhla je prázdná
        }

        public bool JePracovniStul(int r, int s)
        {
            return polozene[r, s] == Predmet.PracovniStul;
        }

        public bool JePostel(int r, int s)
        {
            return polozene[r, s] == Predmet.Postel;
        }

        // kaktus píchá
        public bool JeKaktus(int r, int s)
        {
            if (polozene[r, s] != Predmet.Nic)
                return polozene[r, s] == Predmet.Kaktus;
            return MujTyp == Katalog.TypBiomu.Poust && kusy_biomu[r, s] == (int)Katalog.Poust.Kaktus;
        }

        public void Vykreslit(SpriteBatch davka, Vector2 kamera)
        {
            for (int r = 0; r < POCET; r++)
                for (int s = 0; s < POCET; s++)
                {
                    int kus = kusy_biomu[r, s];
                    Texture2D obrazek = kus == VYTEZENO ? MojeObrazky[0] : MojeObrazky[kus];
                    Color barva = kus == VYTEZENO ? new Color(110, 95, 80) : Color.White;

                    Rectangle kam = new Rectangle(
                        (int)(pozice.X + s * DLAZDICE - kamera.X),
                        (int)(pozice.Y + r * DLAZDICE - kamera.Y),
                        DLAZDICE, DLAZDICE);

                    davka.Draw(obrazek, kam, barva);

                    // položený blok + tmavý okraj, aby šel poznat od země
                    if (polozene[r, s] == Predmet.Cesta)
                    {
                        davka.Draw(Textury.Predmety[Predmet.Cesta], kam, Color.White);   // cesta je "zem", bez okraje
                    }
                    else if (polozene[r, s] != Predmet.Nic)
                    {
                        davka.Draw(Textury.Predmety[polozene[r, s]], kam, Color.White);

                        Color okraj = Color.Black * 0.4f;
                        davka.Draw(Textury.Pixel, new Rectangle(kam.X, kam.Y, kam.Width, 2), okraj);
                        davka.Draw(Textury.Pixel, new Rectangle(kam.X, kam.Bottom - 2, kam.Width, 2), okraj);
                        davka.Draw(Textury.Pixel, new Rectangle(kam.X, kam.Y, 2, kam.Height), okraj);
                        davka.Draw(Textury.Pixel, new Rectangle(kam.Right - 2, kam.Y, 2, kam.Height), okraj);
                    }
                }
        }
    }
}
