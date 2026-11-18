using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using System.Collections.Generic;

namespace Minecraft
{

    public static class Katalog
    {
        public enum TypBiomu { Louka, Doly }

        public enum Louka { Trava, Drevo, Pole }
        public enum Doly { Kamen, Zlato, Diamanty }
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

        List<Biome> svet;

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

            pozice_hrace = new Vector2(_graphics.PreferredBackBufferWidth / 2  - velikost_hrace.Y / 2, _graphics.PreferredBackBufferHeight / 2 - velikost_hrace.X / 2);

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
            textury_vsech_biomu = new Texture2D[2][];
            
            textury_vsech_biomu[0] = new Texture2D[3];
            textury_vsech_biomu[1] = new Texture2D[3];


            // Louka biome
            textury_vsech_biomu[(int)Katalog.TypBiomu.Louka][(int)Katalog.Louka.Trava] = Content.Load<Texture2D>("trava");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Louka][(int)Katalog.Louka.Pole] = Content.Load<Texture2D>("pole");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Louka][(int)Katalog.Louka.Drevo] = Content.Load<Texture2D>("drevo");

            // Doly biome
            textury_vsech_biomu[(int)Katalog.TypBiomu.Doly][(int)Katalog.Doly.Kamen] = Content.Load<Texture2D>("kamen");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Doly][(int)Katalog.Doly.Zlato] = Content.Load<Texture2D>("zlato");
            textury_vsech_biomu[(int)Katalog.TypBiomu.Doly][(int)Katalog.Doly.Diamanty] = Content.Load<Texture2D>("diamanty");


            svet = new List<Biome>() { new Biome(textury_vsech_biomu, (int)Katalog.TypBiomu.Louka) };

            

            // TODO: use this.Content to load your game content here
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
            if (Keyboard.GetState().IsKeyDown(Keys.A)) smer.Y -= 1;





            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            if (Keyboard.GetState().IsKeyDown(Keys.S)) _spriteBatch.Draw(textura_steve_dolu, pozice_hrace, Color.White);
            else if (Keyboard.GetState().IsKeyDown(Keys.A)) _spriteBatch.Draw(textura_steve_vlevo, pozice_hrace, Color.White);
            else if (Keyboard.GetState().IsKeyDown(Keys.D)) _spriteBatch.Draw(textura_steve_vpravo, pozice_hrace, Color.White);
            else _spriteBatch.Draw(textura_steve_nahoru, pozice_hrace, Color.White);

            foreach (Biome biom in svet)
                biom.Vykreslit(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
