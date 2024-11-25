/*
 *  Module Name: Constants.cs
 *  Purpose: This module houses the app's constant values.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Mo Morgan, Jacob Wilkus
 *  Date: 11/3/2024
 *  Last Modified: 11/7/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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

        /// <summary>
        /// The height of a button.
        /// </summary>
        public const int BUTTON_HEIGHT = 64;
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
        /// Position of the check button.
        /// </summary>
        public const int CHECK_BUTTON_POS = 0;

        /// <summary>
        /// Position of the call button.
        /// </summary>
        public const int CALL_BUTTON_POS = 1;

        /// <summary>
        /// Position of the raise button.
        /// </summary>
        public const int RAISE_BUTTON_POS = 2;

        /// <summary>
        /// Position of the all in button.
        /// </summary>
        public const int ALL_IN_BUTTON_POS = 3;

        /// <summary>
        /// Position of the fold button.
        /// </summary>
        public const int FOLD_BUTTON_POS = 4;

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

        #region Poker
        /// <summary>
        /// Used to determine the number of AI players in Texas Hold 'Em and 5 Card Draw.
        /// </summary>
        public const int AI_PLAYER_COUNT = 4;

        /// <summary>
        /// The number of card decks used to play poker.
        /// </summary>
        public const int POKER_DECK_COUNT = 1;

        /// <summary>
        /// The number of buttons in poker.
        /// </summary>
        public const int POKER_BUTTON_COUNT = 5;
        #endregion Poker

        #region MainMenu
        /// <summary>
        /// Timer duration for the main menu.
        /// </summary>
        public const int TIMER_DURATION = 200;

        /// <summary>
        /// The number of buttons in the main menu.
        /// </summary>
        public const int MAIN_MENU_BUTTON_COUNT = 3;

        /// <summary>
        /// The width of a main menu button.
        /// </summary>
        public const int MAIN_MENU_BUTTON_WIDTH = 150;

        /// <summary>
        /// The height of a main menu button.
        /// </summary>
        public const int MAIN_MENU_BUTTON_HEIGHT = 222;

        /// <summary>
        /// The position of the blackjack button.
        /// </summary>
        public const int BLACKJACK_BUTTON_POS = 0;

        /// <summary>
        /// The position of the Five Card Draw button.
        /// </summary>
        public const int FIVE_CARD_DRAW_BUTTON_POS = 1;

        /// <summary>
        /// The position of the Texas Hold 'Em button.
        /// </summary>
        public const int TEXAS_HOLD_EM_BUTTON_POS = 2;

        /// <summary>
        /// The position of the quit button.
        /// </summary>
        public const int QUIT_BUTTON_POS = 3;


        #endregion

          /// <summary>
        /// Event called when a timer times out.
        /// </summary>
        public static void OnTimeoutEvent(object source, ElapsedEventArgs e)
        {
            // Stop and dispose of the timer
            Timer timer = (Timer)source;
            timer.Stop();
            timer.Dispose();
        }
    }
}
