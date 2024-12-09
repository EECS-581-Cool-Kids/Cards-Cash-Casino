/*
 *  Module Name: BlackjackTests.cs
 *  Purpose: Unit tests for blackjack.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Jacob Wilkus
 *  Date: 11/20/2024
 *  Last Modified: 11/20/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */
using CardsCashCasino.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.NUnit
{
    [TestClass]
    public class BlackjackTests
    {
        /// <summary>
        /// The blackjack manager.
        /// </summary>
        private BlackjackManager _blackjackManager = new BlackjackManager();

        /// <summary>
        /// The betting manager.
        /// </summary>
        private BettingManager _bettingManager = new BettingManager();

        /// <summary>
        /// The card manager.
        /// </summary>
        private CardManager _cardManager = new CardManager();

        /// <summary>
        /// The constructor.
        /// </summary>
        public BlackjackTests()
        {
            _blackjackManager.RequestCardManagerCleared = _cardManager.ClearDecks;
            _blackjackManager.RequestDecksOfCards = _cardManager.GenerateDecks;
            _blackjackManager.RequestCard = _cardManager.DrawCard;
            _blackjackManager.RequestBet = _bettingManager.Bet;
            _blackjackManager.RequestPayout = _bettingManager.Payout;

            _blackjackManager.LoadContent();
            _bettingManager.LoadContent();
        }

        /// <summary>
        /// The test for double down.
        /// </summary>
        [TestMethod]
        public void TestDoubleDown()
        {
            // Start the game and bet 50.
            _blackjackManager.StartGameWithoutUI(50);
            _bettingManager.ConfirmBetWithoutUI(50);

            int prevBet = 0 + BettingManager.UserBet; // get the previous bet.

            _blackjackManager.CallActionWithoutUI(2); // call double down.

            Assert.AreEqual(prevBet * 2, BettingManager.UserBet); // check if the bet is doubled.
        }
    }
}
