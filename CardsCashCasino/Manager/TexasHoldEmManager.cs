/*
 *  Module Name: TexasHoldEmManager.cs
 *  Purpose: Manages the game of Texas Hold Em.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Mo Morgan, Ethan Berkley, Derek Norton
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
using System.Linq;
using System.Numerics;
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

    /// <summary>
    /// Identifies what stage of betting the game is currently in
    /// </summary>
    public enum BettingPhase
    {
        PREFLOP,
        FLOP,
        TURN,
        RIVER
    }

    /// <summary>
    /// Identifies the pot type for the TexasHoldEmPotManagerClass
    /// </summary>
    public enum PotType
    {
        MAIN,
        SIDE
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
        /// Variable to hold the Pots Manager class
        /// </summary>
        private TexasHoldEmPotManager _potManager;

        /// <summary>
        /// The cursor.
        /// </summary>
        private HoldEmCursor _cursor;
        #region buttons
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
        #endregion

        #region timers
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
        /// The timeout for a card to be dealt.
        /// </summary>
        private Timer? _cardDealtTimer;
        #endregion

        #region delegates
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
        #endregion delegates

        /// <summary>
        /// Identifies what phase of the gameflow is active regarding dealing cards
        /// </summary>
        enum Phase
        {
            INIT,
            FLOP,
            TURN,
            RIVER,
            CONCLUSION
        }
        /// <summary>
        /// What phase are we currently in?
        /// Set by update.
        /// </summary>
        private Phase _currentPhase;
        /// <summary>
        /// What user is currently active?
        /// Set by update.
        /// </summary>
        private int _currentPlayer;

        #endregion Properties

        #region Methods

        /// <summary>
        /// The LoadContent method for Texas HoldEm.
        /// <param name="location"></param>
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            // Load the textures for the game.
            TexasHoldEmTextures.LoadContent(content);

            int widthBuffer = (Constants.WINDOW_WIDTH - Constants.BUTTON_WIDTH * Constants.POKER_BUTTON_COUNT) / 2;
            int buttonYPos = Constants.WINDOW_HEIGHT - 100;

            _checkButton = new(TexasHoldEmTextures.CheckButtonEnabledTexture!, widthBuffer, buttonYPos, TexasHoldEmTextures.CheckButtonDisabledTexture!);
            _callButton = new(TexasHoldEmTextures.CallButtonEnabledTexture!, widthBuffer + Constants.BUTTON_WIDTH, buttonYPos, TexasHoldEmTextures.CallButtonDisabledTexture!);
            _raiseButton = new(TexasHoldEmTextures.RaiseButtonEnabledTexture!, widthBuffer + Constants.BUTTON_WIDTH * 2, buttonYPos, TexasHoldEmTextures.RaiseButtonDisabledTexture!);
            _allInButton = new(TexasHoldEmTextures.AllInButtonTexture!, widthBuffer + Constants.BUTTON_WIDTH * 3, buttonYPos);
            _foldButton = new(TexasHoldEmTextures.FoldButtonTexture!, widthBuffer + Constants.BUTTON_WIDTH * 4, buttonYPos);

            _cursor = new(TexasHoldEmTextures.CursorTexture!, _checkButton.GetAdjustedPos());
        }

        /// <summary>
        /// The main update loop for blackjack.
        /// </summary>
        public void Update()
        {
            // Does a new round start with the dealer making a bet? 
            if (_currentPlayer == _currentDealer)
            {
                switch (_currentPhase)
                {
                    case Phase.INIT:
                        StartGame();
                        _currentPhase = Phase.FLOP;
                        break;
                    case Phase.FLOP:
                        DealFlop();
                        _currentPhase = Phase.TURN;
                        break;
                    case Phase.TURN:
                        DealTurn();
                        _currentPhase = Phase.RIVER;
                        break;
                    case Phase.RIVER:
                        DealRiver();
                        _currentPhase = Phase.CONCLUSION;
                        break;
                    case Phase.CONCLUSION:
                        RoundConclusion();
                        EndGame();
                        _currentPhase = Phase.INIT; // ?
                        break;
                }

            }

            // If it's currently player's turn
            if (_currentPlayer == 0)
                UpdateWhileUserPlaying();
            else
                UpdateWhileAIPlaying();
        }

        /// <summary>
        /// Update loop while the user is playing.
        /// </summary>
        private void UpdateWhileUserPlaying()
        {
            // Return if the AI is still taking an action.
            if (_AIActionTimeout is not null && _AIActionTimeout.Enabled)
                return;

            // Throw user's options or whatever here
            // This should only get called if this function results in player's turn ending
            _currentPlayer = (_currentPlayer + 1) % (Constants.AI_PLAYER_COUNT + 1);


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

        /// <summary>
        /// Updates the AI player index
        /// </summary>
        private void UpdateWhileAIPlaying()
        {
            // Should have some AI related nonsense here.

            _currentPlayer = (_currentPlayer + 1) % (Constants.AI_PLAYER_COUNT + 1);
            return;
        }

        /// <summary>
        /// The main draw loop for Texas HoldEm.
        /// <param name="spriteBatch"></param>
        /// </summary>
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
        /// <summary>
        /// Gets the new Cursor position.
        /// </summary>
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

        /// <summary>
        /// Enter the TexasHoldEm gameflow
        /// </summary>
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

        /// <summary>
        /// Enter the TexasHoldEm gameflow
        /// </summary>
        public void StartGame()
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
            _playerHands![0].SetCenter(userHandXPos, Constants.WINDOW_HEIGHT - 200);

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

        /// <summary>
        /// Game has been ended by the user
        /// </summary>
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

        /// <summary>
        /// Deals the flop. 1 card is discarded from the deck, and 3 cards are added to the community cards.
        /// </summary>
        public void DealFlop()
        {
            //Discard the first card in the deck.
            RequestCardDiscard!.Invoke(RequestCard!.Invoke());

            // Deal the flop.
            for (int i = 0; i < 3; i++)
            {
                _communityCards.Add(RequestCard!.Invoke());

                // Add a timeout for the card to be drawn to the screen.
                // This will allow the user to see the cards being drawn.
                _cardDealtTimer = new Timer(500);
            }
        }

        /// <summary>
        /// Deals the turn. 1 card is discarded from the deck, and 1 card is added to the community cards.
        /// </summary>
        public void DealTurn()
        {
            // Discard the first card in the deck.
            RequestCardDiscard!.Invoke(RequestCard!.Invoke());

            // Deal the turn.
            _communityCards.Add(RequestCard!.Invoke());

            // Add a timeout for the card to be drawn to the screen.
            // This will allow the user to see the card being drawn.
            _cardDealtTimer = new Timer(500);
        }

        /// <summary>
        /// The top card of the deck is added to the discard pile
        /// The next card of the deck is dealt to the board face up
        /// A round of betting begins with the first player to the left of the dealer who has not folded
        /// </summary>
        public void DealRiver()
        {
            // Discard first card in the deck
            RequestCardDiscard!.Invoke(RequestCard!.Invoke());

            // Deal the river.
            _communityCards.Add(RequestCard!.Invoke());

            // Add a timeout for the card to be drawn to the screen.
            // This will allow the user to see the card being drawn.
            _cardDealtTimer = new Timer(500);
        }

        /// <summary>
        /// The cards of each player are revealed at the same time
        /// Winner is declared for the round
        /// </summary>
        public void RoundConclusion()
        {
            PokerUtil.Ranking bestRanking = PokerUtil.Ranking.HIGH_CARD;
            // List of hands that (so far) are tied for the best rank.
            // Pair of player idx and their optimal 5-card hand. 
            List<Tuple<int, List<Card>>> bestHands = new();
            // For each player
            for (int i = 0; i < _playerHands.Count; i++)
            {
                // Reveal the player's cards
                _playerHands[i].Cards[0].GetTexture();
                _playerHands[i].Cards[1].GetTexture();

                // Get ranking and optimal hand 
                Tuple<List<Card>, PokerUtil.Ranking> pair = PokerUtil.GetScore(_communityCards, _playerHands[i].Cards.ToList());
                // If this ties the best ranking
                if (pair.Item2 == bestRanking)
                {
                    // Mark this hand as needing to be tie broken
                    bestHands.Add(new Tuple<int, List<Card>>(i, pair.Item1));
                }
                // If this hand beats the old best ranking
                else if (pair.Item2 < bestRanking)
                {
                    // Update the value
                    bestRanking = pair.Item2;
                    // Don't worry about losing hands
                    bestHands.Clear();
                    // Keep track of this hand
                    bestHands.Add(new Tuple<int, List<Card>>(i, pair.Item1));
                }
            }
            // Players that will receive a payout.
            List<int> winners = new();

            // If we don't have a tie to break
            if (bestHands.Count == 1)
            {
                // Just add the player to the list and move on
                winners.Add(bestHands[0].Item1);
            }
            // If there is a tie to break
            else
            {
                // What is the best value we have seen out of KickerValue?
                long bestKicker = 0;
                // For each (player,hand) in the tiebreaker
                foreach (Tuple<int, List<Card>> tuple in bestHands)
                {
                    // Get number corresponding to how large the values in their hand are
                    long kickerVal = PokerUtil.KickerValue(tuple.Item2);
                    // If there's another tie
                    if (kickerVal == bestKicker)
                    {
                        // Add the player to the list of winners
                        winners.Add(tuple.Item1);
                    }
                    // If the current player had a better value than the old "winner"s
                    else if (kickerVal > bestKicker)
                    {
                        // Update the best value
                        bestKicker = kickerVal;
                        // Forget about those losers
                        winners.Clear();
                        // Add the player to the list of winners
                        winners.Add(tuple.Item1);
                    }
                }
            }

            // winners is populated correctly at this point.
            // TODO: Pay out bets?

        }

        /// <summary>
        /// handles the current phase the game is in and calls the appropriate method for the phase
        /// </summary>
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

        /// <summary>
        /// handles the initial stage of betting before any community cards appear
        /// </summary>
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

        /// <summary>
        /// handles the poker action enacted by the user or by the AI opponent
        /// /// <param name="playerIndex">The index of the player's hand in _playerHands</param>
        /// </summary>
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

        /// <summary>
        /// Retrieves user selected action
        /// <returns>USER selected PokerAction</returns>
        /// </summary>
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

        /// <summary>
        /// Houses the logic used by AI to decide poker action based on the AI's poker hand
        /// Get the list of cards in the player's hand.
        /// If the hand is a pair or worse. There's a 50% chance the AI will either fold or call/check.
        /// If the hand is between two pairs and a straight, there's a 50% chance the AI will either call/check or raise.
        /// If the hand is a straight or better, there's a 35% chance the AI will call/check and 65% chance it raises.
        /// Currently returns call, AI currently defaults to mimicing user actions to advance game for demo purposes
        /// <param name="playerIndex">The index of the player's hand in _playerHands</param>
        /// <returns>AI selected PokerAction</returns>
        /// </summary>
        private PokerAction GetAIAction(int playerIndex)
        {
            return PokerAction.CALL; //Matches user bet 
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

        /// <summary>
        /// The raise action. Players will increase the current bet to a specified size, all other players must match this bet
        /// </summary>
        /// <param name="playerIndex">The index of the player's hand in _playerHands</param>
        /// <param name="raiseAmount">The index of the player's hand in _playerHands</param>
        private void Raise(int playerIndex, int raiseAmount)
        {

        }

        /// <summary>
        /// The All in action. Player will wager all of their remaining funds, all other players must match this bet
        /// </summary>
        /// <param name="playerIndex">The index of the player's hand in _playerHands</param>
        private void AllIn(int playerIndex)
        {
            // Add logic to add the entire of the player's cash to the pot. This needs Bett

            // Add logic to handle side pot if necessary.
        }

        /// <summary>
        /// The Fold action. The player is no longer participating in this round and is ineligible to recieve any split of the pot
        /// </summary>
        /// <param name="playerIndex">The index of the player's hand in _playerHands</param>
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
    }
    /// <summary>
    /// Manages the pot in a Texas Hold'em game, including adding and distributing chips.
    /// </summary>
    public class TexasHoldEmPot
    {
        /// <summary>
        /// Contains the total amount of money contained in a pot, initiated as empty (0)
        /// </summary>
        public int Total { get; set; } = 0;

        /// <summary>
        /// Whether the pot is the MAIN pot (the active pot that bets are allocated to) or a SIDE pot (the pot that an all-in player is eligible to win)
        /// </summary>
        public PotType PotType { get; set; }

        /// <summary>
        /// The list of players eligible to win a given pot
        /// </summary>
        public List<int> EligiblePlayers { get; set; }

        /// <summary>
        /// Initiating the Pot and with the type of pot as its characteristic
        /// </summary>
        public TexasHoldEmPot(PotType type)
        {
            PotType = type; //importing the pot type to identify a pot as MAIN or SIDE
        }

        /// <summary>
        /// Increment the total of the pot by a specified value.
        /// </summary>
        /// <param name="value">The amount to increment the pot's total by.</param>
        public void IncrementPot(int value)
        {
            Total += value;
        }

        /// <summary>
        /// Decrement the total of the pot by a specified value.
        /// </summary>
        /// <param name="value">The amount to decrement the pot's total by.</param>
        public void DecrementPot(int value)
        {
            //prevents inadvertent Incrementation or the pot total from becoming a negative value
            if (value <= 0 || Total - value < 0) 
            {
                return;
            }

            Total -= value;
        }

        /// <summary>
        /// Decrement the total of the pot by a specified value.
        /// </summary>
        /// <param name="playerIndex">The position of the player being removed from pot eligiblity .</param>
        public void RemoveEligiblePlayer(int playerIndex)
        {
            if (EligiblePlayers.Contains(playerIndex))
            {
                EligiblePlayers.Remove(playerIndex);
            }
        }

    }

    public class TexasHoldEmPotManager
    {
        /// <summary>
        /// Importing list that will house each pot and its attributes
        /// </summary>
        public List<TexasHoldEmPot> Pots { get; set; }

        /// <summary>
        /// creating variable name that will house the list containing the TexasHoldEmPots
        /// </summary>
        public TexasHoldEmPotManager()
        {
            Pots = new List<TexasHoldEmPot>(); // Initialize the Pots list
        }

        /// <summary>
        /// Initialize the main pot, if any players are short side pots will be created.
        /// </summary>
        /// <param name="_ante">The ante needed to be added by each player to the pot.</param>
        /// <param name="playerBets">List of antes values to be added to the pot.</param>
        public void InitializePot(int ante, List<int> playerBets)
        {
            Pots.Add(new TexasHoldEmPot(PotType.MAIN)); //create a new pot

            Pots[0].EligiblePlayers = Enumerable.Range(0, playerBets.Count).ToList(); //add all players as eligible to win the pot

            if (playerBets.All(ante => ante == playerBets[0])) //if all bets match the ante add to main pot
            {
                AddToPot(ante, playerBets);
            }
            else //create side pots if any bets do not match the ante
            {
                CreateSidePots(ante, playerBets);
            }
        }

        /// <summary>
        /// Adds a bets to the pot.
        /// </summary>
        /// <param name="_currentBet">The amount each player is contributing to the pot.</param>
        /// /// <param name="playerBets">List of bet values to be added to the pot.</param>
        public void AddToPot(int currentBet, List<int> playerBets)
        {
            if (playerBets.All(bet => bet == playerBets[0]))
            {
                Pots.First(pot => pot.PotType == PotType.MAIN).IncrementPot(currentBet * playerBets.Count);
            }
            else
            {
                CreateSidePots(currentBet, playerBets);
            }
        }

        /// <summary>
        //Adds bets of players that were folded to the pot
        /// </summary>
        /// <param name="playersBets"> list of bets that were placed but folded due to a raise in that round</param>
        public void AddFoldedBetsToPot(List<int> playerBets)
        {
            if (playerBets.Any())
            {
                for (int bet = 0; bet < playerBets.Count(); bet++)
                {
                    Pots[0].IncrementPot(playerBets[bet]);
                }
            }
        }

        /// <summary>
        ///Removes a player that has folded from eligiblility for all pots
        /// </summary>
        /// <param name="playersIndex"> position of the player that as folded</param>
        public void RemoveFoldedPlayers(int playerIndex)
        {
            for (int pot = 0; pot < Pots.Count; pot++)
            {
                Pots[pot].RemoveEligiblePlayer(playerIndex);
            }
        }

        /// <summary>
        /// Creates side pots as needed at the end of a round of betting.
        /// </summary>
        /// <param name="currentBet">The amount to add to the pot.</param>
        /// <param name="playerBets">The list of bets from players still active after the round of betting.</param>
        public void CreateSidePots(int currentBet, List<int> playerBets)
        {
            int allInBet = 0; //initiating variable that will hold all in wager
            int numBets = playerBets.Count;

            //all-in bet added to the pot, other players matched bets added to pot as well
            allInBet = playerBets.Min();
            AddToPot(allInBet, Enumerable.Repeat(allInBet, numBets).ToList());

            //subtracting all-in bet value from all other bets, the all-in bet value has already been added to a pot
            for (int player = 0; player < numBets; player++)
            {
                playerBets[player] -= allInBet; // Subtract the amount put into pot from each bet
            }
            currentBet -= allInBet;

            //modify list of players that are eligible for the new main pot by removing player(s) that are all-in
            List<int> allInPlayers = playerBets
                .Select((value, index) => new { value, index })  // Keep both the value and the index
                .Where(x => x.value == 0)  // Filter where the bet is 0 (all-in)
                .Select(x => Pots[0].EligiblePlayers[x.index])  // Select the corresponding player from EligiblePlayers at the same index
                .ToList();

            //remove players that are all in from future side pot calculations that are necessary
            numBets -= allInPlayers.Count;
            playerBets.RemoveAll(value => value == 0);

            if (numBets > 1) //if 1 player or less remains, no more side pot manipulation is needed
            {
                //New side pot created. Pot that all-in player can win is shifted to inactive position, no more bets can be added to this pot
                Pots.Add(new TexasHoldEmPot(PotType.SIDE));
                Pots[Pots.Count - 1].IncrementPot(Pots[0].Total); //shifting the pot that the all in player can win to the side, this pot will no longer be added to
                Pots[0].DecrementPot(Pots[Pots.Count - 1].Total); //reseting active pot value to empty
                Pots[Pots.Count - 1].EligiblePlayers = new List<int>(Pots[0].EligiblePlayers); //copys the list of players eligible to win the now side pot

                //removing all players that are all in from eligibility from main pot and any pots created in the future
                for (int player = 0; player < allInPlayers.Count; player++)
                {
                    Pots[0].RemoveEligiblePlayer(allInPlayers[player]);
                }
            }
            //if there is a second player that has gone all in this round, recursively add pots until player that has called max bet has been reached
            if (currentBet != playerBets.Min())
            {
                CreateSidePots(currentBet, playerBets);
                return;
            }
            //protects from scenerio where last remaining player from adding excessive amount to the pot
            if (numBets > 1) 
            {
                //add remainder of bets from players not all-in into new main pot
                AddToPot(currentBet, playerBets);
            }
        }

        /// <summary>
        /// Pays out the winnings to the player for the individual pot. 
        /// </summary>
        /// <param name="winners">Number of players that have won the pot.</param>
        /// /// <param name="potNum">Number of players that have won the pot.</param>
        public int DistributePot(int winners, int potNum)
        {
            if (winners != 0) //prevent division by 0
            {
                int _payout = Pots[potNum].Total / winners; //splits payout if more than 1 winner is present
                return _payout;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns the amount contained in each pot 
        /// </summary>
        public List<int> GetPotAmounts()
        {
            return Pots.Select(pot => pot.Total).ToList();
        }

        /// <summary>
        /// Resets the pots list to an empty state.
        /// </summary>
        public void ResetPots()
        {
            Pots.Clear();
        }
    }
    #endregion Methods
}

    public static class TexasHoldEmTextures
    {
        /// <summary>
        /// The enabled texture for the Call button.
        /// </summary>
        public static Texture2D? CallButtonEnabledTexture { get; private set; }
        
        /// <summary>
        /// The disabled texture for the Call button. For scenerio when no bet has been placed yet.
        /// </summary>
        public static Texture2D? CallButtonDisabledTexture { get; private set; }

        /// <summary>
        /// The enabled texture for the Check button.
        /// </summary>
        public static Texture2D? CheckButtonEnabledTexture { get; private set; }

        /// <summary>
        /// The disabled texture for the Check button. For the occasion that a bet has been placed in the round.
        /// </summary>
        public static Texture2D? CheckButtonDisabledTexture { get; private set; }

        /// <summary>
        /// The enabled texture for the Raise button.
        /// </summary>    
        public static Texture2D? RaiseButtonEnabledTexture { get; private set; }

        /// <summary>
        /// The disabled texture for the Raise button. For the occasion that opponent places a bet that would put the user all in.
        /// </summary> 
        public static Texture2D? RaiseButtonDisabledTexture { get; private set; }

        /// <summary>
        /// The texture for the Fold button.
        /// </summary> 
        public static Texture2D? FoldButtonTexture { get; private set; }

        /// <summary>
        /// The enabled texture for the AllIn button.
        /// </summary> 
        public static Texture2D? AllInButtonTexture { get; private set; }

        /// <summary>
        /// The cursor's texture.
        /// </summary>
        public static Texture2D? CursorTexture { get; private set; }

        /// <summary>
        /// Loads the assets for Texas HoldEm.
        /// </summary>
        public static void LoadContent(ContentManager content)
        {
        // TODO: Create textures for the buttons.
        // CallButtonEnabledTexture = content.Load<Texture2D>("CallButtonEnabled");
        // CallButtonDisabledTexture = content.Load<Texture2D>("CallButtonDisabled");
        // CheckButtonEnablesTexture = content.Load<Texture2D>("CheckButtonEnabled");
        // CheckButtonEnablesTexture = content.Load<Texture2D>("CheckButtonDisabled");
        // RaiseButtonEnabledTexture = content.Load<Texture2D>("RaiseButtonEnabled");
        // RaiseButtonEnabledTexture = content.Load<Texture2D>("RaiseButtonDisabled");
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
        public bool IsEnabled { get; private set; } = false;

        /// <summary>
        /// Whether or not the button is selected.
        /// </summary>
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

        /// <summary>
        /// Gets the location where the cursor will be.
        /// </summary>
        public Point GetAdjustedPos()
        {
            return new Point(_buttonRectangle.X - 8, _buttonRectangle.Y - 8);
        }
        #endregion Methods
    }