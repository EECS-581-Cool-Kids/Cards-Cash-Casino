/*
 *  Module Name: UserHand.cs
 *  Purpose: Models the user's hand of cards.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Derek Norton
 *  Date: 10/21/2024
 *  Last Modified: 10/27/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */

using CardsCashCasino.Data;
using CardsCashCasino.Manager;
using CardsCashCasino.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardsCashCasino
{
    public class CardCashCasinoGame : Microsoft.Xna.Framework.Game
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
        private CardManager _cardManager = new();

        /// <summary>
        /// The chip manager for the game.
        /// </summary>
        private BettingManager _bettingManager = new();

        /// <summary>
        /// The blackjack manager for the game.
        /// </summary>
        private BlackjackManager _blackjackManager = new();

        public CardCashCasinoGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// MonoGame Initialize method. Called at start up.
        /// </summary>
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = Constants.WINDOW_HEIGHT;
            _graphics.PreferredBackBufferWidth = Constants.WINDOW_WIDTH;
            _graphics.ApplyChanges();

            _blackjackManager.RequestCardManagerCleared = _cardManager.ClearDecks;
            _blackjackManager.RequestDecksOfCards = _cardManager.GenerateDecks;
            _blackjackManager.RequestCard = _cardManager.DrawCard;
            _blackjackManager.RequestBet = _bettingManager.Bet;
            _blackjackManager.RequestPayout = _bettingManager.Payout;

            base.Initialize();
        }

        /// <summary>
        /// MonoGame LoadContent method. Called at start up.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the textures
            //MainMenuTextures.LoadContent(Content);
            DisplayIndicatorTextures.LoadContent(Content);
            CardTextures.LoadContent(Content);
            BettingTextures.LoadContent(Content);
            BlackjackTextures.LoadContent(Content);
            //TexasHoldEmTextures.LoadContent(Content);
            //FiveCardDrawTextures.LoadContent(Content);

            // Load the manager's base content.
            _bettingManager.LoadContent();
            _blackjackManager.LoadContent();
        }

        /// <summary>
        /// MonoGame Update method. Called every tick.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // same for the main menu
            _bettingManager.Update();
            if (_blackjackManager.IsPlaying)
                _blackjackManager.Update();
            // same for texas hold em
            // same for five card draw

            base.Update(gameTime);
        }

        /// <summary>
        /// MonoGame Draw method. Called every tick.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            _spriteBatch!.Begin(samplerState: SamplerState.PointClamp);
            // same for the main menu
            if (_blackjackManager.IsPlaying)
                _blackjackManager.Draw(_spriteBatch!);
            // same for texas hold em
            // same for five card draw
            _bettingManager.Draw(_spriteBatch);
            _spriteBatch!.End();

            base.Draw(gameTime);
        }
    }
}
