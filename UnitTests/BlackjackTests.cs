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
        private BlackjackManager _blackjackManager = new BlackjackManager();

        private BettingManager _bettingManager = new BettingManager();

        private CardManager _cardManager = new CardManager();

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

        [TestMethod]
        public void TestDoubleDown()
        {
            _blackjackManager.StartGameWithoutUI(50);
            _bettingManager.ConfirmBetWithoutUI(50);

            int prevBet = 0 + BettingManager.UserBet;

            _blackjackManager.CallActionWithoutUI(2); // call double down.

            Assert.AreEqual(prevBet * 2, BettingManager.UserBet);
        }
    }
}
