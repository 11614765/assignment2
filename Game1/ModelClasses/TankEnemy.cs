using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    class TankEnemy: BasicModel
    {
        Matrix rotation = Matrix.Identity;
        float yawAngle = 0;
        float pitchAngel = 0;
        float rollAngle = 0;
        Vector3 direction;
        Vector3 tankPosition { get; set; } 
        GraphicsDevice device;
        Camera camera;
        Tank tank1;
        int speed;

        public TankEnemy(Model m, Vector3 Position,Tank tank, int speed ) : base(m)
        {
            world =  Matrix.CreateScale(.1f)*Matrix.CreateTranslation(Position) ;
            this.tank1 = tank;
            this.speed = speed;
        }
        public override void Update(GameTime gameTime)
        {
            tankPosition = tank1.CurrentPosition;
            direction = tankPosition - world.Translation;
            direction.Normalize();

            if (this.CollidesWith(tank1.model, tank1.world))
            {

            }
            else
            {
                Vector3 path = direction * speed;
                world *= Matrix.CreateTranslation(path);
            }

        }
       protected override Matrix GetWorld()
        {

            return world;
        }
    }
}
