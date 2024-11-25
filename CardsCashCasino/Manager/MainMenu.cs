/*
 *  Module Name: MainMenu.cs
 *  Purpose: provide a main menu for the user to select a game to play
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Richard Moser
 *  Date: 11/20/2024
 *  Last Modified: 11/24/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */

using CardsCashCasino.Data;
using CardsCashCasino.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace CardsCashCasino.Manager
{

    public class MainMenu
    {
        #region Properties
        /// <summary>
        /// The current position of the cursor.
        /// </summary>
        private int _currentCursorPos = 0;

        /// <summary>
        /// The timeout for the cursor to move.
        /// </summary>
        private Timer? _cursorMoveTimeout;

        /// <summary>
        /// The timeout for the user "hitting" their deck.
        /// </summary>
        private Timer? _userMoveTimeout;

        /// <summary>
        /// The background texture for the main menu.
        /// </summary>
        private Texture2D? _backgroundTexture;

        /// <summary>
        /// The blackjack button for the main menu.
        /// </summary>
        private MainMenuActionButton _blackjackButton;

        /// <summary>
        /// The Five Card Draw button for the main menu.
        /// </summary>
        private MainMenuActionButton _fiveCardDrawButton;

        /// <summary>
        /// The Texas Hold 'Em button for the main menu.
        /// </summary>
        private MainMenuActionButton _texasHoldEmButton;

        /// <summary>
        /// The Quit button for the main menu.
        /// </summary>
        private MainMenuActionButton _quitButton;

        /// <summary>
        /// The cursor for the main menu.
        /// </summary>
        private MainMenuCursor _cursor;
        #endregion Properties

        private CardCashCasinoGame _game; // Field to store reference to CardCashCasinoGame

        public MainMenu(CardCashCasinoGame game)
        {
            _game = game;
            CardCashCasinoGame.GameStartTimeout = new(Constants.TIMER_DURATION);
            CardCashCasinoGame.GameStartTimeout.Elapsed += Constants.OnTimeoutEvent!;
            CardCashCasinoGame.GameStartTimeout.Start();
        }
        /// <summary>
        /// The LoadContent method for the main menu.
        ///</summary>
        public void LoadContent(ContentManager content)
        {
            int widthBuffer = (Constants.WINDOW_WIDTH - Constants.MAIN_MENU_BUTTON_WIDTH * Constants.MAIN_MENU_BUTTON_COUNT) / 2;
            int buttonYPos = Constants.WINDOW_HEIGHT - 350;
            int buffer = 30;

            _backgroundTexture = content.Load<Texture2D>("MenuBackground");

            _blackjackButton = new MainMenuActionButton(
                MainMenuTextures.BlackjackButtonTexture,
                MainMenuTextures.BlackjackButtonTexture,
                widthBuffer - buffer,
                buttonYPos,
                Constants.MAIN_MENU_BUTTON_WIDTH,
                Constants.MAIN_MENU_BUTTON_HEIGHT
                );
            _fiveCardDrawButton = new MainMenuActionButton(MainMenuTextures.FiveCardDrawButtonTexture,
                MainMenuTextures.FiveCardDrawButtonTexture,
                widthBuffer + Constants.MAIN_MENU_BUTTON_WIDTH,
                buttonYPos,
                Constants.MAIN_MENU_BUTTON_WIDTH,
                Constants.MAIN_MENU_BUTTON_HEIGHT
                );
            _texasHoldEmButton = new MainMenuActionButton(MainMenuTextures.TexasHoldEmButtonTexture,
                MainMenuTextures.TexasHoldEmButtonTexture,
                widthBuffer + Constants.MAIN_MENU_BUTTON_WIDTH * 2  + buffer,
                buttonYPos,
                Constants.MAIN_MENU_BUTTON_WIDTH,
                Constants.MAIN_MENU_BUTTON_HEIGHT
                );

            _quitButton = new MainMenuActionButton(MainMenuTextures.QuitButtonTexture,
                MainMenuTextures.QuitButtonTexture,
                Constants.WINDOW_WIDTH / 2 - 150 / 2,
                buttonYPos + 240,
                150,
                80
                );
            _cursor = new MainMenuCursor(MainMenuTextures.CursorTexture, MainMenuTextures.CursorAltTexture,
                new Point(widthBuffer - buffer - 12, buttonYPos - 12));
        }

        public void Update()
        {
            KeyboardState state = Keyboard.GetState();

            // Simulate button selection
            if (state.IsKeyDown(Keys.Enter) && (_userMoveTimeout is null || !_userMoveTimeout.Enabled))
            {
                // if (_blackjackButton.IsSelected)
                if (_currentCursorPos == 0)
                    _game.SetSelectedGame(SelectedGame.BLACKJACK); // Update selected game
                // else if (_fiveCardDrawButton.IsSelected)
                else if (_currentCursorPos == 1)
                    _game.SetSelectedGame(SelectedGame.FIVECARD); // Update selected game
                // else if (_texasHoldEmButton.IsSelected)
                else if (_currentCursorPos == 2)
                    _game.SetSelectedGame(SelectedGame.HOLDEM); // Update selected game
                // else if (_quitButton.IsSelected)
                else if (_currentCursorPos == 3)
                    _game.Exit(); // Quit the game
            }

            // Add logic to navigate the menu (e.g., using arrow keys)
            if (state.IsKeyDown(Keys.Right) && (_cursorMoveTimeout is null || !_cursorMoveTimeout.Enabled) && _currentCursorPos < Constants.MAIN_MENU_BUTTON_COUNT)
            {
                _currentCursorPos++;
                _cursor.UpdateLocation(GetNewCursorPos());
                _cursorMoveTimeout = new Timer(Constants.TIMER_DURATION);
                _cursorMoveTimeout.Elapsed += OnTimeoutEvent!;
                _cursorMoveTimeout.Start();
                Console.WriteLine("Selected Game: " + _currentCursorPos);
            }
            else if (state.IsKeyDown(Keys.Left) && (_cursorMoveTimeout is null || !_cursorMoveTimeout.Enabled) && _currentCursorPos > 0)
            {
                _currentCursorPos--;
                _cursor.UpdateLocation(GetNewCursorPos());
                _cursorMoveTimeout = new Timer(Constants.TIMER_DURATION);
                _cursorMoveTimeout.Elapsed += OnTimeoutEvent!;
                _cursorMoveTimeout.Start();
                Console.WriteLine("Selected Game: " + _currentCursorPos);
            }

        }
        public void OnTimeoutEvent(object source, ElapsedEventArgs e)
        {
            // Stop and dispose of the timer
            Timer timer = (Timer)source;
            timer.Stop();
            timer.Dispose();
        }

        public void HandleButtonClick(MainMenuActionButton button)
        {
            if (button == _blackjackButton)
                _game.SetSelectedGame(SelectedGame.BLACKJACK);
            else if (button == _fiveCardDrawButton)
                _game.SetSelectedGame(SelectedGame.FIVECARD);
            else if (button == _texasHoldEmButton)
                _game.SetSelectedGame(SelectedGame.HOLDEM);
            else if (button == _quitButton)
                _game.Exit();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_backgroundTexture != null)
            {
                spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, Constants.WINDOW_WIDTH, Constants.WINDOW_HEIGHT), Color.White);
            }
            _blackjackButton.Draw(spriteBatch);
            _fiveCardDrawButton.Draw(spriteBatch);
            _texasHoldEmButton.Draw(spriteBatch);
            _quitButton.Draw(spriteBatch);
            _cursor.Draw(spriteBatch, _currentCursorPos);

        }


        private Point GetNewCursorPos()
        {
            return _currentCursorPos switch
            {
                Constants.BLACKJACK_BUTTON_POS => _blackjackButton.GetAdjustedPos(),
                Constants.FIVE_CARD_DRAW_BUTTON_POS => _fiveCardDrawButton.GetAdjustedPos(),
                Constants.TEXAS_HOLD_EM_BUTTON_POS => _texasHoldEmButton.GetAdjustedPos(),
                Constants.QUIT_BUTTON_POS => _quitButton.GetAdjustedPos(),
                _ => _blackjackButton.GetAdjustedPos()


            };
        }

    }

    public static class MainMenuTextures
    {
        /// <summary>
        /// The button texture for the Blackjack button.
        /// </summary>
        public static Texture2D? BlackjackButtonTexture { get; private set; }

        /// <summary>
        /// The button texture for the Five Card Draw button.
        /// </summary>
        public static Texture2D? FiveCardDrawButtonTexture { get; private set; }

        /// <summary>
        /// The button texture for the Texas Hold 'Em button.
        /// </summary>
        public static Texture2D? TexasHoldEmButtonTexture { get; private set; }

        /// <summary>
        /// The button texture for the Quit button.
        /// </summary>
        public static Texture2D? QuitButtonTexture { get; private set; }

        // <summary>
        /// The texture for the cursor.
        /// </summary>
        public static Texture2D? CursorTexture { get; private set; }

        // <summary>
        /// The alt texture for the cursor.
        /// </summary>
        public static Texture2D? CursorAltTexture { get; private set; }


        /// <summary>
        /// Loads the assets for MainMenu.
        /// </summary>
        public static void LoadContent(ContentManager content)
        {
            BlackjackButtonTexture = content.Load<Texture2D>("BlackjackButton");
            FiveCardDrawButtonTexture = content.Load<Texture2D>("FiveCardDrawButton");
            TexasHoldEmButtonTexture = content.Load<Texture2D>("TexasHoldEmButton");
            QuitButtonTexture = content.Load<Texture2D>("QuitButton");
            CursorTexture = content.Load<Texture2D>("MenuCursor");
            CursorAltTexture = content.Load<Texture2D>("MenuAltCursor");
        }


    }

    public class MainMenuCursor
    {
        /// <summary>
        /// The texture for the cursor.
        /// </summary>
        private Texture2D _cursorTexture;

        /// <summary>
        /// The alternate texture for the cursor.
        /// </summary>
        private Texture2D _cursorAltTexture;

        /// <summary>
        /// The rectangle object for the cursor.
        /// </summary>
        private Rectangle _cursorRectangle;

        /// <summary>
        /// The size of the cursor.
        /// </summary>
        private Point _size = new(174, 246);

        public MainMenuCursor(Texture2D cursorTexture, Texture2D cursorAltTexture, Point location)
        {
            _cursorTexture = cursorTexture;
            _cursorAltTexture = cursorAltTexture;
            _cursorRectangle = new Rectangle(location, _size);
        }

        /// <summary>
        /// Updates the location of the cursor.
        /// </summary>
        public void UpdateLocation(Point Location)
        {
            _cursorRectangle.X = Location.X;
            _cursorRectangle.Y = Location.Y;
        }

        /// <summary>
        /// The draw method for the cursor.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, int currentCursorPos)
        {
            if (currentCursorPos == 3) // Use alt texture for the quit button
            {
                spriteBatch.Draw(_cursorAltTexture, _cursorRectangle, Color.White);
            }
            else
            {
                spriteBatch.Draw(_cursorTexture, _cursorRectangle, Color.White);
            }
        }

    }

    public class MainMenuActionButton
    {
        /// <summary>
        /// The disabled button texture.
        /// </summary>
        private Texture2D _disabledTexture;

        /// <summary>
        /// The unselected button texture.
        /// </summary>
        private Texture2D _enabledTexture;

        /// <summary>
        /// The rectangle for the button.
        /// </summary>
        private Rectangle _buttonRectangle;

        /// <summary>
        /// Whether or not the button is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = false;

        /// <summary>
        /// Whether or not the button is selected.
        /// </summary>
        public bool IsSelected { get; set; } = false;

        /// <summary>
        /// The constructor for the MainMenuActionButton.
        /// </summary>
        public MainMenuActionButton(Texture2D enabledTexture, Texture2D disabledTexture, int xPos, int yPos, int width, int height)
        {
            _enabledTexture = enabledTexture;
            _disabledTexture = disabledTexture;
            _buttonRectangle = new Rectangle(xPos, yPos, width, height);
        }

        /// <summary>
        /// The draw method for the button.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsEnabled)
                spriteBatch.Draw(_disabledTexture, _buttonRectangle, Color.White);
            else
                spriteBatch.Draw(_enabledTexture, _buttonRectangle, Color.White);
        }

        /// <summary>
        /// Gets the location where the cursor will be.
        /// </summary>
        public Point GetAdjustedPos()
        {
            return new Point(_buttonRectangle.X - 12, _buttonRectangle.Y - 12);
        }


    }


}