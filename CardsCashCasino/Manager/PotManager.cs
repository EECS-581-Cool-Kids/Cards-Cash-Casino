using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CardsCashCasino.Manager
{
    /// <summary>
    /// Manages the pot in a Texas Hold'em game, including adding and distributing chips.
    /// </summary>
    public class PotManager
    {

        /// <summary>
        /// List of pots for players with different betting amounts.
        /// </summary>
        private List<int> _pots;

        /// <summary>
        /// points to the active pot in which any bets are added.
        /// </summary>
        private int _potPointer;
        

        /// <summary>
        /// Initialize the main pot, if any players are short side pots will be created.
        /// </summary>
        public void InitializePot(List<int> ante, int anteValue)
        {
            _pots.Insert(0, 0);
            if (ante.All(x => x == ante[0]))
            {
                for (int player = 0; player < ante.Count(); player++)
                {
                    _pots[0] += ante[player];
                }
            }
            else
            {
                List<int> allInBets = ante.Where(x => x != anteValue).ToList();
                CreateSidePots(allInBets, ante);
            }
            _potPointer = 0;
        }

        /// <summary>
        /// Adds a bet to the pot.
        /// </summary>
        /// <param name="betAmount">The amount to add to the pot.</param>
        public void AddToPot(int numPlayers, int betAmount)
        {
            _pots[_potPointer] += betAmount * numPlayers;
        }

        /// <summary>
        /// Adds a bet to the pot.
        /// </summary>
        /// <param name="betAmount">The amount to add to the pot.</param>
        public void CreateSidePots(List<int> allIns, List<int> bets)
        {
            allIns.Sort(); //sort the bets and the all in bets from smallest to largest quantity to simplify side pot creation process
            bets.Sort();
            for (int i = _potPointer; i <= allIns.Count + _potPointer; i++) //iterate through the all in bets in round to correctly form side pots needed 
            {
                if (i == _potPointer) //add remainder of funds needed to main pot
                {
                    _pots[i] += allIns[0] * bets.Count;
                }
                else
                {
                    _pots.Insert(0, i); //create side pot
                    for (int j = i; j < bets.Count; j++) //add funds the all in player is not eligible for to side pot
                    {
                        _pots[i] += bets[j] - allIns[i - 1];
                    }
                    if (i + 1 == bets.Count) //if only one player is not all in
                    {
                        //all players are all in, advance to the end of the round
                    }
                    else if (i + 1 != allIns.Count) // if a second all in bet is detected restructure first side pot accordingly
                    {
                        for (int j = i; j < bets.Count; j++)
                        {
                            _pots[i] -= bets[j + 1] - bets[i]; //first side pot total = current side pot total - excess bet to be added to next side pot
                        }
                    }
                    else //no more side pots are needed
                    {
                        break;
                    }
                }

                _potPointer = _pots.Count - 1; //reset the pointer to point to last side pot


            }
        }
            /// <summary>
            /// Pays out the winnings to the player for the individual pot. 
            /// </summary>
            /// <param name="amount">The amount to pay out from the pot.</param>
            public int DistributePot(int winners, int potNum)
            {
                int _payout = _pots[potNum] / winners; //splits payout if more than 1 winner is present
                return _payout;
            }

            /// <summary>
            /// Returns the number of pots in use
            /// </summary>
            public int GetNumPots()
            {
                return _pots.Count;
            }

            /// <summary>
            /// Returns the current total amount in the pot.
            /// </summary>
            /// <returns>The amount in the pot.</returns>
            public List<int> GetPotAmounts()
            {
                return _pots;
            }

            /// <summary>
            /// Resets the pots list to an empty state.
            /// </summary>
            public void ResetPots()
            {
                _pots.Clear();
            }

            /// <summary>
            /// if needed in future development
            /// </summary>
            /// <param name="spriteBatch">The SpriteBatch used to draw the pot display.</param>
            public void Draw(SpriteBatch spriteBatch)
            {
                //draw function if needed for front end design
            }
        }
    }
