﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueSharp;
using RogueSharp.Random;
using System;
using System.Collections.Generic;
using Capstonia.Systems;
using Capstonia.Core;
using Capstonia.Items;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Capstonia
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameManager : Game
    {
        // Game Variable Declarations
        public readonly int levelWidth = 70;
        public readonly int levelHeight = 70;
        public readonly int levelRows = 5;
        public readonly int levelCols = 5;
        public int mapLevel = 1;
        public readonly int tileSize = 48;
        public float scale = 1.0f;

        // Game Actor Constants
        public readonly int BaseStrength = 10;
        public readonly int BaseDexterity = 10;
        public readonly int BaseConstitution = 10;

        // RogueSharp Specific Declarations
        public static IRandom Random { get; private set; }
        public Player Player { get; set; }
        public Monster Monster { get; set; }
        public LevelGrid Level { get; private set; }
        public MessageLog Messages { get; set; }
        public Score ScoreDisplay { get; set; }
        public CommandSystem CommandSystem;
        public PathFinder GlobalPositionSystem;

        // MonoGame Specific Declarations
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Texture2D floor;
        public Texture2D wall;
        public Texture2D exit;
        public SpriteFont mainFont;

        // Items - Gameboard
        public Texture2D armor;
        public Texture2D armor_leather_chest;
        public Texture2D armor_steel_chest;
        public Texture2D armor_gold_chest;
        public Texture2D armor_emerald_chest;
        public Texture2D armor_diamond_chest;
        public Texture2D armor_blood_chest;
        public Texture2D food;
        public Texture2D weapon;
        public Texture2D potion;
        public Texture2D book;
        public Texture2D gem;
        public Texture2D chest;

        // Items - Player Stats
        public Texture2D constitution;
        public Texture2D dexterity;
        public Texture2D experience;
        public Texture2D health;
        public Texture2D level;
        public Texture2D strength;

        // Monsters
        public Texture2D beholder;

        // containers
        public List<Monster> Monsters;
        public List<Item> Items;
        
        //Inventory
        public InventorySystem Inventory;
        public Rectangle inventoryScreen;
        public Texture2D emptyTexture; //used to fill a blank rectangle (i.e., inventoryScreen)
        public Texture2D Outline;
        
        // Player Stats and Equipment
        public Texture2D PlayerStatsOutline;
        public Texture2D PlayerEquipmentOutline;

        // Monster Stats
        public Texture2D MonsterStatsOutline;

        // track keyboard state (i.e. capture key presses)
        public KeyboardState currentKeyboardState;
        public KeyboardState previousKeyboardState;

        public GameManager() : base()
        {
            // MonoGame Graphic/Content setup
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 858;
            Content.RootDirectory = "Content";

            // add player instance
            Player = new Player(this);

            // add monster instance
            Monster = new Monster(this);

            //link the messageLog and game instance
            Messages = new MessageLog(this);

            ScoreDisplay = new Score(this);

            // Player provided commands
            CommandSystem = new CommandSystem(this);

            //Create Inventory for player
            Inventory = new InventorySystem(this);
            inventoryScreen = new Rectangle(200, 100, 1200, 0);   //Height, Width, X, Y

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // initialize lists
            Monsters = new List<Monster>();
            Items = new List<Item>();

            // get seed based on current time and set up RogueSharp Random instance
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            //https://stackoverflow.com/questions/22535699/mouse-cursor-is-not-showing-in-windows-store-game-developing-using-monogame
            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load level textures
            floor = Content.Load<Texture2D>("floor_extra_12");
            wall = Content.Load<Texture2D>("wall_stone_11");
            exit = Content.Load<Texture2D>("floor_set_grey_8");

            // load item textures - gameboard
            armor = Content.Load<Texture2D>("armor");
            armor_leather_chest = Content.Load<Texture2D>("armor_leather_chest");
            armor_steel_chest = Content.Load<Texture2D>("armor_steel_chest");
            armor_gold_chest = Content.Load<Texture2D>("armor_gold_chest");
            armor_emerald_chest = Content.Load<Texture2D>("armor_emerald_chest");
            armor_diamond_chest = Content.Load<Texture2D>("armor_diamond_chest");
            armor_blood_chest = Content.Load<Texture2D>("armor_blood_chest");
            food = Content.Load<Texture2D>("drumstick");
            weapon = Content.Load<Texture2D>("weapon");
            potion = Content.Load<Texture2D>("potion");
            book = Content.Load<Texture2D>("book");
            chest = Content.Load<Texture2D>("chest_gold_open");

            // load item textures - player stats
            constitution = Content.Load<Texture2D>("constitution");
            dexterity = Content.Load<Texture2D>("dexterity");
            experience = Content.Load<Texture2D>("experience");
            health = Content.Load<Texture2D>("health");
            level = Content.Load<Texture2D>("level");
            strength = Content.Load<Texture2D>("strength");

            // load gui textures
            Outline = Content.Load<Texture2D>("inventory_gui");
            PlayerStatsOutline = Content.Load<Texture2D>("player_stats_gui");
            PlayerEquipmentOutline = Content.Load<Texture2D>("player_equipment_gui");
            MonsterStatsOutline = Content.Load<Texture2D>("monster_stats_gui");

            // load actor textures
            Player.Sprite = Content.Load<Texture2D>("dknight_1");
            beholder = Content.Load<Texture2D>("beholder_deep_1");

            // load fonts
            mainFont = Content.Load<SpriteFont>("MainFont");

            //Drawing black screen for inventory inspired by: https://stackoverflow.com/questions/5751732/draw-rectangle-in-xna-using-spritebatch
            emptyTexture = new Texture2D(GraphicsDevice, 1, 1);
            emptyTexture.SetData(new[] { Color.White });

            GenerateLevel();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Handle keyboard input
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // move player
            Player.Move();

            //move Monsters
            foreach( Monster enemy in Monsters)
            {
                enemy.Move();
            }

            // update game state
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // making the spriteBatch.begin(...) change below should fix the
            // rendering issues where layers would randomly render out of order
            // spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Begin();

            Inventory.Draw(spriteBatch);
            Messages.Draw(spriteBatch);
            ScoreDisplay.Draw(spriteBatch);
            Level.Draw(spriteBatch);

            // draw all of the monsters in the list
            foreach (var monster in Monsters)
            {
                monster.Draw(spriteBatch);
            }

            // draw all of the items in the list
            foreach(var item in Items)
            {
                item.Draw(spriteBatch);
            }

            // draw player sprite
            Player.Draw(spriteBatch);

            // draw stats grid for player
            Player.DrawStats(spriteBatch);

            // draw stats grid for monsters
            Monster.DrawStats(spriteBatch);

            // draw equipment grid for player
            Player.DrawEquipment(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }


        // GenerateLevel()
        // DESC:    Generates the entire level grid in which individual rooms will be placed.     
        // PARAMS:  None.
        // RETURNS: None.
        public void GenerateLevel()
        {
            LevelGenerator levelGenerator = new LevelGenerator(this, levelWidth, levelHeight, levelRows, levelCols, mapLevel);
            Level = levelGenerator.CreateLevel();
        }

        // SetLevelCell()
        // DESC:    Takes data from object and passed data to UserInterface for level update
        // PARAMS:  x(int), y(int), type(ObjectType), isExplored(bool)
        // RETURNS: None.
        public void SetLevelCell(int x, int y, ObjectType type, bool isExplored)
        {
            //masterConsole.UpdateLevelCell(x, y, type, isExplored);
        }

        // IsInRoomWithPlayer()
        // DESC:    Determines if coordinates are in the same room as the player
        // PARAMS:  x(int), y(int)
        // RETURNS: Boolean (true if in same room, false otherwise)
        public bool IsInRoomWithPlayer(int x, int y)
        {
            // get room player is in
            int RoomIndex;
            for (RoomIndex = 0; RoomIndex < Level.Rooms.Count; RoomIndex++)
            {
                if(Player.X >= Level.Rooms[RoomIndex].Left && Player.X <= Level.Rooms[RoomIndex].Right && Player.Y >= Level.Rooms[RoomIndex].Top && Player.Y <= Level.Rooms[RoomIndex].Bottom)
                {
                    // found player room, now check if passed in coordinates exist within it
                    if(x >= Level.Rooms[RoomIndex].Left && x <= Level.Rooms[RoomIndex].Right && y >= Level.Rooms[RoomIndex].Top && y <= Level.Rooms[RoomIndex].Bottom)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // player should always be located in the list of Rooms so we should never reach this point
            return false;
        }

        // HandleDeath()
        // DESC:    Handle monster death
        // PARAMS:  Monster
        // RETURNS: None
        public void HandleMonsterDeath(Monster monster)
        {
            int addGlory = Random.Next(monster.MinGlory, monster.MaxGlory);
            Player.Glory += addGlory;

            Messages.AddMessage("You have slaughtered the " + monster.Name + "!!!");            
            Messages.AddMessage("You have earned " + addGlory + " Glory worth of gold and bones!!!");
            
            Level.SetIsWalkable(monster.X, monster.Y, true);
            Monsters.Remove(monster);            
        }

        // HandlePlayerDeath()
        // DESC:    Handle player death
        // PARAMS:  None
        // RETURNS: None
        public void HandlePlayerDeath()
        {
            Messages.AddMessage("You have DIED!  Game Over!");
            Messages.AddMessage("Press <ESC> to Exit Game.");

            while (true)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
            };
        }
    }
}
