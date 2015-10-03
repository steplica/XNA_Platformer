// TO DO LIST
/* Turn the different level arrays into a single, 3D array.
 * Make it so that, each time the level is edited, it makes a temp array to store the levels and updates the original array with the new content
 *
 */



// MAIN BODY OF CODE
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace First_Game
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        //Make every NULL SPOT INTO STONE OR SOMETHING

        // Declare Everything
        public static int TileSize = 16;
        public static int RoomHeight = 480; // Should be divisible by TileSize! (30 x 16)
        public static int RoomWidth = 800; // Should be divisible by TileSize! (50 x 16)

        public static int NumberOfTextures = 0;
        public static int CurrentLevel = 2; // The level that the game starts at
        public static int TextureType = 1; // What is being inserted in Edit Mode

        public static bool DebugMode = false; // Has a bunch of variable information
        public static bool EditMode = false; // Allows player to insert different blocks
        public static int BrushSize = 1;

        // Variable for WriteLine Method
        public static int HorizontalTextIndent = 18;
        public static int HorizontalLineSpacing = 500;
        public static int VerticalTextIndent = 1;
        public static int VerticalLineSpacing = 22;
        public static int MaxLinesPerColumn = 10;
        public static int LineNumber;

        public static int[,] PlayerStartingPositions =  /*Level*/{{1,   2,   3,   4,   5,   6,   7  }, 
        /* Where player starts on each room             /*  X  */ {0,   64,  64,  64,  64,  0,   0  },
                                                        /*  Y  */ {408, 408, 408, 408, 408, 408, 408}};

        public static Texture2D Dirt; // Animation representing the dirt. It must then be loaded
        public static Texture2D Grass; // Animation representing the grass. It must then be loaded
        public static Texture2D Square; // Animation representing the square/player. It must then be loaded
        public static Texture2D Stone; // Animation representing the stone. It must then be loaded
        public static Texture2D Water; // Animation representing the water. It must then be loaded
        public static Texture2D Tile; // A blank 16x16 tile

        public static SpriteFont font; // Font for in-game text. It must then be loaded
        public static GraphicsDeviceManager graphics; // Necessary
        public static SpriteBatch spriteBatch; // Name of the SpriteBatch method declared. It must then be loaded  

        public static KeyboardState CurrentKeyboardState; // Keyboard states used to determine key presses
        public static KeyboardState PreviousKeyboardState; // Keyboard states used to determine key presses

        public static MouseState CurrentMouseState;
        public static MouseState PreviousMouseState;

        public static Actor Player; // Create an instance of the Player class
        public static Cursor Cursor; // Create an instance of the Cursor class
        public static World Level; // Create an instance of the Level class



        // UPDATE - UPDATE - UPDATE - UPDATE - UPDATE
        protected override void Update(GameTime gameTime) // Called ~60 times per second
        {
            GetStates(); // Retrieve keyboard & mouse states
            Player.Update(); // Update the player
            Cursor.Update(); // Update the cursor
            KeyInputs(); // Read key inputs
            base.Update(gameTime);
        }



        // DRAW - DRAW - DRAW - DRAW - DRAW - DRAW
        protected override void Draw(GameTime gameTime) // Draws everything to the screen. Called ~60 times per second.      
        {
            Player.CanMoveLeft = true;
            Player.CanMoveRight = true;
            Player.IsGrounded = false;
            int CurrentBlock = 0;
            string BlockType = "none";

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend); // Begins drawing. Each object created subsequent to the previous gets a z-layer behind the previous        

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Room drawing & Collision detection
            for (int height = 0; height < (RoomHeight / TileSize); height++)
            {
                for (int width = 0; width < (RoomWidth / TileSize); width++)
                {
                    Rectangle ObjectBounds = new Rectangle(0, 0, 0, 0);
                    if (World.EditArray.LevelArray[height, width] != '.') CurrentBlock = World.EditArray.LevelArray[height, width];
                    else CurrentBlock = World.Levels[CurrentLevel - 1].LevelArray[height, width];
                    BlockType = "solid";
                    switch (CurrentBlock)
                    {
                        case 'G': // Grass
                            spriteBatch.Draw(Grass, new Vector2(width * TileSize, height * TileSize), Color.White);
                            break;
                        case 'D': // Dirt
                            spriteBatch.Draw(Dirt, new Vector2(width * TileSize, height * TileSize), Color.White);
                            break;
                        case 'S': // Stone
                            spriteBatch.Draw(Stone, new Vector2(width * TileSize, height * TileSize), Color.White);
                            break;
                        case '~': // Water
                            spriteBatch.Draw(Water, new Vector2(width * TileSize, height * TileSize), Color.White);
                            BlockType = "liquid";
                            break;
                        default:
                            BlockType = "air";
                            break;
                    }
                    Player.CheckCollisions(new Rectangle(width * TileSize, height * TileSize, TileSize, TileSize), BlockType, width, height);
                }
            }

            if (DebugMode) DebugHUD();
            if (EditMode) EditModeCode();

            Player.Draw(spriteBatch); // Draw the player  
            Cursor.Draw(spriteBatch); // Draw the cursor   

            spriteBatch.End(); // Stops drawing

            base.Draw(gameTime);
        }



        // KEY INPUTS - KEY INPUTS - KEY INPUTS
        public void KeyInputs()
        {
            // Check for level changing before drawing. Normally key reading is in the designated function, however this required special coordination with the draw method.
            // If +(NUM) is pressed, go to next level
            if ((CurrentKeyboardState.IsKeyDown(Keys.Add) && PreviousKeyboardState.IsKeyUp(Keys.Add)) && Player.MaxHorizontalSpeed < 35 && CurrentLevel < PlayerStartingPositions.GetLength(1))
            {
                Player.Position.X = PlayerStartingPositions[1, CurrentLevel];
                Player.Position.Y = PlayerStartingPositions[2, CurrentLevel];
                CurrentLevel++;
            }
            // If -(NUM) is pressed, go to previous level
            else if ((CurrentKeyboardState.IsKeyDown(Keys.Subtract) && PreviousKeyboardState.IsKeyUp(Keys.Subtract)) && Player.MaxHorizontalSpeed > 1 && CurrentLevel > 1)
            {
                Player.Position.X = PlayerStartingPositions[1, CurrentLevel - 2];
                Player.Position.Y = PlayerStartingPositions[2, CurrentLevel - 2];
                CurrentLevel--;
            }
            if (CurrentKeyboardState.IsKeyDown(Keys.F1) && PreviousKeyboardState.IsKeyUp(Keys.F1))
                EditMode = (DebugMode) ? DebugMode = false : DebugMode = true;
            if (CurrentKeyboardState.IsKeyDown(Keys.F2) && PreviousKeyboardState.IsKeyUp(Keys.F2))
                EditMode = (EditMode) ? EditMode = false : EditMode = true;
            if (CurrentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();
            if (CurrentKeyboardState.IsKeyDown(Keys.Tab) && PreviousKeyboardState.IsKeyUp(Keys.Tab))
                switch (BrushSize)
                {
                    case 1: BrushSize = 2;
                        break;
                    case 2: BrushSize = 3;
                        break;
                    case 3: BrushSize = 1;
                        break;
                }
        }



        // DEBUG HUD - DEBUG HUD - DEBUG HUD
        public void DebugHUD() // A debug menu displaying vital information (ENTER - Toggle)
        {
            LineNumber = VerticalTextIndent;
            HorizontalTextIndent = 18;
            WriteLine("PlayerPos: (X: " + (int)Player.Position.X + ", Y: " + (int)Player.Position.Y + ")");
            WriteLine("Mouse: (X: " + CurrentMouseState.X + ", Y: " + CurrentMouseState.Y + ")");
            WriteLine("VSpeed: " + (int)(Player.VerticalSpeed));
            WriteLine("HSpeed: " + (int)Player.HorizontalSpeed);
            WriteLine("MaxHSpeed: " + (int)Player.MaxHorizontalSpeed);
            WriteLine("IsFalling: " + Player.IsFalling);
            WriteLine("IsJumping: " + Player.IsJumping);
            WriteLine("IsGrounded:" + Player.IsGrounded);
            WriteLine("IsMovingLeft: " + Player.IsMovingLeft);
            WriteLine("IsMovingRight: " + Player.IsMovingRight);
            WriteLine("IsMovingUp: " + Player.IsMovingUp);
            WriteLine("IsMovingDown: " + Player.IsMovingDown);
            WriteLine("ScrollWheel: " + CurrentMouseState.ScrollWheelValue);
            WriteLine("Edit Mode: " + EditMode);
            WriteLine("CanMoveLeft: " + Player.CanMoveLeft);
            WriteLine("CanMoveRight: " + Player.CanMoveRight);
            WriteLine("CurrentLevel: " + CurrentLevel);
            WriteLine("BrushSize: " + BrushSize);
            if ((LineNumber - 1) / VerticalLineSpacing >= MaxLinesPerColumn)
            {
                LineNumber = 1;
                HorizontalTextIndent += HorizontalLineSpacing;
            }
            switch (TextureType)
            {
                case 1: spriteBatch.Draw(Dirt, new Vector2(HorizontalTextIndent + 90, LineNumber + 3), Color.White);
                    break;
                case 2: spriteBatch.Draw(Grass, new Vector2(HorizontalTextIndent + 90, LineNumber + 3), Color.White);
                    break;
                case 3: spriteBatch.Draw(Stone, new Vector2(HorizontalTextIndent + 90, LineNumber + 3), Color.White);
                    break;
                case 4: spriteBatch.Draw(Water, new Vector2(HorizontalTextIndent + 90, LineNumber + 3), Color.White);
                    break;
            }
            WriteLine("Texture: " + TextureType);
            if (Player.IsMovingUp) spriteBatch.Draw(Square, Player.FutureUpwardBounds, Color.White);
            if (Player.IsMovingDown) spriteBatch.Draw(Square, Player.FutureDownwardBounds, Color.White);
            if (Player.IsMovingLeft) spriteBatch.Draw(Square, Player.FutureLeftBounds, Color.White);
            if (Player.IsMovingRight) spriteBatch.Draw(Square, Player.FutureRightBounds, Color.White);
        }

        public void EditModeCode()
        {
            int EditX = (((CurrentMouseState.X - 8) % TileSize) >= 8) ? ((CurrentMouseState.X - 8) / TileSize + 1) * TileSize : ((CurrentMouseState.X - 8) / TileSize) * TileSize;
            int EditY = (((CurrentMouseState.Y - 8) % TileSize) >= 8) ? ((CurrentMouseState.Y - 8) / TileSize + 1) * TileSize : ((CurrentMouseState.Y - 8) / TileSize) * TileSize;
            Color DrawColor = Color.White;
            Color EraseColor = new Color(0, 215, 0);
            if (CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                spriteBatch.Draw(Tile, new Vector2(EditX, EditY), null, new Color(0, 215, 0) * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                try
                {
                    World.EditArray.LevelArray[(EditY / TileSize), (EditX / TileSize)] = '.';
                }
                catch { }
                if (BrushSize > 1)
                {
                    spriteBatch.Draw(Tile, new Vector2(EditX + 16, EditY), null, EraseColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(Tile, new Vector2(EditX, EditY + 16), null, EraseColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(Tile, new Vector2(EditX - 16, EditY), null, EraseColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(Tile, new Vector2(EditX, EditY - 16), null, EraseColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    try
                    {
                        World.EditArray.LevelArray[(EditY / TileSize) + 1, (EditX / TileSize)] = '.';
                        World.EditArray.LevelArray[(EditY / TileSize), (EditX / TileSize) + 1] = '.';
                        World.EditArray.LevelArray[(EditY / TileSize) - 1, (EditX / TileSize)] = '.';
                        World.EditArray.LevelArray[(EditY / TileSize), (EditX / TileSize) - 1] = '.';
                    }
                    catch { }
                }
                if (BrushSize > 2)
                {
                    spriteBatch.Draw(Tile, new Vector2(EditX + 16, EditY + 16), null, EraseColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(Tile, new Vector2(EditX + 16, EditY - 16), null, EraseColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(Tile, new Vector2(EditX - 16, EditY + 16), null, EraseColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(Tile, new Vector2(EditX - 16, EditY - 16), null, EraseColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    try
                    {
                        World.EditArray.LevelArray[(EditY / TileSize) + 1, (EditX / TileSize) + 1] = '.';
                        World.EditArray.LevelArray[(EditY / TileSize) + 1, (EditX / TileSize) - 1] = '.';
                        World.EditArray.LevelArray[(EditY / TileSize) - 1, (EditX / TileSize) + 1] = '.';
                        World.EditArray.LevelArray[(EditY / TileSize) - 1, (EditX / TileSize) - 1] = '.';
                    }
                    catch { }
                }
            }
            else
            {
                Texture2D InsertedTexture = Tile;
                char InsertedTextureChar = '.';
                switch (TextureType)
                {
                    case 1: InsertedTexture = Dirt;
                        InsertedTextureChar = 'D';
                        break;
                    case 2: InsertedTexture = Grass;
                        InsertedTextureChar = 'G';
                        break;
                    case 3: InsertedTexture = Stone;
                        InsertedTextureChar = 'S';
                        break;
                    case 4: InsertedTexture = Water;
                        InsertedTextureChar = '~';
                        break;
                }
                spriteBatch.Draw(InsertedTexture, new Vector2(EditX, EditY), null, Color.White * 0.5f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                if (BrushSize > 1)
                {
                    spriteBatch.Draw(InsertedTexture, new Vector2(EditX + 16, EditY), null, DrawColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(InsertedTexture, new Vector2(EditX, EditY + 16), null, DrawColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(InsertedTexture, new Vector2(EditX - 16, EditY), null, DrawColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(InsertedTexture, new Vector2(EditX, EditY - 16), null, DrawColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                if (BrushSize > 2)
                {
                    spriteBatch.Draw(InsertedTexture, new Vector2(EditX + 16, EditY + 16), null, DrawColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(InsertedTexture, new Vector2(EditX + 16, EditY - 16), null, DrawColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(InsertedTexture, new Vector2(EditX - 16, EditY + 16), null, DrawColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(InsertedTexture, new Vector2(EditX - 16, EditY - 16), null, DrawColor * 0.6f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                if (CurrentMouseState.LeftButton == ButtonState.Pressed && CurrentMouseState.X >= 0 && CurrentMouseState.X <= RoomWidth && CurrentMouseState.Y >= 0 && CurrentMouseState.Y <= RoomHeight)
                {
                    World.EditArray.LevelArray[EditY / TileSize, EditX / TileSize] = InsertedTextureChar;
                    if (BrushSize > 1)
                    {
                        try
                        {
                            World.EditArray.LevelArray[(EditY / TileSize) + 1, (EditX / TileSize)] = InsertedTextureChar;
                            World.EditArray.LevelArray[(EditY / TileSize), (EditX / TileSize) + 1] = InsertedTextureChar;
                            World.EditArray.LevelArray[(EditY / TileSize) - 1, (EditX / TileSize)] = InsertedTextureChar;
                            World.EditArray.LevelArray[(EditY / TileSize), (EditX / TileSize) - 1] = InsertedTextureChar;
                        }
                        catch { }
                    }
                    if (BrushSize > 2)
                    {
                        try
                        {
                            World.EditArray.LevelArray[(EditY / TileSize) + 1, (EditX / TileSize) + 1] = InsertedTextureChar;
                            World.EditArray.LevelArray[(EditY / TileSize) + 1, (EditX / TileSize) - 1] = InsertedTextureChar;
                            World.EditArray.LevelArray[(EditY / TileSize) - 1, (EditX / TileSize) + 1] = InsertedTextureChar;
                            World.EditArray.LevelArray[(EditY / TileSize) - 1, (EditX / TileSize) - 1] = InsertedTextureChar;
                        }
                        catch { }
                    }
                }
            }
        }


        // WRITELINE - WRITELINE - WRITELINE - WRITELINE
        public void WriteLine(string Text) // Simplifies DebugHUD function
        {
            if ((LineNumber - 1) / VerticalLineSpacing >= MaxLinesPerColumn)
            {
                LineNumber = 1;
                HorizontalTextIndent += HorizontalLineSpacing;
            }
            spriteBatch.DrawString(font, Text, new Vector2(HorizontalTextIndent, LineNumber), Color.White); LineNumber += VerticalLineSpacing;
        }



        // GETSTATE - GETSTATE - GETSTATES - GETSTATES
        public void GetStates()
        {
            PreviousKeyboardState = CurrentKeyboardState; // Save the current keyboard state to the previous state as the new one is set
            CurrentKeyboardState = Keyboard.GetState(); // Save the current keyboard state
            PreviousMouseState = CurrentMouseState; // Save the current mouse state to the previous state as the new one is set
            CurrentMouseState = Mouse.GetState(); // Save the current mouse state
        }








        // CONSTRUCTOR - CONSTRUCTOR - CONSTRUCTOR - CONSTRUCTOR
        public Game()
        {
            IsMouseVisible = false;
            Window.Title = "Rectangle Puzzle Game";
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = RoomHeight;
            graphics.PreferredBackBufferWidth = RoomWidth;
            Content.RootDirectory = "Content";
        }



        // INITIALIZE - INITIALIZE - INITIALIZE - INITIALIZE
        protected override void Initialize() // This is where it can query for any required services and load any non-graphic related content.       
        {
            Player = new Actor();
            Cursor = new Cursor();
            Level = new World();
            base.Initialize();
        }



        // LOAD CONTENT - LOAD CONTENT - LOAD CONTENT
        protected override void LoadContent() // LoadContent will be called once per game and is the place to load all of your content       
        {
            // Load sprites
            Dirt = Content.Load<Texture2D>("spr_dirt"); NumberOfTextures++;      // Loads the dirt Texture2D
            Grass = Content.Load<Texture2D>("spr_grass"); NumberOfTextures++;    // Loads the dirt Texture2D
            Square = Content.Load<Texture2D>("spr_square"); NumberOfTextures++;  // Loads the square/Player Texture2D
            Stone = Content.Load<Texture2D>("spr_stone"); NumberOfTextures++;    // Loads the dirt Texture2D
            Water = Content.Load<Texture2D>("spr_water"); NumberOfTextures++;    // Loads the water Texture2D
            Tile = Content.Load<Texture2D>("spr_blankTile"); NumberOfTextures++; //Loads the tile Texture2D

            // Load the player resources 
            Player.Initialize(Content.Load<Texture2D>("spr_square"), new Vector2(PlayerStartingPositions[1, CurrentLevel - 1], PlayerStartingPositions[2, CurrentLevel - 1]));

            // Load the cursor recources
            Cursor.Initialize(Content.Load<Texture2D>("spr_cursor"), new Vector2(0, 0));

            // Other
            spriteBatch = new SpriteBatch(GraphicsDevice); // Create a new SpriteBatch, which can be used to draw textures.
            font = Content.Load<SpriteFont>("font_Arial"); NumberOfTextures++;  // Loads the font Arial
        }



        // UNLOADCONTENT - UNLOADCONTENT - UNLOADCONTENT
        protected override void UnloadContent() // UnloadContent will be called once per game and is the place to unload all content.
        {
        }
    }
}