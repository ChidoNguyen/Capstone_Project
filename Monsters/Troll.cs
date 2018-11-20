﻿using Capstonia.Core;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Rectangle = RogueSharp.Rectangle;

namespace Capstonia.Monsters
{
    public class Troll : Monster
    {
        int oldPlayerX;
        int oldPlayerY;
        // constructor
        public Troll(GameManager game) : base(game)
        {
            Level = 6;
            // every point above 10 gives a health bonus
            Constitution = 17;
            // every point above 10 gives a dodge bonus
            Dexterity = 10;
            // health total for Capstonian; if the values reaches 0, the Capstonain is killed
            MaxHealth = 16;
            // current health for Capstonian; if the values reaches 0, the Capstonain is killed
            CurrHealth = 16;
            // max dmg Capstonian can cause
            MaxDamage = 8;
            // min dmg Capstonain can cause
            MinDamage = 5;
            // name of monster
            Name = "Troll";
            // every point above 10 gives a dmg bonus
            Strength = 10;

            MinGlory = 6;
            MaxGlory = 9;
            Sprite = game.troll;
            oldPlayerX = game.Player.X;
            oldPlayerY = game.Player.Y;
            
        }
    }
}