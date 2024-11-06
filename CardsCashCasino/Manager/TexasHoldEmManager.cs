/*
 *  Module Name: TexasHoldEmManager.cs
 *  Purpose: Manages the game of Texas Hold Em.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Mo Morgan
 *  Date: 11/32024
 *  Last Modified: 11/3/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */

using System;
using System.Collections.Generic;
using CardsCashCasino.Data;
using MonoGame.Framework.Utilities.Deflate;
using static System.Net.Mime.MediaTypeNames;

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
        /// The cardManager for the game.
        /// </summary>
        private CardManager _cardManager;
        
        /// <summary>
        /// The user's hand of cards.
        /// </summary>
        private UserHand _userHand;
        
        /// <summary>
        /// Flag that indicates if the user is still active in the game.
        /// </summary>
        private bool _gameOver;
        
        /// <summary>
        /// The chip manager for the game.
        /// </summary>
        private ChipManager _chipManager;
        
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
        /// This is the amount that all players must match to stay in the game.
        /// </summary>
        private int _currentBet;
        
        /// <summary>
        /// The Small Blind for the game.
        /// </summary>
        private int _smallBlind;
        
        /// <summary>
        /// The Big Blind for the game.
        /// </summary>
        private int _bigBlind;
        
        /// <summary>
        /// The Ante for the game.
        /// This is the amount that all players must pay to play the game.
        /// </summary>
        private int _ante;
        
        /// <summary>
        /// A dictionary that tracks which player has what role.
        /// </summary>
        private Dictionary<PlayerRole, int> _players { get; set; }
        
        private List<CardHand> _playerHands { get; set; }

        public TexasHoldEmManager()
        {
            _cardManager = new CardManager();
            _userHand = new UserHand();
            _gameOver = false;
            _chipManager = new ChipManager();
            _mainPot = 0;
            _sidePot = 0;
            _currentBet = 0;
            _smallBlind = 0;
            _bigBlind = 0;
            _ante = 0;

            _playerHands = new();


            // Initialize the player hands. The first player is the user.
            for (int i = 0; i < Constants.AI_PLAYER_COUNT; i++)
            {
                if (i == 0)
                {
                    _playerHands[i] = new UserHand();
                }
                else
                {
                    _playerHands[i] = new PokerHand();
                }
            }
            
            // Initialize the player roles. The user starts as the dealer. 
            _players = new Dictionary<PlayerRole, int>
            {
                {PlayerRole.DEALER, 0},
                {PlayerRole.SMALL_BLIND, 1},
                {PlayerRole.BIG_BLIND, 2}
            };
        }

        public void StartNewGame()
        {
            _playerHands.Clear();
            _gameOver = false;
            
            
        }

        /// <summary>
        /// Deals cards to all players in the game starting with the player with the small blind and ending with the dealer.
        /// </summary>
        public void DealCards()
        {
            // The index of the current player with the small blind.
            // Initialized to avoid a null reference exception in line 171.
            int currentIndex = _players[PlayerRole.SMALL_BLIND];

            // Iterates through the players in a circular fashion, starting with the player with the small blind.
            for (int i = 0; i < _players.Count; i++) 
            {
                int playerIndex = (currentIndex + i) % _players.Count;
                
                // Deals two cards to each player.
                _playerHands[playerIndex].AddCard(_cardManager.DrawCard());
                _playerHands[playerIndex].AddCard(_cardManager.DrawCard());
            }
        }

        /// <summary>
        /// The top card of the deck is added to the discard pile
        /// The next card of the deck is dealt to the board face up
        /// A round of betting begins with the first player to the left of the dealer who has not folded
        /// </summary>
        public void River()
        {
            _cardManager.Discard(_cardManager.DrawCard());



        }
        
    }
}