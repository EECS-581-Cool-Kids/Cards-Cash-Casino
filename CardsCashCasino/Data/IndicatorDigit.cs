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
