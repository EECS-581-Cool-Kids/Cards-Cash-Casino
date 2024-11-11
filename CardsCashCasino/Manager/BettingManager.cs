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
        /// The display for the user's cash value.
        /// </summary>
        private CashValueIndicator? _userCashValueIndicator;

        /// <summary>
        /// Loads the betting manager's content.
        /// </summary>
        public void LoadContent()
        {
            _userCashValueIndicator = new();
            _userCashValueIndicator.SetCorner(5, 5);
        }

        /// <summary>
        /// Update method for the ChipManager.
        /// </summary>
        public void Update()
        {
            _userCashValueIndicator!.Update(500);
        }

        /// <summary>
        /// Draw method for the ChipManager.
        /// </summary>
        /// <param name="spriteBatch">Holds the logic that draws the poker chips.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            _userCashValueIndicator!.Draw(spriteBatch);
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
            // intentionally blank
        }
    }

    public class CashValueIndicator
    {
        /// <summary>
        /// The digits representing the number.
        /// </summary>
        private List<IndicatorDigit> _digits = new();

        /// <summary>
        /// The dollar sign.
        /// </summary>
        private IndicatorCharacter _dollarSign;

        /// <summary>
        /// The previous value.
        /// </summary>
        private int _previousValue = 0;

        /// <summary>
        /// The top left corner of the display.
        /// </summary>
        private Point? _topLeftCorner;

        public CashValueIndicator()
        {
            _dollarSign = new(DisplayIndicatorTextures.DollarSignTexture!);
        }

        /// <summary>
        /// Updates the display with a new numerical value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Update(int value)
        {
            if (value == _previousValue)
                return;

            _digits.Clear(); // remove the old digits

            // get each digit
            while (value > 0)
            {
                int digit = value % 10;
                value /= 10;

                IndicatorDigit indicatorDigit = new IndicatorDigit();
                indicatorDigit.Update(digit);
                _digits.Add(indicatorDigit);
            }

            _digits.Reverse(); // put them in the correct order

            if (_topLeftCorner is not null)
                CalculateDigitPositions(((Point)_topLeftCorner).X + 21, ((Point)_topLeftCorner).Y);
        }

        /// <summary>
        /// The draw method for the display.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_topLeftCorner is null || _digits.Count == 0)
                return;

            _dollarSign.Draw(spriteBatch);

            foreach (IndicatorDigit indicatorDigit in _digits)
                indicatorDigit.Draw(spriteBatch);
        }

        /// <summary>
        /// Sets the corner of the display.
        /// </summary>
        /// <param name="xPos">The x coordinate</param>
        /// <param name="yPos">The y coordinate</param>
        public void SetCorner(int xPos, int yPos)
        {
            _topLeftCorner = new Point(xPos, yPos);
            _dollarSign.SetPosition(xPos, yPos);

            xPos += 21;
            CalculateDigitPositions(xPos, yPos);
        }

        /// <summary>
        /// Calculates the positions of the digits on the screen.
        /// </summary>
        /// <param name="xPos">The x coordinate</param>
        /// <param name="yPos">The y coordinate</param>
        private void CalculateDigitPositions(int xPos, int yPos)
        {
            foreach (IndicatorDigit indicatorDigit in _digits)
            {
                indicatorDigit.SetPosition(xPos, yPos);
                xPos += 21;
            }
        }
    }
}
