using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{

    public enum HumanState
    {
        SEEK,
        FLEE
    }
    class Human : BasicModel
    {
        private Tank targetTank;

        private Matrix rotation = Matrix.Identity;

        private Vector3 position;
        private Vector3 targetPosition;
        private Vector3 orintation;
        private Vector3 currentVelocity;
        private Vector3 desiredVelocity;
        private Vector3 steeringForce;

        private double currentSpeed = 0;
        private double rotationSpeed = MathHelper.PiOver4 / 200;
        private double orintationAngle;
        private double tankAngle = 0;//MathHelper.PiOver2;

        private double acceleration = 0.005;
        private float maxSpeed;
        private float minStopSpeed = 0.1f;
        private float fleeDistance = 300;
        private float boundary = 1000f;
        private float scaleRatio = 0.05f;
        private int mass = 10;
        HumanState humanState;
        private bool isMoving;
        private bool isPatrolling;

        public Human (Model m, Vector3 Position, Tank tank, int speed) : base(m)
        {
            humanState = HumanState.SEEK;
            this.position = Position;
            this.targetTank = tank;
            this.maxSpeed = speed;

            currentVelocity = Vector3.Normalize(new Vector3(0, 0, 1)); 

            RandomPatrolPoint();
            translation = Matrix.CreateTranslation(position);
        }

        public override void Update(GameTime gameTime)
        {
            //tankPosition = tank1.CurrentPosition;
            //direction = tankPosition - world.Translation;
            //direction.Normalize();

            //if (this.CollidesWith(tank1.model, tank1.world))
            //{

            //}
            //else
            //{
            //    Vector3 path = direction * speed;
            //    world *= Matrix.CreateTranslation(path);
            //}
            int time = gameTime.ElapsedGameTime.Milliseconds;
            float distance = (targetTank.CurrentPosition - this.position).Length();
            if (distance < fleeDistance)
            {
                isPatrolling = false;
                targetPosition = targetTank.CurrentPosition;
                orintation = position - targetPosition;   //opposite direction of the target 
            }
            else
            {
                isPatrolling = true;
            }
            MovingToTarget(time);
            base.Update(gameTime);

        }

        private void MovingToTarget(int time)
        {
            double turnedAngle = rotationSpeed * time;
            desiredVelocity = Vector3.Normalize(orintation) * maxSpeed;
            orintationAngle = Math.Atan2(orintation.X, orintation.Z);
            if (currentVelocity.Length() > 1)
                currentVelocity.Normalize();
            currentVelocity *= (float)currentSpeed;
            //Flee tank will flee opposite way of player if player is in flee distance range
            if ((targetPosition - position).Length() < fleeDistance)
            {
                isMoving = true;
                
                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += acceleration * time;
                }
                else
                    currentSpeed = maxSpeed;
                currentVelocity *= (float)currentSpeed;
                //steering behavior of the enemy tank
                Steering(time);
                //update 9/5
                RotateTank(turnedAngle);
            }
            else
            {
                //smoothly slow down, acceleration here is also brake force
                if (Math.Abs(currentSpeed) < minStopSpeed)
                    currentSpeed = 0;
                if (currentSpeed > 0)
                {
                    currentSpeed -= acceleration * time;
                }
                else
                    currentSpeed = 0;
                //steering behavior of the enemy tank
                Steering(time);
                RotateTank(turnedAngle);
                isMoving = false;
            }
        }

        private void LimitInBoundary()
        {
            float minBoundary = 1100;
            if (position.X > minBoundary)
                position.X = minBoundary;
            if (position.X < -minBoundary)
                position.X = -minBoundary;
            if (position.Z > minBoundary)
                position.Z = minBoundary;
            if (position.Z < -minBoundary)
                position.Z = -minBoundary;
        }

        private void RotateTank(double turnedAngle)
        {
            //rotate the tank fram axis Y
            if (tankAngle > MathHelper.Pi || tankAngle < -MathHelper.Pi)
            {
                tankAngle = orintationAngle;
            }
            double angleDifference = tankAngle - orintationAngle;
            if (Math.Abs(angleDifference) < MathHelper.PiOver4 / 10)
            {
                rotation = Matrix.CreateRotationY((float)orintationAngle);
            }
            else
            {
                if (tankAngle > 0)
                {
                    if (angleDifference > 0 && angleDifference < MathHelper.Pi)
                        tankAngle -= turnedAngle;
                    else
                        tankAngle += turnedAngle;
                }
                else
                {
                    if (angleDifference > -MathHelper.Pi && angleDifference < 0)
                        tankAngle += turnedAngle;
                    else
                        tankAngle -= turnedAngle;
                }
                rotation = Matrix.CreateRotationY((float)tankAngle);
            }
        }

        private void Steering(int elapsedFrameTime)
        {
            if (isMoving)
            {
                steeringForce = desiredVelocity - currentVelocity;
                steeringForce /= mass;

                if (steeringForce.Length() < 1)
                {
                    currentVelocity = desiredVelocity;
                }
                else
                {
                    currentVelocity += steeringForce * elapsedFrameTime;
                }

            }
            position += currentVelocity;
            LimitInBoundary();
            translation = Matrix.CreateTranslation(position);
        }
        protected override Matrix GetWorld()
        {
            world = Matrix.CreateScale(.1f) * rotation* translation ;
            return world;
        }

        public void RandomPatrolPoint()
        {
            Random ran = new Random();
            int x = ran.Next(-1000, 1000);
            int z = ran.Next(-1000, 1000);
            targetPosition = new Vector3(x, 0, z);
        }
    }
}
