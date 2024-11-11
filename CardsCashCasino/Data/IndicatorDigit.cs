/*
 *  Module Name: IndicatorDigit.cs
 *  Purpose: Used for displaying a digit of some kind on the screen.
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
    public class IndicatorDigit : IndicatorCharacter
    {
        /// <summary>
        /// Updates the digit value.
        /// </summary>
        /// <param name="value"></param>
        public void Update(int value)
        {
            _characterTexture = DisplayIndicatorUtil.GetDigitTexture(value);
        }
    }
}
