/*
 *  Module Name: MainMenu.cs
 *  Purpose: provide a main menu for the user to select a game to play
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Richard Moser
 *  Date: 11/20/2024
 *  Last Modified: 12/8/2024
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
        /// The menu animation sprite sheet.
        /// </summary>
        private Texture2D _menuSplashSheet;

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

        // A timer that stores milliseconds.
        float timer;

    // An int that is the threshold for the timer.
        int threshold;

    // A Rectangle array that stores sourceRectangles for animations.
        Rectangle [] sourceRectangles;

    // These bytes tell the spriteBatch.Draw() what sourceRectangle to display.
        byte previousAnimationIndex;
        byte currentAnimationIndex;


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

            // Set a default timer value.
            timer = 0;

        // Set an initial threshold of 250ms, you can change this to alter the speed of the animation (lower number = faster animation).
            threshold = 10;

        // Three sourceRectangles contain the coordinates of Alex's three down-facing sprites on the charaset.
            sourceRectangles = new Rectangle[128];
            // sourceRectangles[0] = new Rectangle(0, 128, 48, 64);
            // sourceRectangles[1] = new Rectangle(48, 128, 48, 64);
            // sourceRectangles[2] = new Rectangle(96, 128, 48, 64);

            // 128 rectangles 300 wide and 200 tall from a spritesheet 3300 x 2200
            for (int i = 0; i < 112; i++)
            {
                sourceRectangles[i] = new Rectangle(300 * (i % 10), 200 * (i / 10), 300, 200);
            }


        // This tells the animation to start on the left-side sprite.
            // previousAnimationIndex = 2;
            // currentAnimationIndex = 1;

            previousAnimationIndex = 0;
            currentAnimationIndex = 1;

            int widthBuffer = (Constants.WINDOW_WIDTH - Constants.MAIN_MENU_BUTTON_WIDTH * Constants.MAIN_MENU_BUTTON_COUNT) / 2;
            int buttonYPos = Constants.WINDOW_HEIGHT - 350;
            int buffer = 30;

            _backgroundTexture = content.Load<Texture2D>("MenuBackground");

            _blackjackButton = new MainMenuActionButton(
                MainMenuTextures.BlackjackButtonTexture!,
                MainMenuTextures.BlackjackButtonTexture!,
                widthBuffer - buffer,
                buttonYPos,
                Constants.MAIN_MENU_BUTTON_WIDTH,
                Constants.MAIN_MENU_BUTTON_HEIGHT
                );
            _fiveCardDrawButton = new MainMenuActionButton(MainMenuTextures.FiveCardDrawButtonTexture!,
                MainMenuTextures.FiveCardDrawButtonTexture!,
                widthBuffer + Constants.MAIN_MENU_BUTTON_WIDTH,
                buttonYPos,
                Constants.MAIN_MENU_BUTTON_WIDTH,
                Constants.MAIN_MENU_BUTTON_HEIGHT
                );
            _texasHoldEmButton = new MainMenuActionButton(MainMenuTextures.TexasHoldEmButtonTexture!,
                MainMenuTextures.TexasHoldEmButtonTexture!,
                widthBuffer + Constants.MAIN_MENU_BUTTON_WIDTH * 2  + buffer,
                buttonYPos,
                Constants.MAIN_MENU_BUTTON_WIDTH,
                Constants.MAIN_MENU_BUTTON_HEIGHT
                );

            _quitButton = new MainMenuActionButton(MainMenuTextures.QuitButtonTexture!,
                MainMenuTextures.QuitButtonTexture!,
                Constants.WINDOW_WIDTH / 2 - 150 / 2,
                buttonYPos + 240,
                150,
                80
                );
            _cursor = new MainMenuCursor(MainMenuTextures.CursorTexture!, MainMenuTextures.CursorAltTexture!,
                new Point(widthBuffer - buffer - 12, buttonYPos - 12));

            _menuSplashSheet = content.Load<Texture2D>("MenuSplashSheet");
        }

        // public void Update(gameTime gameTime)
        public void Update(GameTime gameTime)
        {
            // Check if the timer has exceeded the threshold.
            // if (timer > threshold)
            if (timer > threshold && currentAnimationIndex < 112)
            {
                // If the current animation index is the last index in the array, then set the current animation index to 0.
                if (currentAnimationIndex == sourceRectangles.Length - 1)
                {
                    currentAnimationIndex = 0;
                    // Reset the timer.
                    timer = 0;
                }
                else if (currentAnimationIndex == 58)
                {
                    // continue the timer until threshold * 10 and do not advance the animation index
                    if (timer < threshold * 50)
                    {
                        timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                        threshold = 50;
                    }
                    else
                    {
                        currentAnimationIndex++;
                        // Reset the timer.
                        timer = 0;
                    }
                }
                // If the current animation index is not the last index in the array, then increment the current animation index by 1.
                else
                {
                    currentAnimationIndex++;
                    // Reset the timer.
                    timer = 0;
                }

            }
            // If the timer has not reached the threshold, then add the milliseconds that have past since the last Update() to the timer.
            else if (currentAnimationIndex < 112)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (_menuSplashSheet != null && currentAnimationIndex > 112)
            {
                // nullify the menuSplashSheet
                _menuSplashSheet = null;
            }

            KeyboardState state = Keyboard.GetState();

            // Simulate button selection
            if (state.IsKeyDown(Keys.Enter) && (_userMoveTimeout is null || !_userMoveTimeout.Enabled))
            {
                if (_currentCursorPos == 0)
                    _game.SetSelectedGame(SelectedGame.BLACKJACK); // Update selected game
                else if (_currentCursorPos == 1)
                    _game.SetSelectedGame(SelectedGame.FIVECARD); // Update selected game
                else if (_currentCursorPos == 2)
                    _game.SetSelectedGame(SelectedGame.HOLDEM); // Update selected game
                else if (_currentCursorPos == 3)
                    _game.QuitGame(); // Quit the game
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
                _game.QuitGame(); // Quit the game       
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

            // if (_menuSplashSheet != null)
            if (currentAnimationIndex < 112)
            {
                spriteBatch.Draw(
                    _menuSplashSheet,
                    // new Vector2(0, 0),
                    new Rectangle(0, 0, Constants.WINDOW_WIDTH, Constants.WINDOW_HEIGHT),
                    sourceRectangles[currentAnimationIndex],
                    Color.White
                );
            }
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
