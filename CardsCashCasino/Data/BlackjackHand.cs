/*
 *  Module Name: BlackjackHand.cs
 *  Purpose: Abstract BlackjackHand class. Implements code used between both hands implemented for blackjack.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Jacob Wilkus
 *  Date: 11/8/2024
 *  Last Modified: 11/8/2024
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
    public abstract class BlackjackHand : CardHand
    {
        /// <summary>
        /// Whether or not the hand is "soft".
        /// </summary>
        public bool IsSoftBlackjackValue { get; private set; } = false;

        /// <summary>
        /// Returns the blackjack value of the hand.
        /// </summary>
        public virtual int GetBlackjackValue()
        {
            int blackjackValue = 0;

            foreach (Card card in _cards)
            {
                if (card.IsBlackjackAce && blackjackValue < 11)
                {
                    blackjackValue += card.GetSecondaryBlackjackValue();
                    IsSoftBlackjackValue = true;
                }
                else
                {
                    blackjackValue += card.GetBlackjackValue();

                    if (IsSoftBlackjackValue && blackjackValue > 21)
                    {
                        blackjackValue -= 10;
                        IsSoftBlackjackValue = false;
                    }
                }
            }

            return blackjackValue;
        }

        /// <summary>
        /// Whether or not the hand is a blackjack initially.
        /// </summary>
        public bool HasBlackjack()
        {
            return _cards.Count == 2 && GetBlackjackValue() == Constants.MAX_BLACKJACK_VALUE;
        }
    }
}
