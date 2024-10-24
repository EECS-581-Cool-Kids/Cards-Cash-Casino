using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
        /// <summary>
        /// The card's suit.
        /// </summary>
        private Suit _suit;

        /// <summary>
        /// The card's value.
        /// </summary>
        private Value _value;

        /// <summary>
        /// The card's rectangle object.
        /// </summary>
        private Rectangle _cardRectangle;

        /// <summary>
        /// The card's texture.
        /// </summary>
        private Texture2D? _cardTexture;

        /// <summary>
        /// Whether or not the card can be drawn.
        /// </summary>
        public bool CanDraw
        {
            get { return _cardTexture is not null; }
        }

        public Card(Suit suit, Value value)
        {
            _suit = suit;
            _value = value;
        }

        /// <summary>
        /// Draws the card object to the screen.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_cardTexture, _cardRectangle, Color.White);
        }

        /// <summary>
        /// Adds the attributes to draw the card to the screen.
        /// </summary>
        public void SetDrawableObject(Rectangle rectangle, Texture2D cardTexture)
        {
            _cardRectangle = rectangle;
            _cardTexture = cardTexture;
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
            if (blackjackValue.HasTwoValues)
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
        public static bool operator ==(Card card1, Card card2) { return card1._value == card2._value && card1._suit == card2._suit; }

        /// <summary>
        /// The not equals operator between two card objects.
        /// </summary>
        public static bool operator !=(Card card1, Card card2) { return card1._value != card2._value || card1._suit != card2._suit; }

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
        [BlackjackValue(11, 1)]
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
        /// Whether or not there are two values.
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
