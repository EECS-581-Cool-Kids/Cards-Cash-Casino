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

        public void LoadContent(ContentManager content)
        {
            foreach (PokerChip.ChipValue value in Enum.GetValues(typeof(PokerChip.ChipValue)))
            {
                PokerChip chip = new PokerChip(value);
                chip.LoadContent(content);
                _chips.Add(chip);
            }
        }
        /// <summary>
        /// Draw method for the ChipManager.
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
        public void AddChip(PokerChip chip)
        {
            UserChips.Add(chip);
        }

        /// <summary>
        /// Removes a chip from the user's chips.
        /// </summary>
        public void RemoveChip(PokerChip chip)
        {
            UserChips.Remove(chip);
        }

        /// <summary>
        /// Takes a value, converts it into the least number of chips, and adds them to the user's current chips.
        /// Uses greedy algorithm to determine the least number of chips.
        /// </summary>
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