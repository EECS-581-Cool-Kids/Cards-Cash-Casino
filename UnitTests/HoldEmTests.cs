using CardsCashCasino.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CardsCashCasino.Manager.PlayerManager;

namespace CardsCashCasino.NUnit
{
    [TestClass]
    public class HoldEmTests
    {
        public TexasHoldEmPotManager _potManager = new TexasHoldEmPotManager();
        public TexasHoldEmManager _holdEmManager = new TexasHoldEmManager();
        PlayerManager _players = new PlayerManager();

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

            List<int> playerBets = new List<int> { 2, 4, 6, 6 };

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
            List<int> preflopBets = new List<int> { 2, 2, 2 };
            List<int> foldedBets = new List<int> { 1 };
            List<int> flopBets = new List<int> { 4, 4 };
            List<int> turnBets = new List<int> { 0, 0 };
            List<int> riverBets = new List<int> { 2, 2 };

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
        /// <summary>
        /// Testing method to initiate players
        /// </summary>
        [TestMethod]
        public void InitiatePlayers()
        {
            int numAIPlayers = 3;

            _players.InitiatePlayers(numAIPlayers);

            int dealerIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER);

            Assert.AreEqual(4, _players.Players.Count); // Ensure four players were created in the round
            Assert.AreEqual(PlayerType.USER, _players.Players[0].PlayerType);
            Assert.AreEqual(PlayerType.AI, _players.Players[1].PlayerType);
            Assert.AreEqual(PlayerType.AI, _players.Players[2].PlayerType);
            Assert.AreEqual(PlayerType.AI, _players.Players[3].PlayerType);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[0].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[1].PlayerStatus);
            Assert.AreEqual(500, _players.Players[0].PlayerStack);
            Assert.AreEqual(500, _players.Players[1].PlayerStack);
            Assert.AreEqual(0, _players.Players[0].PlayerBet);
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));
            Assert.AreEqual((dealerIndex + 3) % 4, _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(dealerIndex, _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual((dealerIndex + 2) % 4, _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND));
            Assert.AreEqual((dealerIndex + 1) % 4, _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));

        }

        /// <summary>
        /// Testing the method to shift the blinds one position to the left at the end of a round
        /// </summary>
        [TestMethod]
        public void SetNextRoundBlinds()
        {
            int numAIPlayers = 3;

            _players.InitiatePlayers(numAIPlayers);

            int firstNoneIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE);
            int firstDealerIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER);
            int firstSmallBlindIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            int firstBigBlindIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);

            _players.SetNextRoundBlinds();

            Assert.AreEqual(4, _players.Players.Count); // Ensure four players were created in the round
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));
            Assert.AreEqual((firstNoneIndex + 1) % 4, _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual((firstDealerIndex + 1) % 4, _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual((firstBigBlindIndex + 1) % 4, _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND));
            Assert.AreEqual((firstSmallBlindIndex + 1) % 4, _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
        }

        /// <summary>
        /// Testing method for getting the first bettor on the preflop round (always player to the left of big blind
        /// </summary>
        [TestMethod]
        public void GetPreflopStartingIndex()
        {
            int numAIPlayers = 3;

            _players.InitiatePlayers(numAIPlayers);
            int startingBettorIndex = _players.GetPreflopStartingBettor();

            Assert.AreEqual(4, _players.Players.Count); // Ensure four players were created in the round
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));
            Assert.AreEqual(startingBettorIndex, _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE));
        }

        /// <summary>
        /// Testing the method for getting the first bettor on any round after the preflop
        /// </summary>
        [TestMethod]
        public void GetStartingBettorIndex()
        {
            int numAIPlayers = 3;

            _players.InitiatePlayers(numAIPlayers);
            int startingBettorIndex = _players.GetStartingBettorIndex();

            Assert.AreEqual(4, _players.Players.Count); // Ensure four players were created in the round
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));
            Assert.AreEqual(startingBettorIndex, _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
        }

        /// <summary>
        /// Testing preflop with an all in at blind collection
        /// </summary>
        [TestMethod]
        public void GetStartingBettorIndex_OneFoldOneAllIn()
        {
            int numAIPlayers = 3;

            _players.InitiatePlayers(numAIPlayers);
                
            //applying conditions for the test
            int bigBlindIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);
            int smallBlindIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            _players.Players[smallBlindIndex].PlayerStatus = PlayerStatus.FOLDED;
            _players.Players[bigBlindIndex].PlayerStatus = PlayerStatus.ALLIN;
            int startingBettorIndex = _players.GetStartingBettorIndex();

            Assert.AreEqual(4, _players.Players.Count); // Ensure four players were created in the round
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));

            //typically small blind gets first bet, when not eligible to bet, the first player to the small blind's left that is eligible gets the first bet
            Assert.AreEqual(startingBettorIndex, _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE));
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

            _players.InitiatePlayers(numAIPlayers);

            //applying conditions for the test
            int bigBlindIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);
            int smallBlindIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            _players.CollectBlinds(smallBlindAmount, bigBlindAmount);

            Assert.AreEqual(4, _players.Players.Count); // Ensure four players were created in the round
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));

            //assure blind wagers were processed properly
            Assert.AreEqual(499, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(1, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(2, _players.Players[bigBlindIndex].PlayerBet);
        }

        [TestMethod]
        public void CollectBlinds_TwoFolds_SmallBlindCalls_BigChecks_AddToPot()
        {
            int numAIPlayers = 3;
            int smallBlindAmount = 1;
            int bigBlindAmount = 2;
            int _currentBet = 2;
            int _ante = 2;

            _players.InitiatePlayers(numAIPlayers);

            Assert.AreEqual(4, _players.Players.Count); // Ensure four players were created in the round
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));

            int bigBlindIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);
            int smallBlindIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            int NoneIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE);
            int DealerIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER);

            //add antes
            _players.GenerateAntes(_ante);
            _potManager.InitializePot(_ante, _players.PackageBets());

            //assure antes were processed properly
            Assert.AreEqual(498, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(2, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(2, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(2, _players.Players[DealerIndex].PlayerBet);
            Assert.AreEqual(2, _players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(4, _players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> { 2, 2, 2, 2 }, _players.PackageBets()); // Verify the list of players eligible for the pot
            Assert.AreEqual(8, _potManager.Pots[0].Total); // Verify the total

            _players.ResetBets();

            Assert.AreEqual(0, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[DealerIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[NoneIndex].PlayerBet);

            //applying conditions for the test
            _players.CollectBlinds(smallBlindAmount, bigBlindAmount);
            _players.Fold(NoneIndex);
            _potManager.RemoveFoldedPlayers(NoneIndex);
            _players.Fold(DealerIndex);
            _potManager.RemoveFoldedPlayers(DealerIndex);
            Assert.AreEqual(1, _players.Players[smallBlindIndex].PlayerBet);
            _players.Call(_currentBet, smallBlindIndex);

            //assure blind wagers were processed properly
            Assert.AreEqual(496, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(496, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(2, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(2, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(2, _players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> { 2, 2 }, _players.PackageBets()); // Verify the list of players eligible for the pot
            _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
            _potManager.AddToPot(_currentBet, _players.PackageBets());
            _players.ResetBets();

            //assure blind wagers were processed properly
            Assert.AreEqual(496, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(496, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(0, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(12, _potManager.Pots[0].Total); // Verify the total

            Assert.IsTrue(
                _potManager.PlayersEligible(0).SequenceEqual(new List<int> { smallBlindIndex, bigBlindIndex }) ||
                _potManager.PlayersEligible(0).SequenceEqual(new List<int> { bigBlindIndex, smallBlindIndex }),
                "Wrong list of eligible players"
            ); // Verify the list of players eligible for the pot
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
            int _currentBet = 2;
            int _ante = 2;

            _players.InitiatePlayers(numAIPlayers);

            Assert.AreEqual(4, _players.Players.Count); // Ensure four players were created in the round
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.NONE));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.DEALER));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.SMALLBLIND));
            Assert.AreEqual(1, _players.Players.Count(player => player.PlayerPosition == PlayerPosition.BIGBLIND));

            int bigBlindIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);
            int smallBlindIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            int NoneIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.NONE);
            int DealerIndex = _players.Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER);

            //add antes
            _players.GenerateAntes(_ante);
            _potManager.InitializePot(_ante, _players.PackageBets());

            //assure antes were processed properly
            Assert.AreEqual(498, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(2, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(2, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(2, _players.Players[DealerIndex].PlayerBet);
            Assert.AreEqual(2, _players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(4, _players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> { 2, 2, 2, 2 }, _players.PackageBets()); // Verify the list of players eligible for the pot
            Assert.AreEqual(8, _potManager.Pots[0].Total); // Verify the total

            _players.ResetBets();

            Assert.AreEqual(0, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[DealerIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[NoneIndex].PlayerBet);

            /// <summary>
            /// Betting for Preflop Round
            /// </summary>
            
            _players.CollectBlinds(smallBlindAmount, bigBlindAmount);
            _players.Fold(DealerIndex);
            _potManager.RemoveFoldedPlayers(DealerIndex);
            Assert.AreEqual(1, _players.Players[smallBlindIndex].PlayerBet);
            Assert.IsTrue(!_players.AdvanceRound(), "Round should not be advanced yet");
            _players.Call(_currentBet, NoneIndex);
            Assert.IsTrue(!_players.AdvanceRound(), "Round should not be advanced yet");
            _players.Call(_currentBet, smallBlindIndex);
            Assert.IsTrue(!_players.AdvanceRound(), "Round should not be advanced yet");
            _players.Check(bigBlindIndex);
            Assert.IsTrue(_players.AdvanceRound(), "Round should be ready to advance");

            //assure blind wagers were processed properly
            Assert.AreEqual(496, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(496, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(496, _players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(2, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(2, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(2, _players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(3, _players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> {2, 2, 2 }, _players.PackageBets()); // Verify the list of players eligible for the pot
            _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
            _potManager.AddToPot(_currentBet, _players.PackageBets());
            _currentBet = 0;
            _players.ResetBets();

            //assure blind wagers were processed properly
            Assert.AreEqual(496, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(496, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(496, _players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(0, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(14, _potManager.Pots[0].Total); // Verify the total

            /// <summary>
            /// Betting for Flop Round
            /// </summary>
            
            int startingBettor = _players.GetStartingBettorIndex();
            Assert.AreEqual(smallBlindIndex, startingBettor);
            _players.Check(smallBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(bigBlindIndex, (startingBettor + 1) % _players.Players.Count);
            _players.Check(bigBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(NoneIndex, (startingBettor + 2) % _players.Players.Count);
            _currentBet = 10;
            _players.Raise(10, NoneIndex);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[NoneIndex].PlayerStatus);
            Assert.IsTrue(!_players.AdvanceRound(), "Round should not be advanced yet");
            Assert.AreEqual(DealerIndex, (startingBettor + 3) % _players.Players.Count);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[DealerIndex].PlayerStatus);
            Assert.IsTrue(!_players.IsActivePlayer(DealerIndex), "Dealer should be inactive");
            Assert.AreEqual(smallBlindIndex, (startingBettor + 4) % _players.Players.Count);
            _players.Call(_currentBet, smallBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.IsTrue(!_players.AdvanceRound(), "Round should not be advanced yet");
            Assert.AreEqual(bigBlindIndex, (startingBettor + 5) % _players.Players.Count);
            _players.Call(_currentBet, bigBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.IsTrue(_players.AdvanceRound(), "Round should be ready to advance");

            //assure blind wagers were processed properly
            Assert.AreEqual(486, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(486, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(486, _players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(10, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(10, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(10, _players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(3, _players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> { 10, 10, 10 }, _players.PackageBets()); // Verify the list of players eligible for the pot
            _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
            _potManager.AddToPot(_currentBet, _players.PackageBets());
            _currentBet = 0;
            _players.ResetBets();

            //assure blind wagers were processed properly
            Assert.AreEqual(486, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(486, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(486, _players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(0, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(44, _potManager.Pots[0].Total); // Verify the total

            /// <summary>
            /// Betting for Turn Round
            /// </summary>

            startingBettor = _players.GetStartingBettorIndex();
            Assert.AreEqual(smallBlindIndex, startingBettor);
            _currentBet = 50;
            _players.Raise(_currentBet, smallBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(bigBlindIndex, (startingBettor + 1) % _players.Players.Count);
            _currentBet = 100;
            _players.Raise(_currentBet, bigBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(NoneIndex, (startingBettor + 2) % _players.Players.Count);
            _currentBet = 200;
            _players.Raise(_currentBet, NoneIndex);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[NoneIndex].PlayerStatus);
            Assert.IsTrue(!_players.AdvanceRound(), "Round should not be advanced yet");
            Assert.AreEqual(DealerIndex, (startingBettor + 3) % _players.Players.Count);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[DealerIndex].PlayerStatus);
            Assert.IsTrue(!_players.IsActivePlayer(DealerIndex), "Dealer should be inactive");
            Assert.AreEqual(smallBlindIndex, (startingBettor + 4) % _players.Players.Count);
            _players.Fold(smallBlindIndex);
            _potManager.RemoveFoldedPlayers(smallBlindIndex);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.IsTrue(!_players.AdvanceRound(), "Round should not be advanced yet");
            Assert.AreEqual(bigBlindIndex, (startingBettor + 5) % _players.Players.Count);
            _players.Call(_currentBet, bigBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.IsTrue(_players.AdvanceRound(), "Round should be ready to advance");

            //assure blind wagers were processed properly
            Assert.AreEqual(436, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(286, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(286, _players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(50, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(200, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(200, _players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(1, _players.PackageFoldedBets().Count);
            Assert.AreEqual(2, _players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> { 50 }, _players.PackageFoldedBets()); // Verify the list of players eligible for the pot
            CollectionAssert.AreEqual(new List<int> { 200, 200 }, _players.PackageBets()); // Verify the list of players eligible for the pot
            _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
            _potManager.AddToPot(_currentBet, _players.PackageBets());
            _currentBet = 0;
            _players.ResetBets();

            //assure blind wagers were processed properly
            Assert.AreEqual(436, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(286, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(286, _players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(0, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(494, _potManager.Pots[0].Total); // Verify the total

            /// <summary>
            /// Betting for River Round
            /// </summary>

            startingBettor = _players.GetStartingBettorIndex();
            Assert.AreEqual(bigBlindIndex, startingBettor);
            _players.Check(bigBlindIndex);
            Assert.AreEqual(PlayerStatus.CALLED, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(NoneIndex, (startingBettor + 1) % _players.Players.Count);
            _currentBet = _players.AllInBet(NoneIndex);
            Assert.AreEqual(286, _currentBet);
            Assert.AreEqual(PlayerStatus.IN, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.ALLIN, _players.Players[NoneIndex].PlayerStatus);
            Assert.IsTrue(!_players.AdvanceRound(), "Round should not be advanced yet");
            Assert.AreEqual(DealerIndex, (startingBettor + 2) % _players.Players.Count);
            Assert.IsTrue(!_players.IsActivePlayer(DealerIndex), "Dealer should be inactive");
            Assert.AreEqual(smallBlindIndex, (startingBettor + 3) % _players.Players.Count);
            Assert.IsTrue(!_players.IsActivePlayer(DealerIndex), "Small Blind player should be inactive");
            Assert.AreEqual(bigBlindIndex, (startingBettor + 4) % _players.Players.Count);
            _players.Call(_currentBet, bigBlindIndex);
            Assert.AreEqual(PlayerStatus.ALLIN, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.IsTrue(_players.AdvanceRound(), "Round should be ready to advance");

            //assure blind wagers were processed properly
            Assert.AreEqual(436, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(0, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(0, _players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(286, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(286, _players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.ALLIN, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.ALLIN, _players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(0, _players.PackageFoldedBets().Count);
            Assert.AreEqual(2, _players.PackageBets().Count);
            CollectionAssert.AreEqual(new List<int> { 286, 286 }, _players.PackageBets()); // Verify the list of players eligible for the pot
            _potManager.AddFoldedBetsToPot(_players.PackageFoldedBets());
            _potManager.AddToPot(_currentBet, _players.PackageBets());
            _currentBet = 0;
            _players.ResetBets();

            //assure blind wagers were processed properly
            Assert.AreEqual(436, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(0, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(0, _players.Players[NoneIndex].PlayerStack);
            Assert.AreEqual(0, _players.Players[smallBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[bigBlindIndex].PlayerBet);
            Assert.AreEqual(0, _players.Players[NoneIndex].PlayerBet);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[smallBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.ALLIN, _players.Players[bigBlindIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.FOLDED, _players.Players[DealerIndex].PlayerStatus);
            Assert.AreEqual(PlayerStatus.ALLIN, _players.Players[NoneIndex].PlayerStatus);
            Assert.AreEqual(1066, _potManager.Pots[0].Total); // Verify the total

            Assert.IsTrue(
                _potManager.PlayersEligible(0).SequenceEqual(new List<int> { NoneIndex, bigBlindIndex }) ||
                _potManager.PlayersEligible(0).SequenceEqual(new List<int> { bigBlindIndex, NoneIndex }),
                "Wrong list of eligible players"
            ); // Verify the list of players eligible for the pot

            /// <summary>
            /// Round conclusion phase
            /// </summary>

            List<int> winners = new List<int> { bigBlindIndex };

            Assert.AreEqual(1066, _potManager.DistributePot(winners.Count, 0)); // Verify the total
            _players.Payout(winners, _potManager.DistributePot(winners.Count, 0));

            Assert.AreEqual(436, _players.Players[smallBlindIndex].PlayerStack);
            Assert.AreEqual(1066, _players.Players[bigBlindIndex].PlayerStack);
            Assert.AreEqual(498, _players.Players[DealerIndex].PlayerStack);
            Assert.AreEqual(0, _players.Players[NoneIndex].PlayerStack);

            _potManager.ResetPots();
            _players.EliminatePlayers();

            Assert.AreEqual(3, _players.Players.Count);
        }
    }
}
