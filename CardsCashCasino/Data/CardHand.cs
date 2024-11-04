/*
 *  Module Name: CardHand.cs
 *  Purpose: Models a hand of cards.
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

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Data
{
    public abstract class CardHand
    {
        /// <summary>
        /// protected collection of cards.
        /// </summary>
        protected Collection<Card> _cards = new Collection<Card>();

        /// <summary>
        /// ReadOnlyCollection of the cards in the hand.
        /// Used for protecting the values of the cards to only be removed with the helper methods.
        /// </summary>
        public ReadOnlyCollection<Card> Cards
        {
            get
            {
                return new(_cards);
            }
        }

        /// <summary>
        /// Adds a card to the hand.
        /// </summary>
        public void AddCard(Card card)
        {
            _cards.Add(card);
        }

        /// <summary>
        /// Removes a card at a given index.
        /// </summary>
        /// <returns>The card removed.</returns>
        public Card RemoveCard(int index)
        {
            Card card = _cards[index];
            _cards.RemoveAt(index);
            return card;
        }

        /// <summary>
        /// Update method for the CardHand
        /// </summary>
        public void Update()
        {

        }

        /// <summary>
        /// Draw method for the CardHand
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {

        }

        /// <summary>
        /// Returns the value of this hand during blackjack.
        /// </summary>
        /// <returns></returns>
        public int GetBlackjackValue()
        {
            int value = 0;
            int aceCount = 0;

            foreach (Card card in _cards)
                if (card.IsAce())
                    aceCount++;
                else
                    value += card.GetBlackjackValue();

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

        public void Clear()
        {
            _cards.Clear();
        }
    }
}
