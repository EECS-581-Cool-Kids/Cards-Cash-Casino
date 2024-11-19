/*
 *  Module Name: CardManager.cs
 *  Purpose: Manages the cards in the game.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Jacob Wilkus, Mo Morgan, Ethan Berkley
 *  Date: 10/26/2024
 *  Last Modified: 10/26/2024
 */

// Import necessary libraries
using CardsCashCasino.Data;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace CardsCashCasino.Manager
{
    /// <summary>
    /// Class <c>CardManager</c> manages the cards in the game.
    /// </summary>
    public class CardManager
    {
        /// <summary>
        /// The list of cards.
        /// </summary>
        public List<Card> Cards { get; set; } = new List<Card>();

        /// <summary>
        /// Internal deck object. Copied into the Cards list when requested. Used to minimize computation time when creating new decks.
        /// Initialized upon construction.
        /// </summary>
        private List<Card> _deck = new List<Card>();

        /// <summary>
        /// Internal discard queue object. Randomization is performed onto the deck when cards are shuffled.
        /// Initialized as empty upon construction.
        /// </summary>
        private List<Card> _discard = new List<Card>();

        public CardManager() 
        {
            List<Suit> suits = Enum.GetValues<Suit>().ToList();
            List<Value> values = Enum.GetValues<Value>().ToList();

            foreach (Suit suit in suits)
            {
                foreach (Value value in values)
                    _deck.Add(new Card(suit, value));
            }
        }

        /// <summary>
        /// Generates however many decks are requested, and places them into the list of cards.
        /// </summary>
        /// <param name="numDecks">The number of decks of cards to be generated.</param>
        public void GenerateDecks(int numDecks)
        {
            for (int i = 0; i < numDecks; i++)
            {
                foreach (Card card in _deck)
                    Cards.Add(new Card(card));
            }

            // Shuffle and cut the deck
            Shuffle();
        }

        /// <summary>
        /// Clears the stored cards.
        /// </summary>
        public void ClearDecks()
        {
            Cards.Clear();
        }

        /// <summary>
        /// Shuffles the cards in the deck.
        /// </summary>
        public void Shuffle()
        {
            Random random = new Random(); // Random number generator
            List<Card> splitDeck = new List<Card>(); // Temporary list to hold the split deck
            int noOfCards = Cards.Count; // Number of cards currently in the game's deck

            // Shuffle the deck twice
            for (int idx = 0; idx < 2; idx++)
            {
                for (int i = 0; i < noOfCards; i++)
                {
                    int randomIndex = random.Next(0, noOfCards);
                    if (randomIndex != i)
                        (Cards[i], Cards[randomIndex]) = (Cards[randomIndex], Cards[i]);
                }
            }

            // Cuts the deck in half and stack the halves so that the last card is in the middle of the deck
            for (int i = noOfCards / 2; i < noOfCards; i++)
            {
                splitDeck.Add(Cards[i]);
            }
            for (int i = 0; i < noOfCards / 2; i++)
            {
                splitDeck.Add(Cards[i]);
            }

            Cards = splitDeck;
        }

        /// <summary>
        /// Draws a card from the deck.
        /// </summary>
        /// <returns>The Card object removed from the deck.</returns>
        public Card DrawCard()
        {
            Card card = Cards[0];
            Cards.RemoveAt(0);
            return card;
        }
        
        /// <summary>
        /// Update method for the CardManager
        /// </summary>
        public void Update()
        {

        }

        /// <summary>
        /// Draw method for the CardManager
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {

        }

        /// <summary>
        /// Adds card to the discard queue. 
        /// Caller must guarantee that card has been removed from whatever collection it was in prior.
        /// </summary>
        public void Discard(Card card)
        {
            _discard.Add(card);
        }

        /// <summary>
        /// Moves all cards from discard into the deck, and randomizes the deck.  
        /// Guarantees that discard's old size + deck's old size = deck's new size, and that discard's new size = 0.
        /// </summary>
        public void Recycle() 
        {
            Cards.AddRange(_discard);
            _discard.Clear();

            Shuffle();
        }
        
        /// <summary>
        /// Gets the current size of a game's playable deck.
        /// </summary>
        /// <returns>An int representing the size of the deck.</returns>
        public int GetDeckSize()
        {
            return Cards.Count;
        }
    }

    /// <summary>
    /// Class <c>CardTextures</c> holds the textures for the cards.
    /// </summary>
    public static class CardTextures
    {
        /// <summary>
        /// Texture for the back of a card.
        /// </summary>
        public static Texture2D? CardBackTexture { get; set; }

        /// <summary>
        /// The texture for the Ace Of Clubs.
        /// </summary>
        public static Texture2D? AceOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the Two Of Clubs.
        /// </summary>
        public static Texture2D? TwoOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the Three Of Clubs.
        /// </summary>
        public static Texture2D? ThreeOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the Four Of Clubs.
        /// </summary>
        public static Texture2D? FourOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the Five Of Clubs.
        /// </summary>
        public static Texture2D? FiveOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the Six Of Clubs.
        /// </summary>
        public static Texture2D? SixOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the Seven Of Clubs.
        /// </summary>
        public static Texture2D? SevenOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the Eight Of Clubs.
        /// </summary>
        public static Texture2D? EightOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the Nine Of Clubs.
        /// </summary>
        public static Texture2D? NineOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the Ten Of Clubs.
        /// </summary>
        public static Texture2D? TenOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the Jack Of Clubs.
        /// </summary>
        public static Texture2D? JackOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the Queen Of Clubs.
        /// </summary>
        public static Texture2D? QueenOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the King Of Clubs.
        /// </summary>
        public static Texture2D? KingOfClubsTexture { get; set; }

        /// <summary>
        /// The texture for the Ace Of Diamonds.
        /// </summary>
        public static Texture2D? AceOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the Two Of Diamonds.
        /// </summary>
        public static Texture2D? TwoOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the Three Of Diamonds.
        /// </summary>
        public static Texture2D? ThreeOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the Four Of Diamonds.
        /// </summary>
        public static Texture2D? FourOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the Five Of Diamonds.
        /// </summary>
        public static Texture2D? FiveOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the Six Of Diamonds.
        /// </summary>
        public static Texture2D? SixOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the Seven Of Diamonds.
        /// </summary>
        public static Texture2D? SevenOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the Eight Of Diamonds.
        /// </summary>
        public static Texture2D? EightOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the Nine Of Diamonds.
        /// </summary>
        public static Texture2D? NineOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the Ten Of Diamonds.
        /// </summary>
        public static Texture2D? TenOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the Jack Of Diamonds.
        /// </summary>
        public static Texture2D? JackOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the Queen Of Diamonds.
        /// </summary>
        public static Texture2D? QueenOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the King Of Diamonds.
        /// </summary>
        public static Texture2D? KingOfDiamondsTexture { get; set; }

        /// <summary>
        /// The texture for the Ace Of Hearts.
        /// </summary>
        public static Texture2D? AceOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the Two Of Hearts.
        /// </summary>
        public static Texture2D? TwoOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the Three Of Hearts.
        /// </summary>
        public static Texture2D? ThreeOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the Four Of Hearts.
        /// </summary>
        public static Texture2D? FourOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the Five Of Hearts.
        /// </summary>
        public static Texture2D? FiveOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the Six Of Hearts.
        /// </summary>
        public static Texture2D? SixOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the Seven Of Hearts.
        /// </summary>
        public static Texture2D? SevenOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the Eight Of Hearts.
        /// </summary>
        public static Texture2D? EightOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the Nine Of Hearts.
        /// </summary>
        public static Texture2D? NineOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the Ten Of Hearts.
        /// </summary>
        public static Texture2D? TenOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the Jack Of Hearts.
        /// </summary>
        public static Texture2D? JackOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the Queen Of Hearts.
        /// </summary>
        public static Texture2D? QueenOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the King Of Hearts.
        /// </summary>
        public static Texture2D? KingOfHeartsTexture { get; set; }

        /// <summary>
        /// The texture for the Ace Of Spades.
        /// </summary>
        public static Texture2D? AceOfSpadesTexture { get; set; }

        /// <summary>
        /// The texture for the Two Of Spades.
        /// </summary>
        public static Texture2D? TwoOfSpadesTexture { get; set; }

        /// <summary>
        /// The texture for the Three Of Spades.
        /// </summary>
        public static Texture2D? ThreeOfSpadesTexture { get; set; }

        /// <summary>
        /// The texture for the Four Of Spades.
        /// </summary>
        public static Texture2D? FourOfSpadesTexture { get; set; }

        /// <summary>
        /// The texture for the Five Of Spades.
        /// </summary>
        public static Texture2D? FiveOfSpadesTexture { get; set; }

        /// <summary>
        /// The texture for the Six Of Spades.
        /// </summary>
        public static Texture2D? SixOfSpadesTexture { get; set; }

        /// <summary>
        /// The texture for the Seven Of Spades.
        /// </summary>
        public static Texture2D? SevenOfSpadesTexture { get; set; }

        /// <summary>
        /// The texture for the Eight Of Spades.
        /// </summary>
        public static Texture2D? EightOfSpadesTexture { get; set; }

        /// <summary>
        /// The texture for the Nine Of Spades.
        /// </summary>
        public static Texture2D? NineOfSpadesTexture { get; set; }

        /// <summary>
        /// The texture for the Ten Of Spades.
        /// </summary>
        public static Texture2D? TenOfSpadesTexture { get; set; }

        /// <summary>
        /// The texture for the Jack Of Spades.
        /// </summary>
        public static Texture2D? JackOfSpadesTexture { get; set; }

        /// <summary>
        /// The texture for the Queen Of Spades.
        /// </summary>
        public static Texture2D? QueenOfSpadesTexture { get; set; }

        /// <summary>
        /// The texture for the King Of Spades.
        /// </summary>
        public static Texture2D? KingOfSpadesTexture { get; set; }

        /// <summary>
        /// Loads the content for the card textures.
        /// </summary>
        public static void LoadContent(ContentManager content)
        {
            CardBackTexture = content.Load<Texture2D>("Card_Back");
            AceOfClubsTexture = content.Load<Texture2D>("Clubs_A");
            TwoOfClubsTexture = content.Load<Texture2D>("Clubs_2");
            ThreeOfClubsTexture = content.Load<Texture2D>("Clubs_3");
            FourOfClubsTexture = content.Load<Texture2D>("Clubs_4");
            FiveOfClubsTexture = content.Load<Texture2D>("Clubs_5");
            SixOfClubsTexture = content.Load<Texture2D>("Clubs_6");
            SevenOfClubsTexture = content.Load<Texture2D>("Clubs_7");
            EightOfClubsTexture = content.Load<Texture2D>("Clubs_8");
            NineOfClubsTexture = content.Load<Texture2D>("Clubs_9");
            TenOfClubsTexture = content.Load<Texture2D>("Clubs_10");
            JackOfClubsTexture = content.Load<Texture2D>("Clubs_J");
            QueenOfClubsTexture = content.Load<Texture2D>("Clubs_Q");
            KingOfClubsTexture = content.Load<Texture2D>("Clubs_K");
            AceOfDiamondsTexture = content.Load<Texture2D>("Diamonds_A");
            TwoOfDiamondsTexture = content.Load<Texture2D>("Diamonds_2");
            ThreeOfDiamondsTexture = content.Load<Texture2D>("Diamonds_3");
            FourOfDiamondsTexture = content.Load<Texture2D>("Diamonds_4");
            FiveOfDiamondsTexture = content.Load<Texture2D>("Diamonds_5");
            SixOfDiamondsTexture = content.Load<Texture2D>("Diamonds_6");
            SevenOfDiamondsTexture = content.Load<Texture2D>("Diamonds_7");
            EightOfDiamondsTexture = content.Load<Texture2D>("Diamonds_8");
            NineOfDiamondsTexture = content.Load<Texture2D>("Diamonds_9");
            TenOfDiamondsTexture = content.Load<Texture2D>("Diamonds_10");
            JackOfDiamondsTexture = content.Load<Texture2D>("Diamonds_J");
            QueenOfDiamondsTexture = content.Load<Texture2D>("Diamonds_Q");
            KingOfDiamondsTexture = content.Load<Texture2D>("Diamonds_K");
            AceOfHeartsTexture = content.Load<Texture2D>("Hearts_A");
            TwoOfHeartsTexture = content.Load<Texture2D>("Hearts_2");
            ThreeOfHeartsTexture = content.Load<Texture2D>("Hearts_3");
            FourOfHeartsTexture = content.Load<Texture2D>("Hearts_4");
            FiveOfHeartsTexture = content.Load<Texture2D>("Hearts_5");
            SixOfHeartsTexture = content.Load<Texture2D>("Hearts_6");
            SevenOfHeartsTexture = content.Load<Texture2D>("Hearts_7");
            EightOfHeartsTexture = content.Load<Texture2D>("Hearts_8");
            NineOfHeartsTexture = content.Load<Texture2D>("Hearts_9");
            TenOfHeartsTexture = content.Load<Texture2D>("Hearts_10");
            JackOfHeartsTexture = content.Load<Texture2D>("Hearts_J");
            QueenOfHeartsTexture = content.Load<Texture2D>("Hearts_Q");
            KingOfHeartsTexture = content.Load<Texture2D>("Hearts_K");
            AceOfSpadesTexture = content.Load<Texture2D>("Spades_A");
            TwoOfSpadesTexture = content.Load<Texture2D>("Spades_2");
            ThreeOfSpadesTexture = content.Load<Texture2D>("Spades_3");
            FourOfSpadesTexture = content.Load<Texture2D>("Spades_4");
            FiveOfSpadesTexture = content.Load<Texture2D>("Spades_5");
            SixOfSpadesTexture = content.Load<Texture2D>("Spades_6");
            SevenOfSpadesTexture = content.Load<Texture2D>("Spades_7");
            EightOfSpadesTexture = content.Load<Texture2D>("Spades_8");
            NineOfSpadesTexture = content.Load<Texture2D>("Spades_9");
            TenOfSpadesTexture = content.Load<Texture2D>("Spades_10");
            JackOfSpadesTexture = content.Load<Texture2D>("Spades_J");
            QueenOfSpadesTexture = content.Load<Texture2D>("Spades_Q");
            KingOfSpadesTexture = content.Load<Texture2D>("Spades_K");
        }

        /// <summary>
        /// Returns the card texture.
        /// </summary>
        public static Texture2D GetCardTexture(Value value, Suit suit)
        {
            return suit switch
            {
                Suit.CLUBS => GetClubsTexture(value),
                Suit.DIAMONDS => GetDiamondsTexture(value),
                Suit.HEARTS => GetHeartsTexture(value),
                _ => GetSpadesTexture(value)
            };
        }

        /// <summary>
        /// Returns the Clubs card texture.
        /// </summary>
        private static Texture2D GetClubsTexture(Value value)
        {
            return value switch
            {
                Value.ACE => AceOfClubsTexture!,
                Value.TWO => TwoOfClubsTexture!,
                Value.THREE => ThreeOfClubsTexture!,
                Value.FOUR => FourOfClubsTexture!,
                Value.FIVE => FiveOfClubsTexture!,
                Value.SIX => SixOfClubsTexture!,
                Value.SEVEN => SevenOfClubsTexture!,
                Value.EIGHT => EightOfClubsTexture!,
                Value.NINE => NineOfClubsTexture!,
                Value.TEN => TenOfClubsTexture!,
                Value.JACK => JackOfClubsTexture!,
                Value.QUEEN => QueenOfClubsTexture!,
                _ => KingOfClubsTexture!
            };
        }

        /// <summary>
        /// Returns the Diamonds card texture.
        /// </summary>
        private static Texture2D GetDiamondsTexture(Value value)
        {
            return value switch
            {
                Value.ACE => AceOfDiamondsTexture!,
                Value.TWO => TwoOfDiamondsTexture!,
                Value.THREE => ThreeOfDiamondsTexture!,
                Value.FOUR => FourOfDiamondsTexture!,
                Value.FIVE => FiveOfDiamondsTexture!,
                Value.SIX => SixOfDiamondsTexture!,
                Value.SEVEN => SevenOfDiamondsTexture!,
                Value.EIGHT => EightOfDiamondsTexture!,
                Value.NINE => NineOfDiamondsTexture!,
                Value.TEN => TenOfDiamondsTexture!,
                Value.JACK => JackOfDiamondsTexture!,
                Value.QUEEN => QueenOfDiamondsTexture!,
                _ => KingOfDiamondsTexture!
            };
        }

        /// <summary>
        /// Returns the Hearts card texture.
        /// </summary>
        private static Texture2D GetHeartsTexture(Value value)
        {
            return value switch
            {
                Value.ACE => AceOfHeartsTexture!,
                Value.TWO => TwoOfHeartsTexture!,
                Value.THREE => ThreeOfHeartsTexture!,
                Value.FOUR => FourOfHeartsTexture!,
                Value.FIVE => FiveOfHeartsTexture!,
                Value.SIX => SixOfHeartsTexture!,
                Value.SEVEN => SevenOfHeartsTexture!,
                Value.EIGHT => EightOfHeartsTexture!,
                Value.NINE => NineOfHeartsTexture!,
                Value.TEN => TenOfHeartsTexture!,
                Value.JACK => JackOfHeartsTexture!,
                Value.QUEEN => QueenOfHeartsTexture!,
                _ => KingOfHeartsTexture!
            };
        }

        /// <summary>
        /// Returns the Spades card texture.
        /// </summary>
        private static Texture2D GetSpadesTexture(Value value)
        {
            return value switch
            {
                Value.ACE => AceOfSpadesTexture!,
                Value.TWO => TwoOfSpadesTexture!,
                Value.THREE => ThreeOfSpadesTexture!,
                Value.FOUR => FourOfSpadesTexture!,
                Value.FIVE => FiveOfSpadesTexture!,
                Value.SIX => SixOfSpadesTexture!,
                Value.SEVEN => SevenOfSpadesTexture!,
                Value.EIGHT => EightOfSpadesTexture!,
                Value.NINE => NineOfSpadesTexture!,
                Value.TEN => TenOfSpadesTexture!,
                Value.JACK => JackOfSpadesTexture!,
                Value.QUEEN => QueenOfSpadesTexture!,
                _ => KingOfSpadesTexture!
            };
        }
    }
}
