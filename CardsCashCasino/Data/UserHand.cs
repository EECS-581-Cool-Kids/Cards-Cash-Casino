/*
 *  Module Name: UserHand.cs
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
    public class UserHand : CardHand
    {
        public UserHand() { }

        // Can actions still be performed on this?
        private bool _active = true;

        // Is this a hand that we will evaluate as possibly winning?
        public bool _valid = true;

        public bool IsActive() { return _active; }
        public bool IsValid() { return _valid; }

        /// <summary>
        /// If this hand is able to be split, return the index of a card that can be split.
        /// Else, return -1.
        /// </summary>
        /// <returns>Idx of card to be removed.</returns>
        private int splittable()
        {
            for (int i = 1; i < _cards.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (_cards[i] == _cards[j])
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public bool CanSplit()
        {
            return splittable() != -1;
        }

        /// <summary>
        /// Create new UserHand instance.
        /// Removes first repeat, puts it into new hand.
        /// If hand was not splittable, throws ApplicationException
        /// </summary>
        /// <returns>The hand that was created.</returns>
        public UserHand Split()
        {
            int i = splittable();
            if (i == -1)
            {
                throw new ApplicationException("Called UserHand.Split() without a splittable hand");
            }
           
            Card card = _cards[i];
            _cards.RemoveAt(i);
            UserHand newHand = new();
            newHand.AddCard(card);
            return newHand;
            
        }
    
        public void Stand()
        {
            _active = false;
        }

        public void Forfeit()
        {
            _active = false;
            _valid = false;
        }

        public void Hit(Card card) { 
            _cards.Add(card); 
            if (GetBlackjackValue() > 21)
            {
                _active = false;
                _valid = false;
            }
        }

        public void DoubleDown(Card card)
        {
            _cards.Add(card);
            if (GetBlackjackValue()>21)
            {
                _valid = false;
            }
            _active = false;
        }




    
    }
}
