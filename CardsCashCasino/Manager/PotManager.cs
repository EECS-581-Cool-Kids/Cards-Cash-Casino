/*
 *  Module Name: PotManager.cs
 *  Purpose: Controls logic and rules for the pots used to distribute funds at the end of the game
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Derek Norton
 *  Date: 11/12/2024
 *  Last Modified: 12/6/2024
 */

using CardsCashCasino.Data;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Manager
{
    /// <summary>
    /// Identifies the pot type for the TexasHoldEmPotManager Class
    /// </summary>
    public enum PotType
    {
        MAIN,
        SIDE
    }

    /// <summary>
    /// Manages the pot in a Texas Hold'em game, including adding and distributing chips.
    /// </summary>
    public class Pot
    {
        /// <summary>
        /// Contains the total amount of money contained in a pot, initiated as empty (0)
        /// </summary>
        public int Total { get; set; } = 0;

        /// <summary>
        /// Whether the pot is the MAIN pot (the active pot that bets are allocated to) or a SIDE pot (the pot that an all-in player is eligible to win)
        /// </summary>
        public PotType PotType { get; set; }

        /// <summary>
        /// The list of players eligible to win a given pot
        /// </summary>
        public List<int> EligiblePlayers { get; set; }

        /// <summary>
        /// Initiating the Pot and with the type of pot as its characteristic
        /// </summary>
        public Pot(PotType type)
        {
            PotType = type; //importing the pot type to identify a pot as MAIN or SIDE
        }

        /// <summary>
        /// Increment the total of the pot by a specified value.
        /// </summary>
        /// <param name="value">The amount to increment the pot's total by.</param>
        public void IncrementPot(int value)
        {
            Total += value;
        }

        /// <summary>
        /// Decrement the total of the pot by a specified value.
        /// </summary>
        /// <param name="value">The amount to decrement the pot's total by.</param>
        public void DecrementPot(int value)
        {
            if (value <= 0 || Total - value < 0)
            {
                return;
            }

            Total -= value;
        }

        /// <summary>
        /// Decrement the total of the pot by a specified value.
        /// </summary>
        /// <param name="playerIndex">The position of the player being removed from pot eligiblity .</param>
        public void RemoveEligiblePlayer(int playerIndex)
        {
            if (EligiblePlayers.Contains(playerIndex))
            {
                EligiblePlayers.Remove(playerIndex);
            }
        }
    }
    public class PotManager
    {
        /// <summary>
        /// Importing list that will house each pot and its attributes
        /// </summary>
        public List<Pot> Pots { get; set; }

        /// <summary>
        /// creating variable name that will house the list containing the TexasHoldEmPots
        /// </summary>
        public PotManager()
        {
            Pots = new List<Pot>(); // Initialize the Pots list
        }

        /// <summary>
        /// Initialize the main pot, if any players are short side pots will be created.
        /// </summary>
        /// <param name="_ante">The ante needed to be added by each player to the pot.</param>
        /// <param name="playerBets">List of antes values to be added to the pot.</param>
        public void InitializePot(int ante, List<int> playerBets)
        {
            Pots.Add(new Pot(PotType.MAIN)); //create a new pot

            Pots[0].EligiblePlayers = Enumerable.Range(0, playerBets.Count).ToList(); //add all players as eligible to win the pot

            if (playerBets.All(ante => ante == playerBets[0])) //if all bets match the ante add to main pot
            {
                AddToPot(ante, playerBets);
            }
            else //create side pots if any bets do not match the ante
            {
                CreateSidePots(ante, playerBets);
            }
        }

        /// <summary>
        /// Adds a bets to the pot.
        /// </summary>
        /// <param name="_currentBet">The amount each player is contributing to the pot.</param>
        /// <param name="playerBets">List of bet values to be added to the pot.</param>
        public void AddToPot(int currentBet, List<int> playerBets)
        {
            if (playerBets.All(bet => bet == playerBets[0]))
            {
                Pots.First(pot => pot.PotType == PotType.MAIN).IncrementPot(currentBet * playerBets.Count);
            }
            else
            {
                CreateSidePots(currentBet, playerBets);
            }
        }

        /// <summary>
        //Adds bets of players that were folded to the pot
        /// </summary>
        /// <param name="playersBets"> list of bets that were placed but folded due to a raise in that round</param>
        public void AddFoldedBetsToPot(List<int> playerBets)
        {
            if (playerBets.Any())
            {
                for (int bet = 0; bet < playerBets.Count(); bet++)
                {
                    Pots[0].IncrementPot(playerBets[bet]);
                }
            }
        }

        /// <summary>
        ///Removes a player that has folded from eligiblility for all pots
        /// </summary>
        /// <param name="playersIndex"> position of the player that as folded</param>
        public void RemoveFoldedPlayers(int playerIndex)
        {
            for (int pot = 0; pot < Pots.Count; pot++)
            {
                Pots[pot].RemoveEligiblePlayer(playerIndex);
            }
        }

        /// <summary>
        /// Creates side pots as needed at the end of a round of betting.
        /// </summary>
        /// <param name="currentBet">The amount to add to the pot.</param>
        /// <param name="playerBets">The list of bets from players still active after the round of betting.</param>
        public void CreateSidePots(int currentBet, List<int> playerBets)
        {
            int allInBet = 0; //initiating variable that will hold all in wager
            int numBets = playerBets.Count;

            //all-in bet added to the pot, other players matched bets added to pot as well
            allInBet = playerBets.Min();
            AddToPot(allInBet, Enumerable.Repeat(allInBet, numBets).ToList());

            //subtracting all-in bet value from all other bets, the all-in bet value has already been added to a pot
            for (int player = 0; player < numBets; player++)
            {
                playerBets[player] -= allInBet; // Subtract the amount put into pot from each bet
            }
            currentBet -= allInBet;

            //modify list of players that are eligible for the new main pot by removing player(s) that are all-in
            List<int> allInPlayers = playerBets
                .Select((value, index) => new { value, index })  // Keep both the value and the index
                .Where(x => x.value == 0)  // Filter where the bet is 0 (all-in)
                .Select(x => Pots[0].EligiblePlayers[x.index])  // Select the corresponding player from EligiblePlayers at the same index
                .ToList();

            //remove players that are all in from future side pot calculations that are necessary
            numBets -= allInPlayers.Count;
            playerBets.RemoveAll(value => value == 0);

            if (numBets > 1) //if 1 player or less remains, no more side pot manipulation is needed
            {
                //New side pot created. Pot that all-in player can win is shifted to inactive position, no more bets can be added to this pot
                Pots.Add(new Pot(PotType.SIDE));
                Pots[Pots.Count - 1].IncrementPot(Pots[0].Total); //shifting the pot that the all in player can win to the side, this pot will no longer be added to
                Pots[0].DecrementPot(Pots[Pots.Count - 1].Total); //reseting active pot value to empty
                Pots[Pots.Count - 1].EligiblePlayers = new List<int>(Pots[0].EligiblePlayers); //copys the list of players eligible to win the now side pot

                //removing all players that are all in from eligibility from main pot and any pots created in the future
                for (int player = 0; player < allInPlayers.Count; player++)
                {
                    Pots[0].RemoveEligiblePlayer(allInPlayers[player]);
                }
            }
            //no more bets left to add to any pots
            if (numBets == 0)
            {
                return;
            }
            //if there is a second player that has gone all in this round, recursively add pots until player that has called max bet has been reached
            if (currentBet != playerBets.Min())
            {
                CreateSidePots(currentBet, playerBets);
                return;
            }
            //protects from scenerio where last remaining player from adding excessive amount to the pot
            if (numBets > 1)
            {
                //add remainder of bets from players not all-in into new main pot
                AddToPot(currentBet, playerBets);
            }
        }

        /// <summary>
        /// Pays out the winnings to the player for the individual pot. 
        /// <param name="winners">Number of players that have won the pot.</param>
        /// <param name="potNumber">The pot that is being referred to.</param>
        /// <returns>The amount that each winner has won</returns>
        /// </summary>
        public int DistributePot(int winners, int potNumber)
        {
            if (winners != 0) //prevent division by 0
            {
                int _payout = Pots[potNumber].Total / winners; //splits payout if more than 1 winner is present
                return _payout;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns the amount contained in each pot
        /// <returns>A list of the amounts contained within each pot</returns>
        /// </summary>
        public List<int> GetPotAmounts()
        {
            return Pots.Select(pot => pot.Total).ToList();
        }

        /// <summary>
        /// Resets the pots list to an empty state.
        /// </summary>
        public void ResetPots()
        {
            Pots.Clear();
        }

        /// <summary>
        /// Resets the pots list to an empty state.
        /// <param name="potNumber">The pot that is being referred to.</param>
        /// <returns>The list of players eligible to win the pot being referred to</returns>
        /// </summary>
        public List<int> PlayersEligible(int potNumber)
        {
            return Pots[potNumber].EligiblePlayers;
        }
    }
    public class PokerPotValueIndicator
    {
        /// <summary>
        /// The first digit.
        /// </summary>
        private IndicatorDigit _firstDigit = new();

        /// <summary>
        /// The second digit.
        /// </summary>
        private IndicatorDigit _secondDigit = new();

        /// <summary>
        /// The third digit.
        /// </summary>
        private IndicatorDigit _thirdDigit = new();

        /// <summary>
        /// The fourth digit.
        /// </summary>
        private IndicatorDigit _fourthDigit = new();

        /// <summary>
        /// The previous value.
        /// </summary>
        private int _previousValue = 0;

        /// <summary>
        /// Sets the position of the indicator.
        /// </summary>
        /// <param name="xPos">The x coordinate</param>
        /// <param name="yPos">The y coordinate</param>
        public void SetPosition(int xPos, int yPos)
        {
            _firstDigit.SetPosition(xPos, yPos);
            _secondDigit.SetPosition(xPos + 21, yPos);
            _thirdDigit.SetPosition(xPos + 42, yPos);
            _fourthDigit.SetPosition(xPos + 63, yPos);
        }

        /// <summary>
        /// Updates the indicator based on the value of the hand.
        /// </summary>
        public void Update(int potValue)
        {

            int firstDigit;
            int secondDigit;
            int thirdDigit;
            int fourthDigit;

            if (potValue == _previousValue)
                return;

            if (potValue < 100)
            {
                firstDigit = 0;
                secondDigit = 0;
                thirdDigit = potValue / 10;
                fourthDigit = potValue % 10;
            }
            else if (potValue < 1000)
            {
                firstDigit = 0;
                secondDigit = potValue / 100;
                thirdDigit = (potValue / 10) % 10;
                fourthDigit = potValue % 10;
            }
            else
            {
                firstDigit = potValue / 1000;
                secondDigit = (potValue / 100) % 10;
                thirdDigit = (potValue / 10) % 10;
                fourthDigit = potValue % 10;
            }

            _firstDigit.Update(firstDigit);
            _secondDigit.Update(secondDigit);
            _thirdDigit.Update(thirdDigit);
            _fourthDigit.Update(fourthDigit);

            _previousValue = potValue;
        }

        /// <summary>
        /// Draw method for the value indicator.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_firstDigit != null)
                _firstDigit.Draw(spriteBatch);
            if (_secondDigit != null)
                _secondDigit.Draw(spriteBatch);
            if (_thirdDigit != null)
                _thirdDigit.Draw(spriteBatch);
            _fourthDigit.Draw(spriteBatch);
            // _firstDigit.Draw(spriteBatch);
            // _secondDigit.Draw(spriteBatch);
            // _thirdDigit.Draw(spriteBatch);
            // _fourthDigit.Draw(spriteBatch);
        }
    }
}
