﻿/*
 *  Module Name: BettingManager.cs
 *  Purpose: Manages the betting and money in the game.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Jacob Wilkus, Richard Moser
 *  Date: 11/8/2024
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
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CardsCashCasino.Manager
{
    /// <summary>
    /// The betting manager. Manages the user's cash value and betting.
    /// </summary>
    public class BettingManager
    {
        /// <summary>
        /// The amount of cash the user has on hand.
        /// Initializes to $500.
        /// </summary>
        public static int UserCashValue { get; private set; } = 500;

        /// <summary>
        /// How much the user bet last time.
        /// Static with internal set so it can be modified in other parts of this file.
        /// </summary>
        public static int UserBet { get; internal set; } = 0;

        /// <summary>
        /// The display for the user's cash value.
        /// </summary>
        private CashValueIndicator? _userCashValueIndicator;

        /// <summary>
        /// The betting menu.
        /// </summary>
        private BettingMenu? _bettingMenu;

        /// <summary>
        /// Whether or not betting is happening.
        /// </summary>
        public bool IsBetting
        {
            get
            {
                return _bettingMenu?.Open ?? false;
            }
        }

        /// <summary>
        /// Whether or not the user has bet.
        /// </summary>
        public bool HasBet
        {
            get
            {
                return UserBet != 0;
            }
        }

        /// <summary>
        /// Requests a return to the main menu.
        /// </summary>
        public static Action<SelectedGame>? RequestMainMenuReturn { get; set; }

        /// <summary>
        /// Loads the betting manager's content.
        /// </summary>
        public void LoadContent()
        {
            _userCashValueIndicator = new();
            _userCashValueIndicator.SetCorner(5, 5);
            _userCashValueIndicator.Update(UserCashValue);

            _bettingMenu = new();

            if (StatisticsUtil.GetPreviousCashValue() > 0)
                UserCashValue = StatisticsUtil.GetPreviousCashValue();
        }

        /// <summary>
        /// Update loop.
        /// </summary>
        public void Update()
        {
            if (_bettingMenu is not null && _bettingMenu.Open)
                _bettingMenu.Update();
        }

        /// <summary>
        /// Draw method for the ChipManager.
        /// </summary>
        /// <param name="spriteBatch">Holds the logic that draws the poker chips.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            _userCashValueIndicator!.Draw(spriteBatch);
            if (IsBetting)
                _bettingMenu!.Draw(spriteBatch);
        }

        /// <summary>
        /// Bet a certain amount.
        /// </summary>
        public void Bet(int value)
        {
            UserCashValue -= value;
            UserBet += value;
            _userCashValueIndicator!.Update(UserCashValue);
        }

        /// <summary>
        /// Payout a value won.
        /// </summary>
        public void Payout(int value)
        {
            UserCashValue += value;
            _userCashValueIndicator!.Update(UserCashValue);
        }

        /// <summary>
        /// Opens the betting menu.
        /// </summary>
        public void OpenBettingMenu()
        {
            _bettingMenu!.OpenMenu();
        }

        /// <summary>
        /// Confirms a bet without the UI.
        /// </summary>
        public void ConfirmBetWithoutUI(int bet)
        {
            UserBet = bet;
        }
    }

    /// <summary>
    /// Textures for betting. Includes the menu for betting, as well as the chip pile textures.
    /// </summary>
    public static class BettingTextures
    {
        /// <summary>
        /// Texture for the button to bet one dollar.
        /// </summary>
        public static Texture2D? BetOneDollarTexture { get; private set; }

        /// <summary>
        /// Texture for the button to bet five dollars.
        /// </summary>
        public static Texture2D? BetFiveDollarsTexture { get; private set; }

        /// <summary>
        /// Texture for the button to bet ten dollars.
        /// </summary>
        public static Texture2D? BetTenDollarsTexture { get; private set; }

        /// <summary>
        /// Texture for the button to bet twenty five dollars.
        /// </summary>
        public static Texture2D? BetTwentyFiveDollarsTexture { get; private set; }

        /// <summary>
        /// Texture for the button to bet fifty dollars.
        /// </summary>
        public static Texture2D? BetFiftyDollarsTexture { get; private set; }

        /// <summary>
        /// Texture for the button to bet one hundred dollars.
        /// </summary>
        public static Texture2D? BetOneHundredDollarsTexture { get; private set; }

        /// <summary>
        /// Texture for the button to subtract one dollar.
        /// </summary>
        public static Texture2D? SubOneDollarTexture { get; private set; }

        /// <summary>
        /// Texture for the button to subtract five dollars.
        /// </summary>
        public static Texture2D? SubFiveDollarsTexture { get; private set; }

        /// <summary>
        /// Texture for the button to subtract ten dollars.
        /// </summary>
        public static Texture2D? SubTenDollarsTexture { get; private set; }

        /// <summary>
        /// Texture for the button to subtract twenty five dollars.
        /// </summary>
        public static Texture2D? SubTwentyFiveDollarsTexture { get; private set; }

        /// <summary>
        /// Texture for the button to subtract fifty dollars.
        /// </summary>
        public static Texture2D? SubFiftyDollarsTexture { get; private set; }

        /// <summary>
        /// Texture for the button to subtract one hundred dollars.
        /// </summary>
        public static Texture2D? SubOneHundredDollarsTexture { get; private set; }

        /// <summary>
        /// Texture for the button to confirm the bet.
        /// </summary>
        public static Texture2D? ConfirmBetTexture { get; private set; }

        /// <summary>
        /// Texture for the button to cancel the bet.
        /// </summary>
        public static Texture2D? CancelBetTexture { get; private set; }

        /// <summary>
        /// Texture for the betting cursor.
        /// </summary>
        public static Texture2D? BettingCursorTexture { get; private set; }

        /// <summary>
        /// LoadContent for the chip pile textures.
        /// </summary>
        public static void LoadContent(ContentManager content)
        {
            BetOneDollarTexture = content.Load<Texture2D>("addOneDollar");
            BetFiveDollarsTexture = content.Load<Texture2D>("addFiveDollar");
            BetTenDollarsTexture = content.Load<Texture2D>("addTenDollar");
            BetTwentyFiveDollarsTexture = content.Load<Texture2D>("addTwentyFiveDollar");
            BetFiftyDollarsTexture = content.Load<Texture2D>("addFiftyDollar");
            BetOneHundredDollarsTexture = content.Load<Texture2D>("addOneHundredDollar");
            SubOneDollarTexture = content.Load<Texture2D>("subOneDollar");
            SubFiveDollarsTexture = content.Load<Texture2D>("subFiveDollar");
            SubTenDollarsTexture = content.Load<Texture2D>("subTenDollar");
            SubTwentyFiveDollarsTexture = content.Load<Texture2D>("subTwentyFiveDollar");
            SubFiftyDollarsTexture = content.Load<Texture2D>("subFiftyDollar");
            SubOneHundredDollarsTexture = content.Load<Texture2D>("subOneHundredDollar");
            ConfirmBetTexture = content.Load<Texture2D>("confirmBet");
            CancelBetTexture = content.Load<Texture2D>("cancelBet");
            BettingCursorTexture = content.Load<Texture2D>("BettingCursor");
        }
    }

    /// <summary>
    /// The betting menu. Manages the user's betting.
    /// </summary>
    public class BettingMenu
    {
        /// <summary>
        /// The cursor through the betting menu.
        /// </summary>
        internal class BettingCursor
        {
            /// <summary>
            /// The rectangle object for the cursor.
            /// </summary>
            private Rectangle _cursorRectangle;

            /// <summary>
            /// The size of the cursor.
            /// </summary>
            private Point _size = new(216, 72);

            /// <summary>
            /// The constructor
            /// </summary>
            public BettingCursor(Point location)
            {
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
                spriteBatch.Draw(BettingTextures.BettingCursorTexture, _cursorRectangle, Color.White);
            }
        }

        /// <summary>
        /// A button for betting.
        /// </summary>
        internal class BettingButton
        {
            /// <summary>
            /// The rectangle for the button.
            /// </summary>
            private Rectangle _buttonRectangle;

            /// <summary>
            /// The unselected button texture.
            /// </summary>
            private Texture2D _buttonTexture;

            /// <summary>
            /// The constructor for the button.
            /// </summary>
            public BettingButton(Texture2D texture, int xPos, int yPos)
            {
                _buttonTexture = texture;
                _buttonRectangle = new Rectangle(xPos, yPos, 197, 53);
            }

            /// <summary>
            /// The draw method for the button.
            /// </summary>
            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(_buttonTexture, _buttonRectangle, Color.White);
            }

            /// <summary>
            /// Gets the location where the cursor will be.
            /// </summary>
            public Point GetAdjustedPos()
            {
                return new Point(_buttonRectangle.X - 10, _buttonRectangle.Y - 10);
            }
        }

        /// <summary>
        /// Button to add one dollar.
        /// </summary>
        private BettingButton _addOneDollarButton;

        /// <summary>
        /// Button to add five dollar.
        /// </summary>
        private BettingButton _addFiveDollarButton;

        /// <summary>
        /// Button to add ten dollar.
        /// </summary>
        private BettingButton _addTenDollarButton;

        /// <summary>
        /// Button to add twenty five dollar.
        /// </summary>
        private BettingButton _addTwentyFiveDollarButton;

        /// <summary>
        /// Button to add fifty dollar.
        /// </summary>
        private BettingButton _addFiftyDollarButton;

        /// <summary>
        /// Button to add one hundred dollar.
        /// </summary>
        private BettingButton _addOneHundredDollarButton;

        /// <summary>
        /// Button to sub one dollar.
        /// </summary>
        private BettingButton _subOneDollarButton;

        /// <summary>
        /// Button to sub five dollar.
        /// </summary>
        private BettingButton _subFiveDollarButton;

        /// <summary>
        /// Button to sub ten dollar.
        /// </summary>
        private BettingButton _subTenDollarButton;

        /// <summary>
        /// Button to sub twenty five dollar.
        /// </summary>
        private BettingButton _subTwentyFiveDollarButton;

        /// <summary>
        /// Button to sub fifty dollar.
        /// </summary>
        private BettingButton _subFiftyDollarButton;

        /// <summary>
        /// Button to sub one hundred dollar.
        /// </summary>
        private BettingButton _subOneHundredDollarButton;

        /// <summary>
        /// Button to confirm bet.
        /// </summary>
        private BettingButton _confirmBetButton;

        /// <summary>
        /// Button to cancel bet.
        /// </summary>
        private BettingButton _cancelBetButton;

        /// <summary>
        /// Rectangle for the background of the menu.
        /// </summary>
        private Rectangle _menuRectangle = new(300, 250, 900, 400);

        /// <summary>
        /// Placeholder texture to draw the rectangle.
        /// </summary>
        private Texture2D? _blankTexture;

        /// <summary>
        /// The cursor.
        /// </summary>
        private BettingCursor _cursor;

        /// <summary>
        /// The display for the user's cash value.
        /// </summary>
        private CashValueIndicator _userBetValue;

        /// <summary>
        /// The timeout for the cursor to move.
        /// </summary>
        private Timer? _cursorMoveTimeout;

        /// <summary>
        /// The cursor's position.
        /// </summary>
        private int _cursorPos = 0;

        /// <summary>
        /// The current bet.
        /// Initializes to $5.
        /// </summary>
        private int _currentBet = 5;

        /// <summary>
        /// If the betting menu is open or not.
        /// </summary>
        public bool Open { get; private set; } = false;

        /// <summary>
        /// The constructor for the betting menu.
        /// </summary>
        public BettingMenu()
        {
            _addOneDollarButton = new(BettingTextures.BetOneDollarTexture!, 63+250, 211+150);
            _addFiveDollarButton = new(BettingTextures.BetFiveDollarsTexture!, 63 + 250, 286 + 150);
            _addTenDollarButton = new(BettingTextures.BetTenDollarsTexture!, 63 + 250, 361 + 150);
            _addTwentyFiveDollarButton = new(BettingTextures.BetTwentyFiveDollarsTexture!, 288 + 250, 211 + 150);
            _addFiftyDollarButton = new(BettingTextures.BetFiftyDollarsTexture!, 288 + 250, 286 + 150);
            _addOneHundredDollarButton = new(BettingTextures.BetOneHundredDollarsTexture!, 288 + 250, 361 + 150);
            _subOneDollarButton = new(BettingTextures.SubOneDollarTexture!, 513 + 250, 211 + 150);
            _subFiveDollarButton = new(BettingTextures.SubFiveDollarsTexture!, 513 + 250, 286 + 150);
            _subTenDollarButton = new(BettingTextures.SubTenDollarsTexture!, 513 + 250, 361 + 150);
            _subTwentyFiveDollarButton = new(BettingTextures.SubTwentyFiveDollarsTexture!, 738 + 250, 211 + 150);
            _subFiftyDollarButton = new(BettingTextures.SubFiftyDollarsTexture!, 738 + 250, 286 + 150);
            _subOneHundredDollarButton = new(BettingTextures.SubOneHundredDollarsTexture!, 738 + 250, 361 + 150);

            _confirmBetButton = new(BettingTextures.ConfirmBetTexture!, 288 + 250, 436 + 150);
            _cancelBetButton = new(BettingTextures.CancelBetTexture!, 513 + 250, 436 + 150);

            // Create the cursor
            _cursor = new(_addOneDollarButton.GetAdjustedPos());

            // Create the user bet value display
            _userBetValue = new();
            _userBetValue.Update(5);
            _userBetValue.SetCorner(100 + 250, 140 + 150);
        }

        /// <summary>
        /// Opens the menu.
        /// </summary>
        public void OpenMenu()
        {
            Open = true;
        }

        /// <summary>
        /// Update loop
        /// </summary>
        public void Update()
        {
            // Return if the cursor is moving or the game is starting
            if ((_cursorMoveTimeout is not null && _cursorMoveTimeout.Enabled) || (CardCashCasinoGame.GameStartTimeout is not null && CardCashCasinoGame.GameStartTimeout.Enabled))
                return;

            // Handle the user input
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) // if the up arrow key is pressed
                HandleUpArrowKey();
            else if (Keyboard.GetState().IsKeyDown(Keys.Down)) // if the down arrow key is pressed
                HandleDownArrowKey();
            else if (Keyboard.GetState().IsKeyDown(Keys.Left)) // if the left arrow key is pressed
                HandleLeftArrowKey();
            else if (Keyboard.GetState().IsKeyDown(Keys.Right)) // if the right arrow key is pressed
                HandleRightArrowKey();
            else if (Keyboard.GetState().IsKeyDown(Keys.Enter)) // if the enter key is pressed
            {
                switch (_cursorPos)
                {
                    case 0: // +$1
                        AddMoney(1);
                        break;
                    case 1: // +$25
                        AddMoney(25);
                        break;
                    case 2: // -$1
                        SubMoney(1);
                        break;
                    case 3: // -$25
                        SubMoney(25);
                        break;
                    case 4: // +$5
                        AddMoney(5);
                        break;
                    case 5: // +$50
                        AddMoney(50);
                        break;
                    case 6: // -$5
                        SubMoney(5);
                        break;
                    case 7: // -$50
                        SubMoney(50);
                        break;
                    case 8: // +$10
                        AddMoney(10);
                        break;
                    case 9: // +$100
                        AddMoney(100);
                        break;
                    case 10: // -$10
                        SubMoney(10);
                        break;
                    case 11: // -$100
                        SubMoney(100);
                        break;
                    case 13: // confirm
                        ConfirmBet();
                        break;
                    case 14: // cancel
                        CancelBet();
                        break;
                    default:
                        break;
                }
            }
            else // if no keys are pressed
            {
                return;
            }

            // Start the cursor move timeout
            _cursorMoveTimeout = new(200);
            _cursorMoveTimeout.Elapsed += Constants.OnTimeoutEvent!;
            _cursorMoveTimeout.Start();
        }

        /// <summary>
        /// Handles pressing up.
        /// </summary>
        private void HandleUpArrowKey()
        {
            if (_cursorPos - 4 < 0) // if the cursor is at the top
                return;

            // move the cursor up
            _cursorPos -= 4;
            _cursor.UpdateLocation(GetNewCursorPosition());
        }

        /// <summary>
        /// Handles pressing down
        /// </summary>
        private void HandleDownArrowKey()
        {
            int newPos = _cursorPos + 4; // get the new position

            if (newPos > 14 || newPos == 12) // if the cursor is at the bottom
                return;

            // move the cursor down
            _cursorPos += 4;
            _cursor.UpdateLocation(GetNewCursorPosition());
        }

        /// <summary>
        /// Handles pressing left
        /// </summary>
        private void HandleLeftArrowKey()
        {
            int newPos = _cursorPos - 1; // get the new position
            bool canGoLeft = (_cursorPos / 4) == (newPos / 4); // check if the cursor can go left

            if (_cursorPos - 1 < 0 || newPos == 12 || !canGoLeft) // if the cursor is at the left
                return;

            // move the cursor left
            _cursorPos--;
            _cursor.UpdateLocation(GetNewCursorPosition());
        }

        /// <summary>
        /// Handles pressing right
        /// </summary>
        private void HandleRightArrowKey()
        {
            int newPos = _cursorPos + 1; // get the new position
            bool canGoRight = (_cursorPos / 4) == (newPos / 4); // check if the cursor can go right

            if (newPos > 14 || newPos == 12 || !canGoRight) // if the cursor is at the right
                return;

            _cursorPos++; // move the cursor right
            _cursor.UpdateLocation(GetNewCursorPosition()); // update the cursor position
        }

        /// <summary>
        /// Gets the updated cursor position
        /// </summary>
        private Point GetNewCursorPosition()
        {
            // return the position of the cursor
            return _cursorPos switch
            {
                0 => _addOneDollarButton!.GetAdjustedPos(),
                1 => _addTwentyFiveDollarButton!.GetAdjustedPos(),
                2 => _subOneDollarButton!.GetAdjustedPos(),
                3 => _subTwentyFiveDollarButton!.GetAdjustedPos(),
                4 => _addFiveDollarButton!.GetAdjustedPos(),
                5 => _addFiftyDollarButton!.GetAdjustedPos(),
                6 => _subFiveDollarButton!.GetAdjustedPos(),
                7 => _subFiftyDollarButton!.GetAdjustedPos(),
                8 => _addTenDollarButton!.GetAdjustedPos(),
                9 => _addOneHundredDollarButton!.GetAdjustedPos(),
                10 => _subTenDollarButton!.GetAdjustedPos(),
                11 => _subOneHundredDollarButton!.GetAdjustedPos(),
                13 => _confirmBetButton!.GetAdjustedPos(),
                14 => _cancelBetButton!.GetAdjustedPos(),
                _ => throw new InvalidOperationException("invalid position")
            };
        }

        /// <summary>
        /// Adds a value to the current bet.
        /// </summary>
        private void AddMoney(int value)
        {
            if (_currentBet + value > 1000000) // if the bet is too high
                return;

            _userBetValue.Add(value); // add the value to the user bet value
            _currentBet += value; // add the value to the current bet
        }

        /// <summary>
        /// Subtracts a value from the current bet.
        /// </summary>
        private void SubMoney(int value)
        {
            if (_currentBet - value < 5) // if the bet is too low
                return;

            _userBetValue.Sub(value); // subtract the value from the user bet value
            _currentBet -= value; // subtract the value from the current bet
        }

        /// <summary>
        /// Confirms the current bet.
        /// </summary>
        private void ConfirmBet()
        {
            BettingManager.UserBet = _currentBet; // set the user bet to the current bet
            Open = false; // close the betting menu
        }

        /// <summary>
        /// Cancels the current bet.
        /// </summary>
        private void CancelBet()
        {
            _currentBet = 0; // reset the current bet
            Open = false; // close the betting menu
            BettingManager.RequestMainMenuReturn!.Invoke(SelectedGame.NONE); // return to the main menu
        }

        /// <summary>
        /// Draw method
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Generate the blank texture if null
            if (_blankTexture is null)
            {
                _blankTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _blankTexture.SetData(new[] { Color.Gray });
            }

            // Draw the background
            spriteBatch.Draw(_blankTexture, _menuRectangle, Color.White);

            // Draw the add buttons
            _addOneDollarButton.Draw(spriteBatch);
            _addFiveDollarButton.Draw(spriteBatch);
            _addTenDollarButton.Draw(spriteBatch);
            _addTwentyFiveDollarButton.Draw(spriteBatch);
            _addFiftyDollarButton.Draw(spriteBatch);
            _addOneHundredDollarButton.Draw(spriteBatch);

            // Draw the subtract buttons
            _subOneDollarButton.Draw(spriteBatch);
            _subFiveDollarButton.Draw(spriteBatch);
            _subTenDollarButton.Draw(spriteBatch);
            _subTwentyFiveDollarButton.Draw(spriteBatch);
            _subFiftyDollarButton.Draw(spriteBatch);
            _subOneHundredDollarButton.Draw(spriteBatch);

            // Draw confirm and cancel
            _confirmBetButton.Draw(spriteBatch);
            _cancelBetButton.Draw(spriteBatch);

            // Draw the cursor
            _cursor!.Draw(spriteBatch);

            // Draw the user bet
            _userBetValue.Draw(spriteBatch);
        }
    }

    /// <summary>
    /// The cash value indicator. Displays the user's cash value.
    /// </summary>
    public class CashValueIndicator
    {
        /// <summary>
        /// The digits representing the number.
        /// </summary>
        private List<IndicatorDigit> _digits = new();

        /// <summary>
        /// The dollar sign.
        /// </summary>
        private IndicatorCharacter _dollarSign;

        /// <summary>
        /// The current value.
        /// </summary>
        private int _currentValue = -1;

        /// <summary>
        /// Width of a character.
        /// </summary>
        private int _characterWidth = 21;

        /// <summary>
        /// The top left corner of the display.
        /// </summary>
        private Point? _topLeftCorner;

        /// <summary>
        /// The constructor for the cash value indicator.
        /// </summary>
        public CashValueIndicator()
        {
            _dollarSign = new(DisplayIndicatorTextures.DollarSignTexture!);
        }

        /// <summary>
        /// Updates the display with a new numerical value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Update(int value)
        {
            if (value == _currentValue) // if the value is the same
                return;

            _currentValue = value; // set the current value

            if (value == 0) // if the value is zero
            {
                _digits.Clear();
                IndicatorDigit zeroDigit = new IndicatorDigit();
                zeroDigit.Update(0);
                _digits.Add(zeroDigit);

                if (_topLeftCorner is not null)
                    CalculateDigitPositions(((Point)_topLeftCorner).X + _characterWidth, ((Point)_topLeftCorner).Y);

                return;
            }

            _digits.Clear(); // remove the old digits

            // get each digit
            while (value > 0)
            {
                int digit = value % 10;
                value /= 10;

                IndicatorDigit indicatorDigit = new IndicatorDigit();
                indicatorDigit.Update(digit);
                _digits.Add(indicatorDigit);
            }

            _digits.Reverse(); // put them in the correct order

            if (_topLeftCorner is not null)
                CalculateDigitPositions(((Point)_topLeftCorner).X + _characterWidth, ((Point)_topLeftCorner).Y);
        }

        /// <summary>
        /// The draw method for the display.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_topLeftCorner is null || _digits.Count == 0) // if the corner is null or there are no digits
                return;

            _dollarSign.Draw(spriteBatch); // draw the dollar sign

            foreach (IndicatorDigit indicatorDigit in _digits) // draw each digit
                indicatorDigit.Draw(spriteBatch);
        }

        /// <summary>
        /// Sets the corner of the display.
        /// </summary>
        /// <param name="xPos">The x coordinate</param>
        /// <param name="yPos">The y coordinate</param>
        public void SetCorner(int xPos, int yPos)
        {
            // set the corner and calculate the digit positions
            _topLeftCorner = new Point(xPos, yPos);
            _dollarSign.SetPosition(xPos, yPos);

            // set the x position
            xPos += _characterWidth;
            CalculateDigitPositions(xPos, yPos);
        }

        /// <summary>
        /// Calculates the positions of the digits on the screen.
        /// </summary>
        /// <param name="xPos">The x coordinate</param>
        /// <param name="yPos">The y coordinate</param>
        private void CalculateDigitPositions(int xPos, int yPos)
        {
            foreach (IndicatorDigit indicatorDigit in _digits) // calculate the position of each digit
            {
                indicatorDigit.SetPosition(xPos, yPos);
                xPos += _characterWidth;
            }
        }

        /// <summary>
        /// Adds a value to the current total.
        /// </summary>
        public void Add(int value)
        {
            Update(_currentValue + value); // update the value
        }

        /// <summary>
        /// Subtracts a value from the current total.
        /// </summary>
        public void Sub(int value)
        {
            Update(_currentValue - value); // update the value
        }
    }
}
