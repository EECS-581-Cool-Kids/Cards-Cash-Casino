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
using CardsCashCasino.Data;

namespace CardsCashCasino.Manager
{

    public enum BlackjackAction
    {
        HIT,
        STAND,
        DOUBLE_DOWN,
        SPLIT,
        FORFEIT
    }

    public enum Status
    {
        [Status("Player Bust! Dealer Wins!")]
        PLAYER_BUST,

        [Status("Dealer Bust! Player Wins!")]
        DEALER_BUST,

        [Status("Push")]
        PUSH,

        [Status("Player Wins!")]
        PLAYER_WIN,

        [Status("Dealer Wins!")]
        DEALER_WIN,

        [Status("Player has Blackjack! Player Wins!")]
        PLAYER_BLACKJACK,

        [Status("Dealer has Blackjack! Dealer Wins!")]
        DEALER_BLACKJACK
    }

    public class StatusAttribute : Attribute
    {
        /// <summary>
        /// The status value.
        /// </summary>
        public string Value { get; private set; }

        internal StatusAttribute(string str)
        {
            Value = str;
        }
    }

    public class BlackjackManager
    {
        /// <summary>
        /// The dealer hand.
        /// </summary>
        private DealerHand _dealerHand = new();

        /// <summary>
        /// The user's hand(s). Can have multiple due to splitting.
        /// </summary>
        private List<UserHand> _playerHands = new();

        /// <summary>
        /// Whether or not the game has finished.
        /// </summary>
        private bool _gameOver = false;

        /// <summary>
        /// Call to request clearing the stored decks of cards.
        /// </summary>
        public Action? RequestCardsCleared { get; set; }

        /// <summary>
        /// Call to request the discarded cards are reshuffled in.
        /// </summary>
        public Action? RequestCardsReshuffled { get; set; }

        /// <summary>
        /// Call to request decks of cards be added to the queue of cards.
        /// </summary>
        public Action<int>? RequestDecksOfCards { get; set; }

        /// <summary>
        /// Call to request an individual card be added.
        /// </summary>
        public Func<Card>? RequestCard { get; set; }

        /// <summary>
        /// Starts a new game by generating and shuffling the deck, then dealing initial cards.
        /// </summary>
        public void StartNewGame()
        {
            _playerHands.Clear();
            _dealerHand.Clear();
            _gameOver = false;

            RequestCardsCleared!.Invoke();
            RequestDecksOfCards!.Invoke(4); // Request four decks of cards. Blackjack only works with multiple decks shuffled in.

            UserHand hand = new();

            hand.AddCard(RequestCard!.Invoke());
            _dealerHand.AddCard(RequestCard!.Invoke());
            hand.AddCard(RequestCard!.Invoke());
            _dealerHand.AddCard(RequestCard!.Invoke());

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
        private BlackjackAction getAction()
        {
            /// TODO: Implement logic to have player select a move.
            return BlackjackAction.HIT;
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
                    case BlackjackAction.HIT:
                        hand.Hit(_cardManager.DrawCard());
                        break;

                    case BlackjackAction.STAND:
                        hand.Stand();
                        break;

                    case BlackjackAction.FORFEIT:
                        // Half wager code here
                        hand.Forfeit();
                        break;

                    case BlackjackAction.DOUBLE_DOWN:
                        // double wager code here
                        hand.DoubleDown(_cardManager.DrawCard());
                        break;

                    case BlackjackAction.SPLIT:
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

            foreach (UserHand hand in _playerHands)
            {
                // TODO: Do something with this, or do something with chips?
                string handOutcome = GetGameStatus(hand).GetAttribute<StatusAttribute>()!.Value;
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