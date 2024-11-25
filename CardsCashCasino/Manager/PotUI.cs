/*
 *  Module Name: PotUI.cs
 *  Purpose: A UI for showing the pot and bet amount.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Richard Moser
 *  Date: 11/20/2024
 *  Last Modified: 11/24/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CardsCashCasino.Manager
{
    public class PotUI
    {
        private Texture2D _chipsLessThan50;    // $0 - $50
        private Texture2D _chipsLessThan250; // $51 - $250
        private Texture2D _chipsLessThan500;   // $251 - $500
        private Texture2D _chipsLessThan1000; // $501 - $1000
        private Texture2D _chipsLessThan5000; // $1001 - $5000
        private Texture2D _chipsGreaterThan5000;  // $5000+
        private int _potValue; // The current value of the pot

        private Vector2 _position; // Position of the pot UI

        public PotUI(Vector2 position)
        {
            _position = position;
        }

        public void LoadContent(ContentManager content)
        {
            _chipsLessThan50 = content.Load<Texture2D>("ChipsLessThan50");
            _chipsLessThan250 = content.Load<Texture2D>("ChipsLessThan250");
            _chipsLessThan500 = content.Load<Texture2D>("ChipsLessThan500");
            _chipsLessThan1000 = content.Load<Texture2D>("ChipsLessThan1000");
            _chipsLessThan5000 = content.Load<Texture2D>("ChipsLessThan5000");
            _chipsGreaterThan5000 = content.Load<Texture2D>("ChipsGreaterThan5000");
        }

        public void UpdatePot(int newPotValue)
        {
            _potValue = newPotValue;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw chips based on the pot value
            if (_potValue <= 50)
            {
                spriteBatch.Draw(_chipsLessThan50, _position, Color.White);
            }
            else if (_potValue <= 250)
            {
                spriteBatch.Draw(_chipsLessThan250, _position, Color.White);
            }
            else if (_potValue <= 500)
            {
                spriteBatch.Draw(_chipsLessThan500, _position, Color.White);
            }
            else if (_potValue <= 1000)
            {
                spriteBatch.Draw(_chipsLessThan1000, _position, Color.White);
            }
            else if (_potValue <= 5000)
            {
                spriteBatch.Draw(_chipsLessThan5000, _position, Color.White);
            }
            else
            {
                spriteBatch.Draw(_chipsGreaterThan5000, _position, Color.White);
            }
        }
    }
}