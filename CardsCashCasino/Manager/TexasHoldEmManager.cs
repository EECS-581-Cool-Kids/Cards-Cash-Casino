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
using static System.Collections.Specialized.BitVector32;

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

    public class TexasHoldEmManager
    {
        #region Properties

        /// <summary>
        /// The pot UI for displaying the pot value.
        /// </summary>
        private PotUI _potUI;

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
        /// Checks if the active round of betting has been initiated.
        /// </summary>
        private bool _roundInit = false;

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
        /// The index of the player whose turn it is
        /// </summary>
        private int playerIndex;

        /// <summary>
        /// The list of community cards. This is the cards that are shared by all players.
        /// </summary>
        private List<Card> _communityCards = new();

        /// <summary>
        /// If the update function is dealing with the raise functionality
        /// </summary>
        private bool _userRaising = false;

        /// <summary>
        /// Call to tell betting manager to start trying to get a raise amount.
        /// </summary>
        public Action StartRaise{ get; set; }

        /// <summary>
        /// Call to ask betting manager if raise code has completed.
        /// </summary>
        //public Func<int> GetRaiseAmount { get; set; }

        /// <summary>
        /// Initializing TexasHoldEmPotManager class
        /// </summary>
        //TexasHoldEmPotManager _potManager = new TexasHoldEmPotManager();

        /// <summary>
        /// Initializing PlayerManager class
        /// </summary>
        PlayerManager _players = new PlayerManager();

        /// <summary>
        /// Variable to hold the Pots Manager class
        /// </summary>
        private PotManager _potManager = new PotManager();

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

            _potUI = new PotUI(new Microsoft.Xna.Framework.Vector2(Constants.WINDOW_WIDTH / 2 - 172, 150)); // Explicitly specify the namespace for Vector2
            _potUI.LoadContent(content); // Load pot textures
        }

        /// <summary>
        /// The main update loop for blackjack.
        /// </summary>
        public void Update()
        {
            // If it's currently player's turn
            if (playerIndex == 0)
                UpdateWhileUserPlaying();
            else
                UpdateWhileAIPlaying();

            int totalPotValue = _potManager.Pots.Sum(pot => pot.Total); // Calculate total pot value
            _potUI.UpdatePot(totalPotValue);
        }

        /// <summary>
        /// Reads the players inputs to see if they have chosen an action.
        /// Will not block the flow of execution.
        /// </summary>
        /// <returns>The action chosen, or null if no action has been chosen</returns>
        private PokerAction? GetPlayerAction()
        {
            // Handle right key press to move the cursor.
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && (_cursorMoveTimeout is null || !_cursorMoveTimeout.Enabled))
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
                    return null;

                _userActionTimeout = new(200);
                _userActionTimeout.Elapsed += OnTimeoutEvent!;
                _userActionTimeout.Start();

                switch (_currentCursorPos)
                {
                    case Constants.CHECK_BUTTON_POS:
                        return PokerAction.CHECK;

                    case Constants.CALL_BUTTON_POS:
                        return PokerAction.CALL;

                    case Constants.RAISE_BUTTON_POS:
                        return PokerAction.RAISE;

                    case Constants.ALL_IN_BUTTON_POS:
                        return PokerAction.ALL_IN;

                    case Constants.FOLD_BUTTON_POS:
                        return PokerAction.FOLD;
                }
            }

            return null;
        }

        /// <summary>
        /// See if the player made a decision on what to raise by.
        /// Should contain just a few if statements, will not block.
        /// Should contain all the code needed to change a player's selection before they make a decision.
        /// </summary>
        /// <returns>Amount the player decided to raise by, or -1 if the player did not decide yet.</returns>
        private int GetPlayerRaise()
        {
            // TODO: Read user's inputs w.r.t. the UI, and if a decision was made, return that value.
            int amount = BettingManager.UserBet;
            BettingManager.UserBet = 0;
            return amount;
        }

        /// <summary>
        /// Goes to the next phase of the game. Called when the current phase is over, which is when the betting round
        /// is over for that phase concludes.
        /// </summary>
        private void NextPhase()
        {
            switch (_currentPhase)
            {
                case Phase.INIT:
                    _currentPhase = Phase.FLOP;
                    DealFlop();
                    return;

                case Phase.FLOP:
                    _currentPhase = Phase.TURN;
                    DealTurn();
                    return;

                case Phase.TURN:
                    _currentPhase = Phase.RIVER;
                    DealRiver();
                    return;

                case Phase.RIVER:
                    _currentPhase = Phase.CONCLUSION;
                    return;
            }
        }

        /// <summary>
        /// Carrys out all the logic executed between each player's turn.
        /// Assumes that the current player has completed their turn, and the next player's turn is about to begin.
        /// Contains the logic to decide who's turn it is next.
        /// </summary>
        private void RoundLogic()
        {
            if (_players.AdvanceToRoundConclusion())
            {
                NextPhase();
                return;
            }

            if (!_roundInit)
            {
                // Set the current bet to the big blind.
                _currentBet = _bigBlindBet;

                // Set the player index to the player to the left of the big blind
                playerIndex = _players.GetStartingBettorIndex();
                _roundInit = true;
            }
            //round init not needed, advance the player index to next player
            else
            {
                playerIndex = (playerIndex + 1) % _playerHands.Count;
            }

            // iterate through the players starting with the player to the left of the big blind. and handle their actions.
            //loop will terminate and betting round will end once conditions are met
            if (!_players.AdvanceRound() && !_players.OnePlayerLeft())
            {
                //if player is either folded or all in, skip the player's turn
                if (_players.IsActivePlayer(playerIndex))
                {
                    return;
                }
                // If the player is the user, set the user playing flag to true.
                if (playerIndex == 0)
                {
                    _userPlaying = true;
                }
                // If the player is an AI player, set the AI playing flag to true.
                else
                {
                    _userPlaying = false;
                }

                //if only one player has not folded, prepare pot to award to player and advance to conclusion
                if (_players.OnePlayerLeft())
                {
                    _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
                    _potManager.AddToPot(_currentBet, _players.PackageBets());
                    _players.ResetBets();
                    _roundInit = false;
                    _currentPhase = Phase.CONCLUSION;
                    return;
                }
            }

            if (_players.AdvanceRound() && !_players.OnePlayerLeft())
            {
                //finalize bets for the round and add them to the pots
                _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
                _potManager.AddToPot(_currentBet, _players.PackageBets());

                //reset the bets for the next round to 0
                _players.ResetBets();
                _roundInit = false;
                NextPhase();
                return;
            }
        }

        /// <summary>
        /// Update loop while the user is playing.
        /// </summary>
        private void UpdateWhileUserPlaying()
        {
            // Return if the AI is still taking an action.
            if (_AIActionTimeout is not null && _AIActionTimeout.Enabled)
                return;

            if (_userRaising)
            {

                // At this point, the UI used to figure out what user wants to raise by should be rendered.
                int raiseAmount = GetPlayerRaise();

                if (raiseAmount == -1)
                {
                    return;
                }
                _userRaising = false;

                // TODO: Verify that the below code is correct.
                _currentBet += raiseAmount;
                Raise(0);

            }
            else
            {
                // Get the action the player selected, if any
                PokerAction? playerAction = GetPlayerAction();

                // if the player did not decide on an action yet
                if (playerAction == null)
                {
                    return;
                }

                switch (playerAction)
                {
                    case PokerAction.FOLD:
                        Fold(0);
                        break;
                    case PokerAction.CHECK:
                        Check(0);
                        break;
                    case PokerAction.CALL:
                        Call(0);
                        break;
                    case PokerAction.RAISE:
                        StartRaise.Invoke();
                        // We have to run the code to get the amount the player raises by.
                        // Since this requires more render calls, we set the flag to start rendering that code
                        // and then we break out of the function early.
                        _userRaising = true;
                        return;
                    case PokerAction.ALL_IN:
                        AllIn(0);
                        break;
                }
            }

            // At this point, the user has made every decision they had to for their turn.
            // We will now carry out logic needed to finish users turn. 
            // At the start of the next call to Update(), it should be the next player's turn.



            RoundLogic();

        }

        /// <summary>
        /// Updates the AI player index
        /// </summary>
        private void UpdateWhileAIPlaying()
        {
            // Should have some AI related nonsense here.
            // TODO: AI turns shoulnd't take one frame, so let's add a timer. 

            // ...

            // Assuming the timer says we are ready to go on at this point, let's finish the AI player's turn.
            Call(playerIndex);
            RoundLogic();
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

            // Draw the PotUI
            _potUI.Draw(spriteBatch);
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
                Constants.ALL_IN_BUTTON_POS => _allInButton!.GetAdjustedPos(),
                _ => _callButton!.GetAdjustedPos()
            };
        }

        /// <summary>
        /// Enter the TexasHoldEm gameflow
        /// </summary>
        private void Initialize()
        {

            //creates user and number of ai opponents
            _players.InitiatePlayers(5);

            _gameOver = false;
            _userPlaying = false;
            _roundFinished = false;
            _userFolded = false;

            //set initial blinds and ante, set countdown to increase blinds after 4 rounds of play
            //blinds start as 1 and 2, will increase at a rate of 50% of the big blind bet each round
            _ante = 2;
            _blindIncreaseCountdown = 4;
            _smallBlindBet = 1;
            _bigBlindBet = 2;

            _playerHands = new List<CardHand>(); // Initialize the list of player hands.
            RequestDecksOfCards!(Constants.POKER_DECK_COUNT); // Generate the deck of cards.
            _capacity = Constants.POKER_DECK_COUNT * 52; // Set the capacity of the deck.
        }

        /// <summary>
        /// Enter the TexasHoldEm gameflow
        /// </summary>
        public void StartGame()
        {
            Initialize();
            //collect antes and create pot
            _players.GenerateAntes(_ante);
            _potManager.InitializePot(_ante, _players.PackageBets());
            IsPlaying = true;
            _roundInit = false;

            //collects and places blind bets from small and big blind players
            _players.CollectBlinds(_smallBlindBet, _bigBlindBet); //collects and places blind bets from small and big blind players

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

            IsPlaying = false;
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
            for (int potNumber = 0; potNumber < _potManager.Pots.Count; potNumber++)
            {
                // For each player
                for (int i = 0; i < _potManager.PlayersEligible(potNumber).Count; i++)
                {
                    // Reveal the player's cards
                    _playerHands[_potManager.PlayersEligible(potNumber)[i]].Cards[0].GetTexture();
                    _playerHands[_potManager.PlayersEligible(potNumber)[i]].Cards[1].GetTexture();

                    // Get ranking and optimal hand 
                    Tuple<List<Card>, PokerUtil.Ranking> pair = PokerUtil.GetScore(_communityCards, _playerHands[_potManager.PlayersEligible(potNumber)[i]].Cards.ToList());
                    // If this ties the best ranking
                    if (pair.Item2 == bestRanking)
                    {
                        // Mark this hand as needing to be tie broken
                        bestHands.Add(new Tuple<int, List<Card>>(_potManager.PlayersEligible(potNumber)[i], pair.Item1));
                    }
                    // If this hand beats the old best ranking
                    else if (pair.Item2 < bestRanking)
                    {
                        // Update the value
                        bestRanking = pair.Item2;
                        // Don't worry about losing hands
                        bestHands.Clear();
                        // Keep track of this hand
                        bestHands.Add(new Tuple<int, List<Card>>(_potManager.PlayersEligible(potNumber)[i], pair.Item1));
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

                // payout the pot to the winning player(s)
                _players.Payout(winners, _potManager.DistributePot(winners.Count, potNumber));
            }

            if (_blindIncreaseCountdown == 0)
            {
                _bigBlindBet = _bigBlindBet + (_bigBlindBet / 2);
                _smallBlindBet = _bigBlindBet / 2;
                _blindIncreaseCountdown = 5;
                _ante += 2;
            }
            else
                //decrement blind countdown after each hand
                _blindIncreaseCountdown -= 1;

            //check if any players are out of money and need to be eliminated
            _players.EliminatePlayers();

            //set the blinds for next round
            _players.SetNextRoundBlinds();
            _potManager.ResetPots();
            _currentPhase = Phase.INIT;
        }


        /// <summary>
        /// handles the initial stage of betting before any community cards appear
        /// </summary>
        private void HandlePreflop()
        {
            if (_players.AdvanceToRoundConclusion())
            {
                _currentPhase = Phase.FLOP;
                DealFlop();
                return;
            }
            if (!_roundInit)
            {
                // Set the current bet to the big blind.
                _currentBet = _bigBlindBet;

                // Set the player index to the player to the left of the big blind
                playerIndex = _players.GetPreflopStartingBettor();
                _roundInit = true;
            }

            //round init not needed, advance the player index to next player
            else
            {
                playerIndex = (playerIndex + 1) % _playerHands.Count;
            }

            // iterate through the players starting with the player to the left of the big blind. and handle their actions.
            //loop will terminate and betting round will end once conditions are met
            if (!_players.AdvanceRound() && !_players.OnePlayerLeft())
            {
                //if player is either folded or all in, skip the player's turn
                if (_players.IsActivePlayer(playerIndex))
                {
                    return;
                }
                // If the player is the user, set the user playing flag to true.
                if (playerIndex == 0)
                {
                    _userPlaying = true;
                }
                // If the player is an AI player, set the AI playing flag to true.
                else
                {
                    _userPlaying = false;
                }
                // Handle the player's action.
                HandlePlayerAction(playerIndex);

                //if only one player has not folded, prepare pot to award to player and advance to conclusion
                if (_players.OnePlayerLeft())
                {
                    _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
                    _potManager.AddToPot(_currentBet, _players.PackageBets());
                    _players.ResetBets();
                    _roundInit = false;
                    _currentPhase = Phase.CONCLUSION;
                    return;
                }
            }
            if (_players.AdvanceRound() && !_players.OnePlayerLeft())
            {
                //finalize bets for the round and add them to the pots
                _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
                _potManager.AddToPot(_currentBet, _players.PackageBets());

                //reset the bets for the next round to 0
                _players.ResetBets();
                _roundInit = false;
                _currentPhase = Phase.FLOP;
                DealFlop();
                return;
            }
        }

        /// <summary>
        /// rules, flow and exceptions for the second round of betting that follows the flop
        /// </summary>
        private void HandleFlop()
        {
            if (_players.AdvanceToRoundConclusion())
            {
                _currentPhase = Phase.TURN;
                DealTurn();
                return;
            }
            if (!_roundInit)
            {
                // Set the current bet to the big blind.
                _currentBet = _bigBlindBet;

                // Set the player index to the player to the left of the big blind
                playerIndex = _players.GetStartingBettorIndex();
                _roundInit = true;
            }

            //round init not needed, advance the player index to next player
            else
            {
                playerIndex = (playerIndex + 1) % _playerHands.Count;
            }

            // iterate through the players starting with the player to the left of the big blind. and handle their actions.
            //loop will terminate and betting round will end once conditions are met
            if (!_players.AdvanceRound() && !_players.OnePlayerLeft())
            {
                if (_players.IsActivePlayer(playerIndex))
                {
                    //player is either folded or all in, skip the player's turn
                    return;
                }
                // If the player is the user, set the user playing flag to true.
                if (playerIndex == 0)
                {
                    _userPlaying = true;
                }
                // If the player is an AI player, set the AI playing flag to true.
                else
                {
                    _userPlaying = false;
                }
                // Handle the player's action.
                HandlePlayerAction(playerIndex);
                if (_players.OnePlayerLeft())
                {
                    _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
                    _potManager.AddToPot(_currentBet, _players.PackageBets());
                    _players.ResetBets();
                    _roundInit = false;
                    _currentPhase = Phase.CONCLUSION;
                    return;
                }
            }
            if (_players.AdvanceRound() && !_players.OnePlayerLeft())
            {
                //finalize bets for the round and add them to the pots
                _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
                _potManager.AddToPot(_currentBet, _players.PackageBets());

                //reset the bets for the next round to 0
                _players.ResetBets();
                _roundInit = false;
                _currentPhase = Phase.TURN;
                DealTurn();
                return;
            }
        }

        /// <summary>
        /// rules, flow and exceptions for the third round of betting that follows the turn
        /// </summary>
        private void HandleTurn()
        {
            if (_players.AdvanceToRoundConclusion())
            {
                _currentPhase = Phase.RIVER;
                DealTurn();
                return;
            }
            if (!_roundInit)
            {
                // Set the current bet to the big blind.
                _currentBet = _bigBlindBet;

                // Set the player index to the player to the left of the big blind
                playerIndex = _players.GetStartingBettorIndex();
                _roundInit = true;
            }

            //round init not needed, advance the player index to next player
            else
            {
                playerIndex = (playerIndex + 1) % _playerHands.Count;
            }

            // iterate through the players starting with the player to the left of the big blind. and handle their actions.
            //loop will terminate and betting round will end once conditions are met
            if (!_players.AdvanceRound() && !_players.OnePlayerLeft())
            {
                if (_players.IsActivePlayer(playerIndex))
                {
                    //player is either folded or all in, skip the player's turn
                    return;
                }
                // If the player is the user, set the user playing flag to true.
                if (playerIndex == 0)
                {
                    _userPlaying = true;
                }
                // If the player is an AI player, set the AI playing flag to true.
                else
                {
                    _userPlaying = false;
                }
                // Handle the player's action.
                HandlePlayerAction(playerIndex);
                if (_players.OnePlayerLeft())
                {
                    _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
                    _potManager.AddToPot(_currentBet, _players.PackageBets());
                    _players.ResetBets();
                    _roundInit = false;
                    _currentPhase = Phase.CONCLUSION;
                    return;
                }
            }
            if (_players.AdvanceRound() && !_players.OnePlayerLeft())
            {
                //finalize bets for the round and add them to the pots
                _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
                _potManager.AddToPot(_currentBet, _players.PackageBets());

                //reset the bets for the next round to 0
                _players.ResetBets();
                _roundInit = false;
                _currentPhase = Phase.RIVER;
                DealTurn();
                return;
            }
        }

        /// <summary>
        /// rules, flow and exceptions for the second round of betting that follows the flop
        /// </summary>
        private void HandleRiver()
        {
            if (_players.AdvanceToRoundConclusion())
            {
                _currentPhase = Phase.CONCLUSION;
                DealTurn();
                return;
            }
            if (!_roundInit)
            {
                // Set the current bet to the big blind.
                _currentBet = _bigBlindBet;

                // Set the player index to the player to the left of the big blind
                playerIndex = _players.GetStartingBettorIndex();
                _roundInit = true;
            }

            //round init not needed, advance the player index to next player
            else
            {
                playerIndex = (playerIndex + 1) % _playerHands.Count;
            }

            // iterate through the players starting with the player to the left of the big blind. and handle their actions.
            //loop will terminate and betting round will end once conditions are met
            if (!_players.AdvanceRound() && !_players.OnePlayerLeft())
            {
                if (_players.IsActivePlayer(playerIndex))
                {
                    //player is either folded or all in, skip the player's turn
                    return;
                }
                // If the player is the user, set the user playing flag to true.
                if (playerIndex == 0)
                {
                    _userPlaying = true;
                }
                // If the player is an AI player, set the AI playing flag to true.
                else
                {
                    _userPlaying = false;
                }
                // Handle the player's action.
                HandlePlayerAction(playerIndex);
                if (_players.OnePlayerLeft())
                {
                    _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
                    _potManager.AddToPot(_currentBet, _players.PackageBets());
                    _players.ResetBets();
                    _roundInit = false;
                    _currentPhase = Phase.CONCLUSION;
                    return;
                }
            }
            if (_players.AdvanceRound() && !_players.OnePlayerLeft())
            {
                //finalize bets for the round and add them to the pots
                _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
                _potManager.AddToPot(_currentBet, _players.PackageBets());

                //reset the bets for the next round to 0
                _players.ResetBets();
                _roundInit = false;
                _currentPhase = Phase.FLOP;
                DealTurn();
                return;
            }
        }

        /// <summary>
        /// handles the poker action enacted by the user or by the AI opponent
        /// <param name="playerIndex">The index of the player's hand in _playerHands</param>

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
                    Fold(playerIndex);
                    break;
                case PokerAction.CHECK:
                    Check(playerIndex);
                    break;
                case PokerAction.CALL:
                    Call(playerIndex);
                    break;
                case PokerAction.RAISE:
                    // _currentBet = Front end method of setting raise amount for user : AI calculated raise amount
                    Raise(playerIndex);
                    break;
                case PokerAction.ALL_IN:
                    AllIn(playerIndex);
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
            return PokerAction.CALL; //Matches user bet for demo purposes, when logic is implemented will return appropriate PokerAction
        }

        /// <summary>
        /// The check action.
        /// </summary>
        /// <param name="playerIndex">The index of the current player</param>
        private void Check(int playerIndex)
        {
            _players.Check(playerIndex);
        }

        /// <summary>
        /// The call action. Players will bet the difference between the total current bet and their current bet.
        /// </summary>
        /// <param name="playerIndex">The index of the player's hand in _playerHands</param>
        private void Call(int playerIndex)
        {
            // This method's logic will need to change to consider different pots
            _players.Call(_currentBet, playerIndex);
        }

        /// <summary>
        /// The raise action. Players will increase the current bet to a specified size, all other players must match this bet
        /// </summary>
        /// <param name="playerIndex">The index of the player's hand in _playerHands</param>
        /// <param name="raiseAmount">The index of the player is betting</param>
        private void Raise(int playerIndex)
        {
            _players.Raise(_currentBet, playerIndex);
        }

        /// <summary>
        /// The All in action. Player will wager all of their remaining funds, all other players must match this bet
        /// </summary>
        /// <param name="playerIndex">The index of the player's hand in _playerHands</param>
        private void AllIn(int playerIndex)
        {
            _currentBet = _players.AllInBet(playerIndex);
        }

        /// <summary>
        /// The Fold action. The player is no longer participating in this round and is ineligible to recieve any split of the pot
        /// </summary>
        /// <param name="playerIndex">The index of the player's hand in _playerHands</param>
        private void Fold(int playerIndex)
        {
            _players.Fold(playerIndex);
            _potManager.RemoveFoldedPlayers(playerIndex);
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
    #endregion Methods

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
             CallButtonEnabledTexture = content.Load<Texture2D>("CallButtonEnabled");
             CallButtonDisabledTexture = content.Load<Texture2D>("CallButtonDisabled");
             CheckButtonEnabledTexture = content.Load<Texture2D>("CheckButtonEnabled");
             CheckButtonDisabledTexture = content.Load<Texture2D>("CheckButtonDisabled");
             RaiseButtonEnabledTexture = content.Load<Texture2D>("RaiseButtonDisabled");
             RaiseButtonDisabledTexture = content.Load<Texture2D>("RaiseButtonEnabled");
             FoldButtonTexture = content.Load<Texture2D>("FoldButton");
             AllInButtonTexture = content.Load<Texture2D>("AllInButton");

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
        private Texture2D? _disabledTexture;

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
        public PokerActionButton(Texture2D enabledTexture, int x, int y, Texture2D disabledTexture = null)
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
            spriteBatch.Draw(IsEnabled ? _enabledTexture : _disabledTexture ?? _enabledTexture, _buttonRectangle, Color.White);
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
}