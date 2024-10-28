/*
 *  Module Name: UserHand.cs
 *  Purpose: Models the user's hand of cards.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Derek Norton
 *  Date: 10/21/2024
 *  Last Modified: 10/27/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using CardsCashCasino.Manager;
using CardsCashCasino.Data;

namespace CardsCashCasino.Game
{

    public enum Action
    {
        HIT,
        STAND,
        DOUBLE_DOWN,
        SPLIT,
        FORFEIT
    }

    public enum Status
    {
        [StatusString("Player Bust! Dealer Wins!")]
        PLAYER_BUST,
        [StatusString("Dealer Bust! Player Wins!")]
        DEALER_BUST,
        [StatusString("Push")]
        PUSH,
        [StatusString("Player Wins!")]
        PLAYER_WIN,
        [StatusString("Dealer Wins!")]
        DEALER_WIN,
        [StatusString("Player has Blackjack! Player Wins!")]
        PLAYER_BLACKJACK,
        [StatusString("Dealer has Blackjack! Dealer Wins!")]
        DEALER_BLACKJACK
    }

    public class StatusStringAttribute : Attribute {
        public string Val { get; private set; }
        internal StatusStringAttribute(string str)
        {
            Val = str;
        }
    }

    public class BlackjackManager
    {

        private CardManager _cardManager; // Instance of CardManager for deck management
        private DealerHand _dealerHand; // Dealer's hand
        private List<UserHand> _playerHands; // List of all player's hands
        private bool _gameOver; // if player busts, allows game to skip dealer action

        private ChipManager _chipManager;

        public BlackjackManager()
        {
            _cardManager = new CardManager();

            // TODO: somehow keep _chipManager static?
            _chipManager = new ChipManager();

            _dealerHand = new();
            _playerHands = new();
            _gameOver = false;

        }

        /// <summary>
        /// Starts a new game by generating and shuffling the deck, then dealing initial cards.
        /// </summary>
        public void StartNewGame()
        {
            _playerHands.Clear();
            _dealerHand.Clear();
            _gameOver = false;

            _cardManager.ClearDecks();
            _cardManager.GenerateDecks(1);

            // Player always starts with 1 hand, may add more later
            UserHand hand = new();

            hand.AddCard(_cardManager.DrawCard());
            _dealerHand.AddCard(_cardManager.DrawCard());
            hand.AddCard(_cardManager.DrawCard());
            _dealerHand.AddCard(_cardManager.DrawCard());

            _playerHands.Add(hand);
        }

        
        /// <summary>
        /// Determines if the player has won, lost, or if the game is still ongoing.
        /// </summary>
        public Status GetGameStatus(UserHand hand)
        {
            int playerTotal = hand.GetBlackjackValue();
            int dealerTotal = _dealerHand.GetBlackjackValue();

            if (playerTotal > 21)
            {
                return Status.PLAYER_BUST;
            }
            else if (dealerTotal > 21)
            {
                return Status.DEALER_BUST;
            }
            else if (playerTotal == dealerTotal)
            {
                return Status.PUSH;
            }
            else if (playerTotal == 21)
            {
                return Status.PLAYER_BLACKJACK;
            }
            else if (dealerTotal == 21)
            {
                return Status.DEALER_BLACKJACK;
            }
            else if (playerTotal > dealerTotal)
            {
                return Status.PLAYER_WIN;
            }
            else
            {
                return Status.DEALER_WIN;
            }
        }

        /// <summary>
        /// Get the action that a player is trying to make for a given hand.
        /// </summary>
        /// <returns>The action selected</returns>
        private Action getAction()
        {
            /// TODO: Implement logic to have player select a move.
            return Action.HIT;
        }

        /// <summary>
        /// Runs a single round of blackjack.
        /// </summary>
        /// <returns>True if no more actions, false otherwise.</returns>
        private bool blackjackRound()
        {
            bool finished = true;
            foreach (UserHand hand in _playerHands)
            {
                if (!hand.IsActive())
                    continue;

                finished = false;
                switch (getAction())
                {
                    case Action.HIT:
                        hand.Hit(_cardManager.DrawCard());
                        break;

                    case Action.STAND:
                        hand.Stand();
                        break;

                    case Action.FORFEIT:
                        // Half wager code here
                        hand.Forfeit();
                        break;

                    case Action.DOUBLE_DOWN:
                        // double wager code here
                        hand.DoubleDown(_cardManager.DrawCard());
                        break;

                    case Action.SPLIT:
                        UserHand newHand = hand.Split();
                        newHand.Hit(_cardManager.DrawCard());
                        _playerHands.Add(newHand);
                        break;

                    default:
                        throw new ApplicationException("??");
                }
            }

            if (_dealerHand.IsActive())
            {
                _dealerHand.Hit(_cardManager.DrawCard());
                finished = false;
            }

            return finished;
        }

        public void PlayBlackjack()
        {
            StartNewGame(); //Initializing game
            _gameOver = false;
            bool finished = false;
            while (!finished)
            {
                finished = blackjackRound();
            }
            _gameOver = true;

            foreach(UserHand hand in _playerHands)
            {
                // TODO: Do something with this, or do something with chips?
                string handOutcome = GetGameStatus(hand).GetAttribute<StatusStringAttribute>()!.Val;
            }

            // Just replace everything so that we don't need to worry about discarding
            _cardManager.ClearDecks();
            return;
        }
        /// <summary>
        /// Displays the current hand for the player or dealer.
        /// </summary>
        public UserHand GetPlayerHand(int i) => _playerHands[i];
        public DealerHand GetDealerHand() => _dealerHand;
    }
}