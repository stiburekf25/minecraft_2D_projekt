using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minecraft
{
    // Textury všech předmětů. Věci, které nemají obrázek v Content,
    // se vyrobí z pixel artu přímo v kódu ('.' = průhledné).
    internal static class Textury
    {
        public static Texture2D Pixel;
        public static Dictionary<Predmet, Texture2D> Predmety = new Dictionary<Predmet, Texture2D>();

        static readonly Color HNEDA = new Color(120, 80, 40);

        static readonly string[] KRUMPAC =
        {
            ".HHHHHH.",
            "H..SS..H",
            "...SS...",
            "...SS...",
            "...SS...",
            "...SS...",
            "...SS...",
            "...SS...",
        };

        static readonly string[] TYCKA =
        {
            "......SS",
            ".....SS.",
            "....SS..",
            "...SS...",
            "..SS....",
            ".SS.....",
            "SS......",
            "S.......",
        };

        static readonly string[] PRKNA =
        {
            "PPPPPPPP",
            "PPPPPPPP",
            "pppppppp",
            "PPPPPPPP",
            "PPPPPPPP",
            "pppppppp",
            "PPPPPPPP",
            "PPPPPPPP",
        };

        static readonly string[] CHLEBA =
        {
            "........",
            "..CCCC..",
            ".CcCcCC.",
            "CCCCCCCC",
            "CCCCCCCC",
            ".CCCCCC.",
            "........",
            "........",
        };

        static readonly string[] PSENICE =
        {
            ".Y..Y..Y",
            "YyY.yY.y",
            ".yYYyYYy",
            "..yYyYy.",
            "...GGG..",
            "...GGG..",
            "..G.G.G.",
            ".G..G..G",
        };

        static readonly string[] MEC =
        {
            "......BB",
            ".....BBB",
            "....BBB.",
            "...BBB..",
            "S.BBB...",
            ".SBB....",
            ".SS.....",
            "S..S....",
        };

        static readonly string[] MASO =
        {
            "........",
            "..MMMM..",
            ".MMmMMM.",
            "MMMMMmMF",
            "MMmMMMMF",
            ".MMMMMF.",
            "..FFFF..",
            "........",
        };

        static readonly string[] PROVAZEK =
        {
            "......W.",
            ".....W.W",
            "....W...",
            "...W....",
            "..W.....",
            ".W.W....",
            "W...W...",
            "........",
        };

        // ---------- moby (pohled shora, hlava nahoře = směr chůze) ----------

        static readonly string[] PRASE =
        {
            "..SSSS..",
            ".PSnnSP.",
            ".PEPPEP.",
            "PPPPPPPP",
            "PPPPPPPP",
            "PPPPPPPP",
            ".PPPPPP.",
            "...PP...",
        };

        static readonly string[] KRAVA =
        {
            "H.KKKK.H",
            ".KEKKEK.",
            "..KNNK..",
            ".WWBWWW.",
            "WBBWWBWW",
            "WWWWBBWW",
            "WBWWWWBW",
            ".WWWWWW.",
        };

        static readonly string[] KURE =
        {
            "...OO...",
            "..WRRW..",
            "..WEEW..",
            ".WWWWWW.",
            "WWWWWWWW",
            ".WWWWWW.",
            "..WWWW..",
            "........",
        };

        static readonly string[] ZOMBIE =
        {
            ".A....A.",
            ".A....A.",
            ".A.HH.A.",
            "TTHhhHTT",
            "TTHhhHTT",
            "TT.HH.TT",
            ".TTTTTT.",
            "..LL.LL.",
        };

        static readonly string[] PAVOUK =
        {
            "L.L..L.L",
            ".LEBBEL.",
            "LLBBBBLL",
            "..BBBB..",
            "LLBBBBLL",
            ".LBBBBL.",
            "L.BBBB.L",
            "...BB...",
        };

        static readonly string[] OVCE =
        {
            "..GGGG..",
            "..GEEG..",
            ".WWWWWW.",
            "WWwWWWwW",
            "WWWWWWWW",
            "WwWWWwWW",
            "WWWWWWWW",
            ".WWWWWW.",
        };

        static readonly string[] VLNA =
        {
            "WWwWWWWw",
            "WWWWwWWW",
            "wWWWWWwW",
            "WWwWWWWW",
            "WWWWWwWW",
            "WwWWWWWw",
            "WWWWwWWW",
            "wWWWWWWW",
        };

        static readonly string[] POSTEL =
        {
            "BWWWWWWB",
            "BWWWWWWB",
            "BRRRRRRB",
            "BRrRRrRB",
            "BRRRRRRB",
            "BRrRRrRB",
            "BRRRRRRB",
            "BBBBBBBB",
        };

        static readonly string[] END_KAMEN =
        {
            "YYyYYYYy",
            "YYYYyYYY",
            "yYYYYYyY",
            "YYyYYYYY",
            "YYYYYyYY",
            "YyYYYYYy",
            "YYYYyYYY",
            "yYYYYYYY",
        };

        static readonly string[] OBSIDIAN =
        {
            "OOoOOOpO",
            "OpOOoOOO",
            "OOOOOOoO",
            "oOOpOOOO",
            "OOOOOoOp",
            "OoOOOOOO",
            "OOOpoOOo",
            "pOOOOOOO",
        };

        static readonly string[] VEJCE =
        {
            "...EE...",
            "..EeEE..",
            ".EEEEeE.",
            ".EeEEEE.",
            "EEEEeEEE",
            "EEeEEEEE",
            ".EEEEeE.",
            "..EEEE..",
        };

        // šipka k Endu (ukazuje nahoru, kreslí se natočená)
        static readonly string[] SIPKA =
        {
            "...PP...",
            "..PPPP..",
            ".PPPPPP.",
            "PPPPPPPP",
            "...PP...",
            "...PP...",
            "...PP...",
            "...PP...",
        };

        // drak shora, hlava nahoře
        static readonly string[] DRAK =
        {
            "......H..H......",
            ".......HH.......",
            "......BEEB......",
            "......BBBB......",
            ".......BB.......",
            "W.....BBBB.....W",
            "WW...BBBBBB...WW",
            "WWW.BBBBBBBB.WWW",
            "WWWWBBBBBBBBWWWW",
            ".WWWWBBBBBBWWWW.",
            "..WWW.BBBB.WWW..",
            "...W..BBBB..W...",
            ".......BB.......",
            ".......BB.......",
            "........B.......",
            ".........B......",
        };

        public static Dictionary<TypMoba, Texture2D> Moby = new Dictionary<TypMoba, Texture2D>();
        public static Texture2D Drak;
        public static Texture2D Sipka;

        // textury End biomu (pořadí podle Katalog.End) - musí se vytvořit dřív než Nacti
        public static Texture2D[] EndBiom(GraphicsDevice zarizeni)
        {
            return new[]
            {
                ZMasky(zarizeni, END_KAMEN, new Dictionary<char, Color>
                    { { 'Y', new Color(222, 222, 160) }, { 'y', new Color(190, 190, 130) } }),
                ZMasky(zarizeni, OBSIDIAN, new Dictionary<char, Color>
                    { { 'O', new Color(20, 15, 30) }, { 'o', new Color(45, 28, 65) }, { 'p', new Color(80, 45, 115) } }),
            };
        }

        // truhla shora: dřevo, tmavý rám a zámek
        static readonly string[] TRUHLA =
        {
            "OOOOOOOO",
            "OTTTTTTO",
            "OTTTTTTO",
            "OOOZZOOO",
            "OTTZZTTO",
            "OTTTTTTO",
            "OTTTTTTO",
            "OOOOOOOO",
        };

        // vyšlapaná cesta ve vesnici
        static readonly string[] CESTA =
        {
            "CCcCCCCc",
            "CcCCCcCC",
            "CCCCCCCC",
            "cCCcCCcC",
            "CCCCCCCC",
            "CCcCCCCc",
            "CCCCcCCC",
            "cCCCCCcC",
        };

        // brnění ('H' = kov, 'h' = tmavší stín)
        public static readonly string[] HELMA =
        {
            "........",
            "..HHHH..",
            ".HHHHHH.",
            "HHhhhhHH",
            "HH....HH",
            "HH....HH",
            "........",
            "........",
        };

        public static readonly string[] KYRYS =
        {
            "HH....HH",
            "HHH..HHH",
            "HHHHHHHH",
            ".HHhhHH.",
            ".HHHHHH.",
            ".HHhhHH.",
            ".HHHHHH.",
            "........",
        };

        public static readonly string[] KALHOTY =
        {
            "HHHHHHHH",
            "HhhhhhhH",
            "HHH..HHH",
            "HHH..HHH",
            "HHH..HHH",
            "HHH..HHH",
            "hhh..hhh",
            "........",
        };

        public static readonly string[] BOTY =
        {
            "........",
            "........",
            "HH....HH",
            "HH....HH",
            "HH....HH",
            "HHH..HHH",
            "hhh..hhh",
            "........",
        };

        public static readonly Color ZLATA = new Color(245, 205, 60);
        public static readonly Color DIAMANTOVA = new Color(90, 225, 220);

        public static void Nacti(GraphicsDevice zarizeni, Texture2D[][] biomy, Texture2D pracovniStul)
        {
            Pixel = new Texture2D(zarizeni, 1, 1);
            Pixel.SetData(new[] { Color.White });

            // bloky = textury z Content
            Predmety[Predmet.Drevo] = biomy[(int)Katalog.TypBiomu.Louka][(int)Katalog.Louka.Drevo];
            Predmety[Predmet.Kamen] = biomy[(int)Katalog.TypBiomu.Doly][(int)Katalog.Doly.Kamen];
            Predmety[Predmet.Zlato] = biomy[(int)Katalog.TypBiomu.Doly][(int)Katalog.Doly.Zlato];
            Predmety[Predmet.Diamant] = biomy[(int)Katalog.TypBiomu.Doly][(int)Katalog.Doly.Diamanty];
            Predmety[Predmet.Snih] = biomy[(int)Katalog.TypBiomu.Snezny][(int)Katalog.Snezny.Snih];
            Predmety[Predmet.Led] = biomy[(int)Katalog.TypBiomu.Snezny][(int)Katalog.Snezny.Led];
            Predmety[Predmet.Dyne] = biomy[(int)Katalog.TypBiomu.Snezny][(int)Katalog.Snezny.Dyne];
            Predmety[Predmet.Pisek] = biomy[(int)Katalog.TypBiomu.Poust][(int)Katalog.Poust.Pisek];
            Predmety[Predmet.Kaktus] = biomy[(int)Katalog.TypBiomu.Poust][(int)Katalog.Poust.Kaktus];

            Predmety[Predmet.Truhla] = ZMasky(zarizeni, TRUHLA, new Dictionary<char, Color>
                { { 'O', new Color(85, 55, 25) }, { 'T', new Color(170, 115, 50) }, { 'Z', new Color(200, 200, 200) } });

            Predmety[Predmet.Cesta] = ZMasky(zarizeni, CESTA, new Dictionary<char, Color>
                { { 'C', new Color(150, 120, 75) }, { 'c', new Color(120, 95, 55) } });

            // brnění: stejné tvary, jiná barva
            Predmet[] zlate = { Predmet.ZlataHelma, Predmet.ZlatyKyrys, Predmet.ZlateKalhoty, Predmet.ZlateBoty };
            Predmet[] diamantove = { Predmet.DiamantovaHelma, Predmet.DiamantovyKyrys, Predmet.DiamantoveKalhoty, Predmet.DiamantoveBoty };
            string[][] tvary = { HELMA, KYRYS, KALHOTY, BOTY };
            for (int i = 0; i < 4; i++)
            {
                Predmety[zlate[i]] = ZMasky(zarizeni, tvary[i], new Dictionary<char, Color>
                    { { 'H', ZLATA }, { 'h', new Color(190, 145, 30) } });
                Predmety[diamantove[i]] = ZMasky(zarizeni, tvary[i], new Dictionary<char, Color>
                    { { 'H', DIAMANTOVA }, { 'h', new Color(40, 160, 160) } });
            }

            // ostatní z pixel artu
            Predmety[Predmet.Psenice] = ZMasky(zarizeni, PSENICE, new Dictionary<char, Color>
                { { 'Y', new Color(230, 195, 80) }, { 'y', new Color(185, 145, 45) }, { 'G', new Color(150, 130, 50) } });
            Predmety[Predmet.Prkna] = ZMasky(zarizeni, PRKNA, new Dictionary<char, Color>
                { { 'P', new Color(190, 145, 85) }, { 'p', new Color(130, 95, 50) } });
            Predmety[Predmet.Tycka] = ZMasky(zarizeni, TYCKA, new Dictionary<char, Color> { { 'S', HNEDA } });
            Predmety[Predmet.Chleba] = ZMasky(zarizeni, CHLEBA, new Dictionary<char, Color>
                { { 'C', new Color(200, 140, 60) }, { 'c', new Color(150, 95, 35) } });
            Predmety[Predmet.PracovniStul] = pracovniStul;   // crafting.png

            Predmety[Predmet.EndKamen] = biomy[(int)Katalog.TypBiomu.End][(int)Katalog.End.EndKamen];
            Predmety[Predmet.Obsidian] = biomy[(int)Katalog.TypBiomu.End][(int)Katalog.End.Obsidian];
            Predmety[Predmet.Vlna] = ZMasky(zarizeni, VLNA, new Dictionary<char, Color>
                { { 'W', new Color(240, 240, 240) }, { 'w', new Color(205, 205, 205) } });
            Predmety[Predmet.Postel] = ZMasky(zarizeni, POSTEL, new Dictionary<char, Color>
                { { 'W', Color.White }, { 'R', new Color(190, 30, 30) }, { 'r', new Color(150, 20, 20) }, { 'B', new Color(130, 90, 45) } });
            Predmety[Predmet.DraciVejce] = ZMasky(zarizeni, VEJCE, new Dictionary<char, Color>
                { { 'E', new Color(25, 15, 35) }, { 'e', new Color(150, 60, 200) } });

            Moby[TypMoba.Ovce] = ZMasky(zarizeni, OVCE, new Dictionary<char, Color>
                { { 'W', new Color(240, 240, 240) }, { 'w', new Color(210, 210, 210) }, { 'G', new Color(150, 140, 130) }, { 'E', new Color(30, 30, 30) } });
            Drak = ZMasky(zarizeni, DRAK, new Dictionary<char, Color>
                { { 'B', new Color(25, 25, 30) }, { 'W', new Color(60, 60, 75) }, { 'E', new Color(200, 80, 255) }, { 'H', new Color(200, 200, 200) } });
            Sipka = ZMasky(zarizeni, SIPKA, new Dictionary<char, Color> { { 'P', new Color(190, 90, 255) } });

            // meče
            Predmety[Predmet.DrevenyMec] = ZMasky(zarizeni, MEC, new Dictionary<char, Color>
                { { 'B', new Color(170, 125, 70) }, { 'S', HNEDA } });
            Predmety[Predmet.KamennyMec] = ZMasky(zarizeni, MEC, new Dictionary<char, Color>
                { { 'B', new Color(150, 150, 150) }, { 'S', HNEDA } });
            Predmety[Predmet.DiamantovyMec] = ZMasky(zarizeni, MEC, new Dictionary<char, Color>
                { { 'B', new Color(90, 225, 220) }, { 'S', HNEDA } });

            // maso
            Color tuk = new Color(240, 225, 210);
            Predmety[Predmet.Veprove] = ZMasky(zarizeni, MASO, new Dictionary<char, Color>
                { { 'M', new Color(235, 140, 140) }, { 'm', new Color(200, 100, 100) }, { 'F', tuk } });
            Predmety[Predmet.Hovezi] = ZMasky(zarizeni, MASO, new Dictionary<char, Color>
                { { 'M', new Color(190, 40, 40) }, { 'm', new Color(140, 25, 25) }, { 'F', tuk } });
            Predmety[Predmet.Kureci] = ZMasky(zarizeni, MASO, new Dictionary<char, Color>
                { { 'M', new Color(245, 190, 170) }, { 'm', new Color(220, 160, 140) }, { 'F', tuk } });
            Predmety[Predmet.ShnileMaso] = ZMasky(zarizeni, MASO, new Dictionary<char, Color>
                { { 'M', new Color(140, 110, 60) }, { 'm', new Color(100, 120, 50) }, { 'F', new Color(120, 90, 50) } });
            Predmety[Predmet.Provazek] = ZMasky(zarizeni, PROVAZEK, new Dictionary<char, Color> { { 'W', Color.WhiteSmoke } });

            // moby
            Color oci = new Color(30, 30, 30);
            Moby[TypMoba.Prase] = ZMasky(zarizeni, PRASE, new Dictionary<char, Color>
                { { 'P', new Color(240, 160, 165) }, { 'S', new Color(225, 125, 135) }, { 'n', new Color(160, 80, 90) }, { 'E', oci } });
            Moby[TypMoba.Krava] = ZMasky(zarizeni, KRAVA, new Dictionary<char, Color>
                { { 'W', new Color(235, 235, 235) }, { 'B', new Color(40, 40, 40) }, { 'K', new Color(75, 55, 40) },
                  { 'N', new Color(200, 160, 150) }, { 'H', new Color(220, 215, 190) }, { 'E', Color.White } });
            Moby[TypMoba.Kure] = ZMasky(zarizeni, KURE, new Dictionary<char, Color>
                { { 'W', Color.White }, { 'R', new Color(210, 40, 40) }, { 'O', new Color(240, 160, 40) }, { 'E', oci } });
            Moby[TypMoba.Zombie] = ZMasky(zarizeni, ZOMBIE, new Dictionary<char, Color>
                { { 'A', new Color(90, 150, 80) }, { 'H', new Color(70, 120, 60) }, { 'h', new Color(45, 85, 40) },
                  { 'T', new Color(40, 130, 140) }, { 'L', new Color(60, 60, 130) } });
            Moby[TypMoba.Pavouk] = ZMasky(zarizeni, PAVOUK, new Dictionary<char, Color>
                { { 'B', new Color(45, 40, 40) }, { 'L', new Color(30, 25, 25) }, { 'E', new Color(220, 30, 30) } });
            Predmety[Predmet.DrevenyKrumpac] = ZMasky(zarizeni, KRUMPAC, new Dictionary<char, Color>
                { { 'H', new Color(170, 125, 70) }, { 'S', HNEDA } });
            Predmety[Predmet.KamennyKrumpac] = ZMasky(zarizeni, KRUMPAC, new Dictionary<char, Color>
                { { 'H', new Color(140, 140, 140) }, { 'S', HNEDA } });
            Predmety[Predmet.DiamantovyKrumpac] = ZMasky(zarizeni, KRUMPAC, new Dictionary<char, Color>
                { { 'H', new Color(90, 225, 220) }, { 'S', HNEDA } });
        }

        // každý znak masky = čtverec 8x8 pixelů, takže 8x8 maska dá texturu 64x64 (ostrou i na velké dlaždici)
        static Texture2D ZMasky(GraphicsDevice zarizeni, string[] maska, Dictionary<char, Color> barvy)
        {
            const int ZVETSENI = 8;
            int w = maska[0].Length * ZVETSENI;
            int h = maska.Length * ZVETSENI;

            Color[] data = new Color[w * h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    char c = maska[y / ZVETSENI][x / ZVETSENI];
                    data[y * w + x] = c == '.' ? Color.Transparent : barvy[c];
                }

            Texture2D textura = new Texture2D(zarizeni, w, h);
            textura.SetData(data);
            return textura;
        }
    }
}
