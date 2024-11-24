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

        /// <summary>
        /// The LoadContent method for the main menu.
        ///</summary>
        public void LoadContent(ContentManager content)
        {
            int widthBuffer = (Constants.WINDOW_WIDTH - Constants.BUTTON_WIDTH * Constants.BLACKJACK_BUTTON_COUNT) / 2;
            int buttonYPos = Constants.WINDOW_HEIGHT - 100;

            _blackjackButton = new MainMenuActionButton(MainMenuTextures.BlackjackButtonTexture, MainMenuTextures.BlackjackButtonTexture, widthBuffer, buttonYPos);
            _fiveCardDrawButton = new MainMenuActionButton(MainMenuTextures.FiveCardDrawButtonTexture, MainMenuTextures.FiveCardDrawButtonTexture, widthBuffer + Constants.BUTTON_WIDTH, buttonYPos);
            _texasHoldEmButton = new MainMenuActionButton(MainMenuTextures.TexasHoldEmButtonTexture, MainMenuTextures.TexasHoldEmButtonTexture, widthBuffer + Constants.BUTTON_WIDTH * 2, buttonYPos);
            _quitButton = new MainMenuActionButton(MainMenuTextures.QuitButtonTexture, MainMenuTextures.QuitButtonTexture, widthBuffer + Constants.BUTTON_WIDTH * 3, buttonYPos);

            // _cursor = new MainMenuCursor(MainMenuTextures.CursorTexture, new Point(widthBuffer, buttonYPos));


        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Environment.Exit(0);
            }
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

        /// <summary>
        /// Loads the assets for MainMenu.
        /// </summary>
        public static void LoadContent(ContentManager content)
        {
            BlackjackButtonTexture = content.Load<Texture2D>("BlackjackButton");
            FiveCardDrawButtonTexture = content.Load<Texture2D>("FiveCardDrawButton");
            TexasHoldEmButtonTexture = content.Load<Texture2D>("TexasHoldEmButton");
            QuitButtonTexture = content.Load<Texture2D>("QuitButton");
        }


    }

    public class MainMenuCursor
    {
        /// <summary>
        /// The texture for the cursor.
        /// </summary>
        private Texture2D _cursorTexture;

        /// <summary>
        /// The rectangle object for the cursor.
        /// </summary>
        private Rectangle _cursorRectangle;

        /// <summary>
        /// The size of the cursor.
        /// </summary>
        private Point _size = new(144, 80);

        public MainMenuCursor(Texture2D cursorTexture, Point location)
        {
            _cursorTexture = cursorTexture;
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
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_cursorTexture, _cursorRectangle, Color.White);
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

        public MainMenuActionButton(Texture2D enabledTexture, Texture2D disabledTexture, int xPos, int yPos)
        {
            _enabledTexture = enabledTexture;
            _disabledTexture = disabledTexture;
            _buttonRectangle = new Rectangle(xPos, yPos, 128, 64);
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
            return new Point(_buttonRectangle.X - 8, _buttonRectangle.Y - 8);
        }


    }
}