/*
 *  Module Name: BlackjackDealerHand.cs
 *  Purpose: Models the dealer's hand of cards. Used in blackjack.
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

using CardsCashCasino.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Data
{
    public class BlackjackDealerHand : BlackjackHand
    {
        /// <summary>
        /// Whether or not the second card is hidden.
        /// </summary>
        private bool _isSecondCardHidden = false;

        public BlackjackDealerHand() { }

        /// <summary>
        /// Overrides "AddCard" to set the texture of the second card to be hidden.
        /// </summary>
        public override void AddCard(Card card)
        {
            if (_cards.Count == 1)
            {
                card.HideTexture();
                _isSecondCardHidden = true;
            }

            base.AddCard(card);
        }

        /// <summary>
        /// Unhides the initial card.
        /// </summary>
        public void UnhideCard()
        {
            _cards[1].GetTexture();
            _isSecondCardHidden = false;
        }

        /// <summary>
        /// Updates the blackjack value displayed.
        /// </summary>
        public override int GetBlackjackValue()
        {
            if (_isSecondCardHidden)
                return base.GetBlackjackValue() - _cards[1].GetBlackjackValue();
            else
                return base.GetBlackjackValue();
        }
    }
}
