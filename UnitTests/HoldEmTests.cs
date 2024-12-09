/*
 *  Module Name: HoldEmTests.cs
 *  Purpose: Unit tests for Texas Hold Em.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Jacob Wilkus, Derek Norton, Mo Morgan
 *  Date: 11/20/2024
 *  Last Modified: 12/2/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */
using CardsCashCasino.Manager;

namespace CardsCashCasino.NUnit
{
    [TestClass]
    public class HoldEmTests
    {
        /// <summary>
        /// The pot manager.
        /// </summary>
        private PotManager PotManager = new PotManager();

        /// <summary>
        /// The hold em manager.
        /// </summary>
        public TexasHoldEmManager HoldEmManager = new TexasHoldEmManager();

        /// <summary>
        /// The player manager.
        /// </summary>
        private PlayerManager Players = new PlayerManager();

        /// <summary>
        /// Testing InitializePot
        /// </summary>
        [TestMethod]
        public void InitializePot()
        {
            int ante = 2; // ante amount

            //list input of antes gathered
            List<int> playerBets = new List<int> { 2, 2, 2, 2 };

            PotManager.InitializePot(ante, playerBets); // Initialize the pot

            Assert.AreEqual(1, PotManager.Pots.Count); // Ensure one pot is created
            Assert.AreEqual(PotType.MAIN, PotManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(ante * playerBets.Count, PotManager.Pots[0].Total); // Verify the total
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, PotManager.Pots[0].EligiblePlayers); // Verify the list of Players eligible for the pot
        }

        /// <summary>
        /// Testing AddFoldedBets
        /// </summary>
        [TestMethod]
        public void AddFoldedBets()
        {
            int ante = 2; // ante amount

            List<int> playerBets = new List<int> { 2, 2, 2, 2 }; // list input of antes gathered
            List<int> foldedBets = new List<int> { 1, 2 }; // list of bets from folded players

            PotManager.InitializePot(ante, playerBets); // Initialize the pot
            PotManager.AddFoldedBetsToPot(foldedBets); // Add the folded bets to the pot

            Assert.AreEqual(1, PotManager.Pots.Count); // Ensure one pot is created
            Assert.AreEqual(8 + 1 + 2, PotManager.Pots[0].Total); // Verify the total
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, PotManager.Pots[0].EligiblePlayers); // Verify the list of Players eligible for the pot
        }

        /// <summary>
        /// Testing GetPotAmounts()
        /// </summary>
        [TestMethod]
        public void GetPotAmounts()
        {
            int ante = 2; // ante amount

            List<int> playerBets = new List<int> { 1, 2, 2, 2 }; // list input of antes gathered

            PotManager.InitializePot(ante, playerBets); // Initialize the pot

            Assert.AreEqual(2, PotManager.Pots.Count); // Ensure two pots are created
            Assert.AreEqual(PotType.MAIN, PotManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, PotManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(3, PotManager.Pots[0].Total); // Verify the total of the new main pot
            Assert.AreEqual(4, PotManager.Pots[1].Total); // Verify the total of the side pot that all-in player is limited to winning
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, PotManager.Pots[1].EligiblePlayers); // Verify everyone is eligible for the side pot
            CollectionAssert.AreEqual(new List<int> { 3, 4 }, PotManager.GetPotAmounts()); // Verify that the return of funds in the pot was successful
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, PotManager.Pots[0].EligiblePlayers); // Verify the list of Players eligible for the new main pot
        }

        /// <summary>
        /// Testing basic Side Pot creation
        /// </summary>
        [TestMethod]
        public void CreateSidePot()
        {
            int ante = 2; // ante amount

            List<int> playerBets = new List<int> { 1, 2, 2, 2 }; // list input of antes gathered

            PotManager.InitializePot(ante, playerBets); // Initialize the pot

            Assert.AreEqual(2, PotManager.Pots.Count); // Ensure two pots were created
            Assert.AreEqual(PotType.MAIN, PotManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, PotManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(3, PotManager.Pots[0].Total); // Verify the total of the new main pot
            Assert.AreEqual(4, PotManager.Pots[1].Total); // Verify the total of the side pot that all-in player can win
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, PotManager.Pots[1].EligiblePlayers); // Verify everyone is eligible for the side pot
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, PotManager.Pots[0].EligiblePlayers); // Verify the list of Players eligible for the new main pot
        }

        /// <summary>
        /// Testing ability for CreateSidePots to deal with multiple all in bets in the same round
        /// </summary>
        [TestMethod]
        public void CreateSidePot_MultipleAllIns()
        {
            int ante = 2; // ante amount

            List<int> playerBets = new List<int> { 1, 1, 1, 2, 2 }; // list input of antes gathered

            PotManager.InitializePot(ante, playerBets); // Initialize the pot

            Assert.AreEqual(2, PotManager.Pots.Count); // Ensure two pots were created
            Assert.AreEqual(PotType.MAIN, PotManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, PotManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(2, PotManager.Pots[0].Total); // Verify the total of the new main pot
            Assert.AreEqual(5, PotManager.Pots[1].Total); // Verify the total of the side pot that all-in player can win
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3, 4 }, PotManager.Pots[1].EligiblePlayers); // Verify everyone is eligible for the side pot
            CollectionAssert.AreEqual(new List<int> { 3, 4 }, PotManager.Pots[0].EligiblePlayers); // Verify the list of Players eligible for the new main pot
        }


        /// <summary>
        /// Testing ability for CreateSidePots to create multiple side pots in one round
        /// </summary>
        [TestMethod]
        public void CreateMoreThanOneSidePots()
        {
            int ante = 6; // ante amount

            List<int> playerBets = new List<int> { 2, 4, 6, 6 }; // list input of antes gathered

            PotManager.InitializePot(ante, playerBets); // Initialize the pot

            Assert.AreEqual(3, PotManager.Pots.Count); // Ensure three pots are created
            Assert.AreEqual(PotType.MAIN, PotManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, PotManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(PotType.SIDE, PotManager.Pots[2].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(4, PotManager.Pots[0].Total); // Verify the total of the new main pot
            Assert.AreEqual(6, PotManager.Pots[2].Total); // Verify the total of the side pot
            Assert.AreEqual(8, PotManager.Pots[1].Total); // Verify the total of the side pot
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, PotManager.Pots[1].EligiblePlayers); // Verify everyone is eligible for the first side pot
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, PotManager.Pots[2].EligiblePlayers); // Verify the list of Players eligible for second side pot
            CollectionAssert.AreEqual(new List<int> { 2, 3 }, PotManager.Pots[0].EligiblePlayers); // Verify the list of Players eligible for the new main pot
        }

        /// <summary>
        /// Testing condition where only one player is not all in, the excess amount the one player bet does not need to be added to the pot, it will be reimbursed
        /// </summary>
        [TestMethod]
        public void ComplexSidePots()
        {
            int ante = 8; // ante amount

            List<int> playerBets = new List<int> { 2, 4, 6, 6 }; // list input of antes gathered

            PotManager.InitializePot(ante, playerBets); // Initialize the pot

            Assert.AreEqual(3, PotManager.Pots.Count); // Ensure three pots are created
            Assert.AreEqual(PotType.MAIN, PotManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, PotManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(PotType.SIDE, PotManager.Pots[2].PotType); // Verify the third pot is SIDE      
            Assert.AreEqual(2 + 2, PotManager.Pots[0].Total); // Verify the total of the side pot two Players have contributed to
            Assert.AreEqual(6, PotManager.Pots[2].Total); // Verify the total of the side pot three Players have contributed to
            Assert.AreEqual(8, PotManager.Pots[1].Total); // Verify the total of the side pot that all Players have contrbuted to
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, PotManager.Pots[1].EligiblePlayers); // Verify everyone is eligible for the first side pot
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, PotManager.Pots[2].EligiblePlayers); // Verify the list of Players eligible for second side pot
            CollectionAssert.AreEqual(new List<int> { 2, 3 }, PotManager.Pots[0].EligiblePlayers); // Verify the list of Players eligible for the new main pot           
        }

        /// <summary>
        /// Testing for a single round of betting
        /// </summary>
        [TestMethod]
        public void FirstRoundOfBetting()
        {
            int currentBet = 2; // current bet amount
            int ante = 2; // ante amount

            List<int> anteBets = new List<int> { 2, 2, 2, 2 }; // list input of antes gathered
            List<int> preflopBets = new List<int> { 2, 2, 2, 2 }; // list of bets placed in the preflop round

            PotManager.InitializePot(ante, anteBets); // Initialize the pot
            PotManager.AddToPot(currentBet, preflopBets);  // Add the preflop bets to the pot

            Assert.AreEqual(1, PotManager.Pots.Count); // Ensure one pot is created
            Assert.AreEqual(PotType.MAIN, PotManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(16, PotManager.Pots[0].Total); // Verify the total
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, PotManager.Pots[0].EligiblePlayers); // Verify the list of Players eligible for the pot
        }

        /// <summary>
        /// Testing for a single round of betting when two Players fold immediately 
        /// </summary>
        [TestMethod]
        public void FirstRoundOfBetting_TwoFolds()
        {
            int currentBet = 2; // current bet amount
            int ante = 2; // ante amount

            List<int> anteBets = new List<int> { 2, 2, 2, 2 }; // list input of antes gathered
            List<int> preflopBets = new List<int> { 2, 2 }; // list of bets placed in the preflop round

            PotManager.InitializePot(ante, anteBets); // Initialize the pot
            PotManager.RemoveFoldedPlayers(2); // index of folded player, this player will be remove from eligibility to win any pot
            PotManager.RemoveFoldedPlayers(0); // index of folded player, this player will be remove from eligibility to win any pot
            PotManager.AddToPot(currentBet, preflopBets); // Add the preflop bets to the pot

            Assert.AreEqual(1, PotManager.Pots.Count); // Ensure one pot is created
            Assert.AreEqual(PotType.MAIN, PotManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(12, PotManager.Pots[0].Total); // Verify the total
            CollectionAssert.AreEqual(new List<int> { 1, 3 }, PotManager.Pots[0].EligiblePlayers); // Verify the list of Players eligible for the pot
        }

        /// <summary>
        /// Testing a complex scenerio where 1 player goes all in on the ante, one player folds in the round
        /// </summary>
        [TestMethod]
        public void FirstRoundofBetting_OneSidePotOnAnte_OneFold()
        {
            int currentBet = 2; // current bet amount
            int ante = 2; // ante amount

            List<int> anteBets = new List<int> { 1, 2, 2, 2 }; // list input of antes gathered
            List<int> preflopBets = new List<int> { 2, 2 }; // list of bets placed in the preflop round

            PotManager.InitializePot(ante, anteBets); // Initialize the pot
            PotManager.RemoveFoldedPlayers(2); // index of folded player, this player will be remove from eligibility to win any pot
            PotManager.AddToPot(currentBet, preflopBets); // Add the preflop bets to the pot

            Assert.AreEqual(2, PotManager.Pots.Count); // Ensure two pots were created
            Assert.AreEqual(PotType.MAIN, PotManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, PotManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(7, PotManager.Pots[0].Total); // Verify the total of the new main pot
            Assert.AreEqual(4, PotManager.Pots[1].Total); // Verify the total of the side pot that all-in player can win
            CollectionAssert.AreEqual(new List<int> { 0, 1, 3 }, PotManager.Pots[1].EligiblePlayers); // Verify everyone is eligible for the side pot
            CollectionAssert.AreEqual(new List<int> { 1, 3 }, PotManager.Pots[0].EligiblePlayers); // Verify the list of Players eligible for the new main pot
        }

        /// <summary>
        /// Testing all functions being called in a single round of Texas Hold em (except ResetPot())
        /// </summary>
        [TestMethod]
        public void FullHoldemGame()
        {
            int _preflopCurrentBet = 2; // current bet amount
            int ante = 2; // ante amount
            int _flopCurrentBet = 4; //equal to highest bet placed in round
            int _turnCurrentBet = 0; // no bets placed in round
            int _riverCurrentBet = 2; // equal to highest bet placed in round
            int foldedPlayerIndex = 2; // index of player that is folding
            int mainPotWinners = 1; // how many Players won the pots
            int sidePotWinners = 2; // how many Players won the pots

            //lists of bets placed each round
            List<int> anteBets = new List<int> { 1, 2, 2, 2 };
            List<int> preflopBets = new List<int> { 2, 2, 2 };
            List<int> foldedBets = new List<int> { 1 };
            List<int> flopBets = new List<int> { 4, 4 };
            List<int> turnBets = new List<int> { 0, 0 };
            List<int> riverBets = new List<int> { 2, 2 };

            //running flow of code for a simulated round of texas hold em
            PotManager.InitializePot(ante, anteBets); //pot initialized, 1 player all-in
            PotManager.AddToPot(_preflopCurrentBet, preflopBets); //Add preflop bets to pot
            PotManager.RemoveFoldedPlayers(foldedPlayerIndex); //player bets, then is raised, folds after raise
            PotManager.AddFoldedBetsToPot(foldedBets); //all bets are in, folded player's bet added to the pot
            PotManager.AddToPot(_flopCurrentBet, flopBets); //add postflop bets to the pot
            PotManager.AddToPot(_turnCurrentBet, turnBets); //both Players check, no bets added to pot
            PotManager.AddToPot(_riverCurrentBet, riverBets); //post river bets added to the pot

            Assert.AreEqual(2, PotManager.Pots.Count); // Ensure two pots were created in the round
            Assert.AreEqual(PotType.MAIN, PotManager.Pots[0].PotType); // Check the pot type is MAIN
            Assert.AreEqual(PotType.SIDE, PotManager.Pots[1].PotType); // Verify the second pot is SIDE
            Assert.AreEqual(22, PotManager.Pots[0].Total); // Verify the total of the new main pot
            Assert.AreEqual(4, PotManager.Pots[1].Total); // Verify the total of the side pot that all-in player can win
            CollectionAssert.AreEqual(new List<int> { 0, 1, 3 }, PotManager.Pots[1].EligiblePlayers); // Verify everyone except the folded player at index 2 is eligible to win the side pot
            CollectionAssert.AreEqual(new List<int> { 22, 4 }, PotManager.GetPotAmounts()); // Check if the attempt to return the pot totals for a graphics update were successful
            CollectionAssert.AreEqual(new List<int> { 1, 3 }, PotManager.Pots[0].EligiblePlayers); // Verify the list of Players eligible for the new main pot
            Assert.AreEqual(2, PotManager.DistributePot(sidePotWinners, 1)); //side pot's total is distributed amongst two Players
            Assert.AreEqual(22, PotManager.DistributePot(mainPotWinners, 0)); //main pot's total is won by only one player
        }
    }
}
