using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AnimationStarter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Mario texture stuff
        private Texture2D marioTexture;
        private Vector2 marioPosition;
        int numSpritesInSheet;
        int widthOfSingleSprite;

        // Animation reqs
        int currentFrame;
        double fps;
        double secondsPerFrame;
        double timeCounter;

        // Mario Statemachine
        private enum MarioState
        {
            WalkLeft,
            FaceLeft,
            Standing,
            FaceRight,
            WalkRight,
            Jump
        }

        // Mario Position
        private float XPos = 200;
        private float YPos = 200;

        private MarioState MarioPreviousState = MarioState.Standing;
        private MarioState MarioCurrentState = MarioState.Standing;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.Position = new Point(                    // Center the game view on the screen
                (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2) -
                    (graphics.PreferredBackBufferWidth / 2),
                (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2) -
                    (graphics.PreferredBackBufferHeight / 2)
            );
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            marioTexture = Content.Load<Texture2D>("MarioSpriteSheet");
            numSpritesInSheet = 4;
            widthOfSingleSprite = marioTexture.Width / numSpritesInSheet;

            marioPosition = new Vector2(XPos, YPos);

            // Set up animation stuff
            currentFrame = 1;
            fps = 10.0;
            secondsPerFrame = 1.0f / fps;
            timeCounter = 0;
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // *** Put code to check and update FINITE STATE MACHINE here

            ProcessInput();
            //UpdateState();

            // Update the animation
            UpdateAnimation(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// Updates the animation time
        /// </summary>
        /// <param name="gameTime">Game time information</param>
        private void UpdateAnimation(GameTime gameTime)
        {
            // Add to the time counter (need TOTALSECONDS here)
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // Has enough time gone by to actually flip frames?
            if (timeCounter >= secondsPerFrame)
            {
                // Update the frame and wrap
                currentFrame++;
                if (currentFrame >= 4)
                    currentFrame = 1;

                // Remove one "frame" worth of time
                timeCounter -= secondsPerFrame;
            }

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            // *** Put code to check FINITE STATE MACHINE
            // *** and properly draw mario here

            // Example call to draw mario walking (replace or adjust this line!)
            //DrawMarioWalking(SpriteEffects.FlipHorizontally);
            DrawMario();


            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws mario with a walking animation
        /// </summary>
        /// <param name="flip">Should he be flipped horizontally?</param>
        private void DrawMarioWalking(SpriteEffects flip)
        {
            spriteBatch.Draw(
                marioTexture,
                marioPosition,
                new Rectangle(widthOfSingleSprite * currentFrame, 0, widthOfSingleSprite, marioTexture.Height),
                Color.White,
                0.0f,
                Vector2.Zero,
                1.0f,
                flip,
                0.0f);
        }

        /// <summary>
        /// Draws mario standing still
        /// </summary>
        /// <param name="flip">Should he be flipped horizontally?</param>
        private void DrawMarioStandinging(SpriteEffects flip)
        {
            spriteBatch.Draw(
                marioTexture,
                marioPosition,
                new Rectangle(0, 0, widthOfSingleSprite, marioTexture.Height),
                Color.White,
                0.0f,
                Vector2.Zero,
                1.0f,
                flip,
                0.0f);
        }

        private void DrawMario()
        {
            Rectangle currentMario = new Rectangle();
            SpriteEffects flips = SpriteEffects.None;

            if (MarioCurrentState == MarioState.FaceLeft ||
                MarioCurrentState == MarioState.WalkLeft)                
            {
                flips = SpriteEffects.FlipHorizontally;

                currentMario = new Rectangle(
                                   widthOfSingleSprite * currentFrame,
                                   0,
                                   widthOfSingleSprite,
                                   marioTexture.Height);
            }
            else if (MarioCurrentState == MarioState.FaceRight ||
                MarioCurrentState == MarioState.WalkRight)
            {
                currentMario = new Rectangle(
                                   widthOfSingleSprite * currentFrame,
                                   0,
                                   widthOfSingleSprite,
                                   marioTexture.Height);
            }
            else
            {
                currentMario = new Rectangle(
                                   0,
                                   0,
                                   widthOfSingleSprite,
                                   marioTexture.Height);
            }
            
            spriteBatch.Draw(
                marioTexture,
                marioPosition,
                currentMario,
                //new Rectangle(
                //    widthOfSingleSprite * currentFrame,
                //    0,
                //    widthOfSingleSprite,
                //    marioTexture.Height),
                Color.White,
                0.0f,
                Vector2.Zero,
                1.0f,
                flips,
                0.0f);
        }

        private void ProcessInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            MarioPreviousState = MarioCurrentState;

            if (keyboardState.IsKeyDown(Keys.A))                    // Move left
            {
                XPos = XPos - 5;

                if (MarioPreviousState == MarioState.WalkRight)
                {
                    MarioCurrentState = MarioState.FaceRight;
                }
                else if (MarioPreviousState == MarioState.FaceRight)
                {
                    MarioCurrentState = MarioState.Standing;
                }
                else if (MarioPreviousState == MarioState.Standing)
                {                    
                    MarioCurrentState = MarioState.FaceLeft;
                }
                else if (MarioPreviousState == MarioState.FaceLeft)
                {
                    MarioCurrentState = MarioState.WalkLeft;
                }
            }

            if (keyboardState.IsKeyDown(Keys.D))                    // Move right
            {
                XPos = XPos + 5;

                if (MarioPreviousState == MarioState.WalkLeft)
                {
                    MarioCurrentState = MarioState.FaceLeft;
                }
                else if (MarioPreviousState == MarioState.FaceLeft)
                {
                    MarioCurrentState = MarioState.Standing;
                }
                else if (MarioPreviousState == MarioState.Standing)
                {
                    MarioCurrentState = MarioState.FaceRight;
                }
                else if (MarioPreviousState == MarioState.FaceRight)
                {
                    MarioCurrentState = MarioState.WalkRight;
                }
            }

            if (keyboardState.IsKeyDown(Keys.W))                    // Move up
            {
                YPos = YPos - 5;
            }

            if (keyboardState.IsKeyDown(Keys.S))                    // Move down
            {
                YPos = YPos + 5;
            }

            if (keyboardState.IsKeyUp(Keys.W) &&
                keyboardState.IsKeyUp(Keys.A) &&
                keyboardState.IsKeyUp(Keys.S) &&
                keyboardState.IsKeyUp(Keys.D))
            {
                MarioCurrentState = MarioState.Standing;
            }

            marioPosition = new Vector2(XPos, YPos);            // Update characterPosition variable
        }

        //private void UpdateState()
        //{
        //    MarioPreviousState = MarioCurrentState;

        //    KeyboardState keyboardState = Keyboard.GetState();

        //    Keys[] pressedKeys = keyboardState.GetPressedKeys();

        //    for (int i = 0; i < pressedKeys.Length; i++)
        //    {
        //        switch (pressedKeys[i])
        //        {
        //            case Keys.A:
        //                if (MarioPreviousState == MarioState.WalkRight)
        //                {
        //                    MarioCurrentState = MarioState.FaceRight;
        //                }
        //                else if (MarioPreviousState == MarioState.FaceRight)
        //                {
        //                    MarioCurrentState = MarioState.Standing;
        //                }
        //                else if (MarioPreviousState == MarioState.Standing)
        //                {
        //                    MarioCurrentState = MarioState.FaceLeft;
        //                }
        //                else if (MarioPreviousState == MarioState.FaceLeft)
        //                {
        //                    MarioCurrentState = MarioState.WalkLeft;
        //                }

        //                break;
        //            case Keys.S:
        //                MarioCurrentState = MarioState.Standing;

        //                break;
        //            case Keys.D:
        //                if (MarioPreviousState == MarioState.WalkLeft)
        //                {
        //                    MarioCurrentState = MarioState.FaceLeft;
        //                }
        //                else if (MarioPreviousState == MarioState.FaceLeft)
        //                {
        //                    MarioCurrentState = MarioState.Standing;
        //                }
        //                else if (MarioPreviousState == MarioState.Standing)
        //                {
        //                    MarioCurrentState = MarioState.FaceRight;
        //                }
        //                else if (MarioPreviousState == MarioState.FaceRight)
        //                {
        //                    MarioCurrentState = MarioState.WalkRight;
        //                }

        //                break;
        //            case Keys.W:
        //                MarioCurrentState = MarioState.Jump;

        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}
    }
}
