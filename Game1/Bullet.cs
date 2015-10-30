using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace Game1
{
    class Bullet : BasicModel
    {

        Vector3 traget;
        Vector3 velocity;
        Matrix scale = Matrix.CreateScale(.05f);
        Matrix translate;
        public Bullet(Model model, Vector3 pos, Vector3 target)
            : base(model)
        {
            this.translate = Matrix.CreateTranslation(pos) ;
            this.traget = target;
            velocity = target - pos;
            velocity.Normalize();

        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {

            base.Draw(device, camera);
        }

        public override void Update(GameTime gameTime)
        {
            translate *= Matrix.CreateTranslation(velocity * gameTime.ElapsedGameTime.Milliseconds/3);
            base.Update(gameTime);
        }
        protected override Matrix GetWorld()
        {
            world = scale * translate;
            return world;
        }
    }
}
