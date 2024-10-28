/*
 *  Module Name: DealerHand.cs
 *  Purpose: Models the dealer's hand of cards.
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
    public class DealerHand : CardHand
    {
        public DealerHand() { }

        private bool _active = true;
        
        /// <summary>
        /// Returns true if Dealer will hit.
        /// False if stand or invalid.
        /// </summary>
        public bool IsActive() { return _active; } 

        private bool _valid = true;

        /// <summary>
        /// True if dealer busted, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool IsValid() { return _valid; } 

        public void Hit(Card card)
        {
            _cards.Add(card);
            int bjv = GetBlackjackValue();
            
            if (bjv < 17)
                return;

            _active = false;
            
            if (bjv > 21)
                _valid = false;
            
        }
    }
}
