using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDevice device { get; protected set; }
        public Camera camera{get; protected set;}
        public Random rnd {get;protected set; }

        //新加代码start
        SpriteFont font;
        SpriteFont scoreFont;
        BasicEffect effect;
        //score
        SplashScreen splashScreen;
        int score = 0;
        int health =13;
        int Health
        {
            get; set;
        } 
        int level = 1;

        int killed = 0;

        //游戏状态管理
        public enum GameState { START, PLAY, LEVEL_CHANGE, END }
        GameState currentGameState = GameState.START;

        //新加代码end---------------


        MousePicking mousePick;


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ModelManager modelManager;

        SoundEffect bgm;
        public SoundEffect soundShot;
        public SoundEffect soundMove;
        public SoundEffect soundHit;
        SoundEffectInstance music;

        float shotCountdown = 0;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            rnd = new Random();
//            graphics.PreferredBackBufferWidth = 1280;
//            graphics.PreferredBackBufferHeight = 1024;
//# if !DEBUG
//            graphics.isFullScreen = trus;
//#endif
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
            camera = new Camera(this, new Vector3(0, 700, 700), new Vector3(0,25,0), Vector3.Up);
            Components.Add(camera);

            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            //开始界面
            modelManager.Enabled = false;
            modelManager.Visible = false;
            // Splash screen component 
            splashScreen = new SplashScreen(this);
            Components.Add(splashScreen);
            splashScreen.SetData("Welcome to Assignment 1", currentGameState);
            //开始界面end-------------

            IsMouseVisible = true;
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
            device = graphics.GraphicsDevice;

            //字体start
            font = Content.Load<SpriteFont>(@"Font/Arial");
            scoreFont = Content.Load<SpriteFont>(@"Font/ScoreFont");


            effect = new BasicEffect(device)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };
            //字体end

            //sound 

            bgm = Content.Load<SoundEffect>(@"Sound/bgm");
            soundShot = Content.Load<SoundEffect>(@"Sound/spray");
            soundMove = Content.Load<SoundEffect>(@"Sound/spray");
            soundHit = Content.Load<SoundEffect>(@"Sound/spray");

            music = bgm.CreateInstance();
            music.IsLooped = true;
            music.Play();
            


            // TODO: use this.Content to load your game content here
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //camera.cameraPosition = modelManager.tankCurrentPosition + (new Vector3(camera.cameraDirection.X, 60, camera.cameraDirection.Z +60));
      

            // TODO: Add your update logic here
            if (shotCountdown <= 0)
            {
                if (Mouse.GetState().RightButton == ButtonState.Pressed)
                {

                    Vector3? pickpos;
                    MousePicking mousePick;

                    mousePick = new MousePicking(device, camera);
                    pickpos = mousePick.GetCollisionPosition();
                    
                    if (pickpos.HasValue)
                    {
                        modelManager.AddBullets(pickpos.Value);
                        soundShot.Play();
                        shotCountdown = 500;
                    }
                }
            }
            else
            {
                shotCountdown -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (health == 0)
            {
                ChangeGameState(GameState.END, level);
            }

            if (killed >= (int)modelManager.levelInfoList[modelManager.currentLevel].numberEnemies)
            {
                if (modelManager.currentLevel > 10)
                {
                    ChangeGameState(GameState.END, level);
                }
                else
                {
                    
                    ChangeGameState(GameState.LEVEL_CHANGE, level++);
                    modelManager.enemyThisLevel = 0; 
                    modelManager.currentLevel++;
                    modelManager.SetNextSpawnTime();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);蓝色界面
            //LineBatch.DrawLine(_spriteBatch,
            //              Color.Black,
            //              Vector2.Zero,
            //              new Vector2(100, 300));

            // UI界面
            device.DepthStencilState = DepthStencilState.Default;
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
            // TODO: Add your drawing code here

            // Vector3 textPosition1 = new Vector3(-50, 75, -30);
            //effect.World = Matrix.CreateScale(1, -1, 1) * Matrix.CreateTranslation(textPosition1);
            //effect.View = camera.view;
            //effect.Projection = camera.projection;

            string message1 = "mousePosition:" + modelManager.PickPositionString;
            string message2 = "currentPosition:" + modelManager.CurrentPosition;
            //string message3 = "count:" + modelManager.count.ToString();
            //string message4 = "pick:" + modelManager.cur.ToString();
            //string message5 = "cur:" + modelManager.pos.ToString();

            Vector2 textOrigin = scoreFont.MeasureString(message1) / 2;
            const float textSize = 0.2f;

            // Draw the current score //分数
            string scoreText = "Score: " + score + " Health: " + health + " MaxEnemies: " + (int)modelManager.levelInfoList[modelManager.currentLevel].numberEnemies;
            base.Draw(gameTime);
            spriteBatch.Begin();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            //spriteBatch.DrawString(scoreFont, scoreText, new Vector2(0, 0), Color.Red);
            //spriteBatch.End();


            //       spriteBatch.Begin(0, null, null, DepthStencilState.Default, RasterizerState.CullNone, effect);
            //spriteBatch.DrawString(font, message1, Vector2.Zero, Color.Red, 0, textOrigin, textSize, 0, 0);
            //spriteBatch.DrawString(font, message2, new Vector2(15, 15), Color.Red, 0, textOrigin, textSize, 0, 0);
            spriteBatch.DrawString(scoreFont, scoreText, new Vector2(0, 0), Color.Red); 
            spriteBatch.DrawString(scoreFont, message2, new Vector2(0, 20), Color.Red);
            //spriteBatch.DrawString(scoreFont, scoreText, new Vector2(0, 0), Color.Red);
            //spriteBatch.DrawString(font, message3, new Vector2(20, 40), Color.Red);
            //spriteBatch.DrawString(font, message4, new Vector2(20, 60), Color.Red);
            //spriteBatch.DrawString(font, message5, new Vector2(20, 80), Color.Red);

            spriteBatch.End();
            
        }
        public void ChangeGameState(GameState state, int level)
        {
            killed = 0;
            modelManager.enemyThisLevel = 0;
            currentGameState = state;
            switch (currentGameState)
            {
                case GameState.LEVEL_CHANGE:
                    splashScreen.SetData("Level " + (level + 1),
                     GameState.LEVEL_CHANGE);
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;
                    // Stop the soundtrack loop 
                    //trackCue.Stop(AudioStopOptions.Immediate); 
                    break;
                case GameState.PLAY:
                    modelManager.Enabled = true;
                    modelManager.Visible = true;
                    splashScreen.Enabled = false;

                    splashScreen.Visible = false;
                    //if (trackCue.IsPlaying) 
                    //trackCue.Stop(AudioStopOptions.Immediate); 
                    // To play a stopped cue, get the cue from the soundbank again 
                    //trackCue = soundBank.GetCue("Tracks"); 
                    //trackCue.Play(); 
                    break;
                case GameState.END:
                    splashScreen.SetData("Game Over.\nLevel: " + (level) +
                 "\nScore: " + score, GameState.END);
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;
                    // Stop the soundtrack loop 
                    //trackCue.Stop(AudioStopOptions.Immediate); 
                    break;
            }

        }

        public void reduceHealth()
        {
            health--;
        }
        public void AddPoints()
        {
            //score += points;
            score+=100;
            killed++;
        }
        public void DeductPoints()
        {
            //score += points;
            score-=200;
        
        }

        public void kill()
        {
            killed++;
        }
    }
}
