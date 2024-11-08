using CardsCashCasino.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Data
{
    public class IndicatorDigit
    {
        /// <summary>
        /// Texture for the second digit.
        /// </summary>
        private Texture2D? _digitTexture;

        /// <summary>
        /// Rectnagle for the first digit.
        /// </summary>
        private Rectangle? _digitRectangle;

        /// <summary>
        /// Sets the position at a given x and y coordinate.
        /// </summary>
        public void SetPosition(int xPos, int yPos)
        {
            _digitRectangle = new Rectangle(xPos, yPos, 21, 24);
        }

        /// <summary>
        /// Updates the digit value.
        /// </summary>
        /// <param name="value"></param>
        public void Update(int value)
        {
            _digitTexture = DisplayIndicatorUtil.GetDigitTexture(value);
        }

        /// <summary>
        /// Draw method for the value indicator.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_digitRectangle is not null)
                spriteBatch.Draw(_digitTexture, (Rectangle)_digitRectangle, Color.White);
        }
    }
}
