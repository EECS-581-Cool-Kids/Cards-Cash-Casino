using CardsCashCasino.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CardsCashCasino.Manager
{
    public class BlackjackManager
    {
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
        /// Whether or not it is still the user's turn.
        /// </summary>
        private bool _userPlaying = true;

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
        /// The timeout for the cursor to move.
        /// </summary>
        private Timer? _cursorMoveTimeout;

        /// <summary>
        /// The timeout for the user "hitting" their deck.
        /// </summary>
        private Timer? _hitTimeout;

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

        /// <summary>
        /// The LoadContent method for blackjack.
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            BlackjackTextures.LoadContent(content);

            _hitButton = new(BlackjackTextures.HitEnabledTexture!, BlackjackTextures.HitDisabledTexture!, 50, 400);
            _standButton = new(BlackjackTextures.StandEnabledTexture!, BlackjackTextures.StandDisabledTexture!, 194, 400);
            _doubleDownButton = new(BlackjackTextures.DoubleDownEnabledTexture!, BlackjackTextures.DoubleDownDisabledTexture!, 338, 400);
            _splitButton = new(BlackjackTextures.SplitEnabledTexture!, BlackjackTextures.SplitDisabledTexture!, 482, 400);
            _forfeitButton = new(BlackjackTextures.ForfeitEnabledTexture!, BlackjackTextures.ForfeitDisabledTexture!, 626, 400);

            _hitButton.IsEnabled = true;
            _standButton.IsEnabled = true;
            _doubleDownButton.IsEnabled = true;
            _forfeitButton.IsEnabled = true;

            _cursor = new(BlackjackTextures.CursorTexture!, _hitButton.GetAdjustedPos());

            StartGame(); // temp
        }

        /// <summary>
        /// The main update loop for blackjack.
        /// </summary>
        public void Update()
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
                switch(_currentCursorPos)
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
        }

        /// <summary>
        /// Initializes and starts the game.
        /// </summary>
        public void StartGame()
        {
            RequestCardManagerCleared!.Invoke();
            RequestDecksOfCards!.Invoke(4);

            UserHand initialHand = new();

            initialHand.SetCenter(400, 300);
            _dealerHand.SetCenter(400, 75);

            initialHand.AddCard(RequestCard!.Invoke());
            _dealerHand.AddCard(RequestCard!.Invoke());
            initialHand.AddCard(RequestCard!.Invoke());
            _dealerHand.AddCard(RequestCard!.Invoke());

            _userHands.Add(initialHand);

            IsPlaying = true;
        }

        /// <summary>
        /// The hit action.
        /// </summary>
        private void Hit()
        {
            if (_hitTimeout is not null && _hitTimeout.Enabled)
                return;

            UserHand currentHand = _userHands[_selectedUserHand];
            currentHand.AddCard(RequestCard!.Invoke());

            if (!currentHand.HasBust)
            {
                _hitTimeout = new(200);
                _hitTimeout.Elapsed += OnTimeoutEvent!;
                _hitTimeout.Start();
            }
            else
            {
                // TODO bust logic
            }
        }

        /// <summary>
        /// The double action.
        /// </summary>
        private void DoubleDown()
        {
            _userHands[_selectedUserHand].AddCard(RequestCard!.Invoke());
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
        }

        /// <summary>
        /// The forfeit action.
        /// </summary>
        private void Forfeit()
        {
            // TODO forfeit logic
        }

        /// <summary>
        /// Finishes the given hand when it has not bust.
        /// </summary>
        private void FinishHand()
        {
            _selectedUserHand++;

            if (_selectedUserHand >= _userHands.Count)
                PlayDealer();
        }

        /// <summary>
        /// Plays the dealer's turn.
        /// </summary>
        private void PlayDealer()
        {
            // TODO dealer turn logic
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
}
