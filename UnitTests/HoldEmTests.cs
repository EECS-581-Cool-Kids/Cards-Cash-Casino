using CardsCashCasino.Manager;

namespace CardsCashCasino.NUnit
{
    [TestClass]
    public class HoldEmTests
    {
        private TexasHoldEmPotManager PotManager = new TexasHoldEmPotManager();
        public TexasHoldEmManager HoldEmManager = new TexasHoldEmManager();
        private HoldEmPlayerManager Players = new ();

        /// <summary>
        /// Testing InitializePot
        /// </summary>
        [TestMethod]
        public void InitializePot()
        {
            int ante = 2;

            //list input of antes gathered
            List<int> playerBets = new List<int> { 2, 2, 2, 2 };

            PotManager.InitializePot(ante, playerBets);

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
            int ante = 2;

            List<int> playerBets = new List<int> { 2, 2, 2, 2 };
            List<int> foldedBets = new List<int> { 1, 2 };

            PotManager.InitializePot(ante, playerBets);
            PotManager.AddFoldedBetsToPot(foldedBets);

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
            int ante = 2;

            List<int> playerBets = new List<int> { 1, 2, 2, 2 };

            PotManager.InitializePot(ante, playerBets);

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
            int ante = 2;

            List<int> playerBets = new List<int> { 1, 2, 2, 2 };

            PotManager.InitializePot(ante, playerBets);

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
            int ante = 2;

            List<int> playerBets = new List<int> { 1, 1, 1, 2, 2 };

            PotManager.InitializePot(ante, playerBets);

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
            int ante = 6;

            List<int> playerBets = new List<int> { 2, 4, 6, 6 };

            PotManager.InitializePot(ante, playerBets);

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
            int ante = 8;

            List<int> playerBets = new List<int> { 2, 4, 6, 6 };

            PotManager.InitializePot(ante, playerBets);

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
            int currentBet = 2;
            int ante = 2;

            List<int> anteBets = new List<int> { 2, 2, 2, 2 };
            List<int> preflopBets = new List<int> { 2, 2, 2, 2 };

            PotManager.InitializePot(ante, anteBets);
            PotManager.AddToPot(currentBet, preflopBets);

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
            int currentBet = 2;
            int ante = 2;

            List<int> anteBets = new List<int> { 2, 2, 2, 2 };
            List<int> preflopBets = new List<int> { 2, 2 };

            PotManager.InitializePot(ante, anteBets);
            PotManager.RemoveFoldedPlayers(2);
            PotManager.RemoveFoldedPlayers(0);
            PotManager.AddToPot(currentBet, preflopBets);

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
            int currentBet = 2;
            int ante = 2;

            List<int> anteBets = new List<int> { 1, 2, 2, 2 };
            List<int> preflopBets = new List<int> { 2, 2 };

            PotManager.InitializePot(ante, anteBets);
            PotManager.RemoveFoldedPlayers(2); // index of folded player, this player will be remove from eligibility to win any pot
            PotManager.AddToPot(currentBet, preflopBets);

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
            int _preflopCurrentBet = 2;
            int ante = 2;
            int _flopCurrentBet = 4; //equal to highest bet placed in round
            int _turnCurrentBet = 0;
            int _riverCurrentBet = 2;
            int foldedPlayerIndex = 2; // index of player that is folding
            int mainPotWinners = 1; // how many Players won the pots
            int sidePotWinners = 2;

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

        /// <summary>
        /// Testing ResetPots() after a full round of texas hold em
        /// </summary>
        [TestMethod]
        public void FullHoldemGame_WithReset()
        {
            int _preflopCurrentBet = 2;
            int ante = 2;
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

            PotManager.InitializePot(ante, anteBets);
            PotManager.AddToPot(_preflopCurrentBet, preflopBets);
            PotManager.AddFoldedBetsToPot(foldedBets);
            PotManager.RemoveFoldedPlayers(foldedPlayerIndex);
            PotManager.AddToPot(_flopCurrentBet, flopBets);
            PotManager.AddToPot(_turnCurrentBet, turnBets);
            PotManager.AddToPot(_riverCurrentBet, riverBets);
            PotManager.ResetPots();

            Assert.AreEqual(0, PotManager.Pots.Count); //see if created pots were deleted successfully
        }
        /// <summary>
        /// Testing method to initiate Players
        /// </summary>
        [TestMethod]
        public void InitiatePlayers()
        {
            int numAIPlayers = 3;

            Players.InitiatePlayers(numAIPlayers);

            int dealerIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER);

            Assert.AreEqual(4, Players.Players.Count); // Ensure four Players were created in the round
            Assert.AreEqual(PlayerType.USER, Players.Players[0].PlayerType);
            Assert.AreEqual(PlayerType.AI, Players.Players[1].PlayerType);
            Assert.AreEqual(PlayerType.AI, Players.Players[2].PlayerType);
            Assert.AreEqual(PlayerType.AI, Players.Players[3].PlayerType);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[0].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[1].PlayerStatus);
            Assert.AreEqual(500, Players.Players[0].PlayerStack);
            Assert.AreEqual(500, Players.Players[1].PlayerStack);
            Assert.AreEqual(0, Players.Players[0].PlayerBet);
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));
            Assert.AreEqual((dealerIndex + 3) % 4, Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(dealerIndex, Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual((dealerIndex + 2) % 4, Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND));
            Assert.AreEqual((dealerIndex + 1) % 4, Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));

        }

        /// <summary>
        /// Testing the method to shift the blinds one position to the left at the end of a round
        /// </summary>
        [TestMethod]
        public void SetNextRoundBlinds()
        {
            int numAIPlayers = 3;

            Players.InitiatePlayers(numAIPlayers);

            int firstNoneIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE);
            int firstDealerIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER);
            int firstSmallBlindIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            int firstBigBlindIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);

            Players.SetNextRoundBlinds();

            Assert.AreEqual(4, Players.Players.Count); // Ensure four Players were created in the round
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));
            Assert.AreEqual((firstNoneIndex + 1) % 4, Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual((firstDealerIndex + 1) % 4, Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual((firstBigBlindIndex + 1) % 4, Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND));
            Assert.AreEqual((firstSmallBlindIndex + 1) % 4, Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
        }

        /// <summary>
        /// Testing method for getting the first bettor on the preflop round (always player to the left of big blind
        /// </summary>
        [TestMethod]
        public void GetPreflopStartingIndex()
        {
            int numAIPlayers = 3;

            Players.InitiatePlayers(numAIPlayers);
            int startingBettorIndex = Players.GetPreflopStartingBettor();

            Assert.AreEqual(4, Players.Players.Count); // Ensure four Players were created in the round
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));
            Assert.AreEqual(startingBettorIndex, Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE));
        }

        /// <summary>
        /// Testing the method for getting the first bettor on any round after the preflop
        /// </summary>
        [TestMethod]
        public void GetStartingBettorIndex()
        {
            int numAIPlayers = 3;

            Players.InitiatePlayers(numAIPlayers);
            int startingBettorIndex = Players.GetStartingBettorIndex();

            Assert.AreEqual(4, Players.Players.Count); // Ensure four Players were created in the round
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));
            Assert.AreEqual(startingBettorIndex, Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
        }

        /// <summary>
        /// Testing preflop with an all in at blind collection
        /// </summary>
        [TestMethod]
        public void GetStartingBettorIndex_OneFoldOneAllIn()
        {
            int numAIPlayers = 3;

            Players.InitiatePlayers(numAIPlayers);
                
            //applying conditions for the test
            int bigBlindIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);
            int smallBlindIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            Players.Players[smallBlindIndex].PlayerStatus = PlayerStatus.FOLDED;
            Players.Players[bigBlindIndex].PlayerStatus = PlayerStatus.ALLIN;
            int startingBettorIndex = Players.GetStartingBettorIndex();

            Assert.AreEqual(4, Players.Players.Count); // Ensure four Players were created in the round
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));

            //typically small blind gets first bet, when not eligible to bet, the first player to the small blind's left that is eligible gets the first bet
            Assert.AreEqual(startingBettorIndex, Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE));
        }

        /// <summary>
        /// Testing blind collection method
        /// </summary>
        [TestMethod]
        public void CollectBlinds()
        {
            int numAIPlayers = 3;
            int smallBlindAmount = 1;
            int bigBlindAmount = 2;

            Players.InitiatePlayers(numAIPlayers);

            //applying conditions for the test
            int bigBlindIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);
            int smallBlindIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            Players.CollectBlinds(smallBlindAmount, bigBlindAmount);

            Assert.AreEqual(4, Players.Players.Count); // Ensure four Players were created in the round
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));

            //assure blind wagers were processed properly
            Assert.AreEqual(499, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(1, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(2, Players.Players[bigBlindIndex].PlayerBet);
        }

        [TestMethod]
        public void CollectBlinds_TwoFolds_SmallBlindCalls_BigChecks_AddToPot()
        {
            int numAIPlayers = 3;
            int smallBlindAmount = 1;
            int bigBlindAmount = 2;
            int currentBet = 2;
            int ante = 2;

            Players.InitiatePlayers(numAIPlayers);

            Assert.AreEqual(4, Players.Players.Count); // Ensure four Players were created in the round
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));

            int bigBlindIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);
            int smallBlindIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            int NoneIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE);
            int DealerIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER);

            //add antes
            Players.GenerateAntes(ante);
            PotManager.InitializePot(ante, Players.PackageBets());

            //assure antes were processed properly
            Assert.AreEqual(498, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(2, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(2, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(2, Players.Players[DealerIndex].PlayerBet);
            Assert.AreEqual(2, Players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(4, Players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> { 2, 2, 2, 2 }, Players.PackageBets()); // Verify the list of Players eligible for the pot
            Assert.AreEqual(8, PotManager.Pots[0].Total); // Verify the total

            Players.ResetBets();

            Assert.AreEqual(0, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[DealerIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[NoneIndex].PlayerBet);

            //applying conditions for the test
            Players.CollectBlinds(smallBlindAmount, bigBlindAmount);
            Players.Fold(NoneIndex);
            PotManager.RemoveFoldedPlayers(NoneIndex);
            Players.Fold(DealerIndex);
            PotManager.RemoveFoldedPlayers(DealerIndex);
            Assert.AreEqual(1, Players.Players[smallBlindIndex].PlayerBet);
            Players.Call(currentBet, smallBlindIndex);

            //assure blind wagers were processed properly
            Assert.AreEqual(496, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(496, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(2, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(2, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(2, Players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> { 2, 2 }, Players.PackageBets()); // Verify the list of Players eligible for the pot
            PotManager.AddFoldedBetsToPot(Players.PackageFoldedBets());
            PotManager.AddToPot(currentBet, Players.PackageBets());
            Players.ResetBets();

            //assure blind wagers were processed properly
            Assert.AreEqual(496, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(496, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(0, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(12, PotManager.Pots[0].Total); // Verify the total

            Assert.IsTrue(
                PotManager.PlayersEligible(0).SequenceEqual(new List<int> { smallBlindIndex, bigBlindIndex }) ||
                PotManager.PlayersEligible(0).SequenceEqual(new List<int> { bigBlindIndex, smallBlindIndex }),
                "Wrong list of eligible Players"
            ); // Verify the list of Players eligible for the pot
        }

        /// <summary>
        /// Complex test for a full round of poker with All-Ins and elimination
        /// </summary>
        [TestMethod]
        public void FullRoundOfPoker()
        {
            int numAIPlayers = 3;
            int smallBlindAmount = 1;
            int bigBlindAmount = 2;
            int currentBet = 2;
            int ante = 2;

            Players.InitiatePlayers(numAIPlayers);

            Assert.AreEqual(4, Players.Players.Count); // Ensure four Players were created in the round
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, Players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));

            int bigBlindIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);
            int smallBlindIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            int NoneIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE);
            int DealerIndex = Players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER);

            //add antes
            Players.GenerateAntes(ante);
            PotManager.InitializePot(ante, Players.PackageBets());

            //assure antes were processed properly
            Assert.AreEqual(498, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(2, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(2, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(2, Players.Players[DealerIndex].PlayerBet);
            Assert.AreEqual(2, Players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(4, Players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> { 2, 2, 2, 2 }, Players.PackageBets()); // Verify the list of Players eligible for the pot
            Assert.AreEqual(8, PotManager.Pots[0].Total); // Verify the total

            Players.ResetBets();

            Assert.AreEqual(0, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[DealerIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[NoneIndex].PlayerBet);
            
            Players.CollectBlinds(smallBlindAmount, bigBlindAmount);
            Players.Fold(DealerIndex);
            PotManager.RemoveFoldedPlayers(DealerIndex);
            Assert.AreEqual(1, Players.Players[smallBlindIndex].PlayerBet);
            Assert.IsTrue(!Players.AdvanceRound(), "Round should not be advanced yet");
            Players.Call(currentBet, NoneIndex);
            Assert.IsTrue(!Players.AdvanceRound(), "Round should not be advanced yet");
            Players.Call(currentBet, smallBlindIndex);
            Assert.IsTrue(!Players.AdvanceRound(), "Round should not be advanced yet");
            Players.Check(bigBlindIndex);
            Assert.IsTrue(Players.AdvanceRound(), "Round should be ready to advance");

            //assure blind wagers were processed properly
            Assert.AreEqual(496, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(496, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(496, Players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(2, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(2, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(2, Players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(3, Players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> {2, 2, 2 }, Players.PackageBets()); // Verify the list of Players eligible for the pot
            PotManager.AddFoldedBetsToPot(Players.PackageFoldedBets());
            PotManager.AddToPot(currentBet, Players.PackageBets());
            currentBet = 0;
            Players.ResetBets();

            //assure blind wagers were processed properly
            Assert.AreEqual(496, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(496, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(496, Players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(0, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(14, PotManager.Pots[0].Total); // Verify the total
            
            // Betting for Flop Round
            
            int startingBettor = Players.GetStartingBettorIndex();
            Assert.AreEqual(smallBlindIndex, startingBettor);
            Players.Check(smallBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(bigBlindIndex, (startingBettor + 1) % Players.Players.Count);
            Players.Check(bigBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(NoneIndex, (startingBettor + 2) % Players.Players.Count);
            currentBet = 10;
            Players.Raise(10, NoneIndex);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[NoneIndex].PlayerStatus);
            Assert.IsTrue(!Players.AdvanceRound(), "Round should not be advanced yet");
            Assert.AreEqual(DealerIndex, (startingBettor + 3) % Players.Players.Count);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[DealerIndex].PlayerStatus);
            Assert.IsTrue(!Players.IsActivePlayer(DealerIndex), "Dealer should be inactive");
            Assert.AreEqual(smallBlindIndex, (startingBettor + 4) % Players.Players.Count);
            Players.Call(currentBet, smallBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.IsTrue(!Players.AdvanceRound(), "Round should not be advanced yet");
            Assert.AreEqual(bigBlindIndex, (startingBettor + 5) % Players.Players.Count);
            Players.Call(currentBet, bigBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.IsTrue(Players.AdvanceRound(), "Round should be ready to advance");

            //assure blind wagers were processed properly
            Assert.AreEqual(486, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(486, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(486, Players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(10, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(10, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(10, Players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(3, Players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> { 10, 10, 10 }, Players.PackageBets()); // Verify the list of Players eligible for the pot
            PotManager.AddFoldedBetsToPot(Players.PackageFoldedBets());
            PotManager.AddToPot(currentBet, Players.PackageBets());
            currentBet = 0;
            Players.ResetBets();

            //assure blind wagers were processed properly
            Assert.AreEqual(486, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(486, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(486, Players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(0, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(44, PotManager.Pots[0].Total); // Verify the total

            /// <summary>
            /// Betting for Turn Round
            /// </summary>

            startingBettor = Players.GetStartingBettorIndex();
            Assert.AreEqual(smallBlindIndex, startingBettor);
            currentBet = 50;
            Players.Raise(currentBet, smallBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(bigBlindIndex, (startingBettor + 1) % Players.Players.Count);
            currentBet = 100;
            Players.Raise(currentBet, bigBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(NoneIndex, (startingBettor + 2) % Players.Players.Count);
            currentBet = 200;
            Players.Raise(currentBet, NoneIndex);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[NoneIndex].PlayerStatus);
            Assert.IsTrue(!Players.AdvanceRound(), "Round should not be advanced yet");
            Assert.AreEqual(DealerIndex, (startingBettor + 3) % Players.Players.Count);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[DealerIndex].PlayerStatus);
            Assert.IsTrue(!Players.IsActivePlayer(DealerIndex), "Dealer should be inactive");
            Assert.AreEqual(smallBlindIndex, (startingBettor + 4) % Players.Players.Count);
            Players.Fold(smallBlindIndex);
            PotManager.RemoveFoldedPlayers(smallBlindIndex);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.IsTrue(!Players.AdvanceRound(), "Round should not be advanced yet");
            Assert.AreEqual(bigBlindIndex, (startingBettor + 5) % Players.Players.Count);
            Players.Call(currentBet, bigBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.IsTrue(Players.AdvanceRound(), "Round should be ready to advance");

            //assure blind wagers were processed properly
            Assert.AreEqual(436, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(286, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(286, Players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(50, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(200, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(200, Players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(1, Players.PackageFoldedBets().Count);
            Assert.AreEqual(2, Players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> { 50 }, Players.PackageFoldedBets()); // Verify the list of Players eligible for the pot
            CollectionAssert.AreEqual(new List<int> { 200, 200 }, Players.PackageBets()); // Verify the list of Players eligible for the pot
            PotManager.AddFoldedBetsToPot(Players.PackageFoldedBets());
            PotManager.AddToPot(currentBet, Players.PackageBets());
            currentBet = 0;
            Players.ResetBets();

            //assure blind wagers were processed properly
            Assert.AreEqual(436, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(286, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(286, Players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(0, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(494, PotManager.Pots[0].Total); // Verify the total

            /// <summary>
            /// Betting for River Round
            /// </summary>

            startingBettor = Players.GetStartingBettorIndex();
            Assert.AreEqual(bigBlindIndex, startingBettor);
            Players.Check(bigBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(NoneIndex, (startingBettor + 1) % Players.Players.Count);
            currentBet = Players.AllInBet(NoneIndex);
            Assert.AreEqual(286, currentBet);
            Assert.AreEqual(PlayerStatus.IN, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.ALLIN, Players.Players[NoneIndex].PlayerStatus);
            Assert.IsTrue(!Players.AdvanceRound(), "Round should not be advanced yet");
            Assert.AreEqual(DealerIndex, (startingBettor + 2) % Players.Players.Count);
            Assert.IsTrue(!Players.IsActivePlayer(DealerIndex), "Dealer should be inactive");
            Assert.AreEqual(smallBlindIndex, (startingBettor + 3) % Players.Players.Count);
            Assert.IsTrue(!Players.IsActivePlayer(DealerIndex), "Small Blind player should be inactive");
            Assert.AreEqual(bigBlindIndex, (startingBettor + 4) % Players.Players.Count);
            Players.Call(currentBet, bigBlindIndex);
            Assert.AreEqual(PlayerStatus.ALLIN, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.IsTrue(Players.AdvanceRound(), "Round should be ready to advance");

            //assure blind wagers were processed properly
            Assert.AreEqual(436, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(0, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(0, Players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(286, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(286, Players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.ALLIN, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.ALLIN, Players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(0, Players.PackageFoldedBets().Count);
            Assert.AreEqual(2, Players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> { 286, 286 }, Players.PackageBets()); // Verify the list of Players eligible for the pot
            PotManager.AddFoldedBetsToPot(Players.PackageFoldedBets());
            PotManager.AddToPot(currentBet, Players.PackageBets());
            currentBet = 0;
            Players.ResetBets();

            //assure blind wagers were processed properly
            Assert.AreEqual(436, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(0, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(0, Players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(0, Players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(0, Players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.ALLIN, Players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, Players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.ALLIN, Players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(1066, PotManager.Pots[0].Total); // Verify the total

            Assert.IsTrue(
                PotManager.PlayersEligible(0).SequenceEqual(new List<int> { NoneIndex, bigBlindIndex }) ||
                PotManager.PlayersEligible(0).SequenceEqual(new List<int> { bigBlindIndex, NoneIndex }),
                "Wrong list of eligible Players"
            ); // Verify the list of Players eligible for the pot

            /// <summary>
            /// Round conclusion phase
            /// </summary>

            List<int> winners = new List<int> { bigBlindIndex };

            Assert.AreEqual(1066, PotManager.DistributePot(winners.Count, 0)); // Verify the total
            Players.Payout(winners, PotManager.DistributePot(winners.Count, 0));

            Assert.AreEqual(436, Players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(1066, Players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, Players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(0, Players.Players[NoneIndex].PlayerStack);

            PotManager.ResetPots();
            Players.EliminatePlayers();

            Assert.AreEqual(3, Players.Players.Count);
        }
    }
}
