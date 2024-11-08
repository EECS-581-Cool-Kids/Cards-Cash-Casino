using CardsCashCasino.Data;
using CardsCashCasino.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Manager
{
    public class BettingManager
    {
        /// <summary>
        /// The amount of cash the user has on hand.
        /// </summary>
        public int UserCashValue { get; private set; } = 0;

        /// <summary>
        /// Update method for the ChipManager.
        /// </summary>
        public void Update()
        {

        }

        /// <summary>
        /// Draw method for the ChipManager.
        /// </summary>
        /// <param name="spriteBatch">Holds the logic that draws the poker chips.</param>
        public void Draw(SpriteBatch spriteBatch)
        {

        }

        /// <summary>
        /// Bet a certain amount.
        /// </summary>
        public void Bet(int value)
        {
            UserCashValue -= value;
        }

        /// <summary>
        /// Payout a value won.
        /// </summary>
        public void Payout(int value)
        {
            UserCashValue += value;
        }
    }

    /// <summary>
    /// Textures for betting. Includes the menu for betting, as well as the chip pile textures.
    /// </summary>
    public static class BettingTextures
    {
        /// <summary>
        /// LoadContent for the chip pile textures.
        /// </summary>
        public static void LoadContent(ContentManager content)
        {

        }
    }

    public class CashValueIndicator
    {
        internal class CashValueDigit
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
}
