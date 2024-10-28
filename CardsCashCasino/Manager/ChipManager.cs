/*
 *  Module Name: ChipManager.cs
 *  Purpose: Manages the poker chips in the game.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Mo Morgan
 *  Date: 10/26/2024
 *  Last Modified: 10/26/2024
 */

using CardsCashCasino.Data;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Manager
{
    public class ChipManager
    {
        /// <summary>
        /// The list of different chips available in the game.
        /// </summary>
        private List<PokerChip> _chips;

        /// <summary>
        /// The user's chips.
        /// </summary>
        public List<PokerChip> UserChips { get; set; } = new List<PokerChip>();

        /// <summary>
        /// The cash value of the user's chips.
        /// </summary>
        public int UserCashValue
        {
            get
            {
                int cashValue = 0;
                foreach (PokerChip chip in UserChips)
                {
                    cashValue += chip.GetChipValue();
                }
                return cashValue;
            }
        }
        
        public ChipManager()
        {
            _chips = new List<PokerChip>();
        }
        
        /// <summary>
        /// Draw method for the ChipManager.
        /// </summary>
        /// <param name="spriteBatch">Holds the logic that draws the poker chips.</param>
        public void Draw(SpriteBatch spriteBatch)
        {

        }

        /// <summary>
        /// Update method for the ChipManager.
        /// </summary>
        public void Update()
        {

        }

        /// <summary>
        /// Clears the list of chips.
        /// </summary>
        public void ClearChips()
        {
            _chips.Clear();
        }

        /// <summary>
        /// Adds a chip to the user's chips.
        /// </summary>
        /// <param name="chip">A <c>PokerChip</c> object</param>
        public void AddChip(PokerChip chip)
        {
            UserChips.Add(chip);
        }

        /// <summary>
        /// Removes a chip from the user's chips.
        /// </summary>
        /// <param name="chip">A <c>PokerChip</c> object</param>
        public void RemoveChip(PokerChip chip)
        {
            UserChips.Remove(chip);
        }

        /// <summary>
        /// Turns the user's cash winnings into chips.
        /// </summary>
        /// <param name="cashValue">The amount of cash to turn into chips.</param>
        public void CashToChips(int cashValue)
        {
            UserChips.Clear();
            int remainingValue = cashValue;
            foreach (PokerChip chip in _chips.OrderByDescending(chip => chip.GetChipValue()))
            {
                while (remainingValue >= chip.GetChipValue())
                {
                    PokerChip newChip = new PokerChip(chip);
                    AddChip(newChip);
                    remainingValue -= chip.GetChipValue();
                }
            }
        }
    }
    
    /// <summary>
    /// Static class <c>ChipTextures</c> holds the textures for the different chips.
    /// </summary>
    public static class ChipTextures
    {
        /// <summary>
        /// The texture for the $1 chip.
        /// </summary>
        public static Texture2D OneChipTexture { get; set; }

        /// <summary>
        /// The texture for the $5 chip.
        /// </summary>
        public static Texture2D FiveChipTexture { get; set; }

        /// <summary>
        /// The texture for the $10 chip.
        /// </summary>
        public static Texture2D TenChipTexture { get; set; }

        /// <summary>
        /// The texture for the $25 chip.
        /// </summary>
        public static Texture2D TwentyFiveChipTexture { get; set; }

        /// <summary>
        /// The texture for the $100 chip.
        /// </summary>
        public static Texture2D OneHundredChipTexture { get; set; }

        /// <summary>
        /// The texture for the $500 chip.
        /// </summary>
        public static Texture2D FiveHundredChipTexture { get; set; }

        /// <summary>
        /// Loads the content for the chip textures.
        /// </summary>
        public static void LoadContent(ContentManager content)
        {
            OneChipTexture = content.Load<Texture2D>("ChipTextures/TEMP_CHIP");
            FiveChipTexture = content.Load<Texture2D>("ChipTextures/TEMP_CHIP");
            TenChipTexture = content.Load<Texture2D>("ChipTextures/TEMP_CHIP");
            TwentyFiveChipTexture = content.Load<Texture2D>("ChipTextures/TEMP_CHIP");
            OneHundredChipTexture = content.Load<Texture2D>("ChipTextures/TEMP_CHIP");
            FiveHundredChipTexture = content.Load<Texture2D>("ChipTextures/TEMP_CHIP");
        }
    }
}