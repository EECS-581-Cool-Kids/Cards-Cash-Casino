using CardsCashCasino.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CardsCashCasino.Manager
{
    public class BlackjackManager
    {
        #region Properties
        /// <summary>
        /// The dealer's hand.
        /// </summary>
        private DealerHand _dealerHand = new();

        /// <summary>
        /// The user hands. Needs to be a list to handle splitting.
        /// </summary>
        private List<UserHand> _userHands = new();

        /// <summary>
        /// The selected user hand.
        /// </summary>
        private int _selectedUserHand = 0;

        /// <summary>
        /// The current position of the cursor.
        /// </summary>
        private int _currentCursorPos = 0;

        /// <summary>
        /// Checks if the game is actively running. Used to call the update and draw loops.
        /// </summary>
        public bool IsPlaying { get; private set; } = false;

        /// <summary>
        /// Checks if the user is still playing.
        /// </summary>
        private bool _userPlaying = false;

        /// <summary>
        /// The hit button.
        /// </summary>
        private BlackjackActionButton? _hitButton;

        /// <summary>
        /// The stand button.
        /// </summary>
        private BlackjackActionButton? _standButton;

        /// <summary>
        /// The double down button.
        /// </summary>
        private BlackjackActionButton? _doubleDownButton;

        /// <summary>
        /// The split button.
        /// </summary>
        private BlackjackActionButton? _splitButton;

        /// <summary>
        /// The hit button.
        /// </summary>
        private BlackjackActionButton? _forfeitButton;

        /// <summary>
        /// The cursor.
        /// </summary>
        private BlackjackCursor? _cursor;

        /// <summary>
        /// The value indicator for the dealer hand.
        /// </summary>
        private BlackjackValueIndicator? _dealerHandValueIndicator;

        /// <summary>
        /// The value indicator for the current user hand.
        /// </summary>
        private BlackjackValueIndicator? _userHandValueIndicator;

        /// <summary>
        /// The timeout for the cursor to move.
        /// </summary>
        private Timer? _cursorMoveTimeout;

        /// <summary>
        /// The timeout for the user "hitting" their deck.
        /// </summary>
        private Timer? _userMoveTimeout;

        /// <summary>
        /// The timeout for the dealer "hitting" their deck.
        /// </summary>
        private Timer? _dealerMoveTimeout;

        /// <summary>
        /// Call to request clearing the stored decks of cards.
        /// </summary>
        public Action? RequestCardManagerCleared { get; set; }

        /// <summary>
        /// Call to request decks of cards be added to the queue of cards.
        /// </summary>
        public Action<int>? RequestDecksOfCards { get; set; }

        /// <summary>
        /// Call to request an individual card be added.
        /// </summary>
        public Func<Card>? RequestCard { get; set; }
        #endregion Properties

        /// <summary>
        /// The LoadContent method for blackjack.
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            BlackjackTextures.LoadContent(content);

            _hitButton = new(BlackjackTextures.HitEnabledTexture!, BlackjackTextures.HitDisabledTexture!, 50, 400); // TODO dynamically get these sizes.
            _standButton = new(BlackjackTextures.StandEnabledTexture!, BlackjackTextures.StandDisabledTexture!, 194, 400); // TODO dynamically get these sizes.
            _doubleDownButton = new(BlackjackTextures.DoubleDownEnabledTexture!, BlackjackTextures.DoubleDownDisabledTexture!, 338, 400); // TODO dynamically get these sizes.
            _splitButton = new(BlackjackTextures.SplitEnabledTexture!, BlackjackTextures.SplitDisabledTexture!, 482, 400); // TODO dynamically get these sizes.
            _forfeitButton = new(BlackjackTextures.ForfeitEnabledTexture!, BlackjackTextures.ForfeitDisabledTexture!, 626, 400); // TODO dynamically get these sizes.

            _hitButton.IsEnabled = true;
            _standButton.IsEnabled = true;
            _doubleDownButton.IsEnabled = true;
            _forfeitButton.IsEnabled = true;

            _cursor = new(BlackjackTextures.CursorTexture!, _hitButton.GetAdjustedPos());

            _dealerHandValueIndicator = new(BlackjackTextures.ZeroTexture!, BlackjackTextures.ZeroTexture!);
            _userHandValueIndicator = new(BlackjackTextures.ZeroTexture!, BlackjackTextures.ZeroTexture!);

            StartGame(); // temp
        }

        /// <summary>
        /// The main update loop for blackjack.
        /// </summary>
        public void Update()
        {
            if (_userPlaying)
                UpdateWhileUserPlaying();
            else
                UpdateWhileDealerPlaying();
        }

        /// <summary>
        /// Update loop while the dealer is playing.
        /// </summary>
        private void UpdateWhileDealerPlaying()
        {
            if (_dealerMoveTimeout is not null && _dealerMoveTimeout.Enabled)
                return;

            int dealerHandValue = _dealerHand.GetBlackjackValue();

            if (dealerHandValue < 17 || (dealerHandValue == 17 && _dealerHand.IsSoftBlackjackValue))
            {
                _dealerHand.AddCard(RequestCard!.Invoke());
                dealerHandValue = _dealerHand.GetBlackjackValue();

                _dealerHandValueIndicator!.Update(dealerHandValue);

                _dealerMoveTimeout = new(1000);
                _dealerMoveTimeout.Elapsed += OnTimeoutEvent!;
                _dealerMoveTimeout.Start();
            }
            else
            {
                if (dealerHandValue > 21)
                {
                    // TODO win logic with poker chips

                    Debug.WriteLine("The dealer has bust and user has won! Restarting the game."); // TODO remove once we decide how to indicate round transition.
                }
                else
                {
                    foreach (UserHand hand in _userHands)
                    {
                        if (dealerHandValue > hand.GetBlackjackValue())
                        {
                            // TODO lose logic with poker chips

                            Debug.WriteLine("The user has lost! Restarting the game."); // TODO remove once we decide how to indicate round transition.
                        }
                        else if (dealerHandValue == hand.GetBlackjackValue())
                        {
                            // TODO push logic with poker chips

                            Debug.WriteLine("The user has pushed! Restarting the game."); // TODO remove once we decide how to indicate round transition.
                        }
                        else
                        {
                            // TODO win logic with poker chips

                            Debug.WriteLine("The user has won! Restarting the game."); // TODO remove once we decide how to indicate round transition.
                        }
                    }
                }

                EndGame();
            }
        }

        /// <summary>
        /// Update loop while the user is playing.
        /// </summary>
        private void UpdateWhileUserPlaying()
        {
            _splitButton!.IsEnabled = _userHands[_selectedUserHand].CanSplit();

            if (Keyboard.GetState().IsKeyDown(Keys.Right) && (_cursorMoveTimeout is null || !_cursorMoveTimeout.Enabled))
            {
                _currentCursorPos++;

                if (_currentCursorPos >= 5)
                    _currentCursorPos = 0;

                if (!_userHands[_selectedUserHand].CanSplit() && _currentCursorPos == 3)
                    _currentCursorPos++;

                _cursor!.UpdateLocation(GetNewCursorPos());

                _cursorMoveTimeout = new Timer(100);
                _cursorMoveTimeout.Elapsed += OnTimeoutEvent!;
                _cursorMoveTimeout.Start();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Left) && (_cursorMoveTimeout is null || !_cursorMoveTimeout.Enabled))
            {
                _currentCursorPos--;

                if (_currentCursorPos < 0)
                    _currentCursorPos = 0;

                if (!_userHands[_selectedUserHand].CanSplit() && _currentCursorPos == 3)
                    _currentCursorPos--;

                _cursor!.UpdateLocation(GetNewCursorPos());

                _cursorMoveTimeout = new Timer(100);
                _cursorMoveTimeout.Elapsed += OnTimeoutEvent!;
                _cursorMoveTimeout.Start();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (_userMoveTimeout is not null && _userMoveTimeout.Enabled)
                    return;

                switch (_currentCursorPos)
                {
                    case 0: // Hit
                        Hit();
                        break;
                    case 1: // Stand
                        FinishHand();
                        break;
                    case 2: // Double
                        DoubleDown();
                        break;
                    case 3: // Split
                        Split();
                        break;
                    case 4: // Forfeit
                        Forfeit();
                        break;
                }

                _userMoveTimeout = new(200);
                _userMoveTimeout.Elapsed += OnTimeoutEvent!;
                _userMoveTimeout.Start();
            }
        }

        /// <summary>
        /// Gets the new Cursor position.
        /// </summary>
        private Point GetNewCursorPos()
        {
            return _currentCursorPos switch
            {
                1 => _standButton!.GetAdjustedPos(),
                2 => _doubleDownButton!.GetAdjustedPos(),
                3 => _splitButton!.GetAdjustedPos(),
                4 => _forfeitButton!.GetAdjustedPos(),
                _ => _hitButton!.GetAdjustedPos()
            };
        }

        /// <summary>
        /// The main draw loop for blackjack.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the buttons
            _hitButton!.Draw(spriteBatch);
            _standButton!.Draw(spriteBatch);
            _doubleDownButton!.Draw(spriteBatch);
            _splitButton!.Draw(spriteBatch);
            _forfeitButton!.Draw(spriteBatch);

            // Draw the cursor
            _cursor!.Draw(spriteBatch);

            // Draw the dealer hand
            _dealerHand.Draw(spriteBatch);

            // Draw the current user hand
            _userHands[_selectedUserHand].Draw(spriteBatch);

            // Draw the value indicators
            _dealerHandValueIndicator!.Draw(spriteBatch);
            _userHandValueIndicator!.Draw(spriteBatch);
        }

        /// <summary>
        /// Initializes and starts the game.
        /// </summary>
        public void StartGame()
        {
            RequestCardManagerCleared!.Invoke();
            RequestDecksOfCards!.Invoke(4);

            UserHand initialHand = new();

            initialHand.SetCenter(400, 300); // TODO do dynamically
            _dealerHand.SetCenter(400, 75); // TODO do dynamically

            _userHandValueIndicator!.SetPosition(379, 200); // TODO do dynamically
            _dealerHandValueIndicator!.SetPosition(379, 150); // TODO do dynamically

            initialHand.AddCard(new Card(Suit.CLUBS, Value.FIVE));
            _dealerHand.AddCard(RequestCard!.Invoke());
            initialHand.AddCard(new Card(Suit.DIAMONDS, Value.FIVE));
            _dealerHand.AddCard(RequestCard!.Invoke());

            _userHands.Add(initialHand);

            _dealerHandValueIndicator.Update(_dealerHand.GetBlackjackValue());
            _userHandValueIndicator.Update(initialHand.GetBlackjackValue());

            IsPlaying = true;
            _userPlaying = true;
        }

        /// <summary>
        /// End game logic.
        /// </summary>
        public void EndGame()
        {
            // TODO add option to select a new game

            _dealerHand.Clear();
            _userHands.Clear();

            System.Threading.Thread.Sleep(500); // temp

            StartGame();
        }

        /// <summary>
        /// The hit action.
        /// </summary>
        private void Hit()
        {
            UserHand currentHand = _userHands[_selectedUserHand];
            currentHand.AddCard(RequestCard!.Invoke());

            if (currentHand.GetBlackjackValue() <= 21)
            {
                _userHandValueIndicator!.Update(currentHand.GetBlackjackValue());
            }
            else
            {
                // TODO bust logic with poker chips

                Debug.WriteLine("The user has bust! Restarting the game."); // TODO remove once we decide how to indicate round transition.

                EndGame();
            }
        }

        /// <summary>
        /// The double action.
        /// </summary>
        private void DoubleDown()
        {
            UserHand currentHand = _userHands[_selectedUserHand];

            currentHand.AddCard(RequestCard!.Invoke());
            _userHandValueIndicator!.Update(currentHand.GetBlackjackValue());

            FinishHand();
        }

        /// <summary>
        /// The split action.
        /// </summary>
        private void Split()
        {
            UserHand currentHand = _userHands[_selectedUserHand];

            UserHand newHand = new();
            newHand.AddCard(currentHand.RemoveLastCard());

            _userHands.Add(newHand);

            currentHand.RecalculateCardPositions();

            _userHandValueIndicator!.Update(currentHand.GetBlackjackValue());
        }

        /// <summary>
        /// The forfeit action.
        /// </summary>
        private void Forfeit()
        {
            // TODO forfeit logic with poker chips.

            Debug.WriteLine("The user has forfeit! Restarting the game."); // TODO remove once we decide how to indicate round transition.

            EndGame();
        }

        /// <summary>
        /// Finishes the given hand when it has not bust.
        /// </summary>
        private void FinishHand()
        {
            _selectedUserHand++;

            if (_selectedUserHand >= _userHands.Count)
            {
                _selectedUserHand--;
                BeginDealerTurn();
            }
            else
            {
                _userHandValueIndicator!.Update(_userHands[_selectedUserHand].GetBlackjackValue());
            }
        }

        /// <summary>
        /// Begins the dealer's turn.
        /// </summary>
        private void BeginDealerTurn()
        {
            _dealerHand.UnhideCard();
            _dealerHandValueIndicator!.Update(_dealerHand.GetBlackjackValue());
            _userPlaying = false;
        }

        /// <summary>
        /// Event called when the placement timer times out.
        /// </summary>
        private static void OnTimeoutEvent(object source, ElapsedEventArgs e)
        {
            // Stop and dispose of the timer
            Timer timer = (Timer)source;
            timer.Stop();
            timer.Dispose();
        }
    }

    public static class BlackjackTextures
    {
        /// <summary>
        /// The enabled texture for the Hit button.
        /// </summary>
        public static Texture2D? HitEnabledTexture { get; private set; }

        /// <summary>
        /// The disabled texture for the Hit button.
        /// </summary>
        public static Texture2D? HitDisabledTexture { get; private set; }

        /// <summary>
        /// The enabled texture for the Stand button.
        /// </summary>
        public static Texture2D? StandEnabledTexture { get; private set; }

        /// <summary>
        /// The disabled texture for the Stand button.
        /// </summary>
        public static Texture2D? StandDisabledTexture { get; private set; }

        /// <summary>
        /// The enabled texture for the Double Down button.
        /// </summary>
        public static Texture2D? DoubleDownEnabledTexture { get; private set; }

        /// <summary>
        /// The disabled texture for the Double Down button.
        /// </summary>
        public static Texture2D? DoubleDownDisabledTexture { get; private set; }

        /// <summary>
        /// The enabled texture for the Split button.
        /// </summary>
        public static Texture2D? SplitEnabledTexture { get; private set; }

        /// <summary>
        /// The disabled texture for the Split button.
        /// </summary>
        public static Texture2D? SplitDisabledTexture { get; private set; }

        /// <summary>
        /// The enabled texture for the Forfeit button.
        /// </summary>
        public static Texture2D? ForfeitEnabledTexture { get; private set; }

        /// <summary>
        /// The disabled texture for the Forfeit button.
        /// </summary>
        public static Texture2D? ForfeitDisabledTexture { get; private set; }

        /// <summary>
        /// The cursor's texture.
        /// </summary>
        public static Texture2D? CursorTexture { get; private set; }

        /// <summary>
        /// Texture for zero.
        /// </summary>
        public static Texture2D? ZeroTexture { get; private set; }

        /// <summary>
        /// Texture for one.
        /// </summary>
        public static Texture2D? OneTexture { get; private set; }

        /// <summary>
        /// Texture for two.
        /// </summary>
        public static Texture2D? TwoTexture { get; private set; }

        /// <summary>
        /// Texture for three.
        /// </summary>
        public static Texture2D? ThreeTexture { get; private set; }

        /// <summary>
        /// Texture for four.
        /// </summary>
        public static Texture2D? FourTexture { get; private set; }

        /// <summary>
        /// Texture for five.
        /// </summary>
        public static Texture2D? FiveTexture { get; private set; }

        /// <summary>
        /// Texture for six.
        /// </summary>
        public static Texture2D? SixTexture { get; private set; }

        /// <summary>
        /// Texture for seven.
        /// </summary>
        public static Texture2D? SevenTexture { get; private set; }

        /// <summary>
        /// Texture for eight.
        /// </summary>
        public static Texture2D? EightTexture { get; private set; }

        /// <summary>
        /// Texture for nine.
        /// </summary>
        public static Texture2D? NineTexture { get; private set; }

        /// <summary>
        /// Loads the assets for Blackjack.
        /// </summary>
        public static void LoadContent(ContentManager content)
        {
            HitEnabledTexture = content.Load<Texture2D>("HitEnabled");
            HitDisabledTexture = content.Load<Texture2D>("HitDisabled");
            StandEnabledTexture = content.Load<Texture2D>("StandEnabled");
            StandDisabledTexture = content.Load<Texture2D>("StandDisabled");
            DoubleDownEnabledTexture = content.Load<Texture2D>("DoubleEnabled");
            DoubleDownDisabledTexture = content.Load<Texture2D>("DoubleDisabled");
            SplitEnabledTexture = content.Load<Texture2D>("SplitEnabled");
            SplitDisabledTexture = content.Load<Texture2D>("SplitDisabled");
            ForfeitEnabledTexture = content.Load<Texture2D>("ForfeitEnabled");
            ForfeitDisabledTexture = content.Load<Texture2D>("ForfeitDisabled");
            CursorTexture = content.Load<Texture2D>("BlackjackCursor");
            ZeroTexture = content.Load<Texture2D>("zero");
            OneTexture = content.Load<Texture2D>("one");
            TwoTexture = content.Load<Texture2D>("two");
            ThreeTexture = content.Load<Texture2D>("three");
            FourTexture = content.Load<Texture2D>("four");
            FiveTexture = content.Load<Texture2D>("five");
            SixTexture = content.Load<Texture2D>("six");
            SevenTexture = content.Load<Texture2D>("seven");
            EightTexture = content.Load<Texture2D>("eight");
            NineTexture = content.Load<Texture2D>("nine");
        }
    }

    public class BlackjackCursor
    {
        /// <summary>
        /// The texture for the cursor.
        /// </summary>
        private Texture2D _cursorTexture;

        /// <summary>
        /// The rectangle object for the cursor.
        /// </summary>
        private Rectangle _cursorRectangle;

        /// <summary>
        /// The size of the cursor.
        /// </summary>
        private Point _size = new(144, 80);

        public BlackjackCursor(Texture2D cursorTexture, Point location)
        {
            _cursorTexture = cursorTexture;
            _cursorRectangle = new Rectangle(location, _size);
        }

        /// <summary>
        /// Updates the location of the cursor.
        /// </summary>
        public void UpdateLocation(Point Location)
        {
            _cursorRectangle.X = Location.X;
            _cursorRectangle.Y = Location.Y;
        }

        /// <summary>
        /// The draw method for the cursor.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_cursorTexture, _cursorRectangle, Color.White);
        }
    }

    public class BlackjackActionButton
    {
        /// <summary>
        /// The disabled button texture.
        /// </summary>
        private Texture2D _disabledTexture;

        /// <summary>
        /// The unselected button texture.
        /// </summary>
        private Texture2D _enabledTexture;

        /// <summary>
        /// The rectangle for the button.
        /// </summary>
        private Rectangle _buttonRectangle;

        /// <summary>
        /// Whether or not the button is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = false;

        /// <summary>
        /// Whether or not the button is selected.
        /// </summary>
        public bool IsSelected { get; set; } = false;

        public BlackjackActionButton(Texture2D enabledTexture, Texture2D disabledTexture, int xPos, int yPos)
        {
            _enabledTexture = enabledTexture;
            _disabledTexture = disabledTexture;
            _buttonRectangle = new Rectangle(xPos, yPos, 128, 64);
        }

        /// <summary>
        /// The draw method for the button.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsEnabled)
                spriteBatch.Draw(_disabledTexture, _buttonRectangle, Color.White);
            else
                spriteBatch.Draw(_enabledTexture, _buttonRectangle, Color.White);
        }

        /// <summary>
        /// Gets the location where the cursor will be.
        /// </summary>
        public Point GetAdjustedPos()
        {
            return new Point(_buttonRectangle.X-8, _buttonRectangle.Y-8);
        }
    }

    public class BlackjackValueIndicator
    {
        /// <summary>
        /// Texture for the first digit.
        /// </summary>
        private Texture2D _firstDigitTexture;

        /// <summary>
        /// Texture for the second digit.
        /// </summary>
        private Texture2D _secondDigitTexture;

        /// <summary>
        /// Rectnagle for the first digit.
        /// </summary>
        private Rectangle? _firstDigitRectangle;

        /// <summary>
        /// Rectangle for the second digit.
        /// </summary>
        private Rectangle? _secondDigitRectangle;

        public BlackjackValueIndicator(Texture2D firstDigitTexture, Texture2D secondDigitTexture)
        {
            _firstDigitTexture = firstDigitTexture;
            _secondDigitTexture = secondDigitTexture;
        }

        /// <summary>
        /// Sets the position of the indicator.
        /// </summary>
        /// <param name="xPos">The x coordinate</param>
        /// <param name="yPos">The y coordinate</param>
        public void SetPosition(int xPos, int yPos)
        {
            _firstDigitRectangle = new Rectangle(xPos, yPos, 21, 24);
            _secondDigitRectangle = new Rectangle(xPos + 21, yPos, 21, 24);
        }

        /// <summary>
        /// Updates the indicator based on the value of the hand.
        /// </summary>
        public void Update(int handValue)
        {
            int firstDigit = handValue / 10;
            int secondDigit = handValue % 10;

            switch (firstDigit)
            {
                case 0:
                    _firstDigitTexture = BlackjackTextures.ZeroTexture!;
                    break;
                case 1:
                    _firstDigitTexture = BlackjackTextures.OneTexture!;
                    break;
                case 2:
                    _firstDigitTexture = BlackjackTextures.TwoTexture!;
                    break;
            }

            switch (secondDigit)
            {
                case 0:
                    _secondDigitTexture = BlackjackTextures.ZeroTexture!;
                    break;
                case 1:
                    _secondDigitTexture = BlackjackTextures.OneTexture!;
                    break;
                case 2:
                    _secondDigitTexture = BlackjackTextures.TwoTexture!;
                    break;
                case 3:
                    _secondDigitTexture = BlackjackTextures.ThreeTexture!;
                    break;
                case 4:
                    _secondDigitTexture = BlackjackTextures.FourTexture!;
                    break;
                case 5:
                    _secondDigitTexture = BlackjackTextures.FiveTexture!;
                    break;
                case 6:
                    _secondDigitTexture = BlackjackTextures.SixTexture!;
                    break;
                case 7:
                    _secondDigitTexture = BlackjackTextures.SevenTexture!;
                    break;
                case 8:
                    _secondDigitTexture = BlackjackTextures.EightTexture!;
                    break;
                case 9:
                    _secondDigitTexture = BlackjackTextures.NineTexture!;
                    break;
            }
        }

        /// <summary>
        /// Draw method for the value indicator.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_firstDigitRectangle is not null)
                spriteBatch.Draw(_firstDigitTexture, (Rectangle)_firstDigitRectangle, Color.White);
            if (_secondDigitRectangle is not null)
                spriteBatch.Draw(_secondDigitTexture, (Rectangle)_secondDigitRectangle, Color.White);
        }
    }
}
