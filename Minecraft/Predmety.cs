namespace Minecraft
{
    // Všechny věci, které může hráč mít v inventáři
    public enum Predmet
    {
        Nic,

        // bloky, které se dají vytěžit
        Drevo, Psenice, Kamen, Zlato, Diamant, Snih, Led, Dyne, Pisek, Kaktus, Cesta,
        EndKamen, Obsidian, DraciVejce,

        // věci, které se vyrábí craftingem
        Prkna, Tycka, Chleba, PracovniStul, Truhla, Postel, Vlna,
        DrevenyKrumpac, KamennyKrumpac, DiamantovyKrumpac,
        DrevenyMec, KamennyMec, DiamantovyMec,

        // z mobů a zvířat
        Veprove, Hovezi, Kureci, ShnileMaso, Provazek,

        // brnění
        ZlataHelma, ZlatyKyrys, ZlateKalhoty, ZlateBoty,
        DiamantovaHelma, DiamantovyKyrys, DiamantoveKalhoty, DiamantoveBoty
    }

    // pořadí slotů na brnění (shora dolů jako v MC)
    public enum TypBrneni { Zadne = -1, Helma = 0, Kyrys = 1, Kalhoty = 2, Boty = 3 }

    public static class InfoPredmetu
    {
        public static string Nazev(Predmet p)
        {
            switch (p)
            {
                case Predmet.Drevo: return "Dřevo";
                case Predmet.Psenice: return "Pšenice";
                case Predmet.Kamen: return "Kámen";
                case Predmet.Zlato: return "Zlato";
                case Predmet.Diamant: return "Diamant";
                case Predmet.Snih: return "Sníh";
                case Predmet.Led: return "Led";
                case Predmet.Dyne: return "Dýně";
                case Predmet.Pisek: return "Písek";
                case Predmet.Kaktus: return "Kaktus";
                case Predmet.Cesta: return "Cesta";
                case Predmet.Prkna: return "Prkna";
                case Predmet.Tycka: return "Tyčka";
                case Predmet.Chleba: return "Chleba";
                case Predmet.PracovniStul: return "Pracovní stůl";
                case Predmet.Truhla: return "Truhla";
                case Predmet.Postel: return "Postel";
                case Predmet.Vlna: return "Vlna";
                case Predmet.EndKamen: return "End kámen";
                case Predmet.Obsidian: return "Obsidián";
                case Predmet.DraciVejce: return "Dračí vejce";
                case Predmet.DrevenyKrumpac: return "Dřevěný krumpáč";
                case Predmet.KamennyKrumpac: return "Kamenný krumpáč";
                case Predmet.DiamantovyKrumpac: return "Diamantový krumpáč";
                case Predmet.DrevenyMec: return "Dřevěný meč";
                case Predmet.KamennyMec: return "Kamenný meč";
                case Predmet.DiamantovyMec: return "Diamantový meč";
                case Predmet.Veprove: return "Vepřové";
                case Predmet.Hovezi: return "Hovězí";
                case Predmet.Kureci: return "Kuřecí";
                case Predmet.ShnileMaso: return "Shnilé maso";
                case Predmet.Provazek: return "Provázek";
                case Predmet.ZlataHelma: return "Zlatá helma";
                case Predmet.ZlatyKyrys: return "Zlatý kyrys";
                case Predmet.ZlateKalhoty: return "Zlaté kalhoty";
                case Predmet.ZlateBoty: return "Zlaté boty";
                case Predmet.DiamantovaHelma: return "Diamantová helma";
                case Predmet.DiamantovyKyrys: return "Diamantový kyrys";
                case Predmet.DiamantoveKalhoty: return "Diamantové kalhoty";
                case Predmet.DiamantoveBoty: return "Diamantové boty";
                default: return "";
            }
        }

        // síla krumpáče: 0 = ruka, 1 = dřevěný, 2 = kamenný, 3 = diamantový
        public static int SilaKrumpace(Predmet p)
        {
            switch (p)
            {
                case Predmet.DrevenyKrumpac: return 1;
                case Predmet.KamennyKrumpac: return 2;
                case Predmet.DiamantovyKrumpac: return 3;
                default: return 0;
            }
        }

        // kolikrát rychleji se s tímhle v ruce těží
        public static float RychlostTezeni(Predmet p)
        {
            switch (p)
            {
                case Predmet.DrevenyKrumpac: return 2f;
                case Predmet.KamennyKrumpac: return 4f;
                case Predmet.DiamantovyKrumpac: return 7f;
                default: return 1f;
            }
        }

        // kolik hladu jídlo doplní (0 = není to jídlo)
        public static int Nasyceni(Predmet p)
        {
            switch (p)
            {
                case Predmet.Dyne: return 4;
                case Predmet.Chleba: return 6;
                case Predmet.Veprove: return 4;
                case Predmet.Hovezi: return 4;
                case Predmet.Kureci: return 2;
                case Predmet.ShnileMaso: return 2;
                default: return 0;
            }
        }

        // kolik ubere úder (ruka = 1, jako v MC)
        public static float Poskozeni(Predmet p)
        {
            switch (p)
            {
                case Predmet.DrevenyMec: return 4;
                case Predmet.KamennyMec: return 5;
                case Predmet.DiamantovyMec: return 7;
                case Predmet.DrevenyKrumpac: return 2;
                case Predmet.KamennyKrumpac: return 3;
                case Predmet.DiamantovyKrumpac: return 5;
                default: return 1;
            }
        }

        public static bool JeMec(Predmet p)
        {
            return p == Predmet.DrevenyMec || p == Predmet.KamennyMec || p == Predmet.DiamantovyMec;
        }

        // dá se to položit na zem jako blok?
        public static bool JeBlok(Predmet p)
        {
            switch (p)
            {
                case Predmet.Drevo:
                case Predmet.Prkna:
                case Predmet.PracovniStul:
                case Predmet.Truhla:
                case Predmet.Postel:
                case Predmet.Vlna:
                case Predmet.EndKamen:
                case Predmet.Obsidian:
                case Predmet.DraciVejce:
                case Predmet.Kamen:
                case Predmet.Snih:
                case Predmet.Led:
                case Predmet.Dyne:
                case Predmet.Pisek:
                case Predmet.Kaktus:
                case Predmet.Cesta:
                    return true;
                default:
                    return false;
            }
        }

        public static TypBrneni TypBrneni(Predmet p)
        {
            switch (p)
            {
                case Predmet.ZlataHelma: case Predmet.DiamantovaHelma: return Minecraft.TypBrneni.Helma;
                case Predmet.ZlatyKyrys: case Predmet.DiamantovyKyrys: return Minecraft.TypBrneni.Kyrys;
                case Predmet.ZlateKalhoty: case Predmet.DiamantoveKalhoty: return Minecraft.TypBrneni.Kalhoty;
                case Predmet.ZlateBoty: case Predmet.DiamantoveBoty: return Minecraft.TypBrneni.Boty;
                default: return Minecraft.TypBrneni.Zadne;
            }
        }

        // body brnění jako v MC (max 20 = plné diamantové)
        public static int BodyBrneni(Predmet p)
        {
            switch (p)
            {
                case Predmet.ZlataHelma: return 2;
                case Predmet.ZlatyKyrys: return 5;
                case Predmet.ZlateKalhoty: return 3;
                case Predmet.ZlateBoty: return 1;
                case Predmet.DiamantovaHelma: return 3;
                case Predmet.DiamantovyKyrys: return 8;
                case Predmet.DiamantoveKalhoty: return 6;
                case Predmet.DiamantoveBoty: return 3;
                default: return 0;
            }
        }

        public static bool JeDiamantove(Predmet p)
        {
            return p == Predmet.DiamantovaHelma || p == Predmet.DiamantovyKyrys ||
                   p == Predmet.DiamantoveKalhoty || p == Predmet.DiamantoveBoty;
        }

        // nástroje a brnění se nestackují
        public static int MaxVStacku(Predmet p)
        {
            if (SilaKrumpace(p) > 0 || JeMec(p) || TypBrneni(p) != Minecraft.TypBrneni.Zadne) return 1;
            return 64;
        }
    }
}
