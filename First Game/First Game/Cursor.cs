using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace First_Game
{
    public class Cursor
    {
        // Declare Variables
        public bool Active; // State of the player
        public int Width { get { return Texture.Width; } } // Get the width of the cursor sprite
        public int Height { get { return Texture.Height; } } // Get the height of the cursor sprite

        // Declare Others
        public Texture2D Texture; // Animation representing the cursor
        public Vector2 Position; // Position of the cursor relative to the upper left side of the screen
        

        public void Initialize(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position; // Set the starting position of the player around the middle of the screen and to the back
            Active = true; // Set the player to be active
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public void Update()
        {
            // Keep in-game cursor with mouse cursor position
            Position.X = Game.CurrentMouseState.X;
            Position.Y = Game.CurrentMouseState.Y;

            if (Game.CurrentMouseState.ScrollWheelValue > Game.PreviousMouseState.ScrollWheelValue)
            {
                Game.TextureType += 1;
                if (Game.TextureType > 4) Game.TextureType = 1;
            }
            else if (Game.CurrentMouseState.ScrollWheelValue < Game.PreviousMouseState.ScrollWheelValue)
            {
                Game.TextureType -= 1;
                if (Game.TextureType < 1) Game.TextureType = 4;
            }
        }
    }
}