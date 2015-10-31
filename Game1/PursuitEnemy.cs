using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class PursuitEnemy : Tank
    {
        private Tank targetTank;
        public List<Vector3> path;
        Pathfinder pathfinder;
        Map map;
        int moveorder;
        public bool isMoving;
        public float distanceTopickPosition; 
        float desThresholdtoplayer = 20f;
        public List<Vector3> pathdebug;
        public MousePicking mousepick;
        public Vector3 pickPosition;
        public PursuitEnemy(Model model, Vector3 position,GraphicsDevice device, Camera camera)
            : base(model, device, camera)
        {
            tankBox = new BoundingBox(MIN, MAX);
            CurrentPosition = position;
            pickPosition = CurrentPosition;
            translation = Matrix.CreateTranslation(Map.MapToWorld(new Point(10, 10)));
            velocity = Vector3.Zero;
            new Tank(model, device, camera);
            steer = new Steering(100f, 100f);
            map = new Map();
            pathfinder = new Pathfinder(map);
            isMoving = false;
            initialAngle = MathHelper.PiOver2;
            moveorder = 0;
            mousepick = new MousePicking(device, camera);

        }

        public override void Update(GameTime gametime)
        { 
            min = MIN + CurrentPosition;
            max = MAX + CurrentPosition;
            tankBox = new BoundingBox(min, max);
            //float distance = Vector3.Subtract(targetTank.CurrentPosition, this.CurrentPosition).Length();
//distance > Tank.destinationThreshold &&            
            if (tankBox.Contains(targetTank.tankBox) == ContainmentType.Disjoint)
            {
                
            //if (Mouse.GetState().LeftButton == ButtonState.Pressed && mousepick.GetCollisionPosition().HasValue == true)
            //{
                //pickPosition = mousepick.GetCollisionPosition().Value; 
                Point start = Map.WorldToMap(CurrentPosition);

                Point end = Map.WorldToMap(targetTank.CurrentPosition);
                //Point end = Map.WorldToMap(pickPosition);
                if (end.X < 20 && end.X>=0&&end.Y < 20&&end.Y>=0)
                {
                pathfinder = new Pathfinder(map);
                path = pathfinder.FindPath(start, end);
                //pathdebug = path; 
                }


            }

            if(path!=null&& moveorder<path.Count)
            {
                distanceTopickPosition = Vector3.Distance(path[moveorder], CurrentPosition);
                if (distanceTopickPosition > desThresholdtoplayer)
                {
                    speed = velocity.Length();
                    currentAngle = (float)Math.Atan2(velocity.Z, velocity.X);
                    moveAngle = currentAngle - initialAngle;
                    rotation = Matrix.CreateRotationY(-moveAngle);
                    //wheelRotationValue = (float)gametime.TotalGameTime.TotalSeconds * 10;
                    //canonRotationValue = (float)Math.Sin(gametime.TotalGameTime.TotalSeconds * 0.25f) * 0.333f - 0.333f;
                    //hatchRotationValue = MathHelper.Clamp((float)Math.Sin(gametime.TotalGameTime.TotalSeconds * 2) * 2, -1, 0);
                    velocity += steer.seek(path[moveorder], CurrentPosition, velocity) * (float)gametime.ElapsedGameTime.TotalSeconds;
                    CurrentPosition += velocity * (float)gametime.ElapsedGameTime.TotalSeconds;
                    translation = Matrix.CreateTranslation(CurrentPosition);
                }
                else moveorder++;

            }
            else
            {
                moveorder = 0;
                if (path != null) path.Clear();
            }
            //base.Update(gametime);
            //turretRorationValue = (float)Math.Sin(gametime.TotalGameTime.TotalSeconds);




        }

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }

        protected override Matrix GetWorld()
        {
            return Matrix.CreateScale(0.1f) * rotation * translation;
        }

        public void TargetPlayer(Tank playerTank)
        {
            this.targetTank = playerTank;
        }
    }
}
