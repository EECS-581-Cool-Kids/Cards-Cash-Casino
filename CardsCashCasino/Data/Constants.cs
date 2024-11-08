using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Data
{
    public static class Constants
    {
        #region General
        /// <summary>
        /// The window height.
        /// </summary>
        public const int WINDOW_HEIGHT = 600;

        /// <summary>
        /// The window width.
        /// </summary>
        public const int WINDOW_WIDTH = 1000;

        /// <summary>
        /// The width of a button.
        /// </summary>
        public const int BUTTON_WIDTH = 150;
        #endregion General

        #region Blackjack
        /// <summary>
        /// The offset for the result label in Blackjack.
        /// </summary>
        public const int RESULT_LABEL_OFFSET = 90;

        /// <summary>
        /// The value at which the dealer stops hitting.
        /// </summary>
        public const int DEALER_HIT_THRESHOLD = 17;

        /// <summary>
        /// The highest value a hand can have in blackjack.
        /// </summary>
        public const int MAX_BLACKJACK_VALUE = 21;

        /// <summary>
        /// The number of buttons in blackjack.
        /// </summary>
        public const int BLACKJACK_BUTTON_COUNT = 5;

        /// <summary>
        /// Position of the hit button.
        /// </summary>
        public const int HIT_BUTTON_POS = 0;

        /// <summary>
        /// Position of the stand button.
        /// </summary>
        public const int STAND_BUTTON_POS = 1;

        /// <summary>
        /// Position of the double button.
        /// </summary>
        public const int DOUBLE_BUTTON_POS = 2;

        /// <summary>
        /// Position of the split button.
        /// </summary>
        public const int SPLIT_BUTTON_POS = 3;

        /// <summary>
        /// Position of the forfeit button.
        /// </summary>
        public const int FORFEIT_BUTTON_POS = 4;
        #endregion Blackjack
    }
}
