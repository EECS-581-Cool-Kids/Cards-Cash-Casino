/*
 *  Module Name: PokerUtil.cs
 *  Purpose: Contains static utility methods used to determine the winner of a game of Texas Hold 'Em.
 *  Inputs: The set of community cards and a player's hole cards.
 *  Outputs: The ranking, optimal 5-card hand, and the value used to break ties.
 *  Additional code sources: None
 *  Developers: Ethan Berkley
 *  Date: 11/08/2024
 *  Last Modified: 11/21/2024
 *  Preconditions: Hole card lists are of length 2, Community card lists are of length >= 3, the input to the tiebreaker function was returned by the GetScore function
 *  Postconditions: None
 *  Error/Exception conditions: Only if Preconditions were violated.
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Data
{
    /// <summary>
    /// Utility class that will be used to determine a hand's ranking and tie breaking score, which is used at the conclusion of a round.
    /// Defines an enum and two functions:
    /// The enum, Ranking, represents the Rank of the poker hand. i.e. Royal Flush, Four of a Kind, Full House, etc.
    /// The first function, GetScore, takes in the community cards and the player's hole cards, and returns the optimal hand and it's ranking.
    /// The second function, KickerValue, takes in a 5-card hand returned by GetScore, 
    ///     and returns the value used to break ties between hands of the same rank.
    /// </summary>
    public class PokerUtil
    {
        /// <summary>
        /// Enum representing the rank of a 5-card poker hand.
        /// </summary>
        public enum Ranking
        {
            ROYAL_FLUSH,
            STRAIGHT_FLUSH,
            FOUR_OF_A_KIND,
            FULL_HOUSE,
            FLUSH,
            STRAIGHT,
            THREE_OF_A_KIND,
            TWO_PAIR,
            PAIR,
            HIGH_CARD
        }

        /// <summary>
        /// Given the current 5-card hand, return the ranking value.
        /// </summary>
        /// <param name="cards">A player's hand of 5 cards</param>
        /// <returns>The ranking of the hand.</returns>
        /// <remarks>Note: this overloaded function is meant to be used by 5-card draw.</remarks>
        public static Ranking GetScore(List<Card> cards)
        {
            Debug.Assert(cards.Count == 5);
            return GetRanking(SortedHand(cards));
        }

        /// <summary>
        /// Given the current card community card pool (which must have at least 3 cards), and the current hand (which must have exactly 2 cards), 
        /// create and return the best possible hand and ranking.
        /// 
        /// In the event of a tie in rankings, the tie would be broken by calling KickerValue on each of the hands. 
        /// If the two return values are the same, split the pot.
        /// </summary>
        /// <param name="community">The 5-card list of community cards</param>
        /// <param name="cards">The 2-card list representing player's hole cards.</param>
        /// <returns>A 5-card list representing the optimal hand, and the ranking it would give.</returns>
        /// <remarks>Note: this overloaded function is meant to be used by Texas Hold 'Em.</remarks>
        public static Tuple<List<Card>, Ranking> GetScore(List<Card> community, List<Card> cards)
        {
            Debug.Assert(cards.Count == 2);
            Debug.Assert(community.Count >= 3);

            Ranking best = Ranking.HIGH_CARD;
            List<List<Card>> bestHands = new();
            List<List<Card>> cardPerms = Get5CardPermutations(community.Concat(cards).ToList());
            foreach (List<Card> iCards in cardPerms)
            {
                List<Card> hand = SortedHand(iCards);
                Ranking current = GetRanking(hand);
                if (current < best)
                {
                    best = current;
                    bestHands.Clear();
                    bestHands.Add(hand);
                }
                else if (current == best)
                {
                    bestHands.Add(hand);
                }
            }

            long bestHandVal = 0;
            int bestI = -1;

            for (int i = 0; i < bestHands.Count; i++)
            {
                long handVal = KickerValue(bestHands[i]);
                if (handVal > bestHandVal)
                {
                    bestHandVal = handVal;
                    bestI = i;
                }
            }

            return new Tuple<List<Card>, Ranking>(bestHands[bestI], best);
        }

        /// <summary>
        /// Used for breaking a tie between two hands of equal rank.
        /// Assigns a value to hand1 that will be greater than some hand2, if hand1 has some card C such that 
        ///     For each card in hand2 with a greater value, hand1 has another card with equal value.  
        /// For example, the hand [A,2,2,2,2] Is worth a greater value than [K,K,K,K,Q]
        /// And the hand [A,A,A,K,5] is worth a greater value than [A,A,K,K,K]
        /// Hands [5,5,4,3,2] and [5,5,4,3,2] are considered tied and should result in a split pot.
        /// </summary>
        /// <param name="hand">A 5-card hand that was returned by GetScore.</param>
        /// <returns>the value that will be compared to other values returned by this function to determine a winner.</returns>
        public static long KickerValue(List<Card> hand)
        {
            Debug.Assert(hand.Count == 5);
            
            // Special case: 2345A
            if (hand[0].Value == Value.TWO && hand[1].Value == Value.THREE && hand[2].Value == Value.FOUR && hand[3].Value == Value.FIVE && hand[4].Value == Value.ACE)
            {
                return (long)(Math.Pow(5, 5) + Math.Pow(4, 4) + Math.Pow(3, 3) + Math.Pow(2, 2) + Math.Pow(1, 1));
            }

            /*
             * Decision for which hand wins in a tie is certainly overtuned but should be complete, 
             *  and operates as if only Ranking.HIGH_CARD is possible.
             * 
             * Algorithm is simply :=   Score = 5^hand[4] + 4^hand[3] + ... + 1^hand[0];
             * 12 12 12 12 12 =>    261,453,379
             * 13 1  1  1  1  =>    1,220,703,135
             * Which is what we want.
             * 
             * AAAAK    => 6,373,561,789
             * INT_MAX  => 2,147,483,647
             * So we will use 64 bit integer.
             */
            long handVal = 0;
            for (int j = 4; j >= 0; j--)
            {
                handVal += (long)Math.Pow((j + 1), hand[j].GetPokerValue());
            }
            return handVal;
        }

        /// <summary>
        /// Orders cards in ascending order by value and suit.
        /// Suits are ordered as SPADES < HEARTS < DIAMONDS < CLUBS.
        /// the hand [4C, 3S, 4H, AS, 7D]  would be ordered as [3S, 4H, 4C, 7D, AS]
        /// </summary>
        /// <param name="cards">Represents a hand of cards. There should be exactly 5 elements in the list.</param>
        /// <returns>The sorted list of cards. There will be exactly 5 elements in the list.</returns>
        private static List<Card> SortedHand(List<Card> cards)
        {
            return cards.OrderBy(card => (card.GetPokerValue() * 4) - card.Suit).ToList();
        }

        /// <summary>
        /// Takes in a list of cards of length > 5 and generates a list of every unique set of 5 cards.
        /// i.e. for cards [2C,JH,5C,6S,KD,1S,4D], the 5-permutation [2C,JH,5C,1S,4D] will be returned, but never [JH,1S,4D,5C,2C].
        /// </summary>
        /// <param name="list">List of cards with length > 5</param>
        /// <returns>All 5-card permutations of the input list</returns>
        private static List<List<Card>> Get5CardPermutations(List<Card> list)
        {
            List<List<Card>> perms = new();
            for (int m = 0; m < (1 << list.Count); m++)
            {
                List<Card> cur = new();
                for (int i = 0; i < list.Count; i++)
                {
                    if ((m & (1 << i)) != 0)
                    {
                        cur.Add(list[i]);
                    }
                }
                if (cur.Count == 5)
                {
                    perms.Add(cur);
                }
            }

            return perms;
            //IEnumerable<IEnumerable<Card>> full_perms = from m in Enumerable.Range(0, 1 << list.Count)
            //                                            select
            //                                                from i in Enumerable.Range(0, list.Count)
            //                                                where (m & (1 << i)) != 0
            //                                                select list[i];

            //return (from m in full_perms.ToList<IEnumerable<Card>>()
            //        select
            //             (from i in Enumerable.Range(0, 5) select m.ToList<Card>()[i]).ToList()).ToList();
        }

        /// <summary>
        /// Call each of the functions used to determine if our hand is of a certain rank back to back. 
        /// Requires that cards is of length 5, and has had SortedHand called on it.
        /// </summary>
        /// <param name="cards">The hand we are trying to determine the rank of.</param>
        /// <returns>the cards rank.</returns>
        private static Ranking GetRanking(List<Card> cards)
        {
            if (IsRoyalFlush(cards)) return Ranking.ROYAL_FLUSH;
            else if (IsStraightFlush(cards)) return Ranking.STRAIGHT_FLUSH;
            else if (IsFourOfAKind(cards)) return Ranking.FOUR_OF_A_KIND;
            else if (IsFullHouse(cards)) return Ranking.FULL_HOUSE;
            else if (IsFlush(cards)) return Ranking.FLUSH;
            else if (IsStraight(cards)) return Ranking.STRAIGHT;
            else if (IsThreeOfAKind(cards)) return Ranking.THREE_OF_A_KIND;

            else return GetNumPairs(cards) switch
            {
                2 => Ranking.TWO_PAIR,
                1 => Ranking.PAIR,
                _ => Ranking.HIGH_CARD,
            };
        }

        /// <summary>
        /// Function to determine if a poker hand is a royal flush.
        /// </summary>
        /// <param name="cards">a 5-card hand.</param>
        /// <returns>True if cards is a royal flush, false otherwise.</returns>
        private static bool IsRoyalFlush(List<Card> cards)
        {
            return IsStraightFlush(cards) && cards[0].Value == Value.TEN;
        }

        /// <summary>
        /// Function to determine if a poker hand is a flush.
        /// </summary>
        /// <param name="cards">a 5-card hand.</param>
        /// <returns>True if cards is a flush, false otherwise.</returns>
        private static bool IsFlush(List<Card> cards)
        {
            return cards.All(card => card.Suit == cards[0].Suit);
        }

        /// <summary>
        /// Function to determine if a poker hand is a straight flush.
        /// </summary>
        /// <param name="cards">a 5-card hand.</param>
        /// <returns>True if cards is a straight flush, false otherwise.</returns>
        private static bool IsStraightFlush(List<Card> cards)
        {
            return IsFlush(cards) && IsStraight(cards);
        }

        /// <summary>
        /// Function to determine if a poker hand is a four of a kind.
        /// </summary>
        /// <param name="cards">a 5-card hand.</param>
        /// <returns>True if cards is a four of a kind, false otherwise.</returns>
        private static bool IsFourOfAKind(List<Card> cards)
        {
            return cards[0] == cards[3];
        }

        /// <summary>
        /// Function to determine if a poker hand is a full house.
        /// </summary>
        /// <param name="cards">a 5-card hand.</param>
        /// <returns>True if cards is a full house, false otherwise.</returns>
        private static bool IsFullHouse(List<Card> cards)
        {
            return cards[0] == cards[1] && cards[3] == cards[4] && (cards[2] == cards[1] || cards[2] == cards[3]);
        }

        /// <summary>
        /// Function to determine if a poker hand is a straight.
        /// </summary>
        /// <param name="cards">a 5-card hand.</param>
        /// <returns>True if cards is a straight, false otherwise.</returns>
        private static bool IsStraight(List<Card> cards)
        {
            // Special case: 2345A
            if (cards[0].Value == Value.TWO && cards[1].Value == Value.THREE && cards[2].Value == Value.FOUR && cards[3].Value == Value.FIVE && cards[4].Value == Value.ACE)
            {
                return true;
            }

            int min = cards[0].GetPokerValue();
            
            return (cards.Sum(card => card.GetPokerValue()) == (min*5) + 10);
        }

        /// <summary>
        /// Function to determine if a poker hand is a three of a kind.
        /// </summary>
        /// <param name="cards">a 5-card hand.</param>
        /// <returns>True if cards is a three of a kind, false otherwise.</returns>
        private static bool IsThreeOfAKind(List<Card> cards)
        {
            return cards[2] == cards[0] || cards[2] == cards[4];
        }

        /// <summary>
        /// Function used to determine if a poker hand is of rank Two Pair, Pair, or High Card.
        /// Avoids the need to call an IsTwoPair and IsPair function that would do virtually the same exact thing.
        /// </summary>
        /// <param name="cards">a 5-card hand.</param>
        /// <returns>2 if cards is a two pair, 1 if cards is a pair, 0 if cards is a high card.</returns>
        private static int GetNumPairs(List<Card> cards)
        {
            int pairs = 0;
            for (int i = 1; i < cards.Count; i++)
            {
                if (cards[i] == cards[i - 1])
                {
                    pairs++;
                }
            }
            return pairs;
        }
    }
}
