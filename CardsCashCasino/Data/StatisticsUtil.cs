using CardsCashCasino.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Data
{
    public static class StatisticsUtil
    {
        /// <summary>
        /// The amount of cash loaded in by the stats file.
        /// This value is only updated once.
        /// </summary>
        private static int _loadedUserCash = 0;

        /// <summary>
        /// The overall amount of cash earned by the player in the history of the game.
        /// </summary>
        private static int _overallCashEarned = 0;

        /// <summary>
        /// The overall amount of cash lost by the player in the history of the game.
        /// </summary>
        private static int _overallCashLost = 0;

        /// <summary>
        /// The overall amount of cash earned by the player in Blackjack.
        /// </summary>
        private static int _blackjackMoneyEarned = 0;

        /// <summary>
        /// The overall amount of cash lost by the player in Blackjack.
        /// </summary>
        private static int _blackjackMoneyLost = 0;

        /// <summary>
        /// The overall amount of games won in Blackjack.
        /// </summary>
        private static int _blackjackGamesWon = 0;

        /// <summary>
        /// The overall amount of games drawn in Blackjack.
        /// </summary>
        private static int _blackjackGamesDrawn = 0;

        /// <summary>
        /// The overall amount of games lost in Blackjack.
        /// </summary>
        private static int _blackjackGamesLost = 0;

        /// <summary>
        /// The overall amount of cash earned by the player in Texas Hold Em.
        /// </summary>
        private static int _holdEmMoneyEarned = 0;

        /// <summary>
        /// The overall amount of cash lost by the player in Texas Hold Em.
        /// </summary>
        private static int _holdEmMoneyLost = 0;

        /// <summary>
        /// The overall amount of games won in Texas Hold Em.
        /// </summary>
        private static int _holdEmGamesWon = 0;

        /// <summary>
        /// The overall amount of games lost in Texas Hold Em.
        /// </summary>
        private static int _holdEmGamesLost = 0;

        /// <summary>
        /// The overall amount of money earned in Five Card Draw.
        /// </summary>
        private static int _fiveCardMoneyEarned = 0;

        /// <summary>
        /// The overall amount of money lost in Five Card Draw.
        /// </summary>
        private static int _fiveCardMoneyLost = 0;

        /// <summary>
        /// The overall amount of games won in Five Card Draw.
        /// </summary>
        private static int _fiveCardGamesWon = 0;

        /// <summary>
        /// The overall amount of games lost in Five Card Draw.
        /// </summary>
        private static int _fiveCardGamesLost = 0;

        /// <summary>
        /// The directory of the game.
        /// </summary>
        private static string _gameDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        /// <summary>
        /// The file path for the game's statistics file.
        /// </summary>
        private static string _statisticsFilePath = Path.Combine(_gameDirectory, "gameStatistics.txt");

        /// <summary>
        /// Handles updating the data when a game of blackjack is won.
        /// </summary>
        public static void WinBlackjackGame(int payout)
        {
            _blackjackGamesWon++;
            _blackjackMoneyEarned += payout;
            _overallCashEarned += payout;
        }

        /// <summary>
        /// Handles updating the data when a game of blackjack is lost.
        /// </summary>
        public static void LoseBlackjackGame(int payout)
        {
            _blackjackMoneyLost++;
            _blackjackMoneyLost += payout;
            _overallCashLost += payout;
        }

        /// <summary>
        /// Handles updating the data when a game of blackjack is drawn.
        /// </summary>
        public static void DrawBlackjackGame()
        {
            _blackjackGamesDrawn++;
        }

        /// <summary>
        /// Handles updating the data when a game of Hold Em is won.
        /// </summary>
        public static void WinHoldEmGame(int payout)
        {
            _holdEmGamesWon++;
            _holdEmMoneyEarned += payout;
            _overallCashEarned += payout;
        }

        /// <summary>
        /// Handles updating the data when a game of Hold Em is lost.
        /// </summary>
        public static void LoseHoldEmGame(int payout)
        {
            _holdEmGamesLost++;
            _holdEmMoneyLost += payout;
            _overallCashLost += payout;
        }

        /// <summary>
        /// Handles updating the data when a game of Five Card Draw is won.
        /// </summary>
        public static void WinFiveCardGame(int payout)
        {
            _fiveCardGamesWon++;
            _fiveCardMoneyEarned += payout;
            _overallCashEarned += payout;
        }

        /// <summary>
        /// Handles updating the data when a game of Five Card Draw is lost.
        /// </summary>
        public static void LoseFiveCardGame(int payout)
        {
            _fiveCardGamesLost++;
            _fiveCardMoneyLost += payout;
            _overallCashLost += payout;
        }

        /// <summary>
        /// Loads the game's statistics file.
        /// </summary>
        public static void LoadStatisticsFile()
        {
            // If the game directory failed to be found, or the statistics file does not exist, return without loading.
            // This behavior happens upon first launch of the game, or launch after deleting the statistics file.
            if (_gameDirectory.Equals(string.Empty) || !File.Exists(_statisticsFilePath))
                return;

            // Read in the data from the statistics file
            List<string> statisticsData = File.ReadAllLines(_statisticsFilePath).ToList();

            // Iterate through each line, and extract the data from the file.
            foreach (string line in statisticsData)
            {
                // Split the line into an array of size two, with the first item being the "id" and the second item being the "value".
                string[] stat = line.Replace("\n", string.Empty).Split("=");

                // Switch case to set the relevant statistic value.
                // Each value is based on the name of the value within the statistics file.
                switch (stat[0])
                {
                    case "currentUserCash": 
                        _loadedUserCash = int.Parse(stat[1]);
                        break;
                    case "overallCashEarned": 
                        _overallCashEarned = int.Parse(stat[1]);
                        break;
                    case "overallCashLost": 
                        _overallCashLost = int.Parse(stat[1]);
                        break;
                    case "blackjackCashEarned":
                        _blackjackMoneyEarned = int.Parse(stat[1]);
                        break;
                    case "blackjackCashLost":
                        _blackjackMoneyLost = int.Parse(stat[1]);
                        break;
                    case "blackjackGamesWon":
                        _blackjackGamesWon = int.Parse(stat[1]);
                        break;
                    case "blackjackGamesLost":
                        _blackjackGamesLost = int.Parse(stat[1]);
                        break;
                    case "blackjackGamesDrawn":
                        _blackjackGamesDrawn = int.Parse(stat[1]);
                        break;
                    case "holdEmCashEarned":
                        _holdEmMoneyEarned = int.Parse(stat[1]);
                        break;
                    case "holdEmCashLost":
                        _holdEmMoneyLost = int.Parse(stat[1]);
                        break;
                    case "holdEmGamesWon":
                        _holdEmGamesWon = int.Parse(stat[1]);
                        break;
                    case "holdEmGamesLost":
                        _holdEmGamesLost = int.Parse(stat[1]);
                        break;
                    case "fiveCardCashEarned":
                        _fiveCardMoneyEarned = int.Parse(stat[1]);
                        break;
                    case "fiveCardCashLost":
                        _fiveCardMoneyLost = int.Parse(stat[1]);
                        break;
                    case "fiveCardGamesWon":
                        _fiveCardGamesWon = int.Parse(stat[1]);
                        break;
                    case "fiveCardGamesLost":
                        _fiveCardGamesLost = int.Parse(stat[1]);
                        break;
                }
            }
        }

        /// <summary>
        /// Saves the game's statistics file.
        /// </summary>
        public static void SaveStatisticsFile()
        {
            // If the game directory fails to load, return.
            if (_gameDirectory.Equals(string.Empty))
                return;

            // Initialize the statistics output file.
            string statisticsOutput = string.Empty;

            // Add each individual statistics value.
            // Most are just integer values from this file. UserCashValue from BettingManager is the only external value.
            statisticsOutput += $"userPreviousCash={BettingManager.UserCashValue}\n";
            statisticsOutput += $"overallCashEarned={_overallCashEarned}\n";
            statisticsOutput += $"overallCashLost={_overallCashLost}\n";
            statisticsOutput += $"blackjackCashEarned={_blackjackMoneyEarned}\n";
            statisticsOutput += $"blackjackCashLost={_blackjackMoneyLost}\n";
            statisticsOutput += $"blackjackGamesWon={_blackjackGamesWon}\n";
            statisticsOutput += $"blackjackGamesLost={_blackjackGamesLost}\n";
            statisticsOutput += $"blackjackGamesDrawn={_blackjackGamesDrawn}\n";
            statisticsOutput += $"holdEmCashEarned={_holdEmMoneyEarned}\n";
            statisticsOutput += $"holdEmCashLost={_holdEmMoneyLost}\n";
            statisticsOutput += $"holdEmGamesWon={_holdEmGamesWon}\n";
            statisticsOutput += $"holdEmGamesLost={_holdEmGamesLost}\n";
            statisticsOutput += $"fiveCardCashEarned={_fiveCardMoneyEarned}\n";
            statisticsOutput += $"fiveCardCashLost={_fiveCardMoneyLost}\n";
            statisticsOutput += $"fiveCardGamesWon={_fiveCardGamesWon}\n";
            statisticsOutput += $"fiveCardGamesLost={_fiveCardGamesLost}\n";

            // Write all of that information to the statistics file.
            File.WriteAllText(_statisticsFilePath, statisticsOutput);
        }

        /// <summary>
        /// Outputs the loaded user cash value.
        /// </summary>
        public static int GetPreviousCashValue()
        {
            return _loadedUserCash;
        }
    }
}
