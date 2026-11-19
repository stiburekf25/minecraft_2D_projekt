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
        public enum TypBiomu { Louka, Doly, Snezny }

        public enum Louka { Trava, Drevo, Pole }
        public enum Doly { Kamen, Zlato, Diamanty }
        public enum Snezny { Snih, Led, Dyne }
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Vector2 velikost_hrace = new Vector2(50, 50);
        Vector2 pozice_hrace;

        Texture2D textura_steve_nahoru;
        Texture2D textura_steve_dolu;
        Texture2D textura_steve_vlevo;
        Texture2D textura_steve_vpravo;


        Texture2D[] [] textury_vsech_biomu;
        Texture2D[] textury_louka;
        Texture2D[] textury_doly;

        Dictionary<Point, Biome> svet;
        Random nahoda = new Random();

        int sirka_sveta = 1 * Biome.VELIKOST;   // 320 px
        int vyska_sveta = 1 * Biome.VELIKOST;   // 320 px

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

            pozice_hrace = new Vector2(135, 135);

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
            textury_vsech_biomu = new Texture2D[3][];
            
            textury_vsech_biomu[0] = new Texture2D[3];
            textury_vsech_biomu[1] = new Texture2D[3];
            textury_vsech_biomu[1] = new Texture2D[3];



            // Louka biome
            textury_vsech_biomu[(int)Katalog.TypBiomu.Louka][(int)Katalog.Louka.Trava] = Content.Load<Texture2D>("trava");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Louka][(int)Katalog.Louka.Pole] = Content.Load<Texture2D>("pole");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Louka][(int)Katalog.Louka.Drevo] = Content.Load<Texture2D>("drevo");

            // Doly biome
            textury_vsech_biomu[(int)Katalog.TypBiomu.Doly][(int)Katalog.Doly.Kamen] = Content.Load<Texture2D>("kamen");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Doly][(int)Katalog.Doly.Zlato] = Content.Load<Texture2D>("zlato");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Doly][(int)Katalog.Doly.Diamanty] = Content.Load<Texture2D>("diamanty");

            textury_vsech_biomu[(int)Katalog.TypBiomu.Snezny][(int)Katalog.Snezny.Led] = Content.Load<Texture2D>("led");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Snezny][(int)Katalog.Snezny.Dyne] = Content.Load<Texture2D>("dyne");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Snezny][(int)Katalog.Snezny.Snih] = Content.Load<Texture2D>("snih");



            svet = new Dictionary<Point, Biome>();


            // TODO: use this.Content to load your game content here
        }
        void GenerujOkoli()
        {
            Vector2 stred_hrace = pozice_hrace + velikost_hrace / 2;

            // v jakém biomu Steve stojí (Floor kvůli záporným souřadnicím)
            int bx = (int)Math.Floor(stred_hrace.X / Biome.VELIKOST);
            int by = (int)Math.Floor(stred_hrace.Y / Biome.VELIKOST);

            for (int dy = -2; dy <= 2; dy++)
                for (int dx = -3; dx <= 3; dx++)
                {
                    Point klic = new Point(bx + dx, by + dy);

                    if (!svet.ContainsKey(klic))
                    {
                        Katalog.TypBiomu typ = (Katalog.TypBiomu)nahoda.Next(2);   // 0 = Louka, 1 = Doly
                        Vector2 pozice = new Vector2(klic.X * Biome.VELIKOST, klic.Y * Biome.VELIKOST);

                        svet.Add(klic, new Biome(textury_vsech_biomu, typ, pozice));
                    }
                }
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            Vector2 smer = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.W)) smer.Y -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) smer.Y += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) smer.X += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.A)) smer.X -= 1;

            pozice_hrace += smer * 3;

            GenerujOkoli();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            Vector2 stred_obrazovky = new Vector2(
                _graphics.PreferredBackBufferWidth / 2f,
                _graphics.PreferredBackBufferHeight / 2f);

            // kamera = střed Steva ve světě minus střed obrazovky
            Vector2 kamera = pozice_hrace + velikost_hrace / 2 - stred_obrazovky;

            // Steve se vždy kreslí na střed obrazovky
            Rectangle steve = new Rectangle(
                (int)(stred_obrazovky.X - velikost_hrace.X / 2),
                (int)(stred_obrazovky.Y - velikost_hrace.Y / 2),
                (int)velikost_hrace.X,
                (int)velikost_hrace.Y);

            _spriteBatch.Begin();

            foreach (Biome biom in svet.Values)
                biom.Vykreslit(_spriteBatch, kamera);

            if (Keyboard.GetState().IsKeyDown(Keys.S)) _spriteBatch.Draw(textura_steve_dolu, steve, Color.White);
            else if (Keyboard.GetState().IsKeyDown(Keys.A)) _spriteBatch.Draw(textura_steve_vlevo, steve, Color.White);
            else if (Keyboard.GetState().IsKeyDown(Keys.D)) _spriteBatch.Draw(textura_steve_vpravo, steve, Color.White);
            else _spriteBatch.Draw(textura_steve_nahoru, steve, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
