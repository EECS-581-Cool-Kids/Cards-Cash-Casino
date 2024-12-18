/*
 *  Module Name: UserHand.cs
 *  Purpose: Models the user's hand of cards.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Derek Norton, Mo Morgan, Richard Moser
 *  Date: 10/21/2024
 *  Last Modified: 12/8/2024
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
using System.Xml.Linq;

namespace CardsCashCasino
{
    /// <summary>
    /// An enumeration of the selected game.
    /// </summary>
    public enum SelectedGame
    {
        NONE,
        BLACKJACK,
        HOLDEM,
        FIVECARD
    }
    
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
        /// The internal Main menu object.
        /// </summary>
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
        /// The Texas Hold 'Em manager for the game.
        /// </summary>
        private FiveCardDrawManager _fiveCardDrawManager = new();

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
            StatisticsUtil.LoadStatisticsFile();
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
            
            _texasHoldEmManager.RequestCardManagerClear = _cardManager.ClearDecks;
            _texasHoldEmManager.RequestDecksOfCards = _cardManager.GenerateDecks;
            _texasHoldEmManager.RequestCard = _cardManager.DrawCard;
            _texasHoldEmManager.RequestShuffle = _cardManager.Shuffle;
            _texasHoldEmManager.RequestRecycle = _cardManager.Recycle;
            _texasHoldEmManager.RequestDeckSize = _cardManager.GetDeckSize;
            _texasHoldEmManager.RequestCardDiscard = _cardManager.Discard;
            _texasHoldEmManager.StartRaise = _bettingManager.OpenBettingMenu;

            _fiveCardDrawManager.RequestCardManagerClear = _cardManager.ClearDecks;
            _fiveCardDrawManager.RequestDecksOfCards = _cardManager.GenerateDecks;
            _fiveCardDrawManager.RequestCard = _cardManager.DrawCard;
            _fiveCardDrawManager.RequestShuffle = _cardManager.Shuffle;
            _fiveCardDrawManager.RequestRecycle = _cardManager.Recycle;
            _fiveCardDrawManager.RequestDeckSize = _cardManager.GetDeckSize;
            _fiveCardDrawManager.RequestCardDiscard = _cardManager.Discard;
            _fiveCardDrawManager.StartRaise = _bettingManager.OpenBettingMenu;

            BettingManager.RequestMainMenuReturn = SetSelectedGame;

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
            _blackjackManager.LoadContent(Content);
            _texasHoldEmManager.LoadContent(Content);
            _fiveCardDrawManager.LoadContent(Content);

            _bettingManager.LoadContent();

            // Initialize MainMenu
            _mainMenu = new MainMenu(this); // Pass the current game instance
            _mainMenu.LoadContent(Content); // Load MainMenu content
        }

        /// <summary>
        /// MonoGame Update method. Called every tick.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Exit the game on Escape
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                SetSelectedGame(SelectedGame.NONE);

            // Main Menu Logic
            if (_selectedGame == SelectedGame.NONE && (GameStartTimeout is null || !GameStartTimeout.Enabled))
            {
                // _mainMenu?.Update(); // Update MainMenu
                _mainMenu?.Update(gameTime); // Pass gameTime here

            }
            else
            {
                // Game-Specific Logic
                switch (_selectedGame)
                {
                    case SelectedGame.BLACKJACK:
                        if (!_bettingManager.HasBet)
                        {
                            if (!_bettingManager.IsBetting)
                                _bettingManager.OpenBettingMenu();
                            _bettingManager.Update();
                            break;
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
                            _texasHoldEmManager.Initialize();
                        }
                        else if (_bettingManager.IsBetting)
                        {
                            _bettingManager.Update();
                            break;
                        }
                        _texasHoldEmManager.Update();
                        break;

                    case SelectedGame.FIVECARD:
                        if (!_fiveCardDrawManager.IsPlaying)
                        {
                            _fiveCardDrawManager.Initialize();
                        }
                        else if (_bettingManager.IsBetting)
                        {
                            _bettingManager.Update();
                            break;
                        }
                        _fiveCardDrawManager.Update();
                        break;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            // _spriteBatch!.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch!.Begin(
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend
            );


            if (_selectedGame == SelectedGame.NONE)
            {
                _mainMenu?.Draw(_spriteBatch); // Draw MainMenu
            }
            else if (_selectedGame == SelectedGame.BLACKJACK && _blackjackManager.IsPlaying)
            {
                _blackjackManager.Draw(_spriteBatch);
            }
            else if (_selectedGame == SelectedGame.HOLDEM && _texasHoldEmManager.IsPlaying)
            {
                _texasHoldEmManager.Draw(_spriteBatch);
            }
            else if (_selectedGame == SelectedGame.FIVECARD && _fiveCardDrawManager.IsPlaying)
            {
                _fiveCardDrawManager.Draw(_spriteBatch);
            }

            _bettingManager.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SetSelectedGame(SelectedGame selectedGame)
        {
            _selectedGame = selectedGame;

            GameStartTimeout = new(1000);
            GameStartTimeout.Elapsed += OnTimeoutEvent!;
            GameStartTimeout.Start();
        }

        /// <summary>
        /// Event called when a timer times out.
        /// </summary>
        private static void OnTimeoutEvent(object source, ElapsedEventArgs e)
        {
            // Stop and dispose of the timer
            Timer timer = (Timer)source;
            timer.Stop();
            timer.Dispose();
        }

        /// <summary>
        /// Quit the game.
        /// </summary>
        public void QuitGame()
        {
            StatisticsUtil.SaveStatisticsFile(); // Save Data
            Exit(); // Quit the game
        }
    }
}
