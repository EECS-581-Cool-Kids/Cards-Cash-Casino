using CardsCashCasino.Data;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Manager
{
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
        /// Shuffles the cards in the deck by randomizing their order twice and splitting the deck.
        /// </summary>
        public void Shuffle()
        {
            Random random = new Random(); // Random number generator
            List<Card> splitDeck = new List<Card>(); // Temporary list to hold the split deck
            noOfCards = Cards.Count; // Number of cards in the deck

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
           for (int i = 0; i < noOfCards / 2; i++)
           {
                splitDeck.Insert((i + noOfCards / 2), Cards[i];
                splitDeck.Insert(i, Cards[i + noOfCards / 2]);
           }

              Cards = splitDeck;
        }

        /// <summary>
        /// Draws a card from the deck and returns it.
        /// </summary>
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
    }

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
            CardBackTexture = content.Load<Texture2D>("TEMP_CARD");
            AceOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            TwoOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            ThreeOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            FourOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            FiveOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            SixOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            SevenOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            EightOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            NineOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            TenOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            JackOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            QueenOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            KingOfClubsTexture = content.Load<Texture2D>("TEMP_CARD");
            AceOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            TwoOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            ThreeOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            FourOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            FiveOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            SixOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            SevenOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            EightOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            NineOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            TenOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            JackOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            QueenOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            KingOfDiamondsTexture = content.Load<Texture2D>("TEMP_CARD");
            AceOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            TwoOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            ThreeOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            FourOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            FiveOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            SixOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            SevenOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            EightOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            NineOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            TenOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            JackOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            QueenOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            KingOfHeartsTexture = content.Load<Texture2D>("TEMP_CARD");
            AceOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
            TwoOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
            ThreeOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
            FourOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
            FiveOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
            SixOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
            SevenOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
            EightOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
            NineOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
            TenOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
            JackOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
            QueenOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
            KingOfSpadesTexture = content.Load<Texture2D>("TEMP_CARD");
        }
    }
}
