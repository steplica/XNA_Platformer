// USED TO MANIPULATE THE PLAYER OBJECT

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
    public class Actor
    {
        // Declare Everything
        public float HorizontalSpeed = 0; // Player's current running speed
        public float HorizontalAcceleration = 0.4f; // The rate at which the player's horizontal speed builds up to max
        public float MaxHorizontalSpeed = 6.5f; // Player's capped running speed
        public float VerticalSpeed; // Stores the vertical speed of player (gravity and jumping affects this)
        public float VerticalAcceleration = 0.5f; // How quickly things accelerate vertically (gravity)
        public float MaxVerticalSpeed = 15; // Player's capped falling speed/terminal velocity

        public float MaxJumpHeight = 10; // Maximum height the player will reach that jump

        public bool IsGrounded = false; // If the player is colliding
        public bool IsFalling = false; // If player is falling
        public bool IsJumping = false; // If player is performing a jump

        public bool IsMovingLeft = false; // If the player is moving left
        public bool IsMovingRight = false; // If the player is moving right
        public bool IsMovingUp = false; // If the player is moving up
        public bool IsMovingDown = false; // If the player is moving down

        public bool CanMoveUp; // Can the player move up?
        public bool CanMoveDown; // Can the player fall down?
        public bool CanMoveLeft; // Can the player move left?
        public bool CanMoveRight; // Can the player move right?

        public int Width { get { return Texture.Width; } } // Get the width of the player sprite
        public int Height { get { return Texture.Height; } } // Get the height of the player sprite

        public Rectangle Bounds; // Rectangle that wraps around the player sprite
        public Rectangle FutureUpwardBounds; // Where the player sprite will be the next time the game updates
        public Rectangle FutureDownwardBounds; // Where the player sprite will be the next time the game updates
        public Rectangle FutureLeftBounds; // Where the player sprite will be the next time the game updates
        public Rectangle FutureRightBounds; // Where the player sprite will be the next time the game updates

        public float LandedX = 0;
        public Vector2 Position; // Position of the player relative to the upper left side of the screen
        public Texture2D Texture; // Animation representing the player


        public void Update()
        {
            IsMovingLeft = false;
            IsMovingRight = false;
            IsMovingUp = false;
            IsMovingDown = false;
            if ((Game.CurrentKeyboardState.IsKeyDown(Keys.Left) || Game.CurrentKeyboardState.IsKeyDown(Keys.A)) && CanMoveLeft)
            {
                if (HorizontalSpeed < MaxHorizontalSpeed) HorizontalSpeed += HorizontalAcceleration;
                else HorizontalSpeed = MaxHorizontalSpeed;
                Position.X -= HorizontalSpeed;
                IsMovingLeft = true;
            }
            else if ((Game.CurrentKeyboardState.IsKeyDown(Keys.Right) || Game.CurrentKeyboardState.IsKeyDown(Keys.D)) && CanMoveRight)
            {
                if (HorizontalSpeed < MaxHorizontalSpeed) HorizontalSpeed += HorizontalAcceleration;
                else HorizontalSpeed = MaxHorizontalSpeed;
                Position.X += HorizontalSpeed;
                IsMovingRight = true;
            }
            else
            {
                if (HorizontalSpeed > 0) HorizontalSpeed -= HorizontalAcceleration;
                else HorizontalSpeed = 0;
            }


            if (VerticalSpeed > 0)
                IsMovingDown = true;
            else if (VerticalSpeed < 0)
                IsMovingUp = true;

            
            if ((Game.CurrentKeyboardState.IsKeyDown(Keys.W) || Game.CurrentKeyboardState.IsKeyDown(Keys.Up)) && VerticalSpeed == 0 && IsGrounded)
            {
                IsJumping = true;
                Position.Y -= 1;
                VerticalSpeed -= MaxJumpHeight;
            }
            if (Game.CurrentKeyboardState.IsKeyDown(Keys.RightShift))
            {
                Position.Y = 0;
                VerticalSpeed = 0;
            }



            //*
            
            // JUMP
            if ((Game.CurrentKeyboardState.IsKeyDown(Keys.W) || Game.CurrentKeyboardState.IsKeyDown(Keys.Up)) && VerticalSpeed == 0 && IsGrounded)
            {
                IsJumping = true;
                Position.Y -= 1;
                VerticalSpeed = 0 - MaxJumpHeight;
            }
            if (Game.CurrentKeyboardState.IsKeyDown(Keys.RightShift))
            {
                Position.Y = 0;
                VerticalSpeed = 0;
            }
            //*/

            // Non-input checking
            if (VerticalSpeed != 0)
            {
                IsFalling = true;
            }
            else if (!IsJumping)
            {
                IsFalling = false;
            }
            if (VerticalAcceleration != 0.5f)
            {
                VerticalAcceleration = 0.5f;
            }
            if (!IsGrounded)
            {
                VerticalSpeed += VerticalAcceleration;
                Position.Y += VerticalSpeed;
            }
            // Make sure that the player does not go out of bounds
            Position.X = MathHelper.Clamp(Position.X, 0, Game.RoomWidth - Width);
            Position.Y = MathHelper.Clamp(Position.Y, 0, Game.RoomHeight - Height);
            if (Position.Y <= 1 && !IsGrounded)
            {
                VerticalSpeed++;
                IsJumping = false;
            }

            // Obtain Player's current and future sprite boundaries to work with collision detection
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); // Recreate Player bounds at play coords
            FutureUpwardBounds = new Rectangle((int)Position.X, (int)Position.Y - Math.Abs((int)VerticalSpeed), Texture.Width, Texture.Height); // Recreate player bounds at future upward coord
            FutureDownwardBounds = new Rectangle((int)Position.X, (int)Position.Y + Math.Abs((int)VerticalSpeed), Texture.Width, Texture.Height); // Recreate player bounds at future downward coords
            FutureLeftBounds = new Rectangle((int)Position.X - 1 - (int)HorizontalSpeed, (int)Position.Y, Texture.Width, Texture.Height); // Recreate player bounds at future left coords
            FutureRightBounds = new Rectangle((int)Position.X + 1 + (int)HorizontalSpeed, (int)Position.Y, Texture.Width, Texture.Height); // Recreate player bounds at future right coords
        }



        // Move player to the outer side of the block that he is closest to--like in minecraft




        public void CheckCollisions(Rectangle ObjectBounds, string BlockType, int width, int height)
        {
            if (BlockType == "solid")
            {




                if (FutureUpwardBounds.Intersects(ObjectBounds) && IsMovingUp) // Future Upward Bounds
                {
                    if (VerticalSpeed < 0)
                    {
                        Position.Y = ObjectBounds.Top + ObjectBounds.Width;
                        VerticalSpeed = 0;
                    }
                }
                if (FutureLeftBounds.Intersects(ObjectBounds)) // Future Left bounds
                {
                    if (IsMovingLeft) Position.X = ObjectBounds.Left + ObjectBounds.Width;
                    CanMoveLeft = false;
                }
                if (FutureRightBounds.Intersects(ObjectBounds)) // Future Right bounds
                {
                    if (IsMovingRight) Position.X = ObjectBounds.Left - Width;
                    CanMoveRight = false;
                }
                if (FutureDownwardBounds.Intersects(ObjectBounds)) // Future Downward Bounds
                {
                    if (Bounds.Intersects(ObjectBounds))
                    {
                        VerticalSpeed = 0;
                        Position.Y = ObjectBounds.Top - 1;
                        IsJumping = false;
                        IsGrounded = true;
                        if (IsMovingLeft && CanMoveLeft) Position.X -= HorizontalSpeed;
                        else if (IsMovingRight && CanMoveRight) Position.X += HorizontalSpeed;
                    }
                }
                if (new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height + 1).Intersects(ObjectBounds)) // Current bounds
                {
                    IsJumping = false;
                    IsGrounded = true;
                    VerticalSpeed = 0;
                    Position.Y = ObjectBounds.Top - Height;
                }
            }





            else if (BlockType == "liquid")
            {

            }
            else if (BlockType == "air")
            {

            }
        }

        public void Initialize(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position; // Set the starting position of the player around the middle of the screen and to the back
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}