using CardsCashCasino.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using static CardsCashCasino.Manager.TexasHoldEmManager;

namespace CardsCashCasino.NUnit
{
    [TestClass]
    public class HoldEmTests
    {
        public TexasHoldEmPotManager _potManager = new TexasHoldEmPotManager();
        public TexasHoldEmManager _holdEmManager = new TexasHoldEmManager();

        /// <summary>
        /// Testing InitializePot
        /// </summary>
        [TestMethod]
        public void InitializePot()
        {
            int _ante = 2;

            //list input of antes gathered
            List<int> playerBets = new List<int> { 2, 2, 2, 2 };

            _potManager.InitializePot(_ante, playerBets);

            Assert.AreEqual(1, _potManager.Pots.Count); // Ensure one pot is created
            Assert.AreEqual(PotType.MAIN, _potManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(_ante * playerBets.Count, _potManager.Pots[0].Total); // Verify the total
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, _potManager.Pots[0].EligiblePlayers); // Verify the list of players eligible for the pot
        }

        /// <summary>
        /// Testing AddFoldedBets
        /// </summary>
        [TestMethod]
        public void AddFoldedBets()
        {
            int _ante = 2;

            List<int> playerBets = new List<int> { 2, 2, 2, 2 };
            List<int> foldedBets = new List<int> { 1, 2 };

            _potManager.InitializePot(_ante, playerBets);
            _potManager.AddFoldedBetsToPot(foldedBets);

            Assert.AreEqual(1, _potManager.Pots.Count); // Ensure one pot is created
            Assert.AreEqual(8 + 1 + 2, _potManager.Pots[0].Total); // Verify the total
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, _potManager.Pots[0].EligiblePlayers); // Verify the list of players eligible for the pot
        }

        /// <summary>
        /// Testing GetPotAmounts()
        /// </summary>
        [TestMethod]
        public void GetPotAmounts()
        {
            int _ante = 2;

            List<int> playerBets = new List<int> { 1, 2, 2, 2 };

            _potManager.InitializePot(_ante, playerBets);

            Assert.AreEqual(2, _potManager.Pots.Count); // Ensure two pots are created
            Assert.AreEqual(PotType.MAIN, _potManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, _potManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(3, _potManager.Pots[0].Total); // Verify the total of the new main pot
            Assert.AreEqual(4, _potManager.Pots[1].Total); // Verify the total of the side pot that all-in player is limited to winning
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, _potManager.Pots[1].EligiblePlayers); // Verify everyone is eligible for the side pot
            CollectionAssert.AreEqual(new List<int> { 3, 4 }, _potManager.GetPotAmounts()); // Verify that the return of funds in the pot was successful
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, _potManager.Pots[0].EligiblePlayers); // Verify the list of players eligible for the new main pot
        }

        /// <summary>
        /// Testing basic Side Pot creation
        /// </summary>
        [TestMethod]
        public void CreateSidePot()
        {
            int _ante = 2;

            List<int> playerBets = new List<int> { 1, 2, 2, 2 };

            _potManager.InitializePot(_ante, playerBets);

            Assert.AreEqual(2, _potManager.Pots.Count); // Ensure two pots were created
            Assert.AreEqual(PotType.MAIN, _potManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, _potManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(3, _potManager.Pots[0].Total); // Verify the total of the new main pot
            Assert.AreEqual(4, _potManager.Pots[1].Total); // Verify the total of the side pot that all-in player can win
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, _potManager.Pots[1].EligiblePlayers); // Verify everyone is eligible for the side pot
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, _potManager.Pots[0].EligiblePlayers); // Verify the list of players eligible for the new main pot
        }

        /// <summary>
        /// Testing ability for CreateSidePots to deal with multiple all in bets in the same round
        /// </summary>
        [TestMethod]
        public void CreateSidePot_MultipleAllIns()
        {
            int _ante = 2;

            List<int> playerBets = new List<int> { 1, 1, 1, 2, 2 };

            _potManager.InitializePot(_ante, playerBets);

            Assert.AreEqual(2, _potManager.Pots.Count); // Ensure two pots were created
            Assert.AreEqual(PotType.MAIN, _potManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, _potManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(2, _potManager.Pots[0].Total); // Verify the total of the new main pot
            Assert.AreEqual(5, _potManager.Pots[1].Total); // Verify the total of the side pot that all-in player can win
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3, 4 }, _potManager.Pots[1].EligiblePlayers); // Verify everyone is eligible for the side pot
            CollectionAssert.AreEqual(new List<int> { 3, 4 }, _potManager.Pots[0].EligiblePlayers); // Verify the list of players eligible for the new main pot
        }


        /// <summary>
        /// Testing ability for CreateSidePots to create multiple side pots in one round
        /// </summary>
        [TestMethod]
        public void CreateMoreThanOneSidePots()
        {
            int _ante = 6;

            List<int> playerBets = new List<int> { 2, 4, 6, 6 };

            _potManager.InitializePot(_ante, playerBets);

            Assert.AreEqual(3, _potManager.Pots.Count); // Ensure three pots are created
            Assert.AreEqual(PotType.MAIN, _potManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, _potManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(PotType.SIDE, _potManager.Pots[2].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(4, _potManager.Pots[0].Total); // Verify the total of the new main pot
            Assert.AreEqual(6, _potManager.Pots[2].Total); // Verify the total of the side pot
            Assert.AreEqual(8, _potManager.Pots[1].Total); // Verify the total of the side pot
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, _potManager.Pots[1].EligiblePlayers); // Verify everyone is eligible for the first side pot
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, _potManager.Pots[2].EligiblePlayers); // Verify the list of players eligible for second side pot
            CollectionAssert.AreEqual(new List<int> { 2, 3 }, _potManager.Pots[0].EligiblePlayers); // Verify the list of players eligible for the new main pot
        }

        /// <summary>
        /// Testing condition where only one player is not all in, the excess amount the one player bet does not need to be added to the pot, it will be reimbursed
        /// </summary>
        [TestMethod]
        public void ComplexSidePots()
        {
            int _ante = 8;

            List<int> playerBets = new List<int> { 2, 4, 6, 8 };

            _potManager.InitializePot(_ante, playerBets);

            Assert.AreEqual(3, _potManager.Pots.Count); // Ensure three pots are created
            Assert.AreEqual(PotType.MAIN, _potManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, _potManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(PotType.SIDE, _potManager.Pots[2].PotType); // Verify the third pot is SIDE      
            Assert.AreEqual(2 + 2, _potManager.Pots[0].Total); // Verify the total of the side pot two players have contributed to
            Assert.AreEqual(6, _potManager.Pots[2].Total); // Verify the total of the side pot three players have contributed to
            Assert.AreEqual(8, _potManager.Pots[1].Total); // Verify the total of the side pot that all players have contrbuted to
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, _potManager.Pots[1].EligiblePlayers); // Verify everyone is eligible for the first side pot
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, _potManager.Pots[2].EligiblePlayers); // Verify the list of players eligible for second side pot
            CollectionAssert.AreEqual(new List<int> { 2, 3 }, _potManager.Pots[0].EligiblePlayers); // Verify the list of players eligible for the new main pot           
        }

        /// <summary>
        /// Testing for a single round of betting
        /// </summary>
        [TestMethod]
        public void FirstRoundOfBetting()
        {
            int _currentBet = 2;
            int _ante = 2;

            List<int> anteBets = new List<int> { 2, 2, 2, 2 };
            List<int> preflopBets = new List<int> { 2, 2, 2, 2 };

            _potManager.InitializePot(_ante, anteBets);
            _potManager.AddToPot(_currentBet, preflopBets);

            Assert.AreEqual(1, _potManager.Pots.Count); // Ensure one pot is created
            Assert.AreEqual(PotType.MAIN, _potManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(16, _potManager.Pots[0].Total); // Verify the total
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, _potManager.Pots[0].EligiblePlayers); // Verify the list of players eligible for the pot
        }

        /// <summary>
        /// Testing for a single round of betting when two players fold immediately 
        /// </summary>
        [TestMethod]
        public void FirstRoundOfBetting_TwoFolds()
        {
            int _currentBet = 2;
            int _ante = 2;

            List<int> anteBets = new List<int> { 2, 2, 2, 2 };
            List<int> preflopBets = new List<int> { 2, 2 };

            _potManager.InitializePot(_ante, anteBets);
            _potManager.RemoveFoldedPlayers(2);
            _potManager.RemoveFoldedPlayers(0);
            _potManager.AddToPot(_currentBet, preflopBets);

            Assert.AreEqual(1, _potManager.Pots.Count); // Ensure one pot is created
            Assert.AreEqual(PotType.MAIN, _potManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(12, _potManager.Pots[0].Total); // Verify the total
            CollectionAssert.AreEqual(new List<int> { 1, 3 }, _potManager.Pots[0].EligiblePlayers); // Verify the list of players eligible for the pot
        }

        /// <summary>
        /// Testing a complex scenerio where 1 player goes all in on the ante, one player folds in the round
        /// </summary>
        [TestMethod]
        public void FirstRoundofBetting_OneSidePotOnAnte_OneFold()
        {
            int _currentBet = 2;
            int _ante = 2;

            List<int> anteBets = new List<int> { 1, 2, 2, 2 };
            List<int> preflopBets = new List<int> { 2, 2 };

            _potManager.InitializePot(_ante, anteBets);
            _potManager.RemoveFoldedPlayers(2); // index of folded player, this player will be remove from eligibility to win any pot
            _potManager.AddToPot(_currentBet, preflopBets);

            Assert.AreEqual(2, _potManager.Pots.Count); // Ensure two pots were created
            Assert.AreEqual(PotType.MAIN, _potManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, _potManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(7, _potManager.Pots[0].Total); // Verify the total of the new main pot
            Assert.AreEqual(4, _potManager.Pots[1].Total); // Verify the total of the side pot that all-in player can win
            CollectionAssert.AreEqual(new List<int> { 0, 1, 3 }, _potManager.Pots[1].EligiblePlayers); // Verify everyone is eligible for the side pot
            CollectionAssert.AreEqual(new List<int> { 1, 3 }, _potManager.Pots[0].EligiblePlayers); // Verify the list of players eligible for the new main pot
        }

        /// <summary>
        /// Testing all functions being called in a single round of Texas Hold em (except ResetPot())
        /// </summary>
        [TestMethod]
        public void FullHoldemGame()
        {
            int _preflopCurrentBet = 2;
            int _ante = 2;
            int _flopCurrentBet = 4; //equal to highest bet placed in round
            int _turnCurrentBet = 0;
            int _riverCurrentBet = 2;
            int foldedPlayerIndex = 2; // index of player that is folding
            int mainPotWinners = 1; // how many players won the pots
            int sidePotWinners = 2;

            //lists of bets placed each round
            List<int> anteBets = new List<int> { 1, 2, 2, 2 };
            List<int> preflopBets = new List<int> { 2, 2, 2};
            List<int> foldedBets = new List<int> { 1 };
            List<int> flopBets = new List<int> { 4, 4 };
            List<int> turnBets = new List<int> { 0, 0 };
            List<int> riverBets = new List<int> { 2, 2};

            //running flow of code for a simulated round of texas hold em
            _potManager.InitializePot(_ante, anteBets); //pot initialized, 1 player all-in
            _potManager.AddToPot(_preflopCurrentBet, preflopBets); //Add preflop bets to pot
            _potManager.RemoveFoldedPlayers(foldedPlayerIndex); //player bets, then is raised, folds after raise
            _potManager.AddFoldedBetsToPot(foldedBets); //all bets are in, folded player's bet added to the pot
            _potManager.AddToPot(_flopCurrentBet, flopBets); //add postflop bets to the pot
            _potManager.AddToPot(_turnCurrentBet, turnBets); //both players check, no bets added to pot
            _potManager.AddToPot(_riverCurrentBet, riverBets); //post river bets added to the pot

            Assert.AreEqual(2, _potManager.Pots.Count); // Ensure two pots were created in the round
            Assert.AreEqual(PotType.MAIN, _potManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, _potManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(22, _potManager.Pots[0].Total); // Verify the total of the new main pot
            Assert.AreEqual(4, _potManager.Pots[1].Total); // Verify the total of the side pot that all-in player can win
            CollectionAssert.AreEqual(new List<int> { 0, 1, 3 }, _potManager.Pots[1].EligiblePlayers); // Verify everyone except the folded player at index 2 is eligible to win the side pot
            CollectionAssert.AreEqual(new List<int> { 22, 4 }, _potManager.GetPotAmounts()); // Check if the attempt to return the pot totals for a graphics update were successful
            CollectionAssert.AreEqual(new List<int> { 1, 3 }, _potManager.Pots[0].EligiblePlayers); // Verify the list of players eligible for the new main pot
            Assert.AreEqual(2, _potManager.DistributePot(sidePotWinners, 1)); //side pot's total is distributed amongst two players
            Assert.AreEqual(22, _potManager.DistributePot(mainPotWinners, 0)); //main pot's total is won by only one player
        }

        /// <summary>
        /// Testing ResetPots() after a full round of texas hold em
        /// </summary>
        [TestMethod]
        public void FullHoldemGame_WithReset()
        {
            int _preflopCurrentBet = 2;
            int _ante = 2;
            int _flopCurrentBet = 4;
            int _turnCurrentBet = 0;
            int _riverCurrentBet = 2;
            int foldedPlayerIndex = 2;

            List<int> anteBets = new List<int> { 1, 2, 2, 2 };
            List<int> preflopBets = new List<int> { 2, 2, 2 };
            List<int> foldedBets = new List<int> { 1 };
            List<int> flopBets = new List<int> { 4, 4 };
            List<int> turnBets = new List<int> { 0, 0 };
            List<int> riverBets = new List<int> { 2, 2 };

            _potManager.InitializePot(_ante, anteBets);
            _potManager.AddToPot(_preflopCurrentBet, preflopBets);
            _potManager.AddFoldedBetsToPot(foldedBets);
            _potManager.RemoveFoldedPlayers(foldedPlayerIndex);
            _potManager.AddToPot(_flopCurrentBet, flopBets);
            _potManager.AddToPot(_turnCurrentBet, turnBets);
            _potManager.AddToPot(_riverCurrentBet, riverBets);
            _potManager.ResetPots();

            Assert.AreEqual(0, _potManager.Pots.Count); //see if created pots were deleted successfully
        }
    }
}
