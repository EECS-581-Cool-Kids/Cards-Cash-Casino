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
    /// The main PokerChip class.
    /// </summary>
    public class PokerChip
    {
        /// <summary>
        /// The chip's value.
        /// </summary>
        private int _value;

        /// <summary>
        /// The chip's rectangle object.
        /// </summary>
        private Rectangle _chipRectangle;

        /// <summary>
        /// The chip's texture.
        /// </summary>
        private Texture2D? _chipTexture;

        /// <summary>
        /// The enumeration value for the chip's value.
        /// </summary>
        public enum ChipValue
        {
            ONE = 1,
            FIVE = 5,
            TEN = 10,
            TWENTY_FIVE = 25,
            ONE_HUNDRED = 100,
            FIVE_HUNDRED = 500
        }

        public PokerChip(ChipValue value)
        {
            _value = (int)ChipValue.value;
        }

        public PokerChip(PokerChip other)
        {
            _value = other._value;
        }

        /// <summary>
        /// Draws the chip object to the screen.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_chipTexture is not null)
            {
                spriteBatch.Draw(_chipTexture, _chipRectangle, Color.White);
            }
        }

        /// <summary>
        /// Sets the chip's texture.
        /// </summary>
        public void SetDrawableObject(Rectangle rectangle, Texture2D texture)
        {
            _chipTexture = texture;
            _chipRectangle = rectangle;
        }

        /// <summary>
        /// Returns the chip's value.
        /// </summary>
        public int GetChipValue()
        {
            return _value;
        }

        /// <summary>
        /// The equivalence operator for the PokerChip class.
        /// </summary>
        public static bool operator ==(PokerChip chip1, PokerChip chip2)
        {
            return chip1._value == chip2._value;
        }

        /// <summary>
        /// The non-equivalence operator for the PokerChip class.
        /// </summary>
        public static bool operator !=(PokerChip chip1, PokerChip chip2)
        {
            return chip1._value != chip2._value;
        }

        /// <summary>
        /// The add operator for the PokerChip class.
        /// </summary>
        public static PokerChip operator +(PokerChip chip1, PokerChip chip2)
        {
            return chip1._value + chip2._value;
        }

        /// <summary>
        /// The add operator for the PokerChip class with an integer.
        /// </summary>
        public static PokerChip operator +(PokerChip chip1, int value)
        {
            return chip1._value + value;
        }

        /// <summary>
        /// The subtract operator for 2 PokerChip class instances.
        /// </summary>
        public static PokerChip operator -(PokerChip chip1, PokerChip chip2)
        {
            return chip1._value - chip2._value;
        }

        /// <summary>
        /// The subtract operator for the PokerChip class with an integer as the subtrahend.
        /// </summary>
        public static PokerChip operator -(PokerChip chip1, int value)
        {
            return chip1._value - value;
        }

        /// <summary>
        /// The subtract operator for the PokerChip class with an integer as the minuend.
        /// </summary>
        public static PokerChip operator -(int value, PokerChip chip1)
        {
            return value - chip1._value;
        }

        /// <summary>
        /// The unary subtract operator for the PokerChip class.
        /// </summary>
        public static PokerChip operator -(PokerChip chip1) => -chip1._value;

        /// <summary>
        /// The unary add operator for the PokerChip class.
        /// </summary>
        public static PokerChip operator +(PokerChip chip1) => chip1;

    }
}