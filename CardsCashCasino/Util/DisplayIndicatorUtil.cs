/*
 *  Module Name: DisplayIndicatorUtil.cs
 *  Purpose: Utility methods for displaying value indicators on the screen.
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

using CardsCashCasino.Manager;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Util
{
    public static class DisplayIndicatorUtil
    {
        /// <summary>
        /// Returns the texture for a given digit.
        /// </summary>
        public static Texture2D GetDigitTexture(int digit)
        {
            return digit switch
            {
                1 => DisplayIndicatorTextures.OneTexture!,
                2 => DisplayIndicatorTextures.TwoTexture!,
                3 => DisplayIndicatorTextures.ThreeTexture!,
                4 => DisplayIndicatorTextures.FourTexture!,
                5 => DisplayIndicatorTextures.FiveTexture!,
                6 => DisplayIndicatorTextures.SixTexture!,
                7 => DisplayIndicatorTextures.SevenTexture!,
                8 => DisplayIndicatorTextures.EightTexture!,
                9 => DisplayIndicatorTextures.NineTexture!,
                _ => DisplayIndicatorTextures.ZeroTexture!
            };
        }
        
        public static Texture2D GetTurnTexture()
        {
            return DisplayIndicatorTextures.TurnTexture!;
        }
    }

    public static class DisplayIndicatorTextures
    {
        /// <summary>
        /// Texture for zero.
        /// </summary>
        public static Texture2D? ZeroTexture { get; private set; }

        /// <summary>
        /// Texture for one.
        /// </summary>
        public static Texture2D? OneTexture { get; private set; }

        /// <summary>
        /// Texture for two.
        /// </summary>
        public static Texture2D? TwoTexture { get; private set; }

        /// <summary>
        /// Texture for three.
        /// </summary>
        public static Texture2D? ThreeTexture { get; private set; }

        /// <summary>
        /// Texture for four.
        /// </summary>
        public static Texture2D? FourTexture { get; private set; }

        /// <summary>
        /// Texture for five.
        /// </summary>
        public static Texture2D? FiveTexture { get; private set; }

        /// <summary>
        /// Texture for six.
        /// </summary>
        public static Texture2D? SixTexture { get; private set; }

        /// <summary>
        /// Texture for seven.
        /// </summary>
        public static Texture2D? SevenTexture { get; private set; }

        /// <summary>
        /// Texture for eight.
        /// </summary>
        public static Texture2D? EightTexture { get; private set; }

        /// <summary>
        /// Texture for nine.
        /// </summary>
        public static Texture2D? NineTexture { get; private set; }

        /// <summary>
        /// Texture for the dollar sign.
        /// </summary>
        public static Texture2D? DollarSignTexture { get; private set; }
        
        /// <summary>
        ///
        /// </summary>
        public static Texture2D? TurnTexture { get; private set; }

        /// <summary>
        /// Loads the content for the util.
        /// </summary>
        public static void LoadContent(ContentManager content)
        {
            ZeroTexture = content.Load<Texture2D>("zero");
            OneTexture = content.Load<Texture2D>("one");
            TwoTexture = content.Load<Texture2D>("two");
            ThreeTexture = content.Load<Texture2D>("three");
            FourTexture = content.Load<Texture2D>("four");
            FiveTexture = content.Load<Texture2D>("five");
            SixTexture = content.Load<Texture2D>("six");
            SevenTexture = content.Load<Texture2D>("seven");
            EightTexture = content.Load<Texture2D>("eight");
            NineTexture = content.Load<Texture2D>("nine");
            DollarSignTexture = content.Load<Texture2D>("dollarSign");
            TurnTexture = content.Load<Texture2D>("TEMP_CHIP");
        }
    }
}
