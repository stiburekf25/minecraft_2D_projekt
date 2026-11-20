using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Minecraft
{
    enum DruhSlotu { Inventar, Mrizka, Vystup, Brneni, Truhla }

    // Obrazovka inventáře jako v Minecraftu (E):
    //  - vlevo nahoře 4 sloty na brnění a postavička
    //  - vpravo crafting 2x2 (u pracovního stolu 3x3, pak bez postavičky) a výsledek
    //  - pod tím 27 slotů inventáře a hotbar
    //  - věci se přenáší myší:
    //      LMB = vzít / položit celý stack
    //      PMB = vzít půlku / položit jeden kus
    //      Shift + LMB = rychle přesunout (brnění se rovnou oblékne,
    //                    výsledek craftingu = vyrobit kolik to jde)
    internal class InventarObrazovka
    {
        const int SLOT = 50;
        const int MEZERA = 4;
        const int KROK = SLOT + MEZERA;
        const int OKRAJ = 16;

        const int SIRKA = 9 * KROK - MEZERA + 2 * OKRAJ;
        const int VYSKA = OKRAJ + 10 + 4 * KROK + 16 + 3 * KROK + 12 + SLOT + OKRAJ;

        const int JEDNOTKA = 6;     // velikost "pixelu" postavičky

        Hud hud;
        Inventar inv;
        int sirkaObrazovky, vyskaObrazovky;

        public bool Otevreno = false;
        int n = 2;                                  // velikost mřížky (2 nebo 3)
        Slot[] mrizka = new Slot[9];
        Slot vystup = new Slot();
        Recept recept;
        Slot kurzor = new Slot();                   // co hráč drží myší
        Slot[] truhla;                              // otevřená truhla (null = není otevřená)

        static readonly Predmet[] ZASTUPNE_BRNENI =  // šedé obrysy v prázdných slotech
            { Predmet.ZlataHelma, Predmet.ZlatyKyrys, Predmet.ZlateKalhoty, Predmet.ZlateBoty };

        public InventarObrazovka(Hud hud, Inventar inv, int sirkaObrazovky, int vyskaObrazovky)
        {
            this.hud = hud;
            this.inv = inv;
            this.sirkaObrazovky = sirkaObrazovky;
            this.vyskaObrazovky = vyskaObrazovky;

            for (int i = 0; i < mrizka.Length; i++)
                mrizka[i] = new Slot();
        }

        public void Otevri(int velikostMrizky)
        {
            n = velikostMrizky;
            truhla = null;
            Otevreno = true;
            PrepocitejVystup();
        }

        public void OtevriTruhlu(Slot[] obsah)
        {
            n = 2;
            truhla = obsah;
            Otevreno = true;
            PrepocitejVystup();
        }

        // vrátí true, když se všechno vrátilo do inventáře
        public bool Zavri()
        {
            bool vseSeVeslo = true;

            // věci z mřížky a z myši se vrací do inventáře
            foreach (Slot slot in mrizka)
                vseSeVeslo &= VratDoInventare(slot);
            vseSeVeslo &= VratDoInventare(kurzor);

            Otevreno = false;
            truhla = null;
            PrepocitejVystup();
            return vseSeVeslo;
        }

        bool VratDoInventare(Slot slot)
        {
            if (slot.JePrazdny) return true;
            bool veslo = inv.Pridej(slot.Predmet, slot.Pocet);
            slot.Vyprazdni();
            return veslo;
        }

        // ---------- rozložení ----------

        bool JeTruhla => truhla != null;
        bool UkazPostavu => n == 2 && !JeTruhla;     // u pracovního stolu a truhly jako v MC bez postavičky

        int PanelX => (sirkaObrazovky - SIRKA) / 2;
        int PanelY => (vyskaObrazovky - VYSKA) / 2;
        Rectangle Panel => new Rectangle(PanelX, PanelY, SIRKA, VYSKA);

        int LevyOkraj => PanelX + OKRAJ;
        int HorniCastY => PanelY + OKRAJ + 10;

        int MrizkaX => UkazPostavu ? LevyOkraj + 4 * KROK : LevyOkraj + 2 * KROK;
        int MrizkaY => HorniCastY + (4 - n) * KROK / 2;

        Rectangle RectVystupu => new Rectangle(MrizkaX + n * KROK + KROK + 10, MrizkaY + (n * KROK - MEZERA - SLOT) / 2, SLOT, SLOT);

        Rectangle RectBrneni(int i) => new Rectangle(LevyOkraj, HorniCastY + i * KROK, SLOT, SLOT);

        Rectangle RectPostavy => new Rectangle(LevyOkraj + KROK + 6, HorniCastY, 112, 4 * KROK - MEZERA);

        Rectangle RectInventare(int i)
        {
            int hlavniY = HorniCastY + 4 * KROK + 16;

            if (i < Inventar.HOTBAR)      // hotbar je dole
                return new Rectangle(LevyOkraj + i * KROK, hlavniY + 3 * KROK + 12, SLOT, SLOT);

            int j = i - Inventar.HOTBAR;
            return new Rectangle(LevyOkraj + (j % 9) * KROK, hlavniY + (j / 9) * KROK, SLOT, SLOT);
        }

        Rectangle RectMrizky(int i)
        {
            return new Rectangle(MrizkaX + (i % n) * KROK, MrizkaY + (i / n) * KROK, SLOT, SLOT);
        }

        Rectangle RectTruhly(int i)
        {
            return new Rectangle(LevyOkraj + (i % 9) * KROK, HorniCastY + 30 + (i / 9) * KROK, SLOT, SLOT);
        }

        List<(Rectangle Rect, DruhSlotu Druh, int Index)> VsechnySloty()
        {
            var sloty = new List<(Rectangle, DruhSlotu, int)>();
            for (int i = 0; i < Inventar.POCET_SLOTU; i++) sloty.Add((RectInventare(i), DruhSlotu.Inventar, i));

            if (JeTruhla)
            {
                for (int i = 0; i < Inventar.TRUHLA; i++) sloty.Add((RectTruhly(i), DruhSlotu.Truhla, i));
                return sloty;
            }

            for (int i = 0; i < n * n; i++) sloty.Add((RectMrizky(i), DruhSlotu.Mrizka, i));
            sloty.Add((RectVystupu, DruhSlotu.Vystup, 0));
            if (UkazPostavu)
                for (int i = 0; i < 4; i++) sloty.Add((RectBrneni(i), DruhSlotu.Brneni, i));
            return sloty;
        }

        Slot DejSlot(DruhSlotu druh, int index)
        {
            switch (druh)
            {
                case DruhSlotu.Inventar: return inv.Sloty[index];
                case DruhSlotu.Mrizka: return mrizka[index];
                case DruhSlotu.Brneni: return inv.Brneni[index];
                case DruhSlotu.Truhla: return truhla[index];
                default: return vystup;
            }
        }

        Slot SlotPodMysi(Point mys)
        {
            foreach (var s in VsechnySloty())
                if (s.Rect.Contains(mys))
                    return DejSlot(s.Druh, s.Index);
            return null;
        }

        // ---------- ovládání ----------

        public void Aktualizuj(MouseState mys, MouseState predchozi, KeyboardState klav)
        {
            bool levy = mys.LeftButton == ButtonState.Pressed && predchozi.LeftButton == ButtonState.Released;
            bool pravy = mys.RightButton == ButtonState.Pressed && predchozi.RightButton == ButtonState.Released;
            bool shift = klav.IsKeyDown(Keys.LeftShift) || klav.IsKeyDown(Keys.RightShift);

            if (!levy && !pravy) return;

            foreach (var s in VsechnySloty())
            {
                if (!s.Rect.Contains(mys.Position)) continue;

                if (s.Druh == DruhSlotu.Vystup)
                    VezmiVystup(shift);
                else if (s.Druh == DruhSlotu.Brneni)
                    KlikNaBrneni(s.Index, shift);
                else
                {
                    Slot slot = DejSlot(s.Druh, s.Index);

                    if (levy && shift) RychlyPresun(s.Druh, s.Index);
                    else if (levy) LevyKlik(slot);
                    else PravyKlik(slot);

                    PrepocitejVystup();
                }
                return;
            }
        }

        void LevyKlik(Slot slot)
        {
            if (kurzor.JePrazdny)
            {
                // vzít celý stack
                kurzor.Nastav(slot.Predmet, slot.Pocet);
                slot.Vyprazdni();
            }
            else if (slot.JePrazdny)
            {
                // položit všechno
                slot.Nastav(kurzor.Predmet, kurzor.Pocet);
                kurzor.Vyprazdni();
            }
            else if (slot.Predmet == kurzor.Predmet)
            {
                // doplnit stack
                int max = InfoPredmetu.MaxVStacku(slot.Predmet);
                int kolik = Math.Min(max - slot.Pocet, kurzor.Pocet);
                slot.Pocet += kolik;
                kurzor.Nastav(kurzor.Predmet, kurzor.Pocet - kolik);
            }
            else Prohod(slot, kurzor);
        }

        void PravyKlik(Slot slot)
        {
            if (kurzor.JePrazdny)
            {
                if (slot.JePrazdny) return;
                // vzít půlku (zaokrouhleno nahoru)
                int pul = (slot.Pocet + 1) / 2;
                kurzor.Nastav(slot.Predmet, pul);
                slot.Nastav(slot.Predmet, slot.Pocet - pul);
            }
            else if (slot.JePrazdny ||
                     (slot.Predmet == kurzor.Predmet && slot.Pocet < InfoPredmetu.MaxVStacku(slot.Predmet)))
            {
                // položit jeden kus
                slot.Nastav(kurzor.Predmet, slot.Pocet + 1);
                kurzor.Nastav(kurzor.Predmet, kurzor.Pocet - 1);
            }
        }

        static void Prohod(Slot a, Slot b)
        {
            Predmet p = a.Predmet;
            int pocet = a.Pocet;
            a.Nastav(b.Predmet, b.Pocet);
            b.Nastav(p, pocet);
        }

        // do slotu na brnění jde jen správný kus brnění
        void KlikNaBrneni(int index, bool shift)
        {
            Slot slot = inv.Brneni[index];

            if (shift)
            {
                // sundat do inventáře
                if (!slot.JePrazdny)
                    slot.Nastav(slot.Predmet, inv.PridejDo(slot.Predmet, slot.Pocet, 0, Inventar.POCET_SLOTU));
                return;
            }

            if (kurzor.JePrazdny || (int)InfoPredmetu.TypBrneni(kurzor.Predmet) == index)
                Prohod(slot, kurzor);
        }

        void RychlyPresun(DruhSlotu druh, int index)
        {
            Slot slot = DejSlot(druh, index);
            if (slot.JePrazdny) return;

            // u truhly: truhla <-> inventář
            if (druh == DruhSlotu.Truhla)
            {
                slot.Nastav(slot.Predmet, inv.PridejDo(slot.Predmet, slot.Pocet, 0, Inventar.POCET_SLOTU));
                return;
            }
            if (JeTruhla)
            {
                slot.Nastav(slot.Predmet, Inventar.PridejDoSlotu(truhla, slot.Predmet, slot.Pocet, 0, Inventar.TRUHLA));
                return;
            }

            // brnění se rovnou oblékne, když je jeho slot volný
            TypBrneni typ = InfoPredmetu.TypBrneni(slot.Predmet);
            if (druh == DruhSlotu.Inventar && typ != TypBrneni.Zadne && inv.Brneni[(int)typ].JePrazdny)
            {
                Prohod(slot, inv.Brneni[(int)typ]);
                return;
            }

            int zbytek;
            if (druh == DruhSlotu.Mrizka)
                zbytek = inv.PridejDo(slot.Predmet, slot.Pocet, 0, Inventar.POCET_SLOTU);
            else if (index < Inventar.HOTBAR)
                zbytek = inv.PridejDo(slot.Predmet, slot.Pocet, Inventar.HOTBAR, Inventar.POCET_SLOTU);   // hotbar -> inventář
            else
                zbytek = inv.PridejDo(slot.Predmet, slot.Pocet, 0, Inventar.HOTBAR);                      // inventář -> hotbar

            slot.Nastav(slot.Predmet, zbytek);
        }

        void VezmiVystup(bool shift)
        {
            if (recept == null) return;

            if (shift)
            {
                // vyrábět, dokud to jde a dokud je místo
                Recept tenhle = recept;
                while (recept == tenhle && inv.Kopie().Pridej(tenhle.Vysledek, tenhle.Pocet))
                {
                    inv.Pridej(tenhle.Vysledek, tenhle.Pocet);
                    SpotrebujMrizku();
                }
                return;
            }

            if (kurzor.JePrazdny)
                kurzor.Nastav(recept.Vysledek, recept.Pocet);
            else if (kurzor.Predmet == recept.Vysledek &&
                     kurzor.Pocet + recept.Pocet <= InfoPredmetu.MaxVStacku(recept.Vysledek))
                kurzor.Pocet += recept.Pocet;
            else
                return;

            SpotrebujMrizku();
        }

        // z každého políčka mřížky ubude jeden kus
        void SpotrebujMrizku()
        {
            for (int i = 0; i < n * n; i++)
                if (!mrizka[i].JePrazdny)
                    mrizka[i].Nastav(mrizka[i].Predmet, mrizka[i].Pocet - 1);

            PrepocitejVystup();
        }

        void PrepocitejVystup()
        {
            recept = Crafting.Najdi(mrizka, n);
            if (recept != null) vystup.Nastav(recept.Vysledek, recept.Pocet);
            else vystup.Vyprazdni();
        }

        // ---------- kreslení ----------

        void KresliPozadiSlotu(SpriteBatch sb, Rectangle r, Point mys)
        {
            hud.Obdelnik(sb, r, new Color(139, 139, 139));
            // "zapuštěný" okraj jako v MC
            hud.Obdelnik(sb, new Rectangle(r.X, r.Y, r.Width, 2), new Color(55, 55, 55));
            hud.Obdelnik(sb, new Rectangle(r.X, r.Y, 2, r.Height), new Color(55, 55, 55));
            hud.Obdelnik(sb, new Rectangle(r.X, r.Bottom - 2, r.Width, 2), Color.White);
            hud.Obdelnik(sb, new Rectangle(r.Right - 2, r.Y, 2, r.Height), Color.White);

            if (r.Contains(mys))
                hud.Obdelnik(sb, r, Color.White * 0.3f);
        }

        void KresliSlot(SpriteBatch sb, Slot slot, Rectangle r, Point mys)
        {
            KresliPozadiSlotu(sb, r, mys);
            hud.KresliObsahSlotu(sb, slot, r);
        }

        void KresliSipku(SpriteBatch sb, int x, int y, Color barva)
        {
            hud.Obdelnik(sb, new Rectangle(x, y - 3, 22, 6), barva);
            for (int i = 0; i < 9; i++)
                hud.Obdelnik(sb, new Rectangle(x + 22 + i, y - 9 + i, 1, 18 - 2 * i), barva);
        }

        public void Kresli(SpriteBatch sb, Point mys)
        {
            // ztmavení hry pod inventářem
            hud.Obdelnik(sb, new Rectangle(0, 0, sirkaObrazovky, vyskaObrazovky), Color.Black * 0.5f);

            // panel
            hud.Obdelnik(sb, Panel, new Color(198, 198, 198));
            hud.Ramecek(sb, Panel, new Color(40, 40, 40), 3);

            Color sedyText = new Color(60, 60, 60);

            // brnění + postavička
            if (UkazPostavu)
            {
                for (int i = 0; i < 4; i++)
                {
                    Rectangle r = RectBrneni(i);
                    Slot slot = inv.Brneni[i];
                    KresliPozadiSlotu(sb, r, mys);

                    if (slot.JePrazdny)
                    {
                        int o = SLOT / 6;
                        sb.Draw(Textury.Predmety[ZASTUPNE_BRNENI[i]],
                                new Rectangle(r.X + o, r.Y + o, SLOT - 2 * o, SLOT - 2 * o), Color.Black * 0.25f);
                    }
                    else hud.KresliObsahSlotu(sb, slot, r);
                }

                KresliPostavu(sb, RectPostavy);
            }

            if (JeTruhla)
            {
                // obsah truhly
                sb.DrawString(hud.Pismo, "Truhla", new Vector2(LevyOkraj, HorniCastY + 4), sedyText);
                for (int i = 0; i < Inventar.TRUHLA; i++)
                    KresliSlot(sb, truhla[i], RectTruhly(i), mys);
            }
            else
            {
                // crafting
                string nadpis = n == 3 ? "Pracovní stůl" : "Crafting";
                sb.DrawString(hud.Pismo, nadpis, new Vector2(MrizkaX, MrizkaY - 22), sedyText);

                for (int i = 0; i < n * n; i++)
                    KresliSlot(sb, mrizka[i], RectMrizky(i), mys);

                Rectangle v = RectVystupu;
                KresliSipku(sb, MrizkaX + n * KROK + 8, v.Center.Y, new Color(90, 90, 90));
                KresliSlot(sb, vystup, v, mys);
            }

            // inventář
            sb.DrawString(hud.Pismo, "Inventář", new Vector2(LevyOkraj, HorniCastY + 4 * KROK - 2), sedyText);
            for (int i = 0; i < Inventar.POCET_SLOTU; i++)
                KresliSlot(sb, inv.Sloty[i], RectInventare(i), mys);

            // držená věc u myši
            if (!kurzor.JePrazdny)
            {
                hud.KresliObsahSlotu(sb, kurzor, new Rectangle(mys.X - SLOT / 2, mys.Y - SLOT / 2, SLOT, SLOT));
            }
            else
            {
                // název věci pod myší
                Slot podMysi = SlotPodMysi(mys);
                if (podMysi != null && !podMysi.JePrazdny)
                {
                    string nazev = InfoPredmetu.Nazev(podMysi.Predmet);
                    Vector2 rozmer = hud.Pismo.MeasureString(nazev);
                    Rectangle bublina = new Rectangle(mys.X + 14, mys.Y - 26, (int)rozmer.X + 12, (int)rozmer.Y + 6);
                    hud.Obdelnik(sb, bublina, new Color(20, 5, 30) * 0.95f);
                    hud.Ramecek(sb, bublina, new Color(70, 20, 140), 2);
                    sb.DrawString(hud.Pismo, nazev, new Vector2(bublina.X + 6, bublina.Y + 3), Color.White);
                }
            }
        }

        // ---------- postavička ----------

        // obdélník v "pixelech" postavičky (postavička je 16 x 32 jednotek)
        void Kus(SpriteBatch sb, Rectangle box, int x, int y, int w, int h, Color barva)
        {
            int ox = box.X + (box.Width - 16 * JEDNOTKA) / 2;
            int oy = box.Y + (box.Height - 33 * JEDNOTKA) / 2 + JEDNOTKA;   // +1 jednotka místa na helmu
            hud.Obdelnik(sb, new Rectangle(ox + x * JEDNOTKA, oy + y * JEDNOTKA, w * JEDNOTKA, h * JEDNOTKA), barva);
        }

        Color BarvaBrneni(Slot slot) => InfoPredmetu.JeDiamantove(slot.Predmet) ? Textury.DIAMANTOVA : Textury.ZLATA;

        void KresliPostavu(SpriteBatch sb, Rectangle box)
        {
            hud.Obdelnik(sb, box, Color.Black);
            hud.Ramecek(sb, box, new Color(55, 55, 55), 2);

            Color kuze = new Color(205, 155, 115);
            Color vlasy = new Color(75, 50, 30);
            Color triko = new Color(60, 140, 75);
            Color kalhoty = new Color(95, 70, 45);
            Color boty = new Color(60, 60, 60);

            // hlava
            Kus(sb, box, 4, 0, 8, 8, kuze);
            Kus(sb, box, 4, 0, 8, 2, vlasy);
            Kus(sb, box, 4, 2, 1, 2, vlasy);
            Kus(sb, box, 11, 2, 1, 2, vlasy);
            Kus(sb, box, 6, 4, 1, 1, Color.White);
            Kus(sb, box, 9, 4, 1, 1, Color.White);
            Kus(sb, box, 7, 6, 2, 1, new Color(150, 95, 70));

            // tělo a ruce
            Kus(sb, box, 4, 8, 8, 12, triko);
            Kus(sb, box, 0, 8, 4, 4, triko);
            Kus(sb, box, 12, 8, 4, 4, triko);
            Kus(sb, box, 0, 12, 4, 8, kuze);
            Kus(sb, box, 12, 12, 4, 8, kuze);

            // nohy
            Kus(sb, box, 4, 20, 4, 12, kalhoty);
            Kus(sb, box, 8, 20, 4, 12, kalhoty);
            Kus(sb, box, 4, 30, 8, 2, boty);

            // brnění přes postavičku
            Slot helma = inv.Brneni[(int)TypBrneni.Helma];
            Slot kyrys = inv.Brneni[(int)TypBrneni.Kyrys];
            Slot kalh = inv.Brneni[(int)TypBrneni.Kalhoty];
            Slot bot = inv.Brneni[(int)TypBrneni.Boty];

            if (!kalh.JePrazdny)
            {
                Color c = BarvaBrneni(kalh);
                Kus(sb, box, 4, 19, 8, 2, c);
                Kus(sb, box, 4, 20, 4, 9, c);
                Kus(sb, box, 8, 20, 4, 9, c);
                Kus(sb, box, 7, 22, 2, 7, Color.Black);       // mezera mezi nohama
            }
            if (!bot.JePrazdny)
            {
                Color c = BarvaBrneni(bot);
                Kus(sb, box, 3, 29, 5, 3, c);
                Kus(sb, box, 8, 29, 5, 3, c);
            }
            if (!kyrys.JePrazdny)
            {
                Color c = BarvaBrneni(kyrys);
                Kus(sb, box, 3, 8, 10, 12, c);
                Kus(sb, box, 0, 8, 3, 5, c);
                Kus(sb, box, 13, 8, 3, 5, c);
                Kus(sb, box, 7, 9, 2, 10, c * 0.8f);          // stín uprostřed
            }
            if (!helma.JePrazdny)
            {
                Color c = BarvaBrneni(helma);
                Kus(sb, box, 3, -1, 10, 3, c);
                Kus(sb, box, 3, 2, 1, 5, c);
                Kus(sb, box, 12, 2, 1, 5, c);
            }
        }
    }
}
