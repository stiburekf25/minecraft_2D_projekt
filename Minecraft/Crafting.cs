using System;
using System.Collections.Generic;

namespace Minecraft
{
    // Recept s tvarem jako v Minecraftu. Mezera ve vzoru = prázdné políčko.
    // Na pozici v mřížce nezáleží (prkna můžeš dát do kteréhokoli rohu),
    // a recept funguje i zrcadlově.
    public class Recept
    {
        public Predmet Vysledek;
        public int Pocet;
        public Predmet[,] Vzor;     // [řádek, sloupec]

        public int Vyska => Vzor.GetLength(0);
        public int Sirka => Vzor.GetLength(1);

        public Recept(Predmet vysledek, int pocet, string[] vzor, params (char Znak, Predmet Predmet)[] klic)
        {
            Vysledek = vysledek;
            Pocet = pocet;

            var slovnik = new Dictionary<char, Predmet>();
            foreach (var k in klic) slovnik[k.Znak] = k.Predmet;

            Vzor = new Predmet[vzor.Length, vzor[0].Length];
            for (int r = 0; r < vzor.Length; r++)
                for (int s = 0; s < vzor[0].Length; s++)
                    Vzor[r, s] = vzor[r][s] == ' ' ? Predmet.Nic : slovnik[vzor[r][s]];
        }

        // sedí oříznutá mřížka na tenhle recept?
        public bool Sedi(Predmet[,] mrizka)
        {
            if (mrizka.GetLength(0) != Vyska || mrizka.GetLength(1) != Sirka)
                return false;

            bool normalne = true, zrcadlove = true;
            for (int r = 0; r < Vyska; r++)
                for (int s = 0; s < Sirka; s++)
                {
                    if (mrizka[r, s] != Vzor[r, s]) normalne = false;
                    if (mrizka[r, s] != Vzor[r, Sirka - 1 - s]) zrcadlove = false;
                }
            return normalne || zrcadlove;
        }
    }

    public static class Crafting
    {
        public static readonly Recept[] Recepty =
        {
            new Recept(Predmet.Prkna, 4, new[] { "D" }, ('D', Predmet.Drevo)),

            new Recept(Predmet.Tycka, 4, new[] { "P",
                                                 "P" }, ('P', Predmet.Prkna)),

            new Recept(Predmet.PracovniStul, 1, new[] { "PP",
                                                        "PP" }, ('P', Predmet.Prkna)),

            // pracovní stůl jde udělat i rovnou ze 4 dřev
            new Recept(Predmet.PracovniStul, 1, new[] { "DD",
                                                        "DD" }, ('D', Predmet.Drevo)),

            new Recept(Predmet.DrevenyKrumpac, 1, new[] { "PPP",
                                                          " T ",
                                                          " T " }, ('P', Predmet.Prkna), ('T', Predmet.Tycka)),

            new Recept(Predmet.KamennyKrumpac, 1, new[] { "KKK",
                                                          " T ",
                                                          " T " }, ('K', Predmet.Kamen), ('T', Predmet.Tycka)),

            new Recept(Predmet.DiamantovyKrumpac, 1, new[] { "DDD",
                                                             " T ",
                                                             " T " }, ('D', Predmet.Diamant), ('T', Predmet.Tycka)),

            new Recept(Predmet.Chleba, 1, new[] { "WWW" }, ('W', Predmet.Psenice)),

            // meče: 2 kusy materiálu nad tyčkou (jako v MC, potřeba stůl)
            new Recept(Predmet.DrevenyMec, 1, new[] { "X", "X", "T" }, ('X', Predmet.Prkna), ('T', Predmet.Tycka)),
            new Recept(Predmet.KamennyMec, 1, new[] { "X", "X", "T" }, ('X', Predmet.Kamen), ('T', Predmet.Tycka)),
            new Recept(Predmet.DiamantovyMec, 1, new[] { "X", "X", "T" }, ('X', Predmet.Diamant), ('T', Predmet.Tycka)),

            // postel: 3 vlny nahoře, 3 prkna (nebo 3 dřeva) dole
            new Recept(Predmet.Postel, 1, new[] { "VVV",
                                                  "PPP" }, ('V', Predmet.Vlna), ('P', Predmet.Prkna)),
            new Recept(Predmet.Postel, 1, new[] { "VVV",
                                                  "DDD" }, ('V', Predmet.Vlna), ('D', Predmet.Drevo)),

            new Recept(Predmet.Truhla, 1, new[] { "PPP",
                                                  "P P",
                                                  "PPP" }, ('P', Predmet.Prkna)),

            // brnění (stejné tvary jako v MC, potřeba pracovní stůl)
            new Recept(Predmet.ZlataHelma, 1, new[] { "XXX",
                                                      "X X" }, ('X', Predmet.Zlato)),
            new Recept(Predmet.ZlatyKyrys, 1, new[] { "X X",
                                                      "XXX",
                                                      "XXX" }, ('X', Predmet.Zlato)),
            new Recept(Predmet.ZlateKalhoty, 1, new[] { "XXX",
                                                        "X X",
                                                        "X X" }, ('X', Predmet.Zlato)),
            new Recept(Predmet.ZlateBoty, 1, new[] { "X X",
                                                     "X X" }, ('X', Predmet.Zlato)),

            new Recept(Predmet.DiamantovaHelma, 1, new[] { "XXX",
                                                           "X X" }, ('X', Predmet.Diamant)),
            new Recept(Predmet.DiamantovyKyrys, 1, new[] { "X X",
                                                           "XXX",
                                                           "XXX" }, ('X', Predmet.Diamant)),
            new Recept(Predmet.DiamantoveKalhoty, 1, new[] { "XXX",
                                                             "X X",
                                                             "X X" }, ('X', Predmet.Diamant)),
            new Recept(Predmet.DiamantoveBoty, 1, new[] { "X X",
                                                          "X X" }, ('X', Predmet.Diamant)),
        };

        // mřížka n x n (sloty po řádcích) -> odpovídající recept nebo null
        public static Recept Najdi(Slot[] mrizka, int n)
        {
            // najdeme obdélník, ve kterém něco leží
            int minR = n, maxR = -1, minS = n, maxS = -1;
            for (int r = 0; r < n; r++)
                for (int s = 0; s < n; s++)
                    if (!mrizka[r * n + s].JePrazdny)
                    {
                        minR = Math.Min(minR, r); maxR = Math.Max(maxR, r);
                        minS = Math.Min(minS, s); maxS = Math.Max(maxS, s);
                    }

            if (maxR < 0) return null;   // mřížka je prázdná

            Predmet[,] orez = new Predmet[maxR - minR + 1, maxS - minS + 1];
            for (int r = minR; r <= maxR; r++)
                for (int s = minS; s <= maxS; s++)
                {
                    Slot slot = mrizka[r * n + s];
                    orez[r - minR, s - minS] = slot.JePrazdny ? Predmet.Nic : slot.Predmet;
                }

            foreach (Recept recept in Recepty)
                if (recept.Sedi(orez))
                    return recept;

            return null;
        }
    }
}
