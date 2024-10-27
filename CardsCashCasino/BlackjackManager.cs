using System;
using System.Collections.Generic;
using System.Reflection;

namespace CardsCashCasino.Game
{
    public class BlackjackManager
    {
        private CardManager _cardManager; // Instance of CardManager for deck management
        private List<Card> _dealerHand; // Dealer's hand
        private List<Card> _playerHand; // Player's hand
        private List<Card> _playerSplitHand; // Player's split hand
        private bool _gameOver; //if player busts, allows game to skip dealer action
        private bool _splitHand; //activates split hand functionality if player splits

        public BlackjackManager()
        {
            _cardManager = new CardManager();
            _dealerHand = new List<Card>();
            _playerHand = new List<Card>();
            _playerSplitHand = new List<Card>();
            _gameOver = false;
            _splitHand = false;
        }

        /// <summary>
        /// Starts a new game by generating and shuffling the deck, then dealing initial cards.
        /// </summary>
        public void StartNewGame()
        {
            _playerHand.Clear();
            _playerSplitHand.Clear();
            _dealerHand.Clear();
            _gameOver = false;
            _splitHand = false;
            _cardManager.ClearDecks();
            _cardManager.GenerateDecks(1);
            _playerHand.Add(_cardManager.DrawCard());
            _dealerHand.Add(_cardManager.DrawCard());
            _playerHand.Add(_cardManager.DrawCard());
            _dealerHand.Add(_cardManager.DrawCard());
        }

        public int GetBlackjackValue(List<Card> hand)
        {
            int value = 0;
            int aceCount = 0;

            foreach (Card card in hand)
            {
                switch (card.Value)
                {
                    case Value.Two:
                    case Value.Three:
                    case Value.Four:
                    case Value.Five:
                    case Value.Six:
                    case Value.Seven:
                    case Value.Eight:
                    case Value.Nine:
                    case Value.Ten:
                        value += (int)card.Value; // Number cards are worth their face value
                        break;
                    case Value.Jack:
                    case Value.Queen:
                    case Value.King:
                        value += 10; // Face cards are worth 10 points each
                        break;
                    case Value.Ace:
                        aceCount++; // Track the number of aces
                        break;
                }
            }

            // Add ace values, either as 11 or 1, as appropriate
            for (int i = 0; i < aceCount; i++)
            {
                if (value + 11 <= 21)
                {
                    value += 11; // Ace is worth 11 if it doesn’t push over 21
                }
                else
                {
                    value += 1; // Otherwise, Ace is worth 1
                }
            }

            return value;
        }
        public void SplitHand()
        {
            // Check if the player has exactly two cards of the same value
            if (_playerHand.Count == 2 && _playerHand[0].Value == _playerHand[1].Value)
            {
                // Move the second card to the split hand
                _playerSplitHand.Add(_playerHand[1]);
                _playerHand.RemoveAt(1);

                // Draw a new card for each hand
                _playerHand.Add(_cardManager.DrawCard());
                _playerSplitHand.Add(_cardManager.DrawCard());
            }
        }
        /// <summary>
        /// Determines if the player has won, lost, or if the game is still ongoing.
        /// </summary>
        public string GetGameStatus(List<Card> hand)
        {
            int playerTotal = GetBlackjackValue(hand);
            int dealerTotal = GetBlackjackValue(_dealerHand);

            if (playerTotal > 21)
            {
                _gameOver = true;
                return "Player Bust! Dealer Wins!";
            }
            else if (dealerTotal > 21)
            {
                return "Dealer Bust! Player Wins!";
            }
            else if (playerTotal == dealerTotal)
            {
                return "Push";
            }
            else if (playerTotal == 21)
            {
                return "Player has Blackjack! Player Wins!";
            }
            else if (dealerTotal == 21)
            {
                return "Dealer has Blackjack! Dealer Wins!";
            }
            else if (playerTotal > dealerTotal)
            {
                return "Player wins!";
            }
            else
            {
                return "Dealer wins!";
            }
        }


        public void PlayBlackjack()
        {
            StartNewGame(); //Initializing game

            while (GetBlackjackValue(_playerHand) < 21)
            {

                if (playerHit) //Link Hit button here when implemented
                {
                    _playerHand.Add(_cardManager.DrawCard());
                    GetGameStatus(_playerHand); //checks if player busted
                }
                else if (playerDoubleDown) //Link Double down button here when implemented
                {
                    _playerHand.Add(_cardManager.DrawCard());
                    //double wager code here
                    GetGameStatus(_playerHand); //checks if player busted
                    break;
                }
                else if (_splitHand == false && playerSplit) //Link Split button here when implemented
                {
                    _splitHand = true;
                    //add additional wager 
                    SplitHand();
                }
                else if (playerStand) //Link stand button here when implemented
                {
                    break;
                }
            }
            while (GetBlackjackValue(_playerSplitHand) < 21 && _splitHand == true)
            {

                if (playerHit) //Link Hit button here when implemented
                {
                    _playerSplitHand.Add(_cardManager.DrawCard());
                    GetGameStatus(_playerSplitHand);
                }
                else if (playerDoubleDown) //Link Double down button here when implemented
                {
                    _playerHand.Add(_cardManager.DrawCard());
                    //double wager
                    GetGameStatus(_playerSplitHand);
                    break;
                }
                else if (playerStand) //Link Split button here when implemented
                {
                    break;
                }
            }
            while (GetBlackjackValue(_dealerHand) < 17 && !_gameOver) //dealer continues to hit until value reaches or exceeds 17
            {
                _dealerHand.Add(_cardManager.DrawCard());
            }
            if (GetBlackjackValue(_playerHand) =< 21) //runs if player didn't bust
            {
                GetGameStatus(_playerHand);
            }
            if (GetBlackjackValue(_playerSplitHand) <= 21 && _splitHand == true)
            {
                GetGameStatus(_playerSplitHand);
            }
            _cardManager.ClearDecks();
            return;

            /// <summary>
            /// Displays the current hand for the player or dealer.
            /// </summary>
            public List<Card> GetPlayerHand() => _playerHand;
            public List<Card> GetDealerHand() => _dealerHand;
        }
    }