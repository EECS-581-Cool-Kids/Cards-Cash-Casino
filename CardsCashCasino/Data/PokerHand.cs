﻿/*
 *  Module Name: UserHand.cs
 *  Purpose: Models the user's hand of cards.
 *  Inputs: None
 *  Outputs: None
 *  Additional code sources: None
 *  Developers: Jacob Wilkus
 *  Date: 11/10/2024
 *  Last Modified: 12/3/2024
 *  Preconditions: None
 *  Postconditions: None
 *  Error/Exception conditions: None
 *  Side effects: None
 *  Invariants: None
 *  Known Faults: None encountered
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsCashCasino.Data
{
    public class PokerHand : CardHand
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void AddCard(Card card)
        {
            card.HideTexture();
            base.AddCard(card);
        }
    }
}
