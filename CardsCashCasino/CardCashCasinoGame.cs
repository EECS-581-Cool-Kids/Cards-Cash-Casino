/*
 *  Module Name: UserHand.cs
 *  Purpose: Models the user's hand of cards.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Derek Norton, Mo Morgan
 *  Date: 10/21/2024
 *  Last Modified: 11/10/2024
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
using System;
using System.Timers;

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
        
        // /// <summary>
        // /// The internal Main menu object.
        // /// </summary>
        // private MainMenu? _mainMenu; // TODO: Implement the main menu in MainMenu.cs.
        private MainMenu? _mainMenu;

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
        
        /// <summary>
        /// The Texas Hold 'Em manager for the game.
        /// </summary>
        private TexasHoldEmManager _texasHoldEmManager = new();

        /// <summary>
        /// The currently selected game.
        /// </summary>
        private SelectedGame _selectedGame = SelectedGame.NONE;

        /// <summary>
        /// The game start timeout.
        /// </summary>
        public static Timer? GameStartTimeout;

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
            _blackjackManager.RequestMainMenuReturn = EndBlackjack;
            
            _texasHoldEmManager.RequestCardManagerClear = _cardManager.ClearDecks;
            _texasHoldEmManager.RequestDecksOfCards = _cardManager.GenerateDecks;
            _texasHoldEmManager.RequestCard = _cardManager.DrawCard;
            _texasHoldEmManager.RequestShuffle = _cardManager.Shuffle;
            _texasHoldEmManager.RequestRecycle = _cardManager.Recycle;
            _texasHoldEmManager.RequestDeckSize = _cardManager.GetDeckSize;
            _texasHoldEmManager.RequestCardDiscard = _cardManager.Discard;
            _texasHoldEmManager.StartRaise = _bettingManager.OpenBettingMenu;

            base.Initialize();
        }

        /// <summary>
        /// MonoGame LoadContent method. Called at start up.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the textures
            MainMenuTextures.LoadContent(Content); // Load the MainMenu textures
            DisplayIndicatorTextures.LoadContent(Content);
            CardTextures.LoadContent(Content);
            BettingTextures.LoadContent(Content);
            BlackjackTextures.LoadContent(Content);

            // Load game managers
            _blackjackManager.LoadContent();
            _texasHoldEmManager.LoadContent(Content);
            _bettingManager.LoadContent();

            // Initialize MainMenu
            _mainMenu = new MainMenu(this); // Pass the current game instance
            _mainMenu.LoadContent(Content); // Load MainMenu content

            // _selectedGame = SelectedGame.NONE; // temp, remove when main menu is implemented OR change to other games.

        }

        /// <summary>
        /// MonoGame Update method. Called every tick.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {

            // Exit the game on Escape
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Main Menu Logic
            if (_selectedGame == SelectedGame.NONE)
            {
                _mainMenu?.Update(); // Update MainMenu
            }
            else
            {
                // Game-Specific Logic
                switch (_selectedGame)
                {
                    case SelectedGame.BLACKJACK:
                        if (!_bettingManager.HasBet)
                        {
                            _bettingManager.Update();
                            return;
                        }
                        else if (!_blackjackManager.IsPlaying)
                        {
                            _blackjackManager.StartGame();
                        }
                        _blackjackManager.Update();
                        break;

                    case SelectedGame.HOLDEM:
                        if (!_texasHoldEmManager.IsPlaying)
                        {
                            _texasHoldEmManager.StartGame();
                        }
                        _texasHoldEmManager.Update();
                        break;

                    case SelectedGame.FIVECARD:
                        // TODO: Add Five Card Draw logic here
                        break;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            _spriteBatch!.Begin(samplerState: SamplerState.PointClamp);

            if (_selectedGame == SelectedGame.NONE)
            {
                _mainMenu?.Draw(_spriteBatch); // Draw MainMenu
            }
            else if (_selectedGame == SelectedGame.BLACKJACK)
            {
                _blackjackManager.Draw(_spriteBatch);
            }
            else if (_selectedGame == SelectedGame.HOLDEM)
            {
                _texasHoldEmManager.Draw(_spriteBatch);
            }
            // Add logic for Five Card Draw if needed

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SetSelectedGame(SelectedGame selectedGame)
        {
            _selectedGame = selectedGame;
        //     // Optionally, you can include logic to initialize the selected game here.
        //     // For example:
        //     switch (_selectedGame)
        //     {
        //         case SelectedGame.BLACKJACK:
        //             _blackjackManager.StartGame();
        //             break;
        //         case SelectedGame.HOLDEM:
        //             _texasHoldEmManager.StartGame();
        //             break;
        //         case SelectedGame.FIVECARD:
        //             // TODO: Add initialization for Five Card Draw.
        //             break;
        //         case SelectedGame.NONE:
        //             // Reset or return to the main menu state, if needed.
        //             break;
        //     }
        }

        /// <summary>
        /// Ends the current game by resetting the user bet to zero.
        /// </summary>
        private void EndBlackjack()
        {
            BettingManager.UserBet = 0;
            //_selectedGame = SelectedGame.NONE // uncomment to reset to a state of none, once main menu is built out.
        }
    }

    public enum SelectedGame
    {
        NONE,
        BLACKJACK,
        HOLDEM,
        FIVECARD,
        MAINMENU //delete this once main menu is implemented
    }

}
