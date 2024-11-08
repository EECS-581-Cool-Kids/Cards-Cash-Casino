/*
 *  Module Name: BlackjackUserHand.cs
 *  Purpose: Models the user's hand of cards.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Jacob Wilkus
 *  Date: 10/26/2024
 *  Last Modified: 10/26/2024
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
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Data
{
    public class BlackjackUserHand : BlackjackHand
    {
        public BlackjackUserHand() { }

        /// <summary>
        /// Whether or not you can double down the hand in a game of blackjack.
        /// </summary>
        public bool CanDoubleDown()
        {
            return _cards.Count == 2;
        }

        /// <summary>
        /// Whether or not you can split the hand in a game of blackjack
        /// </summary>
        public bool CanSplit()
        {
            if (_cards.Count != 2)
                return false;

            return _cards[0] == _cards[1];
        }

        /// <summary>
        /// Removes the last card and returns it.
        /// </summary>
        public Card RemoveLastCard()
        {
            Card toReturn = _cards.Last();
            _cards.Remove(_cards.Last());
            return toReturn;
        }

        /// <summary>
        /// Returns the poker value of the hand.
        /// </summary>
        public int GetPokerValue()
        {
            return 0; // TODO
        }
    }
}
