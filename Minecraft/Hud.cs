using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minecraft
{
    // Kreslení UI ve hře: hotbar, srdíčka, hlad, zprávy + pomocné kreslicí funkce
    internal class Hud
    {
        public const int SLOT = 54;
        const int MEZERA = 4;
        const int PIXEL_IKONY = 3;          // velikost jednoho "pixelu" u srdíček a jídla

        public SpriteFont Pismo;
        int sirka, vyska;

        // ---------- pixel art ('.' = průhledné) ----------
        static readonly string[] SRDCE =
        {
            ".XX.XX.",
            "XXXXXXX",
            "XXXXXXX",
            ".XXXXX.",
            "..XXX..",
            "...X...",
        };

        static readonly string[] JIDLO =
        {
            "....MM.",
            "...MMMM",
            "..MMMMM",
            "..MMMM.",
            ".BMM...",
            "B.B....",
            ".B.....",
        };

        static readonly Dictionary<char, Color> barvySrdce = new Dictionary<char, Color> { { 'X', new Color(220, 20, 30) } };
        static readonly Dictionary<char, Color> barvyJidla = new Dictionary<char, Color> { { 'M', new Color(170, 90, 40) }, { 'B', new Color(235, 225, 200) } };

        public Hud(SpriteFont pismo, int sirka, int vyska)
        {
            Pismo = pismo;
            this.sirka = sirka;
            this.vyska = vyska;
        }

        // ---------- pomocné kreslení ----------

        public void Obdelnik(SpriteBatch sb, Rectangle r, Color barva)
        {
            sb.Draw(Textury.Pixel, r, barva);
        }

        public void Ramecek(SpriteBatch sb, Rectangle r, Color barva, int tloustka)
        {
            Obdelnik(sb, new Rectangle(r.X, r.Y, r.Width, tloustka), barva);
            Obdelnik(sb, new Rectangle(r.X, r.Bottom - tloustka, r.Width, tloustka), barva);
            Obdelnik(sb, new Rectangle(r.X, r.Y, tloustka, r.Height), barva);
            Obdelnik(sb, new Rectangle(r.Right - tloustka, r.Y, tloustka, r.Height), barva);
        }

        public void Text(SpriteBatch sb, string text, Vector2 pozice, Color barva)
        {
            sb.DrawString(Pismo, text, pozice + new Vector2(2, 2), Color.Black * (barva.A / 255f));
            sb.DrawString(Pismo, text, pozice, barva);
        }

        // maxSloupec: nakreslí jen levou část obrázku (pro půlky srdíček)
        // prebarvit: celý obrázek jednou barvou (pro prázdná srdíčka)
        void PixelArt(SpriteBatch sb, string[] maska, Dictionary<char, Color> barvy, Vector2 pozice, int vel,
                      int maxSloupec = int.MaxValue, Color? prebarvit = null)
        {
            for (int r = 0; r < maska.Length; r++)
                for (int s = 0; s < maska[r].Length && s < maxSloupec; s++)
                {
                    char c = maska[r][s];
                    if (c == '.') continue;

                    Color barva = prebarvit ?? barvy[c];
                    Obdelnik(sb, new Rectangle((int)pozice.X + s * vel, (int)pozice.Y + r * vel, vel, vel), barva);
                }
        }

        public void KresliIkonu(SpriteBatch sb, Predmet p, Rectangle kam)
        {
            if (p != Predmet.Nic && Textury.Predmety.TryGetValue(p, out Texture2D textura))
                sb.Draw(textura, kam, Color.White);
        }

        // ikonka + počet v pravém dolním rohu (používá hotbar i inventář)
        public void KresliObsahSlotu(SpriteBatch sb, Slot slot, Rectangle kam)
        {
            if (slot.JePrazdny) return;

            int okraj = kam.Width / 6;
            KresliIkonu(sb, slot.Predmet, new Rectangle(kam.X + okraj, kam.Y + okraj, kam.Width - 2 * okraj, kam.Height - 2 * okraj));

            if (slot.Pocet > 1)
            {
                string pocet = slot.Pocet.ToString();
                Vector2 rozmer = Pismo.MeasureString(pocet);
                Text(sb, pocet, new Vector2(kam.Right - rozmer.X - 3, kam.Bottom - rozmer.Y), Color.White);
            }
        }

        // ---------- hotbar ----------

        int HotbarX => (sirka - (Inventar.HOTBAR * SLOT + (Inventar.HOTBAR - 1) * MEZERA)) / 2;
        int HotbarY => vyska - SLOT - 10;
        int HotbarPravo => HotbarX + Inventar.HOTBAR * SLOT + (Inventar.HOTBAR - 1) * MEZERA;

        public void KresliHotbar(SpriteBatch sb, Inventar inv)
        {
            for (int i = 0; i < Inventar.HOTBAR; i++)
            {
                Rectangle slot = new Rectangle(HotbarX + i * (SLOT + MEZERA), HotbarY, SLOT, SLOT);
                bool vybrany = i == inv.Vybrany;

                Obdelnik(sb, slot, Color.Black * 0.55f);
                if (vybrany)
                {
                    // tmavý obrys, aby byl bílý rámeček vidět i na sněhu
                    Rectangle obrys = slot;
                    obrys.Inflate(2, 2);
                    Ramecek(sb, obrys, Color.Black, 2);
                    Ramecek(sb, slot, Color.White, 3);
                }
                else Ramecek(sb, slot, Color.Gray, 2);

                KresliObsahSlotu(sb, inv.Sloty[i], slot);
            }

            // název vybrané věci nad hotbarem
            Slot vybranySlot = inv.VybranySlot;
            if (!vybranySlot.JePrazdny)
            {
                string nazev = InfoPredmetu.Nazev(vybranySlot.Predmet);
                Vector2 rozmer = Pismo.MeasureString(nazev);
                Text(sb, nazev, new Vector2((sirka - rozmer.X) / 2, HotbarY - 74), Color.White);
            }
        }

        // ---------- srdíčka a hlad ----------

        // hodnoty 0..20, jedna ikonka = 2 body (jako v Minecraftu)
        public void KresliZdraviAHlad(SpriteBatch sb, int zdravi, int hlad)
        {
            int krok = 7 * PIXEL_IKONY + 3;
            int y = HotbarY - 26;

            for (int i = 0; i < 10; i++)
            {
                // srdíčka zleva
                Vector2 poz = new Vector2(HotbarX + i * krok, y);
                PixelArt(sb, SRDCE, barvySrdce, poz, PIXEL_IKONY, prebarvit: new Color(50, 10, 10));
                int body = zdravi - i * 2;
                if (body >= 2) PixelArt(sb, SRDCE, barvySrdce, poz, PIXEL_IKONY);
                else if (body == 1) PixelArt(sb, SRDCE, barvySrdce, poz, PIXEL_IKONY, maxSloupec: 4);

                // hlad zprava
                Vector2 pozH = new Vector2(HotbarPravo - (i + 1) * krok + 3, y - 2);
                PixelArt(sb, JIDLO, barvyJidla, pozH, PIXEL_IKONY, prebarvit: new Color(40, 25, 15));
                int bodyH = hlad - i * 2;
                if (bodyH >= 2) PixelArt(sb, JIDLO, barvyJidla, pozH, PIXEL_IKONY);
                else if (bodyH == 1) PixelArt(sb, JIDLO, barvyJidla, pozH, PIXEL_IKONY, maxSloupec: 4);
            }
        }

        // body brnění 0..20 nad srdíčky (když žádné nemáš, nic se nekreslí)
        static readonly Dictionary<char, Color> barvyBrneni = new Dictionary<char, Color>
            { { 'H', new Color(200, 200, 210) }, { 'h', new Color(150, 150, 160) } };

        public void KresliBrneni(SpriteBatch sb, int body)
        {
            if (body <= 0) return;

            int krok = 7 * PIXEL_IKONY + 3;
            int y = HotbarY - 26 - 22;

            for (int i = 0; i < 10; i++)
            {
                Vector2 poz = new Vector2(HotbarX + i * krok + 2, y);
                PixelArt(sb, Textury.KYRYS, barvyBrneni, poz, 2, prebarvit: new Color(40, 40, 45));
                int b = body - i * 2;
                if (b >= 2) PixelArt(sb, Textury.KYRYS, barvyBrneni, poz, 2);
                else if (b == 1) PixelArt(sb, Textury.KYRYS, barvyBrneni, poz, 2, maxSloupec: 4);
            }
        }

        // ---------- těžení ----------

        public void KresliCil(SpriteBatch sb, Rectangle naObrazovce, bool aktivni, float postup)
        {
            Ramecek(sb, naObrazovce, aktivni ? Color.White : Color.Red * 0.8f, 2);

            if (postup > 0)
            {
                Rectangle pozadi = new Rectangle(naObrazovce.X + 6, naObrazovce.Bottom - 12, naObrazovce.Width - 12, 6);
                Obdelnik(sb, pozadi, Color.Black * 0.7f);
                Obdelnik(sb, new Rectangle(pozadi.X, pozadi.Y, (int)(pozadi.Width * postup), pozadi.Height), Color.LimeGreen);
            }
        }

        // ---------- texty ----------

        public void KresliZpravu(SpriteBatch sb, string zprava, float alfa)
        {
            Vector2 rozmer = Pismo.MeasureString(zprava);
            Text(sb, zprava, new Vector2((sirka - rozmer.X) / 2, 112), Color.White * alfa);
        }

        public void KresliNapovedu(SpriteBatch sb)
        {
            Text(sb, "WASD pohyb   LMB těžit / bojovat   PMB položit / jíst / stůl / truhla   1-9   E inventář",
                 new Vector2(10, 10), Color.White * 0.85f);
        }

        // health bar draka nahoře uprostřed (jako boss bar v MC)
        public void KresliZdraviDraka(SpriteBatch sb, float podil)
        {
            const int SIRKA_BARU = 500;
            string nazev = "Ender drak";
            Vector2 rozmer = Pismo.MeasureString(nazev);
            Text(sb, nazev, new Vector2((sirka - rozmer.X) / 2, 40), new Color(230, 170, 255));

            Rectangle bar = new Rectangle((sirka - SIRKA_BARU) / 2, 64, SIRKA_BARU, 14);
            Obdelnik(sb, bar, new Color(40, 10, 50));
            Obdelnik(sb, new Rectangle(bar.X, bar.Y, (int)(bar.Width * MathHelper.Clamp(podil, 0, 1)), bar.Height), new Color(200, 60, 230));
            Ramecek(sb, bar, Color.Black, 2);
        }

        // fialová šipka ukazující směr k Endu + vzdálenost v blocích
        public void KresliSipkuKEndu(SpriteBatch sb, float uhel, int bloku)
        {
            Vector2 stred = new Vector2(sirka / 2f, 62);
            Texture2D t = Textury.Sipka;
            sb.Draw(t, stred, null, Color.White, uhel, new Vector2(t.Width / 2f, t.Height / 2f), 32f / t.Width, SpriteEffects.None, 0);

            string text = "End: " + bloku + " bloků";
            Vector2 rozmer = Pismo.MeasureString(text);
            Text(sb, text, new Vector2((sirka - rozmer.X) / 2, 82), new Color(220, 170, 255));
        }

        // vpravo nahoře slunce nebo měsíc
        public void KresliCasDne(SpriteBatch sb, bool noc)
        {
            Rectangle r = new Rectangle(sirka - 46, 10, 32, 32);
            if (noc)
            {
                Obdelnik(sb, r, new Color(220, 220, 235));
                Obdelnik(sb, new Rectangle(r.X + 6, r.Y + 6, 8, 8), new Color(180, 180, 200));   // krátery
                Obdelnik(sb, new Rectangle(r.X + 18, r.Y + 18, 6, 6), new Color(180, 180, 200));
            }
            else
            {
                Obdelnik(sb, r, new Color(255, 220, 60));
                Obdelnik(sb, new Rectangle(r.X + 6, r.Y + 6, 20, 20), new Color(255, 245, 150));
            }
            Ramecek(sb, r, Color.Black * 0.5f, 2);
        }
    }
}
