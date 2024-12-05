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
    /// The type of player being referred to in the HoldEmPlayerManager Class
    /// </summary>
    public enum PlayerType
    {
        USER,
        AI
    }

    /// <summary>
    /// The status of the player in the current round
    /// </summary>
    public enum PlayerStatus
    {
        IN,
        FOLDED,
        CALLED,
        ALLIN,
        BROKE,
    }

    /// <summary>
    /// The identifier applied to each player regarding blinds and player betting order
    /// </summary>
    public enum PlayerPosition
    {
        DEALER,
        SMALLBLIND,
        BIGBLIND,
        NONE
    }

    public class Player
    {
        /// <summary>
        /// how much money the user has available in the game
        /// </summary>
        public int PlayerStack { get; set; } = 500; // will be set to users account funds

        /// <summary>
        /// represents if the player is the USER or an AI
        /// </summary>
        public PlayerType PlayerType { get; set; }

        /// <summary>
        /// represents if the player is IN (still participating in the round), FOLDED, CALLED, or ALL-IN
        /// </summary>
        public PlayerStatus PlayerStatus { get; set; } = PlayerStatus.IN;

        /// <summary>
        /// if the player is the DEALER, BIGBLIND, SMALLBLIND, or NONE
        /// </summary>
        public PlayerPosition PlayerPosition { get; set; } = PlayerPosition.NONE;

        /// <summary>
        /// holds the value of the players bet in the current round
        /// </summary>
        public int PlayerBet { get; set; } = 0;

        /// <summary>
        /// Defining constructors for the class
        /// </summary>
        public Player(PlayerType type)
        {
            PlayerType = type; //defines whether the player is a USER or AI
        }

        /// <summary>
        /// Adds winnings to a players chip stack
        /// <param name="value">The amount added</param>
        /// </summary>
        public void IncrementStack(int value)
        {
            PlayerStack += value;
        }

        /// <summary>
        /// Subtracts funds from a players chip stack
        /// <param name="value">The amount subtracted</param>
        /// </summary>
        public void DecrementStack(int value)
        {
            if (value <= 0 || PlayerStack - value < 0)
            {
                return;
            }
            PlayerStack -= value;
        }

        /// <summary>
        /// Adds funds from a players chip bet
        /// /// <param name="value">The amount added</param>
        /// </summary>
        public void IncrementBet(int value)
        {
            PlayerBet += value;
        }

        /// <summary>
        /// Subtracts from the player bet total in the round
        /// <param name="value">The amount subtracted</param>
        /// </summary>
        public void DecrementBet(int value)
        {
            if (value <= 0 || PlayerBet - value < 0)
            {
                return;
            }
            PlayerBet -= value;
        }
    }

    public class PlayerManager
    {
        /// <summary>
        /// Creating list to hold players
        /// </summary>
        public List<Player> Players;

        /// <summary>
        /// Initiating list to hold player characteristics
        /// </summary>
        public PlayerManager()
        {
            Players = new List<Player>(); // Initialize the Players list
        }

        /// <summary>
        /// Creates USER and specified number of AI opponents
        /// </summary>
        public void InitiatePlayers(int numAIs)
        {
            Players.Clear();
            Players.Add(new Player(PlayerType.USER));

            for (int players = 0; players < numAIs; players++)
            {
                Players.Add(new Player(PlayerType.AI));
            }
            Random random = new Random();
            int dealer = random.Next(0, Players.Count);
            Players[dealer].PlayerPosition = PlayerPosition.DEALER;
            Players[(dealer + 1) % Players.Count].PlayerPosition = PlayerPosition.SMALLBLIND;
            Players[(dealer + 2) % Players.Count].PlayerPosition = PlayerPosition.BIGBLIND;
        }

        /// <summary>
        /// Collect each player's antes for the beginning of the round
        /// </summary>
        public void GenerateAntes(int ante)
        {
            for (int player = 0; player < Players.Count; player++)
            {
                if (ante > Players[player].PlayerStack)
                {
                    Players[player].IncrementBet(Players[player].PlayerStack);
                    Players[player].DecrementStack(Players[player].PlayerStack);
                    Players[player].PlayerStatus = PlayerStatus.ALLIN;
                }
                else
                {
                    Call(ante, player);
                }
            }
        }
        /// <summary>
        /// Gets the position of the first bet to be placed in the preflop round
        /// </summary>
        public int GetPreflopStartingBettor()
        {
            int index = Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);
            index = (index + 1) % Players.Count;
            return index;
        }

        /// <summary>
        /// Gets the position of the first bet to be placed in all rounds following the preflop
        /// </summary>
        public int GetStartingBettorIndex()
        {
            int index = Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            bool activePlayer = false;
            while (!activePlayer)
            {
                if (!IsActivePlayer(index))
                {
                    index = (index + 1) % Players.Count;
                }
                else
                {
                    activePlayer = true;
                }
            }
            return index;
        }

        /// <summary>
        /// If the player is eligible to place a bet returns true
        /// <param name="playerIndex">The index of the current player</param>
        /// </summary>
        public bool IsActivePlayer(int playerIndex)
        {
            PlayerStatus status = Players[playerIndex].PlayerStatus;
            return (status == PlayerStatus.IN || status == PlayerStatus.CALLED); 
        }

        /// <summary>
        /// Places a bet
        /// <param name="amount">The amount of the highest bet placed in the round</param>
        /// <param name="playerIndex">The index of the current player</param>
        /// </summary>
        public void Call(int amount, int playerIndex)
        {
            int bet = amount - Players[playerIndex].PlayerBet; //subtracts the player's existing bet from the amount needed
            Players[playerIndex].IncrementBet(bet);
            Players[playerIndex].DecrementStack(bet);
            if (Players[playerIndex].PlayerStack == 0)
            {
                Players[playerIndex].PlayerStatus = PlayerStatus.ALLIN;
                return;
            }
            Players[playerIndex].PlayerStatus = PlayerStatus.CALLED;
        }

        /// <summary>
        /// Increase the player's bet amount, remove funds from player stack, reset status of all players that can still bet
        /// <param name="playerIndex">The index of the current player</param>
        /// <param name="playerIndex">The index of the current player</param>
        /// </summary>
        public void Raise(int amount, int playerIndex)
        {
            Players[playerIndex].IncrementBet(amount);
            Players[playerIndex].DecrementStack(amount);

            // Iterate through list of players to reset the status for those who called
            // Those players will need to match the raise in order to continue playing
            for (int player = 0; player < Players.Count; player++)
            {
                if (player == playerIndex)
                {
                    Players[player].PlayerStatus = PlayerStatus.CALLED;
                }
                else if (Players[player].PlayerStatus == PlayerStatus.CALLED)
                {
                    Players[player].PlayerStatus = PlayerStatus.IN;
                }
            }
        }

        /// <summary>
        /// Places an all in bet, Increase the player's bet amount, remove funds from player stack, reset status of all players that can still bet
        /// <param name="playerIndex">The index of the current player</param>
        /// </summary>
        public int AllInBet(int playerIndex)
        {
            Players[playerIndex].IncrementBet(Players[playerIndex].PlayerStack);
            Players[playerIndex].DecrementStack(Players[playerIndex].PlayerStack);

            // Iterate through list of players to reset the status for those who called
            // Those players will need to match the all-in bet in order to continue playing
            for (int player = 0; player < Players.Count; player++)
            {
                if (player == playerIndex)
                {
                    Players[player].PlayerStatus = PlayerStatus.ALLIN;
                }
                else if (Players[player].PlayerStatus == PlayerStatus.CALLED)
                {
                    Players[player].PlayerStatus = PlayerStatus.IN;
                }
            }
            return Players[playerIndex].PlayerBet;
        }

        /// <summary>
        /// Folds the active player
        /// <param name="playerIndex">The index of the current player</param>
        /// </summary>
        public void Fold(int playerIndex)
        {
            Players[playerIndex].PlayerStatus = PlayerStatus.FOLDED;
        }

        /// <summary>
        /// Active player bets nothing
        /// <param name="playerIndex">The index of the current player</param>
        /// </summary>
        public void Check(int playerIndex)
        {
            Players[playerIndex].PlayerStatus = PlayerStatus.CALLED;
        }

        /// <summary>
        /// Conditions to be met to advance to the next round
        /// </summary>
        public bool AdvanceRound()
        {
            // if all players have had their turn, have called the existing bet, folded, are broke, or are all in
            return !Players.Any(player => player.PlayerStatus == PlayerStatus.IN);
        }

        /// <summary>
        /// If true, only one player is able to place a bet, the rest are all-in.
        /// All player hands will become visible
        /// The game will bypass all remaining betting opportunities and advance through stages to the RoundConclusion
        /// </summary>
        public bool AdvanceToRoundConclusion()
        {
            int called = Players.Count(player => player.PlayerStatus == PlayerStatus.CALLED);
            int _in = Players.Count(player => player.PlayerStatus == PlayerStatus.IN);
            int allIn = Players.Count(player => player.PlayerStatus == PlayerStatus.ALLIN);
            return (allIn > 0) && (_in + called <= 1);
        }

        /// <summary>
        /// If true, only one player is not folded.
        /// The game will bypass all remaining stages and payout MAIN pot to remaining player
        /// </summary>
        public bool OnePlayerLeft()
        {
            int folded = Players.Count(player => player.PlayerStatus == PlayerStatus.FOLDED);
            int broke = Players.Count(player => player.PlayerStatus == PlayerStatus.BROKE);
            
            if (folded + broke == Players.Count - 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// At the end of a round, resets all player bets to 0
        /// </summary>
        public void ResetBets()
        {
            for (int player = 0; player < Players.Count; player++)
            {
                if (Players[player].PlayerBet != 0)
                {
                    Players[player].DecrementBet(Players[player].PlayerBet);
                }
                PlayerStatus status = Players[player].PlayerStatus;
                if (status != PlayerStatus.BROKE)
                {
                    Players[player].PlayerStatus = PlayerStatus.IN;
                }
            }
        }

        /// <summary>
        /// Packages bets placed by folded players to be added to the main pot
        /// </summary>
        public List<int> PackageFoldedBets()
        {
            List<int> foldedBets = new List<int>();

            for (int player = 0; player < Players.Count; player++)
            {
                if (Players[player].PlayerStatus == PlayerStatus.FOLDED && Players[player].PlayerBet > 0)
                {
                    foldedBets.Add(Players[player].PlayerBet);
                }
            }
            return foldedBets;
        }

        /// <summary>
        /// Packages bets made by active players to be added to main pot, creating side pots if needed
        /// </summary>
        public List<int> PackageBets()
        {
            List<int> bets = new List<int>();

            for (int player = 0; player < Players.Count; player++)
            {
                if (Players[player].PlayerStatus != PlayerStatus.FOLDED && Players[player].PlayerBet > 0)
                {
                    bets.Add(Players[player].PlayerBet);
                }
            }
            return bets;
        }

        /// <summary>
        ///Eliminates players that are out of money before a new round begins
        /// </summary>
        public void EliminatePlayers()
        {
            for (int player = 0; player < Players.Count; player++)
            {
                if (Players[player].PlayerStack == 0)
                {
                    if (Players[player].PlayerPosition == PlayerPosition.DEALER) //shift dealer chip 1 to left if dealer was eliminated
                    {
                        Players[(player + 1) % Players.Count].PlayerPosition = PlayerPosition.DEALER;
                    }
                    Players[player].PlayerStatus = PlayerStatus.BROKE;
                }
            }
        }

        /// <summary>
        ///shifts the blind to the next positions
        /// </summary>
        public void SetNextRoundBlinds()
        {
            //blind shift rules if there are only 2 players remaining 
            if (Players.Count < 3)
            {
                if (Players[0].PlayerPosition == PlayerPosition.BIGBLIND)
                {
                    Players[0].PlayerPosition = PlayerPosition.SMALLBLIND;
                    Players[1].PlayerPosition = PlayerPosition.BIGBLIND;
                }
                else
                {
                    Players[0].PlayerPosition = PlayerPosition.BIGBLIND;
                    Players[1].PlayerPosition = PlayerPosition.SMALLBLIND;
                }
            }
            else
            {
                int dealer = Players.FindIndex(player => player.PlayerPosition == PlayerPosition.DEALER);
                if (Players.Count > 3) //if a only 3 players exist, no player is set to NONE
                {
                    Players[dealer].PlayerPosition = PlayerPosition.NONE;
                }
                Players[(dealer + 1) % Players.Count].PlayerPosition = PlayerPosition.DEALER;
                Players[(dealer + 2) % Players.Count].PlayerPosition = PlayerPosition.SMALLBLIND;
                Players[(dealer + 3) % Players.Count].PlayerPosition = PlayerPosition.BIGBLIND;
            }
        }

        /// <summary>
        ///collect blind bets from small and big blind players
        ///<param name="smallBlind">The amount the small blind must wager</param>
        ///<param name="bigBlind">The amount the big blind must wager</param>
        /// </summary>
        public void CollectBlinds(int _smallBlind, int _bigBlind)
        {
            int smallBlindPosition = Players.FindIndex(player => player.PlayerPosition == PlayerPosition.SMALLBLIND);
            if (Players[smallBlindPosition].PlayerStack <= _smallBlind) //if blind causes player to go all in
            {
                Players[smallBlindPosition].IncrementBet(Players[smallBlindPosition].PlayerStack);
                Players[smallBlindPosition].DecrementStack(Players[smallBlindPosition].PlayerStack);
                Players[smallBlindPosition].PlayerStatus = PlayerStatus.ALLIN;
            }
            else
            {
                Players[smallBlindPosition].IncrementBet(_smallBlind);
                Players[smallBlindPosition].DecrementStack(_smallBlind);
            }
            int bigBlindPosition = Players.FindIndex(player => player.PlayerPosition == PlayerPosition.BIGBLIND);
            if (Players[bigBlindPosition].PlayerStack <= _bigBlind) //if blind causes player to go all in
            {
                Players[bigBlindPosition].IncrementBet(Players[bigBlindPosition].PlayerStack);
                Players[bigBlindPosition].DecrementStack(Players[bigBlindPosition].PlayerStack);
                Players[bigBlindPosition].PlayerStatus = PlayerStatus.ALLIN;
            }
            else
            {
                Players[bigBlindPosition].IncrementBet(_bigBlind);
                Players[bigBlindPosition].DecrementStack(_bigBlind);
            }
        }

        /// <summary>
        /// adds the amount from the pot that the player has won
        /// <param name="winningPlayers">The list of indexes of the players that have won the pot</param>
        /// <param name="payout">The amount each player has won</param>
        /// </summary>
        public void Payout(List<int> winningPlayers, int payout)
        {
            for (int player = 0; player < winningPlayers.Count; player++)
            {
                Players[winningPlayers[player]].IncrementStack(payout);
            }
        }
    }
    public class PlayerValuesIndicator
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
            _firstDigit.Draw(spriteBatch);
            _secondDigit.Draw(spriteBatch);
            _thirdDigit.Draw(spriteBatch);
            _fourthDigit.Draw(spriteBatch);
        }
    }
}
