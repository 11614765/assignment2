using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class Text : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public int Score { get; private set; } // display score
        public int Life { get; private set; } //display life

        private Vector2 left_pos, right_pos;

        private string left_text, right_text;


        public Text(Game game) : base(game) { }

        public override void Initialize()
        {
            Life = 13;

            left_text = "Score: " + Score;
            
            base.Initialize();
        }

        
    }
}
