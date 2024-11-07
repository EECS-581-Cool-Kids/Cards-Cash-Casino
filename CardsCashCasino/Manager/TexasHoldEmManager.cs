/*
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
using CardsCashCasino.Data;

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
    /// The possible roles that a player can have in Texas Hold Em.
    /// The Small Blind is always left of the Dealer.
    /// </summary>
    public enum PlayerRole
    {
        DEALER,
        SMALL_BLIND,
        BIG_BLIND,
    }
    
    public class TexasHoldEmManager
    {
        /// <summary>
        /// The user's hand of cards.
        /// </summary>
        private UserHand _userHand;
        
        /// <summary>
        /// Flag that indicates if the user is still active in the game.
        /// </summary>
        private bool _gameOver;
        
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
        /// The cardManager for the game.
        /// </summary>
        private CardManager _cardManager;

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
        
        public TexasHoldEmManager()
        {
            _userHand = new UserHand();
            _gameOver = false;
            _playerHands = new List<CardHand>();
            _mainPot = 0;
            _sidePot = 0;
            _currentBet = 0;
            _bigBlindBet = 0;
            _smallBlindBet = 0;
            _ante = 0;
            _cardManager = new CardManager();

            // Initialize the player roles. Randomly selects the dealer. The small and big blinds are to the left of the dealer.
            _currentDealer = new Random().Next(0, _playerHands.Count);
            _currentSmallBlind = (_currentDealer + 1) % _playerHands.Count;
            _currentBigBlind = (_currentDealer + 2) % _playerHands.Count;
        }

        /// <summary>
        /// Starts a new round of Texas Hold Em.
        /// </summary>
        public void StartNewGame()
        {
            _playerHands.Clear();
            _gameOver = false;
            GeneratePlayerHands();
            DealCards();
            
            // TODO: Implement a check for whether the blinds and ante should be increased.
            // TODO: Implement the logic for going through the phases of the game.
            
            // change the roles of the players for the next round.
            _currentDealer = _currentSmallBlind;
            _currentSmallBlind = _currentBigBlind;
            _currentBigBlind = (_currentBigBlind + 1) % _playerHands.Count;
        }

        /// <summary>
        /// Deals cards to all players in the game starting with the player with the small blind and ending with the dealer.
        /// </summary>
        public void DealCards()
        {
            for (int i = 0; i < 2; i++)
            {
                // The index of the current player with the small blind.
                // Initialized to avoid a null reference exception in line 171.
                int currentIndex = _currentSmallBlind;

                // Iterates through the players in a circular fashion, starting with the player with the small blind.
                for (int j = 0; j < _playerHands.Count; j++)
                {
                    int playerIndex = (currentIndex + i) % _playerHands.Count;

                    // Deals a card to the player.
                    _playerHands[playerIndex].AddCard(_cardManager.DrawCard());
                    
                    // Delay to simulate dealing cards.
                    Thread.Sleep(500);
                }
            }
        }
    }
}