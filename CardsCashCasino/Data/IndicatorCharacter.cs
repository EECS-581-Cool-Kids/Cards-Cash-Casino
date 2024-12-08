/*
 *  Module Name: IndicatorCharacter.cs
 *  Purpose: Used for displaying a character of some kind on the screen.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Jacob Wilkus
 *  Date: 11/7/2024
 *  Last Modified: 11/10/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Data
{
    public class IndicatorCharacter
    {
        /// <summary>
        /// Texture for the character.
        /// </summary>
        public Texture2D? _characterTexture;

        /// <summary>
        /// Rectnagle for the character.
        /// </summary>
        private Rectangle? _characterRectangle;

        public IndicatorCharacter() { }

        public IndicatorCharacter(Texture2D characterTexture)
        {
            _characterTexture = characterTexture;
        }

        /// <summary>
        /// Sets the position at a given x and y coordinate.
        /// </summary>
        public void SetPosition(int xPos, int yPos)
        {
            _characterRectangle = new Rectangle(xPos, yPos, 21, 24);
        }

        /// <summary>
        /// Draw method for the character.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_characterRectangle is not null && _characterTexture is not null)
                spriteBatch.Draw(_characterTexture, (Rectangle)_characterRectangle, Color.White);
        }

        /// <summary>
        /// Scales the digit.
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(double scale)
        {
            if (_characterRectangle is null)
                return;

            int xPos = _characterRectangle!.Value.X;
            int yPos = _characterRectangle!.Value.Y;

            _characterRectangle = new(xPos, yPos, Convert.ToInt32(30 * scale), Convert.ToInt32(30 * scale));
        }
    }
}
