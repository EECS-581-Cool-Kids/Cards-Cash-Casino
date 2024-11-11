using CardsCashCasino.Data;
using CardsCashCasino.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Timers;

namespace CardsCashCasino.Manager
{
    public class BlackjackManager
    {
        #region Properties
        /// <summary>
        /// The dealer's hand.
        /// </summary>
        private BlackjackDealerHand _dealerHand = new();

        /// <summary>
        /// The user hands. Needs to be a list to handle splitting.
        /// </summary>
        private List<BlackjackUserHand> _userHands = new();

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
        /// Checks if the round is finished at the end of the dealer's turn. Forces the game to continue.
        /// </summary>
        private bool _roundFinished = false;

        /// <summary>
        /// Checks if the user has bust.
        /// </summary>
        private bool _userBust = false;

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
        private BlackjackHandValueIndicator? _dealerHandValueIndicator;

        /// <summary>
        /// The value indicator for the current user hand.
        /// </summary>
        private BlackjackHandValueIndicator? _userHandValueIndicator;

        /// <summary>
        /// The result label for the game.
        /// </summary>
        private BlackjackResultLabel? _resultLabel;

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
        /// The timeout for the round finish between the label appearing and disappearing.
        /// </summary>
        private Timer? _roundFinishTimeout;

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
        public void LoadContent()
        {
            int widthBuffer = (Constants.WINDOW_WIDTH - Constants.BUTTON_WIDTH * Constants.BLACKJACK_BUTTON_COUNT) / 2;
            int buttonYPos = Constants.WINDOW_HEIGHT - 100;

            _hitButton = new(BlackjackTextures.HitEnabledTexture!, BlackjackTextures.HitDisabledTexture!, widthBuffer, buttonYPos);
            _standButton = new(BlackjackTextures.StandEnabledTexture!, BlackjackTextures.StandDisabledTexture!, widthBuffer + Constants.BUTTON_WIDTH, buttonYPos); 
            _doubleDownButton = new(BlackjackTextures.DoubleDownEnabledTexture!, BlackjackTextures.DoubleDownDisabledTexture!, widthBuffer + Constants.BUTTON_WIDTH*2, buttonYPos); 
            _splitButton = new(BlackjackTextures.SplitEnabledTexture!, BlackjackTextures.SplitDisabledTexture!, widthBuffer + Constants.BUTTON_WIDTH*3, buttonYPos); 
            _forfeitButton = new(BlackjackTextures.ForfeitEnabledTexture!, BlackjackTextures.ForfeitDisabledTexture!, widthBuffer + Constants.BUTTON_WIDTH*4, buttonYPos); 

            _cursor = new(BlackjackTextures.CursorTexture!, _hitButton.GetAdjustedPos());

            _dealerHandValueIndicator = new();
            _userHandValueIndicator = new();

            _resultLabel = new((Constants.WINDOW_WIDTH / 2) - Constants.RESULT_LABEL_OFFSET, (Constants.WINDOW_HEIGHT / 2) - Constants.RESULT_LABEL_OFFSET);

            // StartGame(); // temporary call. TODO remove when main menu is implemented or comment out to test Poker implementation.
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

            if (dealerHandValue < Constants.DEALER_HIT_THRESHOLD || (dealerHandValue == Constants.DEALER_HIT_THRESHOLD && _dealerHand.IsSoftBlackjackValue))
            {
                _dealerHand.AddCard(RequestCard!.Invoke());
                dealerHandValue = _dealerHand.GetBlackjackValue();

                _dealerHandValueIndicator!.Update(dealerHandValue);

                _dealerMoveTimeout = new(500);
                _dealerMoveTimeout.Elapsed += OnTimeoutEvent!;
                _dealerMoveTimeout.Start();
            }
            else if ((_roundFinishTimeout is null || !_roundFinishTimeout.Enabled) && !_roundFinished)
            {
                if (dealerHandValue > Constants.MAX_BLACKJACK_VALUE)
                {
                    // TODO win logic with poker chips

                    _resultLabel!.SetTexture(BlackjackResult.WIN);

                    _resultLabel!.CanDraw = true;
                    _roundFinished = true;

                    _roundFinishTimeout = new Timer(500);
                    _roundFinishTimeout.Elapsed += OnRoundFinishTimeoutEvent!;
                    _roundFinishTimeout.Start();
                }
                else if (_selectedUserHand < _userHands.Count && (_roundFinishTimeout is null || !_roundFinishTimeout.Enabled))
                {
                    BlackjackUserHand currentHand = _userHands[_selectedUserHand];

                    if (dealerHandValue > currentHand.GetBlackjackValue())
                    {
                        // TODO lose logic with poker chips

                        _resultLabel!.SetTexture(BlackjackResult.LOSS);
                    }
                    else if (dealerHandValue == currentHand.GetBlackjackValue())
                    {
                        // TODO push logic with poker chips

                        _resultLabel!.SetTexture(BlackjackResult.PUSH);
                    }
                    else
                    {
                        // TODO win logic with poker chips

                        _resultLabel!.SetTexture(BlackjackResult.WIN);
                    }

                    _resultLabel!.CanDraw = true;
                    _roundFinishTimeout = new Timer(500);
                    _roundFinishTimeout.Elapsed += OnRoundFinishTimeoutEvent!;
                    _roundFinishTimeout.Start();

                    _roundFinished = true;
                }
                else
                {
                    EndGame();
                }
            }
            else
            {
                EndGame();
            }
        }

        /// <summary>
        /// Update loop while the user is playing.
        /// </summary>
        private void UpdateWhileUserPlaying()
        {
            _doubleDownButton!.IsEnabled = _userHands[_selectedUserHand].CanDoubleDown();
            _splitButton!.IsEnabled = _userHands[_selectedUserHand].CanSplit();

            if (Keyboard.GetState().IsKeyDown(Keys.Right) && (_cursorMoveTimeout is null || !_cursorMoveTimeout.Enabled))
            {
                _currentCursorPos++;

                if (_currentCursorPos >= Constants.BLACKJACK_BUTTON_COUNT)
                    _currentCursorPos = 0;

                if (!_doubleDownButton!.IsEnabled && _currentCursorPos == Constants.DOUBLE_BUTTON_POS)
                    _currentCursorPos++;
                if (!_splitButton!.IsEnabled && _currentCursorPos == Constants.SPLIT_BUTTON_POS)
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
                    _currentCursorPos = 4;

                if (!_userHands[_selectedUserHand].CanSplit() && _currentCursorPos == Constants.SPLIT_BUTTON_POS)
                    _currentCursorPos--;
                if (!_doubleDownButton!.IsEnabled && _currentCursorPos == Constants.DOUBLE_BUTTON_POS)
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
                    case Constants.HIT_BUTTON_POS:
                        Hit();
                        break;
                    case Constants.STAND_BUTTON_POS:
                        FinishHand();
                        break;
                    case Constants.DOUBLE_BUTTON_POS:
                        DoubleDown();
                        break;
                    case Constants.SPLIT_BUTTON_POS:
                        Split();
                        break;
                    case Constants.FORFEIT_BUTTON_POS:
                        Forfeit();
                        break;
                }

                _userMoveTimeout = new(200);
                _userMoveTimeout.Elapsed += OnTimeoutEvent!;
                _userMoveTimeout.Start();
            }

            if (_userBust && (_roundFinishTimeout is null || !_roundFinishTimeout.Enabled))
            {
                _userBust = false;

                if (_selectedUserHand < _userHands.Count - 1)
                    FinishHand();
                else
                    EndGame();
            }
        }

        /// <summary>
        /// Gets the new Cursor position.
        /// </summary>
        private Point GetNewCursorPos()
        {
            return _currentCursorPos switch
            {
                Constants.STAND_BUTTON_POS => _standButton!.GetAdjustedPos(),
                Constants.DOUBLE_BUTTON_POS => _doubleDownButton!.GetAdjustedPos(),
                Constants.SPLIT_BUTTON_POS => _splitButton!.GetAdjustedPos(),
                Constants.FORFEIT_BUTTON_POS => _forfeitButton!.GetAdjustedPos(),
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
            if (_selectedUserHand < _userHands.Count)
                _userHands[_selectedUserHand].Draw(spriteBatch);

            // Draw the value indicators
            _dealerHandValueIndicator!.Draw(spriteBatch);
            _userHandValueIndicator!.Draw(spriteBatch);

            // Draw the result label.
            _resultLabel!.Draw(spriteBatch);
        }

        /// <summary>
        /// Initializes and starts the game.
        /// </summary>
        public void StartGame()
        {
            // Reset the cards.
            RequestCardManagerCleared!.Invoke();
            RequestDecksOfCards!.Invoke(4);

            BlackjackUserHand initialHand = new();

            // Calculate the basic position locations.
            int handXPos = Constants.WINDOW_WIDTH / 2;
            int valueIndicatorXPos = handXPos - 21;

            // Set position of the card hands
            initialHand.SetCenter(handXPos, Constants.WINDOW_HEIGHT - 200);
            _dealerHand.SetCenter(handXPos, 85);

            // Add the two initial cards.
            initialHand.AddCard(RequestCard!.Invoke());
            _dealerHand.AddCard(RequestCard!.Invoke());
            initialHand.AddCard(RequestCard!.Invoke());
            _dealerHand.AddCard(RequestCard!.Invoke());

            // Add the initial hand to the list of user hands.
            _userHands.Add(initialHand);

            // Set the position of the hand value indicators
            _userHandValueIndicator!.SetPosition(valueIndicatorXPos, Constants.WINDOW_HEIGHT - 300);
            _userHandValueIndicator!.Update(initialHand.GetBlackjackValue());
            _dealerHandValueIndicator!.SetPosition(valueIndicatorXPos, 160);
            _dealerHandValueIndicator!.Update(_dealerHand.GetBlackjackValue());

            IsPlaying = true;
            _userPlaying = true;

            _hitButton!.IsEnabled = true;
            _standButton!.IsEnabled = true;
            _forfeitButton!.IsEnabled = true;

            // TODO Check if either user or dealer has blackjack to begin. Do this with the payout ticket.
        }

        /// <summary>
        /// End game logic.
        /// </summary>
        public void EndGame()
        {
            _dealerHand.Clear();
            _userHands.Clear();
            _selectedUserHand = 0;

            System.Threading.Thread.Sleep(500);

            StartGame(); // TODO add option to select a new game
        }

        /// <summary>
        /// The hit action.
        /// </summary>
        private void Hit()
        {
            BlackjackUserHand currentHand = _userHands[_selectedUserHand];
            currentHand.AddCard(RequestCard!.Invoke());

            if (currentHand.GetBlackjackValue() <= Constants.MAX_BLACKJACK_VALUE)
            {
                _userHandValueIndicator!.Update(currentHand.GetBlackjackValue());
            }
            else if (_selectedUserHand < _userHands.Count - 1)
            {
                _resultLabel!.SetTexture(BlackjackResult.BUST);

                _resultLabel!.CanDraw = true;
                
                _roundFinishTimeout = new Timer(500);
                _roundFinishTimeout.Elapsed += OnRoundFinishTimeoutEvent!;
                _roundFinishTimeout.Start();

                _userBust = true;
            }
            else
            {
                // TODO bust logic with poker chips

                _resultLabel!.SetTexture(BlackjackResult.BUST);

                _resultLabel!.CanDraw = true;
                
                _roundFinishTimeout = new Timer(500);
                _roundFinishTimeout.Elapsed += OnRoundFinishTimeoutEvent!;
                _roundFinishTimeout.Start();

                _userBust = true;
            }
        }

        /// <summary>
        /// The double action.
        /// </summary>
        private void DoubleDown()
        {
            BlackjackUserHand currentHand = _userHands[_selectedUserHand];

            currentHand.AddCard(RequestCard!.Invoke());
            _userHandValueIndicator!.Update(currentHand.GetBlackjackValue());

            FinishHand();
        }

        /// <summary>
        /// The split action.
        /// </summary>
        private void Split()
        {
            BlackjackUserHand currentHand = _userHands[_selectedUserHand];

            BlackjackUserHand newHand = new();
            newHand.AddCard(currentHand.RemoveLastCard());
            newHand.SetCenter(Constants.WINDOW_WIDTH / 2, Constants.WINDOW_HEIGHT - 200);

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

            EndGame();
        }

        /// <summary>
        /// Finishes the given hand when it has not bust.
        /// </summary>
        private void FinishHand()
        {
            if (_selectedUserHand < _userHands.Count - 1)
            {
                _selectedUserHand++;
                BlackjackUserHand nextHand = _userHands[_selectedUserHand];

                _userHandValueIndicator!.Update(nextHand.GetBlackjackValue());
                nextHand.RecalculateCardPositions();
            }
            else
            {
                BeginDealerTurn();
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

            _hitButton!.IsEnabled = false;
            _standButton!.IsEnabled = false;
            _doubleDownButton!.IsEnabled = false;
            _splitButton!.IsEnabled = false;
            _forfeitButton!.IsEnabled = false;

            _dealerMoveTimeout = new(500);
            _dealerMoveTimeout.Elapsed += OnTimeoutEvent!;
            _dealerMoveTimeout.Start();
        }

        /// <summary>
        /// Event called when a timer times out.
        /// </summary>
        private static void OnTimeoutEvent(object source, ElapsedEventArgs e)
        {
            // Stop and dispose of the timer
            Timer timer = (Timer)source;
            timer.Stop();
            timer.Dispose();
        }


        private void OnRoundFinishTimeoutEvent(object source, ElapsedEventArgs e)
        {
            OnTimeoutEvent(source, e);
            if (_selectedUserHand < _userHands.Count - 1)
                _selectedUserHand++;
            _resultLabel!.CanDraw = false;
            _roundFinished = false;
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
        /// Texture for "bust" at the end of the game.
        /// </summary>
        public static Texture2D? BustTexture { get; private set; }

        /// <summary>
        /// Texture for "win" at the end of the game.
        /// </summary>
        public static Texture2D? WinTexture { get; private set; }

        /// <summary>
        /// Texture for "loss" at the end of the game.
        /// </summary>
        public static Texture2D? LossTexture { get; private set; }

        /// <summary>
        /// Texture for "push" at the end of the game.
        /// </summary>
        public static Texture2D? PushTexture { get; private set; }

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
            BustTexture = content.Load<Texture2D>("bustIcon");
            LossTexture = content.Load<Texture2D>("lossIcon");
            WinTexture = content.Load<Texture2D>("winIcon");
            PushTexture = content.Load<Texture2D>("pushIcon");
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

    public class BlackjackHandValueIndicator
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
            _secondDigit.SetPosition(xPos+21, yPos);
        }

        /// <summary>
        /// Updates the indicator based on the value of the hand.
        /// </summary>
        public void Update(int handValue)
        {
            if (handValue == _previousValue)
                return;

            int firstDigit = handValue / 10;
            int secondDigit = handValue % 10;

            _firstDigit.Update(firstDigit);
            _secondDigit.Update(secondDigit);

            _previousValue = handValue;
        }

        /// <summary>
        /// Draw method for the value indicator.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            _firstDigit.Draw(spriteBatch);
            _secondDigit.Draw(spriteBatch);
        }
    }

    public class BlackjackResultLabel
    {
        /// <summary>
        /// The texture for the label.
        /// </summary>
        private Texture2D? _resultTexture;

        /// <summary>
        /// The rectangle for the label.
        /// </summary>
        private Rectangle _resultRectangle;

        /// <summary>
        /// Whether or not it should be drawn.
        /// </summary>
        public bool CanDraw { get; set; } = false;

        public BlackjackResultLabel(int xPos, int yPos)
        {
            _resultRectangle = new(xPos, yPos, 180, 75);
        }

        /// <summary>
        /// Sets the texture for the label.
        /// </summary>
        /// <param name="texture"></param>
        public void SetTexture(BlackjackResult result)
        {
            switch(result)
            {
                case BlackjackResult.WIN:
                    _resultTexture = BlackjackTextures.WinTexture;
                    break;
                case BlackjackResult.LOSS:
                    _resultTexture = BlackjackTextures.LossTexture;
                    break;
                case BlackjackResult.PUSH:
                    _resultTexture = BlackjackTextures.PushTexture;
                    break;
                case BlackjackResult.BUST:
                    _resultTexture = BlackjackTextures.BustTexture;
                    break;
            }
        }

        /// <summary>
        /// Draws the result label.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (CanDraw && _resultTexture is not null)
                spriteBatch.Draw(_resultTexture, _resultRectangle, Color.White);
        }
    }

    public enum BlackjackResult
    {
        WIN,
        LOSS,
        PUSH,
        BUST
    }
}
