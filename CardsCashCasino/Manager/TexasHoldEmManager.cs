﻿/*
 *  Module Name: TexasHoldEmManager.cs
 *  Purpose: Manages the game of Texas Hold Em.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Mo Morgan
 *  Date: 11/3/2024
 *  Last Modified: 11/7/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using CardsCashCasino.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CardsCashCasino.Manager
{
    /// <summary>
    /// The possible actions that a player can take in Texas Hold Em.
    /// </summary>
    public enum PokerAction
    {
        FOLD,
        CHECK,
        CALL,
        RAISE,
        ALL_IN
    }

    public class TexasHoldEmManager
    {
        #region Properties

        /// <summary>
        /// The user's hand of cards.
        /// </summary>
        private UserHand _userHand = new();

        /// <summary>
        /// Flag that indicates if the user is still active in the game.
        /// </summary>
        private bool _gameOver;

        /// <summary>
        /// The cursor's current position.
        /// </summary>
        private int _currentCursorPos = 0;

        /// <summary>
        /// Checks if the game is actively running.
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Checks if the the user is still playing
        /// </summary>
        private bool _userPlaying;

        /// <summary>
        /// Set when the round has been completed, and the game is ready to move to the next round.
        /// </summary>
        private bool _roundFinished = false;

        /// <summary>
        /// Set when the user has folded. The game will continue without the user.
        /// </summary>
        private bool _userFolded = false;

        /// <summary>
        /// The main pot for the game.
        /// </summary>
        private int _mainPot;

        /// <summary>
        /// The side pot for the game.
        /// </summary>
        private int _sidePot;

        /// <summary>
        /// The current bet for the game.
        /// This is the amount that all players must match to stay in the game. It's initially set to the Big Blind.
        /// This amount is updated as players raise the bet.
        /// It's used to caluclate the minimum amount that a player must bet to stay in the game.
        /// </summary>
        private int _currentBet;

        /// <summary>
        /// The Small Blind for the game.
        /// </summary>
        private int _smallBlindBet;

        /// <summary>
        /// The Big Blind for the round.
        /// </summary>
        private int _bigBlindBet;

        /// <summary>
        /// The Ante for the game.
        /// This is the amount that all players must pay to play the game.
        /// </summary>
        private int _ante;

        /// <summary>
        /// The index of the player that is the dealer for the current round.
        /// Increments by one each round.
        /// </summary>
        private int _currentDealer;

        /// <summary>
        /// The index of the player that is the small blind for the current round.
        /// Increments by one each round.
        /// </summary>
        private int _currentSmallBlind;

        /// <summary>
        /// The index of the player that is the big blind for the current round.
        /// </summary>
        private int _currentBigBlind;

        /// <summary>
        /// The number of rounds left until the blinds increase.
        /// If this value is zero at the top of the round, the blinds increase.
        /// </summary>
        private int _blindIncreaseCountdown;

        /// <summary>
        /// A list of each player's hand.
        /// </summary>
        private List<CardHand> _playerHands { get; set; }
        
        /// <summary>
        /// The cursor.
        /// </summary>
        private HoldEmCursor _cursor;
        
        /// <summary>
        /// The check button.
        /// </summary>
        private PokerActionButton? _checkButton;
        
        /// <summary>
        /// The call button.
        /// </summary>
        private PokerActionButton? _callButton;
        
        /// <summary>
        /// The raise button.
        /// </summary>
        private PokerActionButton? _raiseButton;
        
        /// <summary>
        /// The fold button.
        /// </summary>
        private PokerActionButton? _foldButton;
        
        /// <summary>
        /// The all in button.
        /// </summary>
        private PokerActionButton? _allInButton;

        /// <summary>
        /// The timeout for the cursor to move.
        /// </summary>
        private Timer? _cursorMoveTimer;
        
        /// <summary>
        /// The timeout for the AI to take an action.
        /// </summary>
        private Timer? _AIActionTimer;
        
        /// <summary>
        /// The timeout for the user to take an action.
        /// </summary>
        private Timer? _userActionTimer;
        
        /// <summary>
        /// Call to request the card manager to clear the deck.
        /// </summary>
        public Action? RequestCardManagerClear { get; set; }
        
        /// <summary>
        /// Call to request the card manager to generate a deck of cards.
        /// </summary>
        public Action<int>? RequestDecksOfCards { get; set; }
        
        /// <summary>
        /// Call to request the card manager to shuffle the deck
        /// </summary>
        public Action? RequestShuffle { get; set; }
        
        /// <summary>
        /// Call to request the card manager to draw a card.
        /// </summary>
        public Func<Card>? RequestCard { get; set; }
        
        /// <summary>
        /// Call to request the card manager to recycle the discard into the deck.
        /// </summary>
        public Action? RequestRecycle { get; set; }
        #endregion Properties
        
        #region Methods
        public void LoadContent(ContentManager content)
        {
            // Load the textures for the game.
            TexasHoldEmTextures.LoadContent(content);
            
            int widthBuffer = (Constants.WINDOW_WIDTH - Constants.BUTTON_WIDTH * Constants.POKER_BUTTON_COUNT) / 2;
            int buttonYPos = Constants.WINDOW_HEIGHT - 100;

            _checkButton = new (TexasHoldEmTextures.CheckButtonTexture!, disabledTexture=null,widthBuffer, buttonYPos);
            _callButton = new (TexasHoldEmTextures.CallButtonEnabledTexture!, TexasHoldEmTextures.CallButtonDisabledTexture!, widthBuffer + Constants.BUTTON_WIDTH, buttonYPos);
            _raiseButton = new (TexasHoldEmTextures.RaiseButtonTexture!, disabledTexture=null, widthBuffer + Constants.BUTTON_WIDTH * 2, buttonYPos);
            _allInButton = new (TexasHoldEmTextures.AllInButtonTexture!, disabledTexture=null, widthBuffer + Constants.BUTTON_WIDTH * 3, buttonYPos);
            _foldButton = new (TexasHoldEmTextures.FoldButtonTexture!, disabledTexture=null , widthBuffer + Constants.BUTTON_WIDTH * 4, buttonYPos);

            _cursor = new(TexasHoldEmTextures.CursorTexture!, _checkButton.GetAdjustedPos());

            StartGame(); // TODO: Remove when main menu is implemented or comment out to test Blackjack.
        }

        public void Update()
        {
            if (_userPlaying)
                UpdateWhileUserPlaying();
            else
                UpdateWhileAIPlaying();
        }

        private void UpdateWhileUserPlaying()
        {
            // Return if the AI is still taking an action.
            if (_AIActionTimer is not null && _AIActionTimer.Enabled)
                return;
        }

        private void UpdateWhileAIPlaying()
        {
            return;
        }

        private StartGame()
        {
            // Reset the cards.
            RequestCardManagerClear!.Invoke();
            RequestDecksOfCards!.Invoke(Constants.POKER_DECK_COUNT);
        }
        /// <summary>
        /// Creates the list of player hands. The first player is the user.
        /// </summary>
        private void GeneratePlayerHands()
        {
            _playerHands[0] = _userHand;
            for (int i = 0; i < Constants.AI_PLAYER_COUNT; i++)
            {
                _playerHands.Add(new PokerAIHand());
            }
        }

        #endregion Methods
    }

    public static class TexasHoldEmTextures
    {
        public static Texture2D? CallButtonEnabledTexture { get; private set; }
        public static Texture2D? CallButtonDisabledTexture { get; private set; }
        public static Texture2D? CheckButtonTexture { get; private set; }
        public static Texture2D? RaiseButtonTexture { get; private set; }
        public static Texture2D? FoldButtonTexture { get; private set; }
        public static Texture2D? AllInButtonTexture { get; private set; }
        public static Texture2D? CursorTexture { get; private set; }

        public static void LoadContent(ContentManager content)
        {
            // TODO: Create textures for the buttons.
            // CallButtonEnabledTexture = content.Load<Texture2D>("CallButtonEnabled");
            // CallButtonDisabledTexture = content.Load<Texture2D>("CallButtonDisabled");
            // CheckButtonTexture = content.Load<Texture2D>("CheckButton");
            // RaiseButtonTexture = content.Load<Texture2D>("RaiseButton");
            // FoldButtonTexture = content.Load<Texture2D>("FoldButton");
            // AllInButtonTexture = content.Load<Texture2D>("AllInButton");
            CursorTexture = content.Load<Texture2D>("Cursor");
        }
    }

    public class HoldEmCursor
    {
        #region Properties
        /// <summary>
        /// Texture for the cursor.
        /// </summary>
        private Texture2D _cursorTexture;
        
        /// <summary>
        /// Current position of the cursor.
        /// </summary>
        private Rectangle _cursorRectangle;

        /// <summary>
        /// The size of the cursor.
        /// </summary>
        private Point _size = new(144, 80);
        #endregion Properties

        #region  Methods
        public HoldEmCursor(Texture2D cursorTexture, Point location)
        {
            _cursorTexture = cursorTexture;
            _cursorRectangle = new Rectangle(location, _size);
        }

        /// <summary>
        /// Updates the location of the cursor.
        /// </summary>
        /// <param name="location"></param>
        public void UpdateLocation(Point location)
        {
            _cursorRectangle.X = location.X;
            _cursorRectangle.Y = location.Y;
        }

        /// <summary>
        /// Draw method for the cursor.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_cursorTexture, _cursorRectangle, Color.White);
        }
        #endregion Methods
    }

    public class PokerActionButton
    {
        #region Properties
        private Texture2D _disabledTexture;
        private Texture2D _enabledTexture;
        private Rectangle _buttonRectangle;
        public bool IsEnabled { get; private set; } = false;
        public bool IsSelected { get; private set; } = false;
        #endregion Properties
        
        #region Methods
        public PokerActionButton(Texture2D enabledTexture, Texture2D disabledTexture, int x, int y)
        {
            _enabledTexture = enabledTexture;
            _disabledTexture = disabledTexture;
            _buttonRectangle = new Rectangle(x, y, Constants.BUTTON_WIDTH, Constants.BUTTON_HEIGHT);
        }

        /// <summary>
        /// The Draw method for PokerActionButton.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(IsEnabled ? _enabledTexture : _disabledTexture, _buttonRectangle, Color.White);
        }

        public Point GetAdjustedPos()
        {
            return new Point(_buttonRectangle.X - 8, _buttonRectangle.Y - 8);
        }
        #endregion Methods
    }
}