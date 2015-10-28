using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;

namespace Game1
{
    class SplashScreen:Microsoft.Xna.Framework.DrawableGameComponent
    {
        string textToDraw;
        string secondaryTextToDraw;
        SpriteFont spriteFont;
        SpriteFont secondarySpriteFont;
        SpriteBatch spriteBatch;
        Game1.GameState currentGameState;

        public SplashScreen(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        protected override void LoadContent( ) 
            { 
             // Load fonts 
                spriteFont = Game.Content.Load<SpriteFont>(@"Font\SplashScreenFontLarge");
                secondarySpriteFont = Game.Content.Load<SpriteFont>(@"Font\SplashScreenFont"); 
             // Create sprite batch 
             spriteBatch = new SpriteBatch(Game.GraphicsDevice); 
            base.LoadContent( ); 
            }

        public override void Update(GameTime gameTime)
        {
            //player hit enter
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                // If we're not in end game, move to play state 
                if (currentGameState == Game1.GameState.LEVEL_CHANGE || 
                currentGameState == Game1.GameState.START) 
                ((Game1)Game).ChangeGameState(Game1.GameState.PLAY, 0); 
                 
                    // If we are in end game, exit 
                else if (currentGameState == Game1.GameState.END) 
                Game.Exit( ); 
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            // Get size of string 
            Vector2 TitleSize = spriteFont.MeasureString(textToDraw);
            // Draw main text 
            spriteBatch.DrawString(spriteFont, textToDraw,
                new Vector2(Game.Window.ClientBounds.Width / 2 - TitleSize.X / 2,
                Game.Window.ClientBounds.Height / 2),
                Color.Black);
            // Draw subtext 
            spriteBatch.DrawString(secondarySpriteFont,
            secondaryTextToDraw,
              new Vector2(Game.Window.ClientBounds.Width / 2 - secondarySpriteFont.MeasureString(
                secondaryTextToDraw).X / 2,
                Game.Window.ClientBounds.Height / 2 +
                TitleSize.Y + 10),
                Color.Black);
            //spriteBatch.DrawString(spriteFont, textToDraw, new Vector2(30, 30), Color.Red, 0, new Vector2(30, 30), 10, 0, 0);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void SetData(string main, Game1.GameState currGameState)
        {
            textToDraw = main;
            this.currentGameState = currGameState;
            switch (currentGameState)
            {
                case Game1.GameState.START:
                case Game1.GameState.LEVEL_CHANGE:
                    secondaryTextToDraw = "Press ENTER to begin";
                    break;
                case Game1.GameState.END:
                    secondaryTextToDraw = "Press ENTER to quit";
                    break;
            }
        } 
    
   
    }
}
