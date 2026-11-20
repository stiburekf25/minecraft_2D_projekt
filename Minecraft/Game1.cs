using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using System.Collections.Generic;

namespace Minecraft
{

    public static class Katalog
    {
        public enum TypBiomu { Louka, Doly, Snezny, Poust, End }

        public enum Louka { Trava, Drevo, Pole }
        public enum Doly { Kamen, Zlato, Diamanty }
        public enum Snezny { Snih, Led, Dyne }
        public enum Poust { Pisek, Kaktus, Krovi }
        public enum End { EndKamen, Obsidian }
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Vector2 velikost_hrace = new Vector2(50, 50);
        Vector2 pozice_hrace;
        Vector2 start_hrace = new Vector2(135, 135);

        Texture2D textura_steve_nahoru;
        Texture2D textura_steve_dolu;
        Texture2D textura_steve_vlevo;
        Texture2D textura_steve_vpravo;


        Texture2D[] [] textury_vsech_biomu;

        Dictionary<Point, Biome> svet;
        Random nahoda = new Random();

        // ---------- hráč: inventář, zdraví, hlad ----------
        const int MAX_ZDRAVI = 20;          // 10 srdíček
        const int MAX_HLAD = 20;            // 10 stehýnek
        const float DOSAH = 3.5f * Biome.DLAZDICE;   // jak daleko Steve dosáhne myší

        Inventar inventar = new Inventar();
        int zdravi = MAX_ZDRAVI;
        int hlad = MAX_HLAD;
        float vycerpani = 0;        // když dojde na 1, ubude 1 bod hladu
        float cas_regenerace = 0;
        float cas_hladoveni = 0;
        float poskozeni = 0;        // nasčítané poškození (kaktus, moby), celé body se odečtou ze zdraví
        float cas_bolesti = 0;      // červené bliknutí obrazovky po zásahu

        // ---------- moby a zvířata ----------
        List<Mob> moby = new List<Mob>();
        Mob mob_pod_mysi;           // na koho míří myš (v dosahu)
        bool drak_pod_mysi;
        float cas_utoku = 0;        // pauza mezi údery hráče
        float cas_spawnu = 0;       // kdy se v noci objeví další nepřítel
        const int MAX_NEPRATEL = 8;

        // ---------- den a noc ----------
        const float DELKA_DNE = 60f;    // sekund
        const float DELKA_NOCI = 45f;
        const float PRECHOD = 6f;       // soumrak / svítání
        float cas_dne = 0;              // 0 = ráno
        bool JeNoc => cas_dne >= DELKA_DNE;

        // ---------- End a drak ----------
        const int VELIKOST_ENDU = 3;    // End je 3x3 biomy
        Point end_klic;                 // levý horní biom Endu
        Rectangle end_oblast;           // End ve světových souřadnicích
        Drak drak;

        bool HracVEndu => end_oblast.Contains(StredHrace().ToPoint());

        // ---------- těžení ----------
        bool ma_cil;
        Biome cil_biom;
        int cil_r, cil_s;
        float postup_tezeni = 0;    // 0..1

        // ---------- UI ----------
        Hud hud;
        InventarObrazovka obrazovka_inventare;
        string zprava = "";
        float cas_zpravy = 0;

        KeyboardState predchozi_klavesnice;
        MouseState predchozi_mys;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            _graphics.PreferredBackBufferHeight = 720;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.ApplyChanges();

            pozice_hrace = start_hrace;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            textura_steve_nahoru = Content.Load<Texture2D>("steve_nahoru");
            textura_steve_dolu = Content.Load<Texture2D>("steve_dolu");
            textura_steve_vlevo = Content.Load<Texture2D>("steve_vlevo");
            textura_steve_vpravo = Content.Load<Texture2D>("steve_vpravo");

            // load textur specifického biomu
            textury_vsech_biomu = new Texture2D[5][];
            textury_vsech_biomu[(int)Katalog.TypBiomu.End] = Textury.EndBiom(GraphicsDevice);

            textury_vsech_biomu[0] = new Texture2D[3];
            textury_vsech_biomu[1] = new Texture2D[3];
            textury_vsech_biomu[2] = new Texture2D[3];
            textury_vsech_biomu[3] = new Texture2D[3];



            // Louka biome
            textury_vsech_biomu[(int)Katalog.TypBiomu.Louka][(int)Katalog.Louka.Trava] = Content.Load<Texture2D>("trava");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Louka][(int)Katalog.Louka.Pole] = Content.Load<Texture2D>("pole");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Louka][(int)Katalog.Louka.Drevo] = Content.Load<Texture2D>("drevo");

            // Doly biome
            textury_vsech_biomu[(int)Katalog.TypBiomu.Doly][(int)Katalog.Doly.Kamen] = Content.Load<Texture2D>("kamen");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Doly][(int)Katalog.Doly.Zlato] = Content.Load<Texture2D>("zlato");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Doly][(int)Katalog.Doly.Diamanty] = Content.Load<Texture2D>("diamanty");

            // Snezny biome
            textury_vsech_biomu[(int)Katalog.TypBiomu.Snezny][(int)Katalog.Snezny.Led] = Content.Load<Texture2D>("led");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Snezny][(int)Katalog.Snezny.Dyne] = Content.Load<Texture2D>("dyne");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Snezny][(int)Katalog.Snezny.Snih] = Content.Load<Texture2D>("snih");

            // Poust biome
            textury_vsech_biomu[(int)Katalog.TypBiomu.Poust][(int)Katalog.Poust.Pisek] = Content.Load<Texture2D>("pisek");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Poust][(int)Katalog.Poust.Kaktus] = Content.Load<Texture2D>("kaktus");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Poust][(int)Katalog.Poust.Krovi] = Content.Load<Texture2D>("krovi");

            // textury předmětů (bloky z Content + pixel art pro ostatní)
            Textury.Nacti(GraphicsDevice, textury_vsech_biomu, Content.Load<Texture2D>("crafting"));

            int sirka = _graphics.PreferredBackBufferWidth;
            int vyska = _graphics.PreferredBackBufferHeight;
            hud = new Hud(Content.Load<SpriteFont>("pismo"), sirka, vyska);
            obrazovka_inventare = new InventarObrazovka(hud, inventar, sirka, vyska);

            // End leží náhodným směrem asi 6 biomů od startu
            double smer_endu = nahoda.NextDouble() * Math.PI * 2;
            end_klic = new Point((int)Math.Round(Math.Cos(smer_endu) * 6) - 1,
                                 (int)Math.Round(Math.Sin(smer_endu) * 6) - 1);
            end_oblast = new Rectangle(end_klic.X * Biome.VELIKOST, end_klic.Y * Biome.VELIKOST,
                                       VELIKOST_ENDU * Biome.VELIKOST, VELIKOST_ENDU * Biome.VELIKOST);
            drak = new Drak(end_oblast);

            svet = new Dictionary<Point, Biome>();
        }

        Vector2 StredHrace() => pozice_hrace + velikost_hrace / 2;

        Vector2 Kamera()
        {
            Vector2 stred_obrazovky = new Vector2(
                _graphics.PreferredBackBufferWidth / 2f,
                _graphics.PreferredBackBufferHeight / 2f);

            // kamera = střed Steva ve světě minus střed obrazovky
            return StredHrace() - stred_obrazovky;
        }

        void UkazZpravu(string text, float delka = 2.5f)
        {
            zprava = text;
            cas_zpravy = delka;
        }

        bool Stisknuto(KeyboardState klav, Keys klavesa)
        {
            return klav.IsKeyDown(klavesa) && !predchozi_klavesnice.IsKeyDown(klavesa);
        }

        // biom podle "teploty" a "vlhkosti" ze šumu -> sousední biomy bývají stejné,
        // takže vznikají větší oblasti (zima vedle zimy, poušť vedle pouště...)
        Katalog.TypBiomu VyberBiom(Point klic)
        {
            if (klic.X >= end_klic.X && klic.X < end_klic.X + VELIKOST_ENDU &&
                klic.Y >= end_klic.Y && klic.Y < end_klic.Y + VELIKOST_ENDU)
                return Katalog.TypBiomu.End;

            double teplota = Sum.Hodnota(klic.X, klic.Y, 3, 10);
            double vlhkost = Sum.Hodnota(klic.X, klic.Y, 3, 20);

            if (teplota < 0.35) return Katalog.TypBiomu.Snezny;
            if (teplota > 0.65) return Katalog.TypBiomu.Poust;
            if (vlhkost < 0.4) return Katalog.TypBiomu.Doly;
            return Katalog.TypBiomu.Louka;
        }

        void GenerujOkoli()
        {
            Vector2 stred_hrace = StredHrace();

            // v jakém biomu Steve stojí (Floor kvůli záporným souřadnicím)
            int bx = (int)Math.Floor(stred_hrace.X / Biome.VELIKOST);
            int by = (int)Math.Floor(stred_hrace.Y / Biome.VELIKOST);

            for (int dy = -2; dy <= 2; dy++)
                for (int dx = -3; dx <= 3; dx++)
                {
                    Point klic = new Point(bx + dx, by + dy);

                    if (!svet.ContainsKey(klic))
                    {
                        Katalog.TypBiomu typ = VyberBiom(klic);
                        Vector2 pozice = new Vector2(klic.X * Biome.VELIKOST, klic.Y * Biome.VELIKOST);

                        svet.Add(klic, new Biome(textury_vsech_biomu, typ, pozice));
                        SpawnniZvirata(typ, pozice);
                    }
                }
        }

        // najde políčko pod myší; vrátí false, když tam žádné není nebo je moc daleko
        bool NajdiCil(MouseState mys, out Biome biom, out int r, out int s)
        {
            biom = null;
            r = 0;
            s = 0;
            if (!IsActive) return false;

            Vector2 svetova = mys.Position.ToVector2() + Kamera();
            int bx = (int)Math.Floor(svetova.X / Biome.VELIKOST);
            int by = (int)Math.Floor(svetova.Y / Biome.VELIKOST);

            if (!svet.TryGetValue(new Point(bx, by), out biom)) return false;

            s = Math.Clamp((int)((svetova.X - bx * Biome.VELIKOST) / Biome.DLAZDICE), 0, Biome.POCET - 1);
            r = Math.Clamp((int)((svetova.Y - by * Biome.VELIKOST) / Biome.DLAZDICE), 0, Biome.POCET - 1);

            Rectangle kus = biom.RectKusu(r, s);
            return Vector2.Distance(kus.Center.ToVector2(), StredHrace()) <= DOSAH;
        }

        // ---------- moby ----------

        // na nové louce se občas objeví stádo zvířat (na sněhu jen kuřata)
        void SpawnniZvirata(Katalog.TypBiomu typ, Vector2 pozice_biomu)
        {
            TypMoba druh;
            if (typ == Katalog.TypBiomu.Louka && nahoda.NextDouble() < 0.5)
                druh = (TypMoba)nahoda.Next(4);                 // prase, kráva, kuře nebo ovce
            else if (typ == Katalog.TypBiomu.Snezny && nahoda.NextDouble() < 0.2)
                druh = TypMoba.Kure;
            else
                return;

            Vector2 stred = pozice_biomu + new Vector2(nahoda.Next(Biome.VELIKOST), nahoda.Next(Biome.VELIKOST));
            int pocet = 2 + nahoda.Next(3);
            for (int i = 0; i < pocet; i++)
                moby.Add(new Mob(druh, stred + new Vector2(nahoda.Next(-80, 81), nahoda.Next(-80, 81))));
        }

        // v noci se kolem hráče (mimo obrazovku) objevují nepřátelé
        void SpawnniNepratele(float dt)
        {
            if (!JeNoc) return;

            cas_spawnu -= dt;
            if (cas_spawnu > 0) return;
            cas_spawnu = 2 + (float)nahoda.NextDouble() * 3;

            int nepratel = 0;
            foreach (Mob m in moby)
                if (m.JeNepritel) nepratel++;
            if (nepratel >= MAX_NEPRATEL) return;

            double uhel = nahoda.NextDouble() * Math.PI * 2;
            float vzdalenost = 700 + nahoda.Next(250);
            Vector2 kde = StredHrace() + new Vector2((float)Math.Cos(uhel), (float)Math.Sin(uhel)) * vzdalenost;
            if (end_oblast.Contains(kde.ToPoint())) return;     // v Endu je jen drak

            moby.Add(new Mob(nahoda.NextDouble() < 0.7 ? TypMoba.Zombie : TypMoba.Pavouk, kde));
        }

        void AktualizujMoby(float dt)
        {
            Vector2 hrac = StredHrace();

            foreach (Mob mob in moby)
            {
                float utok = mob.Aktualizuj(dt, hrac, !JeNoc);
                if (utok > 0)
                    ZranHrace(utok, mob.Typ == TypMoba.Zombie ? "Sežral tě zombie!" : "Zabil tě pavouk!");
            }

            // mrtví a příliš vzdálení nepřátelé zmizí
            moby.RemoveAll(m => m.Zdravi <= 0 ||
                                (m.JeNepritel && Vector2.Distance(m.Pozice, hrac) > 1600));
        }

        // levé tlačítko na moba = úder; vrací true, když myš míří na moba (pak se netěží)
        bool AktualizujBoj(MouseState mys, float dt)
        {
            cas_utoku -= dt;
            mob_pod_mysi = null;
            if (!IsActive) return false;

            Vector2 mys_svet = mys.Position.ToVector2() + Kamera();
            bool kliknuto = mys.LeftButton == ButtonState.Pressed && predchozi_mys.LeftButton == ButtonState.Released;

            // drak (je velký, proto dosah počítáme k okraji jeho těla)
            drak_pod_mysi = drak.Zivy && drak.Rect.Contains(mys_svet) &&
                            Vector2.Distance(drak.Pozice, StredHrace()) <= DOSAH + drak.Rect.Width / 2;
            if (drak_pod_mysi)
            {
                if (kliknuto && cas_utoku <= 0)
                {
                    cas_utoku = 0.4f;
                    drak.Zasah(InfoPredmetu.Poskozeni(inventar.VybranySlot.Predmet));
                    vycerpani += 0.1f;

                    if (!drak.Zivy)
                    {
                        inventar.Pridej(Predmet.DraciVejce, 1);
                        UkazZpravu("Porazil jsi Ender draka! VYHRÁL JSI!", 8f);
                    }
                }
                return true;
            }

            foreach (Mob mob in moby)
                if (mob.Rect.Contains(mys_svet) && Vector2.Distance(mob.Pozice, StredHrace()) <= DOSAH)
                    mob_pod_mysi = mob;

            if (mob_pod_mysi == null) return false;
            if (kliknuto && cas_utoku <= 0)
            {
                cas_utoku = 0.4f;
                mob_pod_mysi.Zasah(InfoPredmetu.Poskozeni(inventar.VybranySlot.Predmet), StredHrace());
                vycerpani += 0.1f;

                if (mob_pod_mysi.Zdravi <= 0)
                {
                    var drop = mob_pod_mysi.Drop();
                    if (drop.Pocet > 0 && !inventar.Pridej(drop.Predmet, drop.Pocet))
                        UkazZpravu("Inventář je plný!");
                }
            }
            return true;
        }

        // poškození hráče, brnění ubírá 4 % za každý bod (jako v MC)
        void ZranHrace(float body, string zprava_pri_smrti)
        {
            poskozeni += body * (1f - inventar.BodyBrneni() * 0.04f);
            if (poskozeni >= 1)
            {
                zdravi -= (int)poskozeni;
                poskozeni -= (int)poskozeni;
                cas_bolesti = 0.3f;
            }
            if (zdravi <= 0) Zemri(zprava_pri_smrti);
        }

        // 0 = den, 1 = plná noc
        float Tma()
        {
            float zacatek_soumraku = DELKA_DNE - PRECHOD;
            float zacatek_svitani = DELKA_DNE + DELKA_NOCI - PRECHOD;

            if (cas_dne < zacatek_soumraku) return 0;
            if (cas_dne < DELKA_DNE) return (cas_dne - zacatek_soumraku) / PRECHOD;
            if (cas_dne < zacatek_svitani) return 1;
            return 1 - (cas_dne - zacatek_svitani) / PRECHOD;
        }

        void AktualizujTezeni(MouseState mys, float dt)
        {
            if (AktualizujBoj(mys, dt))
            {
                ma_cil = false;
                postup_tezeni = 0;
                return;
            }

            ma_cil = NajdiCil(mys, out Biome biom, out int r, out int s);
            if (!ma_cil)
            {
                postup_tezeni = 0;
                return;
            }

            // když myš přejede na jiný blok, začíná se od nuly
            bool stejny_blok = biom == cil_biom && r == cil_r && s == cil_s;
            cil_biom = biom;
            cil_r = r;
            cil_s = s;
            if (!stejny_blok) postup_tezeni = 0;

            if (mys.LeftButton != ButtonState.Pressed)
            {
                postup_tezeni = 0;
                return;
            }

            InfoBloku info = biom.InfoKusu(r, s);
            if (info == null) return;

            Predmet v_ruce = inventar.VybranySlot.Predmet;
            if (InfoPredmetu.SilaKrumpace(v_ruce) < info.PotrebnaSila)
            {
                if (predchozi_mys.LeftButton == ButtonState.Released)
                    UkazZpravu(info.PotrebnaSila == 1 ? "Na tohle potřebuješ krumpáč!" : "Na tohle potřebuješ aspoň kamenný krumpáč!");
                return;
            }

            postup_tezeni += dt * InfoPredmetu.RychlostTezeni(v_ruce) / info.Tvrdost;

            if (postup_tezeni >= 1)
            {
                if (inventar.Pridej(info.Drop, 1))
                {
                    // z rozbité truhly vypadne i její obsah
                    if (biom.JeTruhla(r, s))
                    {
                        bool vseSeVeslo = true;
                        foreach (Slot slot in biom.Truhla(r, s))
                            if (!slot.JePrazdny)
                                vseSeVeslo &= inventar.Pridej(slot.Predmet, slot.Pocet);
                        if (!vseSeVeslo) UkazZpravu("Inventář je plný, něco z truhly se ztratilo!");
                    }

                    biom.Vytez(r, s);
                    vycerpani += 0.1f;      // těžení unavuje
                }
                else
                {
                    UkazZpravu("Inventář je plný!");
                }
                postup_tezeni = 0;
            }
        }

        // pravé tlačítko: pracovní stůl > jídlo > položení bloku
        void AktualizujPraveTlacitko(MouseState mys)
        {
            bool kliknuto = mys.RightButton == ButtonState.Pressed && predchozi_mys.RightButton == ButtonState.Released;
            if (!kliknuto || !IsActive) return;

            // 1) otevřít pracovní stůl
            if (ma_cil && cil_biom.JePracovniStul(cil_r, cil_s))
            {
                obrazovka_inventare.Otevri(3);
                return;
            }

            // 1a) vyspat se v posteli (přeskočí noc a nastaví místo, kde se oživíš)
            if (ma_cil && cil_biom.JePostel(cil_r, cil_s))
            {
                Spat(cil_biom.RectKusu(cil_r, cil_s));
                return;
            }

            // 1b) otevřít truhlu
            if (ma_cil && cil_biom.JeTruhla(cil_r, cil_s))
            {
                obrazovka_inventare.OtevriTruhlu(cil_biom.Truhla(cil_r, cil_s));
                return;
            }

            Predmet v_ruce = inventar.VybranySlot.Predmet;

            // 2) najíst se
            int nasyceni = InfoPredmetu.Nasyceni(v_ruce);
            if (nasyceni > 0)
            {
                if (hlad >= MAX_HLAD)
                {
                    UkazZpravu("Nemáš hlad.");
                    return;
                }

                hlad = Math.Min(MAX_HLAD, hlad + nasyceni);
                inventar.OdeberZVybraneho(1);
                return;
            }

            // 3) položit blok
            if (InfoPredmetu.JeBlok(v_ruce) && ma_cil)
            {
                if (cil_biom.MuzePolozit(cil_r, cil_s))
                {
                    cil_biom.Poloz(cil_r, cil_s, v_ruce);
                    inventar.OdeberZVybraneho(1);
                }
                else UkazZpravu("Sem se nedá nic položit.");
            }
        }

        void Spat(Rectangle postel)
        {
            // oživovat se budeš u postele
            start_hrace = postel.Center.ToVector2() - velikost_hrace / 2;

            if (!JeNoc)
            {
                UkazZpravu("Spát můžeš jen v noci. Místo oživení nastaveno.");
                return;
            }

            foreach (Mob m in moby)
                if (m.JeNepritel && Vector2.Distance(m.Pozice, StredHrace()) < 8 * Biome.DLAZDICE)
                {
                    UkazZpravu("Teď nemůžeš spát, poblíž jsou příšery!");
                    return;
                }

            cas_dne = 0;    // ráno
            UkazZpravu("Dobré ráno!");
        }

        void ZavriInventar()
        {
            if (!obrazovka_inventare.Zavri())
                UkazZpravu("Inventář je plný, něco se ztratilo!");
        }

        void AktualizujHladAZdravi(float dt, bool hybe_se)
        {
            // hlad ubývá: při chůzi 1 bod za ~12 s, v klidu za ~40 s
            vycerpani += dt * (hybe_se ? 1f / 12f : 1f / 40f);
            if (vycerpani >= 1)
            {
                vycerpani -= 1;
                if (hlad > 0) hlad--;
            }

            // najedený hráč se léčí
            if (hlad >= 18 && zdravi < MAX_ZDRAVI)
            {
                cas_regenerace += dt;
                if (cas_regenerace >= 3)
                {
                    cas_regenerace = 0;
                    zdravi++;
                    vycerpani += 0.5f;
                }
            }
            else cas_regenerace = 0;

            // hladový hráč ztrácí životy
            if (hlad == 0)
            {
                cas_hladoveni += dt;
                if (cas_hladoveni >= 4)
                {
                    cas_hladoveni = 0;
                    zdravi--;
                }
            }
            else cas_hladoveni = 0;

            if (zdravi <= 0)
            {
                Zemri("Umřel jsi hlady! Příště se nezapomeň najíst.");
                return;
            }

            // kaktus píchá: 1 bod za 0,5 s
            if (StojiNaKaktusu())
                ZranHrace(dt * 2f, "Upíchal tě kaktus!");
        }

        bool StojiNaKaktusu()
        {
            Vector2 stred = StredHrace();
            int bx = (int)Math.Floor(stred.X / Biome.VELIKOST);
            int by = (int)Math.Floor(stred.Y / Biome.VELIKOST);
            if (!svet.TryGetValue(new Point(bx, by), out Biome biom)) return false;

            int s = Math.Clamp((int)((stred.X - bx * Biome.VELIKOST) / Biome.DLAZDICE), 0, Biome.POCET - 1);
            int r = Math.Clamp((int)((stred.Y - by * Biome.VELIKOST) / Biome.DLAZDICE), 0, Biome.POCET - 1);
            return biom.JeKaktus(r, s);
        }

        void Zemri(string proc)
        {
            zdravi = MAX_ZDRAVI;
            hlad = MAX_HLAD;
            vycerpani = 0;
            poskozeni = 0;
            pozice_hrace = start_hrace;
            UkazZpravu(proc);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState klav = Keyboard.GetState();
            MouseState mys = Mouse.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            bool otevreno = obrazovka_inventare.Otevreno;

            // Esc zavře inventář, jinak ukončí hru
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Stisknuto(klav, Keys.Escape))
            {
                if (otevreno) ZavriInventar();
                else Exit();
            }

            // E otevře / zavře inventář (crafting 2x2)
            if (Stisknuto(klav, Keys.E))
            {
                if (otevreno) ZavriInventar();
                else obrazovka_inventare.Otevri(2);
            }

            // výběr slotu v hotbaru: klávesy 1-9 nebo kolečko myši
            for (int i = 0; i < Inventar.HOTBAR; i++)
                if (klav.IsKeyDown((Keys)((int)Keys.D1 + i)))
                    inventar.Vybrany = i;

            int kolecko = mys.ScrollWheelValue - predchozi_mys.ScrollWheelValue;
            if (kolecko < 0) inventar.Vybrany = (inventar.Vybrany + 1) % Inventar.HOTBAR;
            if (kolecko > 0) inventar.Vybrany = (inventar.Vybrany + Inventar.HOTBAR - 1) % Inventar.HOTBAR;

            Vector2 smer = Vector2.Zero;

            if (!obrazovka_inventare.Otevreno)
            {
                if (klav.IsKeyDown(Keys.W)) smer.Y -= 1;
                if (klav.IsKeyDown(Keys.S)) smer.Y += 1;
                if (klav.IsKeyDown(Keys.D)) smer.X += 1;
                if (klav.IsKeyDown(Keys.A)) smer.X -= 1;
            }

            pozice_hrace += smer * 3;

            GenerujOkoli();

            if (obrazovka_inventare.Otevreno)
            {
                ma_cil = false;
                postup_tezeni = 0;
                obrazovka_inventare.Aktualizuj(mys, predchozi_mys, klav);
            }
            else
            {
                AktualizujTezeni(mys, dt);
                AktualizujPraveTlacitko(mys);
            }

            AktualizujHladAZdravi(dt, smer != Vector2.Zero);

            // den a noc
            cas_dne = (cas_dne + dt) % (DELKA_DNE + DELKA_NOCI);
            SpawnniNepratele(dt);
            AktualizujMoby(dt);

            // drak útočí jen na hráče v Endu
            if (drak.Zivy)
            {
                float utok = drak.Aktualizuj(dt, StredHrace(), HracVEndu);
                if (utok > 0) ZranHrace(utok, "Zabil tě Ender drak!");
            }

            if (cas_zpravy > 0) cas_zpravy -= dt;
            if (cas_bolesti > 0) cas_bolesti -= dt;

            predchozi_klavesnice = klav;
            predchozi_mys = mys;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            Vector2 stred_obrazovky = new Vector2(
                _graphics.PreferredBackBufferWidth / 2f,
                _graphics.PreferredBackBufferHeight / 2f);

            Vector2 kamera = Kamera();

            // Steve se vždy kreslí na střed obrazovky
            Rectangle steve = new Rectangle(
                (int)(stred_obrazovky.X - velikost_hrace.X / 2),
                (int)(stred_obrazovky.Y - velikost_hrace.Y / 2),
                (int)velikost_hrace.X,
                (int)velikost_hrace.Y);

            _spriteBatch.Begin();

            foreach (Biome biom in svet.Values)
                biom.Vykreslit(_spriteBatch, kamera);

            // zvýraznění bloku pod myší
            if (ma_cil)
            {
                Rectangle kus = cil_biom.RectKusu(cil_r, cil_s);
                kus.Offset(-(int)kamera.X, -(int)kamera.Y);

                Predmet v_ruce = inventar.VybranySlot.Predmet;
                InfoBloku info = cil_biom.InfoKusu(cil_r, cil_s);
                bool lze_tezit = info != null && InfoPredmetu.SilaKrumpace(v_ruce) >= info.PotrebnaSila;
                bool lze_polozit = InfoPredmetu.JeBlok(v_ruce) && cil_biom.MuzePolozit(cil_r, cil_s);

                hud.KresliCil(_spriteBatch, kus, lze_tezit || lze_polozit, postup_tezeni);
            }

            // moby (jen ty na obrazovce)
            float cas = (float)gameTime.TotalGameTime.TotalSeconds;
            Rectangle obrazovka = new Rectangle((int)kamera.X - 64, (int)kamera.Y - 64,
                _graphics.PreferredBackBufferWidth + 128, _graphics.PreferredBackBufferHeight + 128);
            foreach (Mob mob in moby)
                if (obrazovka.Contains(mob.Pozice.ToPoint()))
                    mob.Kresli(_spriteBatch, kamera, cas);

            // rámeček kolem moba, na kterého míříš
            if (mob_pod_mysi != null)
            {
                Rectangle r = mob_pod_mysi.Rect;
                r.Offset(-(int)kamera.X, -(int)kamera.Y);
                hud.Ramecek(_spriteBatch, r, Color.White * 0.8f, 2);
            }

            KeyboardState klav = Keyboard.GetState();
            bool otevreno = obrazovka_inventare.Otevreno;
            Color barva_steva = cas_bolesti > 0 ? new Color(255, 110, 110) : Color.White;
            if (!otevreno && klav.IsKeyDown(Keys.S)) _spriteBatch.Draw(textura_steve_dolu, steve, barva_steva);
            else if (!otevreno && klav.IsKeyDown(Keys.A)) _spriteBatch.Draw(textura_steve_vlevo, steve, barva_steva);
            else if (!otevreno && klav.IsKeyDown(Keys.D)) _spriteBatch.Draw(textura_steve_vpravo, steve, barva_steva);
            else _spriteBatch.Draw(textura_steve_nahoru, steve, barva_steva);

            // drak létá nad vším ostatním
            if (drak.Zivy)
            {
                drak.Kresli(_spriteBatch, kamera);
                if (drak_pod_mysi)
                {
                    Rectangle r = drak.Rect;
                    r.Offset(-(int)kamera.X, -(int)kamera.Y);
                    hud.Ramecek(_spriteBatch, r, Color.White * 0.8f, 2);
                }
            }

            // noc = ztmavení celé obrazovky
            Rectangle cela = new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            float tma = Tma();
            if (tma > 0)
                hud.Obdelnik(_spriteBatch, cela, new Color(5, 5, 30) * (tma * 0.6f));

            // zásah = červené bliknutí
            if (cas_bolesti > 0)
                hud.Obdelnik(_spriteBatch, cela, Color.Red * (cas_bolesti * 0.5f));

            // UI
            hud.KresliNapovedu(_spriteBatch);
            hud.KresliCasDne(_spriteBatch, JeNoc);

            if (drak.Zivy)
            {
                if (HracVEndu)
                    hud.KresliZdraviDraka(_spriteBatch, drak.Zdravi / Drak.MAX_ZDRAVI);
                else
                {
                    // šipka ukazuje, kterým směrem je End
                    Vector2 k_endu = end_oblast.Center.ToVector2() - StredHrace();
                    hud.KresliSipkuKEndu(_spriteBatch, (float)Math.Atan2(k_endu.X, -k_endu.Y),
                                         (int)(k_endu.Length() / Biome.DLAZDICE));
                }
            }
            hud.KresliZdraviAHlad(_spriteBatch, zdravi, hlad);
            hud.KresliBrneni(_spriteBatch, inventar.BodyBrneni());
            hud.KresliHotbar(_spriteBatch, inventar);

            if (otevreno)
                obrazovka_inventare.Kresli(_spriteBatch, Mouse.GetState().Position);

            if (cas_zpravy > 0)
                hud.KresliZpravu(_spriteBatch, zprava, Math.Min(1f, cas_zpravy));

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
