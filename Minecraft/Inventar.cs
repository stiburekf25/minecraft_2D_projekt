using System;

namespace Minecraft
{
    public class Slot
    {
        public Predmet Predmet = Predmet.Nic;
        public int Pocet = 0;

        public bool JePrazdny => Predmet == Predmet.Nic || Pocet <= 0;

        public void Nastav(Predmet p, int pocet)
        {
            Predmet = p;
            Pocet = pocet;
            if (Pocet <= 0) Vyprazdni();
        }

        public void Vyprazdni()
        {
            Predmet = Predmet.Nic;
            Pocet = 0;
        }
    }

    // Inventář jako v Minecraftu: sloty 0-8 = hotbar, 9-35 = hlavní inventář
    public class Inventar
    {
        public const int HOTBAR = 9;
        public const int POCET_SLOTU = 36;
        public const int TRUHLA = 27;       // počet slotů v truhle

        public Slot[] Sloty = new Slot[POCET_SLOTU];
        public Slot[] Brneni = new Slot[4];     // helma, kyrys, kalhoty, boty
        public int Vybrany = 0;     // vybraný slot v hotbaru (0-8)

        public Inventar()
        {
            for (int i = 0; i < POCET_SLOTU; i++)
                Sloty[i] = new Slot();
            for (int i = 0; i < Brneni.Length; i++)
                Brneni[i] = new Slot();
        }

        // součet bodů brnění (0-20), každý bod snižuje poškození o 4 %
        public int BodyBrneni()
        {
            int soucet = 0;
            foreach (Slot slot in Brneni)
                soucet += InfoPredmetu.BodyBrneni(slot.Predmet);
            return soucet;
        }

        public Slot VybranySlot => Sloty[Vybrany];

        // kolik kusů daného předmětu hráč má (ve všech slotech dohromady)
        public int Spocitej(Predmet p)
        {
            int soucet = 0;
            foreach (Slot slot in Sloty)
                if (slot.Predmet == p)
                    soucet += slot.Pocet;
            return soucet;
        }

        // vrátí true, když se všechno vešlo
        public bool Pridej(Predmet p, int pocet)
        {
            return PridejDo(p, pocet, 0, POCET_SLOTU) == 0;
        }

        // přidá do slotů od..do-1, vrátí kolik se NEvešlo
        public int PridejDo(Predmet p, int pocet, int od, int doSlotu)
        {
            return PridejDoSlotu(Sloty, p, pocet, od, doSlotu);
        }

        // funguje pro jakékoli pole slotů (inventář, truhla...), vrátí kolik se NEvešlo
        public static int PridejDoSlotu(Slot[] sloty, Predmet p, int pocet, int od, int doSlotu)
        {
            int max = InfoPredmetu.MaxVStacku(p);

            // nejdřív doplnit sloty, kde už ten předmět je
            for (int i = od; i < doSlotu && pocet > 0; i++)
            {
                Slot slot = sloty[i];
                if (slot.Predmet == p && slot.Pocet < max)
                {
                    int kolik = Math.Min(max - slot.Pocet, pocet);
                    slot.Pocet += kolik;
                    pocet -= kolik;
                }
            }

            // pak prázdné sloty
            for (int i = od; i < doSlotu && pocet > 0; i++)
            {
                Slot slot = sloty[i];
                if (slot.JePrazdny)
                {
                    int kolik = Math.Min(max, pocet);
                    slot.Nastav(p, kolik);
                    pocet -= kolik;
                }
            }

            return pocet;
        }

        public void OdeberZVybraneho(int pocet)
        {
            Slot slot = VybranySlot;
            slot.Nastav(slot.Predmet, slot.Pocet - pocet);
        }

        // kopie se hodí, když chceme něco zkusit "nanečisto"
        public Inventar Kopie()
        {
            Inventar kopie = new Inventar();
            for (int i = 0; i < POCET_SLOTU; i++)
                kopie.Sloty[i].Nastav(Sloty[i].Predmet, Sloty[i].Pocet);
            kopie.Vybrany = Vybrany;
            return kopie;
        }
    }
}
