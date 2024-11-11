/*
 *  Module Name: TexasHoldEmManager.cs
 *  Purpose: Manages the game of Texas Hold Em.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Mo Morgan
 *  Date: 11/3/2024
 *  Last Modified: 11/10/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */

using System;
using System.Collections.Generic;
using System.Timers;
using CardsCashCasino.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
    
    public enum BettingPhase
    {
        PREFLOP,
        FLOP,
        TURN,
        RIVER
    }

    public class TexasHoldEmManager
    {
        #region Properties

        /// <summary>
        /// The user's hand of cards.
        /// </summary>
        private UserHand _userHand = new();
        
        /// <summary>
        /// The internal poker utility object. Used to determine the winner of the game and for AI decision-making.
        /// </summary>
        private PokerUtil _pokerUtil = new();

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
        /// Checks if the user is still playing.
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
        /// The current betting phase of the game.
        /// </summary>
        private BettingPhase _bettingPhase = BettingPhase.PREFLOP;

        /// <summary>
        /// The main pot for the game.
        /// </summary>
        private int _mainPot;

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
        private static int _currentDealer = new Random().Next(0, Constants.AI_PLAYER_COUNT + 1);

        /// <summary>
        /// The index of the player that is the small blind for the current round.
        /// Increments by one each round.
        /// </summary>
        private static int _currentSmallBlind = (_currentDealer + 1) % (Constants.AI_PLAYER_COUNT + 1);

        /// <summary>
        /// The index of the player that is the big blind for the current round.
        /// </summary>
        private static int _currentBigBlind = (_currentSmallBlind + 1) % (Constants.AI_PLAYER_COUNT + 1);

        /// <summary>
        /// The number of rounds left until the blinds increase.
        /// If this value is zero at the top of the round, the blinds increase.
        /// </summary>
        private int _blindIncreaseCountdown;
        
        /// <summary>
        /// The capacity of the deck.
        /// </summary>
        private int _capacity;

        /// <summary>
        /// A list of each player's hand.
        /// </summary>
        private List<CardHand> _playerHands { get; set; }
        
        /// <summary>
        /// The list of community cards. This is the cards that are shared by all players.
        /// </summary>
        private List<Card> _communityCards = new();
        
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
        private Timer? _cursorMoveTimeout;
        
        /// <summary>
        /// The timeout for the AI to take an action.
        /// </summary>
        private Timer? _AIActionTimeout;
        
        /// <summary>
        /// The timeout for the user to take an action.
        /// </summary>
        private Timer? _userActionTimeout;
        
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
        /// Call to request the deck's size from card manager.
        /// </summary>
        public Func<int>? RequestDeckSize { get; set; }
        
        /// <summary>
        /// Call to request the card manager to draw a card.
        /// </summary>
        public Func<Card>? RequestCard { get; set; }
        
        /// <summary>
        /// Call to request the card manager to recycle the discard into the deck.
        /// </summary>
        public Action? RequestRecycle { get; set; }
        
        /// <summary>
        /// Call to request the card manager to put a card in the discard pile.
        /// </summary>
        public Action<Card>? RequestCardDiscard { get; set; }
        #endregion Properties
        
        #region Methods
        public void LoadContent(ContentManager content)
        {
            // Load the textures for the game.
            TexasHoldEmTextures.LoadContent(content);
            
            int widthBuffer = (Constants.WINDOW_WIDTH - Constants.BUTTON_WIDTH * Constants.POKER_BUTTON_COUNT) / 2;
            int buttonYPos = Constants.WINDOW_HEIGHT - 100;

            _checkButton = new (TexasHoldEmTextures.CheckButtonTexture!, widthBuffer, buttonYPos);
            _callButton = new (TexasHoldEmTextures.CallButtonEnabledTexture!, widthBuffer + Constants.BUTTON_WIDTH, buttonYPos,TexasHoldEmTextures.CallButtonDisabledTexture!);
            _raiseButton = new (TexasHoldEmTextures.RaiseButtonTexture!,widthBuffer + Constants.BUTTON_WIDTH * 2, buttonYPos);
            _allInButton = new (TexasHoldEmTextures.AllInButtonTexture!,widthBuffer + Constants.BUTTON_WIDTH * 3, buttonYPos);
            _foldButton = new (TexasHoldEmTextures.FoldButtonTexture!, widthBuffer + Constants.BUTTON_WIDTH * 4, buttonYPos);

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
            if (_AIActionTimeout is not null && _AIActionTimeout.Enabled)
                return;
            
            // Handle right key press to move the cursor.
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && (_cursorMoveTimeout is null || _cursorMoveTimeout.Enabled))
            {
                _currentCursorPos++;
                
                // Wrap the cursor around if it goes past the last button.
                if (_currentCursorPos >= Constants.POKER_BUTTON_COUNT)
                    _currentCursorPos = 0;
                
                _cursor!.UpdateLocation(GetNewCursorPos());
                
                // Reset the cursor move timer.
                _cursorMoveTimeout = new Timer(100);
                _cursorMoveTimeout.Elapsed += OnTimeoutEvent!;
                _cursorMoveTimeout.Start();
            }
            // Handle left key press to move the cursor.
            else if (Keyboard.GetState().IsKeyDown(Keys.Left) && (_cursorMoveTimeout is null || !_cursorMoveTimeout.Enabled))
            {
                _currentCursorPos--;

                // Wrap the cursor around if it goes past the first button.
                if (_currentCursorPos < 0)
                    _currentCursorPos = Constants.POKER_BUTTON_COUNT - 1;

                _cursor!.UpdateLocation(GetNewCursorPos());

                _cursorMoveTimeout = new Timer(100);
                _cursorMoveTimeout.Elapsed += OnTimeoutEvent!;
                _cursorMoveTimeout.Start();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (_userActionTimeout is not null && _userActionTimeout.Enabled)
                    return;

                switch (_currentCursorPos)
                {
                    case Constants.CHECK_BUTTON_POS:
                        Check();
                        break;
                    case Constants.CALL_BUTTON_POS:
                        // Call();
                        break;
                    case Constants.RAISE_BUTTON_POS:
                        // Raise();
                        break;
                    case Constants.ALL_IN_BUTTON_POS:
                        // AllIn();
                        break;
                    case Constants.FOLD_BUTTON_POS:
                        // Fold();
                        break;
                }

                _userActionTimeout = new(200);
                _userActionTimeout.Elapsed += OnTimeoutEvent!;
                _userActionTimeout.Start();
            }
        }

        private void UpdateWhileAIPlaying()
        {
            return;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the buttons
            _callButton!.Draw(spriteBatch);
            _checkButton!.Draw(spriteBatch);
            _raiseButton!.Draw(spriteBatch);
            _allInButton!.Draw(spriteBatch);
            _foldButton!.Draw(spriteBatch);
            
            // Draw the cursor
            _cursor.Draw(spriteBatch);
            
            // Draw the player hands
            foreach (CardHand hand in _playerHands)
            {
                hand.Draw(spriteBatch);
            }
        }
        
        private Point GetNewCursorPos()
        {
            return _currentCursorPos switch
            {
                Constants.CHECK_BUTTON_POS => _checkButton!.GetAdjustedPos(),
                Constants.CALL_BUTTON_POS => _callButton!.GetAdjustedPos(),
                Constants.RAISE_BUTTON_POS => _raiseButton!.GetAdjustedPos(),
                Constants.FOLD_BUTTON_POS => _foldButton!.GetAdjustedPos(),
                _ => _callButton!.GetAdjustedPos()
            };
        }

        private void Initialize()
        {
            _gameOver = false;
            _userPlaying = false;
            _roundFinished = false;
            _userFolded = false;
            _mainPot = 0;
            // _blindIncreaseCountdown = 5;
            _playerHands = new List<CardHand>(); // Initialize the list of player hands.
            RequestDecksOfCards!(Constants.POKER_DECK_COUNT); // Generate the deck of cards.
            _capacity = Constants.POKER_DECK_COUNT * 52; // Set the capacity of the deck.
        }
        private void StartGame()
        {
            // If the size of the card deck is less than 50% of its capacity, recycle the discard pile.
            if (RequestDeckSize!.Invoke() < (_capacity / 2))
                RequestRecycle!();
            
            // If there are no hands, generate the player hands
            if (_playerHands.Count == 0)
                GeneratePlayerHands();
            
            // Calculate the position of the user hand.
            int userHandXPos = Constants.WINDOW_WIDTH / 2;
            
            // Calculate the horizontal position of the intital AI hand. It is positioned at 100 pixels from the left of the screen.
            int aiHandXPos = 100;
            
            
            // Set the position of the card hands. The user hand is centered at the bottom of the screen.
            // The AI hands are positioned along the top of the screen with a buffer of 100 pixels.
            _playerHands![0].SetCenter( userHandXPos, Constants.WINDOW_HEIGHT - 200);
            
            for (int i = 1; i < Constants.AI_PLAYER_COUNT; i++)
            {
                _playerHands[i].SetCenter(aiHandXPos, 100);
                aiHandXPos += 200;
            }
            
            // Deal 2 cards to each player one at a time, starting with the small blind.
            int dealStartIndex = _currentSmallBlind;
            
            for (int i = 0; i < _playerHands.Count * 2; i++)
            {
                _playerHands[dealStartIndex].AddCard(RequestCard!());
                dealStartIndex = (dealStartIndex + 1) % _playerHands.Count;
            }
        }

        private void EndGame()
        {
            // Discard cards from, and clear, each hand.
            foreach (CardHand hand in _playerHands)
            {
                foreach (Card card in hand.Cards)
                {
                    RequestCardDiscard!(card);
                }
                hand.Clear();
            }   
            
            // Increment the player roles.
            _currentDealer = _currentSmallBlind;
            _currentSmallBlind = _currentBigBlind;
            _currentBigBlind = (_currentBigBlind + 1) % _playerHands.Count;
        }
        /// <summary>
        /// Creates the list of player hands. The first player is the user.
        /// </summary>
        private void GeneratePlayerHands()
        {
            _playerHands.Add(_userHand);
            for (int i = 0; i < Constants.AI_PLAYER_COUNT; i++)
            {
                _playerHands.Add(new PokerAIHand());
            }
        }

        private void HandleBettingPhase()
        {
            switch (_bettingPhase)
            {
                case BettingPhase.PREFLOP:
                    HandlePreflop();
                    break;
                case BettingPhase.FLOP:
                    // HandleFlop();
                    break;
                case BettingPhase.TURN:
                    // HandleTurn();
                    break;
                case BettingPhase.RIVER:
                    // HandleRiver();
                    break;
            }
        }

        private void HandlePreflop()
        {
            // Set the current bet to the big blind.
            _currentBet = _bigBlindBet;
            
            // Set the player index to the player with the small blind
            int playerIndex = _currentSmallBlind;
            
            // iterate through the players starting with the small blind. and handle their actions.
            for (int i = 0; i < _playerHands.Count; i++)
            {
                // If the player is the user, set the user playing flag to true.
                if (playerIndex == 0)
                    _userPlaying = true;
                
                // If the player is an AI player, set the AI playing flag to true.
                else
                    _userPlaying = false;
                
                // Handle the player's action.
                HandlePlayerAction(playerIndex);
                
                // Increment the player index.
                playerIndex = (playerIndex + 1) % _playerHands.Count;
            }
        }

        private void HandlePlayerAction(int playerIndex)
        {
            // Check if the player is the user or AI.
            bool isUser = playerIndex == 0;
            
            // Get the player's hand.
            CardHand playerHand = _playerHands[playerIndex];
            
            PokerAction action = isUser ? GetUserAction() : GetAIAction(playerIndex);
            
            switch (action)
            {
                case PokerAction.FOLD:
                    // HandleFold(playerIndex);
                    break;
                case PokerAction.CHECK:
                    // HandleCheck(playerIndex);
                    break;
                case PokerAction.CALL:
                    // HandleCall(playerIndex);
                    break;
                case PokerAction.RAISE:
                    // HandleRaise(playerIndex);
                    break;
                case PokerAction.ALL_IN:
                    // HandleAllIn(playerIndex);
                    break;
            }
        }
        
        private PokerAction GetUserAction()
        {
            return _currentCursorPos switch
            {
                Constants.CHECK_BUTTON_POS => PokerAction.CHECK,
                Constants.CALL_BUTTON_POS => PokerAction.CALL,
                Constants.RAISE_BUTTON_POS => PokerAction.RAISE,
                Constants.FOLD_BUTTON_POS => PokerAction.FOLD,
                Constants.ALL_IN_BUTTON_POS => PokerAction.ALL_IN,
                _ => PokerAction.CHECK
            };
        }
        
        private PokerAction GetAIAction(int playerIndex)
        {
            // Get the list of cards in the player's hand.
            
            //If the hand is a pair or worse. There's a 50% chance the AI will either fold or call/check.
            
            //If the hand is between two pairs and a straight, there's a 50% chance the AI will either call/check or raise.
            
            //If the hand is a straight or better, there's a 35% chance the AI will call/check and 65% chance it raises.
            return PokerAction.CHECK;
        }

        /// <summary>
        /// The check action.
        /// </summary>
        /// <returns>The false boolean</returns>
        private void Check()
        {
            
        }

        /// <summary>
        /// The call action. Players will bet the difference between the total current bet and their current bet.
        /// </summary>
        /// <param name="playerIndex">The index of the player's hand in _playerHands</param>
        private void Call(int playerIndex)
        {
                // This method's logic will need to change to consider different pots

        }

        private void Raise(int playerIndex, int raiseAmount)
        {

        }

        private void AllIn(int playerIndex)
        {
            // Add logic to add the entire of the player's cash to the pot. This needs Bett
            
            // Add logic to handle side pot if necessary.
        }
        
        private void Fold(int playerIndex)
        {
            _playerHands[playerIndex].Clear();
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
            CursorTexture = content.Load<Texture2D>("BlackjackCursor");
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
        public PokerActionButton(Texture2D enabledTexture, int x, int y, Texture2D disabledTexture=null)
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