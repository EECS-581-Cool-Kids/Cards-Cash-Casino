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
using System.Drawing;
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
        /// The center of the card hand on the screen.
        /// </summary>
        private Point? _center;

        /// <summary>
        /// Adds a card to the hand.
        /// </summary>
        public virtual void AddCard(Card newCard)
        {
            newCard.HideTexture();
            _cards.Add(newCard);
            RecalculateCardPositions();
        }

        public void UnhideCards()
        {
            _cards[0].GetTexture();
            _cards[1].GetTexture();
        }

        public void FCDUnhideCards()
        {
            _cards[0]?.GetTexture();
            _cards[1]?.GetTexture();
            _cards[2]?.GetTexture();
            _cards[3]?.GetTexture();
            _cards[4]?.GetTexture();
        }

        public void FCDHideByIndex(int _cardIndex)
        {
            _cards[_cardIndex].HideTexture();
        }

        public void FCDUnhideByIndex(int _cardIndex)
        {
            _cards[_cardIndex].GetTexture();
        }

        public Card FCDSelectCard(int index)
        {
            return _cards[index];
        }

        /// <summary>
        /// Removes a card at a given index.
        /// </summary>
        /// <returns>The card removed.</returns>
        public Card RemoveCard(int index)
        {
            Card card = _cards[index];
            _cards.RemoveAt(index);
            RecalculateCardPositions();
            return card;
        }

        /// <summary>
        /// Recalculates the card positions.
        /// </summary>
        public void RecalculateCardPositions()
        {
            if (_center is null)
                return;

            int cardCount = _cards.Count;
            int width = (cardCount * 99) + ((cardCount - 1) * 25);
            int xPos = ((Point)_center).X - (width / 2);
            int yPos = ((Point)_center).Y - 70;

            foreach (Card card in _cards)
            {
                card.SetRectangle(xPos, yPos);
                xPos += 30;
            }
        }

        /// <summary>
        /// Sets the center of the card hand.
        /// </summary>
        public void SetCenter(int xPos, int yPos)
        {
            _center = new Point(xPos, yPos);
        }

        /// <summary>
        /// Draw method for the CardHand
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Card card in _cards)
            {
                card.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Clears the hand.
        /// </summary>
        public void Clear()
        {
            _cards.Clear();
        }
    }
}
