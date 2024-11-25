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

    /// <summary>
    /// Identifies the pot type for the TexasHoldEmPotManager Class
    /// </summary>
    public enum PotType
    {
        MAIN,
        SIDE
    }

    /// <summary>
    /// The type of player being referred to in the HoldEmPlayerMangager Class
    /// </summary>
    public enum PlayerType
    {
        USER,
        AI
    }

    /// <summary>
    /// The status of the player in the current round
    /// </summary>
    public enum PlayerStatus
    {
        IN,
        FOLDED,
        CALLED,
        ALLIN
    }

    /// <summary>
    /// The identifier applied to each player regarding blinds and player betting order
    /// </summary>
    public enum PlayerPosition
    {
        DEALER,
        SMALLBLIND,
        BIGBLIND,
        NONE
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
        HoldEmPlayerManager _players = new HoldEmPlayerManager();

        /// <summary>
        /// Variable to hold the Pots Manager class
        /// </summary>
        private TexasHoldEmPotManager _potManager = new TexasHoldEmPotManager();

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


    public class Player
    {
        /// <summary>
        /// how much money the user has available in the game
        /// </summary>
        public int PlayerStack { get; set; } = 500; // will be set to users account funds

        /// <summary>
        /// represents if the player is the USER or an AI
        /// </summary>
        public PlayerType PlayerType { get; set; }

        /// <summary>
        /// represents if the player is IN (still participating in the round), FOLDED, CALLED, or ALL-IN
        /// </summary>
        public PlayerStatus PlayerStatus { get; set; } = PlayerStatus.IN;

        /// <summary>
        /// if the player is the DEALER, BIGBLIND, SMALLBLIND, or NONE
        /// </summary>
        public PlayerPosition PlayerPosition { get; set; } = PlayerPosition.NONE;

        /// <summary>
        /// holds the value of the players bet in the current round
        /// </summary>
        public int PlayerBet { get; set; } = 0;

        /// <summary>
        /// Defining constructors for the class
        /// </summary>
        public Player(PlayerType type)
        {
            PlayerType = type; //defines whether the player is a USER or AI
        }

        /// <summary>
        /// Adds winnings to a players chip stack
        /// <param name="value">The amount added</param>
        /// </summary>
        public void IncrementStack(int value)
        {
            PlayerStack += value;
        }

        /// <summary>
        /// Subtracts funds from a players chip stack
        /// <param name="value">The amount subtracted</param>
        /// </summary>
        public void DecrementStack(int value)
        {
            if (value <= 0 || PlayerStack - value < 0)
            {
                return;
            }
            PlayerStack -= value;
        }

        /// <summary>
        /// Adds funds from a players chip bet
        /// /// <param name="value">The amount added</param>
        /// </summary>
        public void IncrementBet(int value)
        {
            PlayerBet += value;
        }

        /// <summary>
        /// Subtracts from the player bet total in the round
        /// <param name="value">The amount subtracted</param>
        /// </summary>
        public void DecrementBet(int value)
        {
            if (value <= 0 || PlayerBet - value < 0)
            {
                return;
            }
            PlayerBet -= value;
        }
    }

    public class HoldEmPlayerManager
    {
        /// <summary>
        /// Creating list to hold players
        /// </summary>
        public List<Player> Players;

        /// <summary>
        /// Initiating list to hold player characteristics
        /// </summary>
        public HoldEmPlayerManager()
        {
            Players = new List<Player>(); // Initialize the Players list
        }

        /// <summary>
        /// Creates USER and specified number of AI opponents
        /// </summary>
        public void InitiatePlayers(int numAIs)
        {
            Players.Add(new Player(PlayerType.USER));

            for (int players = 0; players < numAIs; players++)
            {
                Players.Add(new Player(PlayerType.AI));
            }
            Random random = new Random();
            int dealer = random.Next(0, Players.Count);
            Players[dealer].PlayerPosition = PlayerPosition.DEALER;
            Players[(dealer + 1) % Players.Count].PlayerPosition = PlayerPosition.SMALLBLIND;
            Players[(dealer + 2) % Players.Count].PlayerPosition = PlayerPosition.BIGBLIND;
        }

        /// <summary>
        /// Collect each player's antes for the beginning of the round
        /// </summary>
        public void GenerateAntes(int ante)
        {
            for (int player = 0; player < Players.Count; player++)
            {
                if (ante > Players[player].PlayerStack)
                {
                    Players[player].IncrementBet(Players[player].PlayerStack);
                    Players[player].DecrementStack(Players[player].PlayerStack);
                    Players[player].PlayerStatus = PlayerStatus.ALLIN;
                }
                else
                {
                    Call(ante, player);
                }
            }
        }
        /// <summary>
        /// Gets the position of the first bet to be placed in the preflop round
        /// </summary>
        public int GetPreflopStartingBettor()
        {
            int index = Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);
            index = (index + 1) % Players.Count;
            return index;
        }

        /// <summary>
        /// Gets the position of the first bet to be placed in all rounds following the preflop
        /// </summary>
        public int GetStartingBettorIndex()
        {
            int index = Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            bool activePlayer = false;
            while (!activePlayer)
            {
                if (!IsActivePlayer(index))
                {
                    index = (index + 1) % Players.Count;
                }
                else
                {
                    activePlayer = true;
                }
            }
            return index;
        }

        /// <summary>
        /// If the player is eligible to place a bet returns true
        /// <param name="playerIndex">The index of the current player</param>
        /// </summary>
        public bool IsActivePlayer(int playerIndex)
        {
            if (Players[playerIndex].PlayerStatus == PlayerStatus.FOLDED || Players[playerIndex].PlayerStatus == PlayerStatus.ALLIN)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Places a bet
        /// <param name="amount">The amount of the highest bet placed in the round</param>
        /// <param name="playerIndex">The index of the current player</param>
        /// </summary>
        public void Call(int amount, int playerIndex)
        {
            int bet = amount - Players[playerIndex].PlayerBet; //subtracts the player's existing bet from the amount needed
            Players[playerIndex].IncrementBet(bet);
            Players[playerIndex].DecrementStack(bet);
            if (Players[playerIndex].PlayerStack == 0)
            {
                Players[playerIndex].PlayerStatus = PlayerStatus.ALLIN;
                return;
            }
            Players[playerIndex].PlayerStatus = PlayerStatus.CALLED;
        }

        /// <summary>
        /// Increase the player's bet amount, remove funds from player stack, reset status of all players that can still bet
        /// <param name="playerIndex">The index of the current player</param>
        /// <param name="playerIndex">The index of the current player</param>
        /// </summary>
        public void Raise(int amount, int playerIndex)
        {
            Players[playerIndex].IncrementBet(amount);
            Players[playerIndex].DecrementStack(amount);

            // Iterate through list of players to reset the status for those who called
            // Those players will need to match the raise in order to continue playing
            for (int player = 0; player < Players.Count; player++)
            {
                if (player == playerIndex)
                {
                    Players[player].PlayerStatus = PlayerStatus.CALLED;
                }
                else if (Players[player].PlayerStatus == PlayerStatus.CALLED)
                {
                    Players[player].PlayerStatus = PlayerStatus.IN;
                }
            }
        }

        /// <summary>
        /// Places an all in bet, Increase the player's bet amount, remove funds from player stack, reset status of all players that can still bet
        /// <param name="playerIndex">The index of the current player</param>
        /// </summary>
        public int AllInBet(int playerIndex)
        {
            Players[playerIndex].IncrementBet(Players[playerIndex].PlayerStack);
            Players[playerIndex].DecrementStack(Players[playerIndex].PlayerStack);

            // Iterate through list of players to reset the status for those who called
            // Those players will need to match the all-in bet in order to continue playing
            for (int player = 0; player < Players.Count; player++)
            {
                if (player == playerIndex)
                {
                    Players[player].PlayerStatus = PlayerStatus.ALLIN;
                }
                else if (Players[player].PlayerStatus == PlayerStatus.CALLED)
                {
                    Players[player].PlayerStatus = PlayerStatus.IN;
                }
            }
            return Players[playerIndex].PlayerBet;
        }

        /// <summary>
        /// Folds the active player
        /// <param name="playerIndex">The index of the current player</param>
        /// </summary>
        public void Fold(int playerIndex)
        {
            Players[playerIndex].PlayerStatus = PlayerStatus.FOLDED;
        }

        /// <summary>
        /// Active player bets nothing
        /// <param name="playerIndex">The index of the current player</param>
        /// </summary>
        public void Check(int playerIndex)
        {
            Players[playerIndex].PlayerStatus = PlayerStatus.CALLED;
        }

        /// <summary>
        /// Conditions to be met to advance to the next round
        /// </summary>
        public bool AdvanceRound()
        {
            if (!Players.Any(player => player.PlayerStatus == PlayerStatus.IN)) //if all players have had their turn, have called the existing bet, folded, or are all in
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// If true, only one player is able to place a bet, the rest are all-in.
        /// All player hands will become visible
        /// The game will bypass all remaining betting opportunities and advance through stages to the RoundConclusion
        /// </summary>
        public bool AdvanceToRoundConclusion()
        {
            if (Players.Count(player => player.PlayerStatus == PlayerStatus.CALLED) <= 1 && Players.Count(player => player.PlayerStatus == PlayerStatus.ALLIN) >= 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// If true, only one player is not folded.
        /// The game will bypass all remaining stages and payout MAIN pot to remaining player
        /// </summary>
        public bool OnePlayerLeft()
        {
            if (Players.Count(player => player.PlayerStatus == PlayerStatus.FOLDED) == Players.Count - 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// At the end of a round, resets all player bets to 0
        /// </summary>
        public void ResetBets()
        {
            for (int player = 0; player < Players.Count; player++)
            {
                if (Players[player].PlayerBet != 0)
                {
                    Players[player].DecrementBet(Players[player].PlayerBet);
                }
                if (Players[player].PlayerStatus == PlayerStatus.CALLED)
                {
                    Players[player].PlayerStatus = PlayerStatus.IN;
                }
            }
        }

        /// <summary>
        /// Packages bets placed by folded players to be added to the main pot
        /// </summary>
        public List<int> PackageFoldedBets()
        {
            List<int> foldedBets = new List<int>();

            for (int player = 0; player < Players.Count; player++)
            {
                if (Players[player].PlayerStatus == PlayerStatus.FOLDED && Players[player].PlayerBet > 0)
                {
                    foldedBets.Add(Players[player].PlayerBet);
                }
            }
            return foldedBets;
        }

        /// <summary>
        /// Packages bets made by active players to be added to main pot, creating side pots if needed
        /// </summary>
        public List<int> PackageBets()
        {
            List<int> bets = new List<int>();

            for (int player = 0; player < Players.Count; player++)
            {
                if (Players[player].PlayerStatus != PlayerStatus.FOLDED && Players[player].PlayerBet > 0)
                {
                    bets.Add(Players[player].PlayerBet);
                }
            }
            return bets;
        }

        /// <summary>
        ///Eliminates players that are out of money before a new round begins
        /// </summary>
        public void EliminatePlayers()
        {
            for (int player = 0; player < Players.Count; player++)
            {
                if (Players[player].PlayerStack == 0)
                {
                    if (Players[player].PlayerPosition == PlayerPosition.DEALER) //shift dealer chip 1 to left if dealer was eliminated
                    {
                        Players[player + 1 % Players.Count].PlayerPosition = PlayerPosition.DEALER;
                    }
                    Players.RemoveAt(player);
                }
            }
        }

        /// <summary>
        ///shifts the blind to the next positions
        /// </summary>
        public void SetNextRoundBlinds()
        {
            //blind shift rules if there are only 2 players remaining 
            if (Players.Count < 3)
            {
                if (Players[0].PlayerPosition == PlayerPosition.BIGBLIND)
                {
                    Players[0].PlayerPosition = PlayerPosition.SMALLBLIND;
                    Players[1].PlayerPosition = PlayerPosition.BIGBLIND;
                }
                else
                {
                    Players[0].PlayerPosition = PlayerPosition.BIGBLIND;
                    Players[1].PlayerPosition = PlayerPosition.SMALLBLIND;
                }
            }
            else
            {
                int dealer = Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER);
                if (Players.Count > 3) //if a only 3 players exist, no player is set to NONE
                {
                    Players[dealer].PlayerPosition = PlayerPosition.NONE;
                }
                Players[(dealer + 1) % Players.Count].PlayerPosition = PlayerPosition.DEALER;
                Players[(dealer + 2) % Players.Count].PlayerPosition = PlayerPosition.SMALLBLIND;
                Players[(dealer + 3) % Players.Count].PlayerPosition = PlayerPosition.BIGBLIND;
            }
        }

        /// <summary>
        ///collect blind bets from small and big blind players
        ///<param name="smallBlind">The amount the small blind must wager</param>
        ///<param name="bigBlind">The amount the big blind must wager</param>
        /// </summary>
        public void CollectBlinds(int _smallBlind, int _bigBlind)
        {
            int smallBlindPosition = Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            if (Players[smallBlindPosition].PlayerStack <= _smallBlind) //if blind causes player to go all in
            {
                Players[smallBlindPosition].IncrementBet(Players[smallBlindPosition].PlayerStack);
                Players[smallBlindPosition].DecrementStack(Players[smallBlindPosition].PlayerStack);
                Players[smallBlindPosition].PlayerStatus = PlayerStatus.ALLIN;
            }
            else
            {
                Players[smallBlindPosition].IncrementBet(_smallBlind);
                Players[smallBlindPosition].DecrementStack(_smallBlind);
            }
            int bigBlindPosition = Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);
            if (Players[bigBlindPosition].PlayerStack <= _bigBlind) //if blind causes player to go all in
            {
                Players[bigBlindPosition].IncrementBet(Players[bigBlindPosition].PlayerStack);
                Players[bigBlindPosition].DecrementStack(Players[bigBlindPosition].PlayerStack);
                Players[bigBlindPosition].PlayerStatus = PlayerStatus.ALLIN;
            }
            else
            {
                Players[bigBlindPosition].IncrementBet(_bigBlind);
                Players[bigBlindPosition].DecrementStack(_bigBlind);
            }
        }

        /// <summary>
        /// adds the amount from the pot that the player has won
        /// <param name="winningPlayers">The list of indexes of the players that have won the pot</param>
        /// <param name="payout">The amount each player has won</param>
        /// </summary>
        public void Payout(List<int> winningPlayers, int payout)
        {
            for (int player = 0; player < winningPlayers.Count; player++)
            {
                Players[winningPlayers[player]].IncrementStack(payout);
            }
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
        /// <param name="playerBets">List of bet values to be added to the pot.</param>
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
            //no more bets left to add to any pots
            if (numBets == 0)
            {
                return;
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
        /// <param name="winners">Number of players that have won the pot.</param>
        /// <param name="potNumber">The pot that is being referred to.</param>
        /// <returns>The amount that each winner has won</returns>
        /// </summary>
        public int DistributePot(int winners, int potNumber)
        {
            if (winners != 0) //prevent division by 0
            {
                int _payout = Pots[potNumber].Total / winners; //splits payout if more than 1 winner is present
                return _payout;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns the amount contained in each pot
        /// <returns>A list of the amounts contained within each pot</returns>
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

        /// <summary>
        /// Resets the pots list to an empty state.
        /// <param name="potNumber">The pot that is being referred to.</param>
        /// <returns>The list of players eligible to win the pot being referred to</returns>
        /// </summary>
        public List<int> PlayersEligible(int potNumber)
        {
            return Pots[potNumber].EligiblePlayers;
        }
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