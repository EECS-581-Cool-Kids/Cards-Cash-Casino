/*
 *  Module Name: Card.cs
 *  Purpose: Models a card object.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Jacob Wilkus, Ethan Berkley
 *  Date: 10/26/2024
 *  Last Modified: 11/08/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */

using CardsCashCasino.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Data
{
    /// <summary>
    /// The main Card class.
    /// </summary>
    public class Card
    {
        private Suit _suit;
        /// <summary>
        /// The card's suit.
        /// </summary>
        public Suit Suit { get { return _suit; } }

        /// <summary>
        /// The card's value.
        /// </summary>
        private Value _value;

        /// <summary>
        /// The card's rectangle object.
        /// </summary>
        private Rectangle? _cardRectangle;

        /// <summary>
        /// The card's texture.
        /// </summary>
        private Texture2D? _cardTexture;

        /// <summary>
        /// The static size of each card.
        /// </summary>
        private static Point _cardSize = new Point(99, 141);

        /// <summary>
        /// Whether or not the card can be drawn.
        /// </summary>
        public bool CanDraw
        {
            get { return _cardTexture is not null; }
        }

        /// <summary>
        /// Whether or not it is an ace in blackjack.
        /// </summary>
        public bool IsBlackjackAce
        {
            get { return _value.GetAttribute<BlackjackValueAttribute>()!.HasTwoValues; }
        }

        public Card(Suit suit, Value value)
        {
            _suit = suit;
            _value = value;
            GetTexture();
        }

        public Card(Card other)
        {
            _suit = other._suit;
            _value = other._value;
            GetTexture();
        }

        /// <summary>
        /// Draws the card object to the screen.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_cardRectangle is null)
                return;

            if (_cardTexture is null)
                GetTexture();

            spriteBatch.Draw(_cardTexture, (Rectangle)_cardRectangle, Color.White);
        }

        /// <summary>
        /// Creates the rectangle so it can be drawn to the screen.
        /// </summary>
        public void SetRectangle(int xPos, int yPos)
        {
            _cardRectangle = new Rectangle(xPos, yPos, _cardSize.X, _cardSize.Y);
        }

        /// <summary>
        /// Changes the card texture to hide the value.
        /// </summary>
        public void HideTexture()
        {
            _cardTexture = CardTextures.CardBackTexture;
        }

        /// <summary>
        /// Changes the card texture to show the value.
        /// </summary>
        public void GetTexture()
        {
            _cardTexture = CardTextures.GetCardTexture(_value, _suit);
        }

        /// <summary>
        /// Returns the initial blackjack value.
        /// </summary>
        public int GetBlackjackValue()
        {
            return _value.GetAttribute<BlackjackValueAttribute>()!.BlackjackValue;
        }

        /// <summary>
        /// Returns the secondary blackjack value.
        /// </summary>
        public int GetSecondaryBlackjackValue()
        {
            BlackjackValueAttribute blackjackValue = _value.GetAttribute<BlackjackValueAttribute>()!;
            if (!blackjackValue.HasTwoValues)
                return -1;
            else
                return (int)blackjackValue.SecondaryValue!;
        }

        /// <summary>
        /// Returns the poker value.
        /// </summary>
        public int GetPokerValue()
        {
            return _value.GetAttribute<PokerValueAttribute>()!.PokerValue;
        }

        /// <summary>
        /// The equals operator between two card objects.
        /// </summary>
        public static bool operator ==(Card card1, Card card2) { return card1._value == card2._value; }

        /// <summary>
        /// The not equals operator between two card objects.
        /// </summary>
        public static bool operator !=(Card card1, Card card2) { return card1._value != card2._value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool Equals(object? obj) { return ((obj is not null) && (obj is Card)) ? (this == (Card)obj) : false; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override int GetHashCode() { return base.GetHashCode(); }
    }

    /// <summary>
    /// The enumeration value for the card's suit.
    /// </summary>
    public enum Suit
    {
        CLUBS,
        DIAMONDS,
        HEARTS,
        SPADES
    }

    /// <summary>
    /// The enumeration value for the card's value.
    /// Contains two attributes: Blackjack and Poker Value.
    /// </summary>
    public enum Value
    {
        [BlackjackValue(1, 11)]
        [PokerValue(1)]
        ACE,
        
        [BlackjackValue(2)]
        [PokerValue(2)]
        TWO,
        
        [BlackjackValue(3)]
        [PokerValue(3)]
        THREE,
        
        [BlackjackValue(4)]
        [PokerValue(4)]
        FOUR,
        
        [BlackjackValue(5)]
        [PokerValue(5)]
        FIVE,
        
        [BlackjackValue(6)]
        [PokerValue(6)]
        SIX,

        [BlackjackValue(7)]
        [PokerValue(7)]
        SEVEN,

        [BlackjackValue(8)]
        [PokerValue(8)]
        EIGHT,

        [BlackjackValue(9)]
        [PokerValue(9)]
        NINE,

        [BlackjackValue(10)]
        [PokerValue(10)]
        TEN,

        [BlackjackValue(10)]
        [PokerValue(11)]
        JACK,

        [BlackjackValue(10)]
        [PokerValue(12)]
        QUEEN,

        [BlackjackValue(10)]
        [PokerValue(13)]
        KING
    }

    /// <summary>
    /// Enumeration attribute for the Blackjack value.
    /// Can either be a single integer, or two integers.
    /// </summary>
    public class BlackjackValueAttribute : Attribute
    {
        internal BlackjackValueAttribute(int value)
        {
            BlackjackValue = value;
            SecondaryValue = null;
        }

        internal BlackjackValueAttribute(int value1, int value2)
        {
            BlackjackValue = value1;
            SecondaryValue = value2;
        }

        /// <summary>
        /// Whether or not a card has two values.
        /// </summary>
        public bool HasTwoValues
        {
            get { return SecondaryValue is not null; }
        }

        /// <summary>
        /// The main blackjack value.
        /// </summary>
        public int BlackjackValue { get; private set; }

        /// <summary>
        /// The secondary blackjack value.
        /// </summary>
        public int? SecondaryValue { get; private set; }
    }

    /// <summary>
    /// Enumeration attribute for the Poker value. Takes one integer.
    /// </summary>
    public class PokerValueAttribute : Attribute
    {
        internal PokerValueAttribute(int value)
        {
            PokerValue = value;
        }

        /// <summary>
        /// The poker value.
        /// </summary>
        public int PokerValue { get; private set; }
    }

    /// <summary>
    /// Static utility methods for the card class.
    /// </summary>
    public static class CardUtils
    {
        /// <summary>
        /// Extracts an attribute from an enumeration value.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute's type.</typeparam>
        /// <param name="value">The enumeration value.</param>
        public static TAttribute? GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            Type? type = value.GetType();
            string? name = Enum.GetName(type, value);

            if (type is null || name is null)
                return null;

            return type.GetField(name)!.GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }
    }
}
