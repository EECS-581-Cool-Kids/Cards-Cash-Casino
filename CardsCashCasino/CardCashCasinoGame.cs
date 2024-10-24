using CardsCashCasino.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardsCashCasino
{
    public class CardCashCasinoGame : Game
    {
        /// <summary>
        /// The graphics device manager for the project.
        /// </summary>
        private GraphicsDeviceManager _graphics;

        /// <summary>
        /// The sprite batch for the project.
        /// </summary>
        private SpriteBatch? _spriteBatch;

        /// <summary>
        /// The card manager for the game.
        /// </summary>
        private CardManager cardManager = new();

        public CardCashCasinoGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            cardManager.GenerateDecks(5);
        }

        /// <summary>
        /// MonoGame Initialize method. Called at start up.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// MonoGame LoadContent method. Called at start up.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the card textures.
            CardTextures.LoadContent(Content);
        }

        /// <summary>
        /// MonoGame Update method. Called every tick.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// MonoGame Draw method. Called every tick.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
