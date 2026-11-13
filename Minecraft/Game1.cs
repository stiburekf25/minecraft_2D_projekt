using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;

namespace Minecraft
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Vector2 velikost_hrace = new Vector2(50, 50);
        Vector2 pozice_hrace;


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

            _spriteBatch.FillRectangle(pozice_hrace, velikost_hrace, Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
