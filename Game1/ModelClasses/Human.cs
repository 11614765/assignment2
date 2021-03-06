﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

namespace Game1
{

    public enum GhostState
    {
        IDLE,
        PURSUE,
        FLEE
    }

    public enum GhostConditions
    {
        PLAYERNEAR,
        PLAYERFAR,
        PLAYERTOOFAR
    }

    class Human : BasicModel
    {
        private Tank targetTank;
        private delegate void Behavious(GameTime gameTime);
        Behavious[] behavious;
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
        private double ghostAngle = 0;//MathHelper.PiOver2;

        private double acceleration = 0.005;
        private float maxSpeed;
        private float minStopSpeed = 0.1f;
        private float fleeDistance = 160;
        private float boundary = 1000f;
        private float scale = 0.05f;
        private int mass = 10;
        GhostState ghostState;
        GhostConditions ghostCondition;
        Steering steer = new Steering(100f, 100f);
        private bool isMoving;
        XElement states = XElement.Load(@"Content/config/fsm_Human.xml");

        public Human(Model m, Vector3 Position, Tank tank, int speed)
            : base(m)
        {

            ghostState = GhostState.IDLE;
            this.position = Position;
            this.targetTank = tank;
            this.maxSpeed = speed;

            currentVelocity = Vector3.Normalize(new Vector3(0, 0, 1)); 

            //RandomPatrolPoint();
            translation = Matrix.CreateTranslation(position);
            //behavious[0] = Flee;
            behavious = new Behavious[2];
        }


        public override void Update(GameTime gameTime)
            {
            float distance = (targetTank.CurrentPosition - this.position).Length();

            if (distance < fleeDistance)
                {
                ghostCondition = GhostConditions.PLAYERNEAR;
            }
            else if (distance > 160&& distance < 700)
                    {
                ghostCondition = GhostConditions.PLAYERFAR;
        }
            else
                    {
                ghostCondition = GhostConditions.PLAYERTOOFAR;
                    }

            foreach (XElement state in states.Elements())
            {
                foreach (XElement changestate in state.Elements())
                {
                    if(state.Attribute("fromState").Value == ghostState.ToString())
                    {
                        if (changestate.Attribute("condition").Value==ghostCondition.ToString())
                        {
                            string toState = changestate.Attribute("toState").Value;
                            if(toState== GhostState.IDLE.ToString())
                            {
                                ghostState = GhostState.IDLE;

                            }
                            else if (toState == GhostState.PURSUE.ToString())
                    {
                                ghostState = GhostState.PURSUE;
                                
                    }
                            else if (toState == GhostState.FLEE.ToString())
                    {
                                ghostState = GhostState.FLEE;
                                
                            }

                        }
                    }
                    }

                }

            if (ghostState == GhostState.IDLE)
            {
                IDLE(gameTime);
            }
            if (ghostState == GhostState.PURSUE)
            {
                Pursue(gameTime);
        }

            if (ghostState == GhostState.FLEE)
        {
                Flee(gameTime);
            }


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

                

               
            base.Update(gameTime);

        }
        private void Flee(GameTime gameTime)
        {
            int time = gameTime.ElapsedGameTime.Milliseconds;
            targetPosition = targetTank.CurrentPosition;
            orintation = position - targetPosition;   //opposite direction of the target 
            MovingToTarget(time);
        }
        private void Pursue(GameTime gameTime)
        {
            currentVelocity += steer.pursue(targetTank.CurrentPosition, targetTank.velocity, position, currentVelocity);
            //currentVelocity += steer.seek(targetTank.CurrentPosition, position, currentVelocity);
            position += currentVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            translation = Matrix.CreateTranslation(position);
            //targetPosition = targetTank.CurrentPosition;
            double turnedAngle = rotationSpeed * gameTime.ElapsedGameTime.Milliseconds;
            orintation = targetPosition - position;
            orintationAngle = Math.Atan2(orintation.X, orintation.Z);
            RotateGhost(turnedAngle);
        }

        private void IDLE(GameTime gameTime)
        {

        }


        private void MovingToTarget(int time)
        {
            double turnedAngle = rotationSpeed * time;
            desiredVelocity = Vector3.Normalize(orintation) * maxSpeed;
            orintationAngle = Math.Atan2(orintation.X, orintation.Z);
            if (currentVelocity.Length() > 1)
                currentVelocity.Normalize();
            currentVelocity *= (float)currentSpeed;
            //ghost will flee away from player if player is in flee distance range
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
                //steering behavior of the ghost
                Steering(time);
                RotateGhost(turnedAngle);
            }
            else
            {

                if (Math.Abs(currentSpeed) < minStopSpeed)
                    currentSpeed = 0;
                if (currentSpeed > 0)
                {
                    currentSpeed -= acceleration * time;
                }
                else
                    currentSpeed = 0;

                Steering(time);
                RotateGhost(turnedAngle);
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

        private void RotateGhost(double turnedAngle)
        {

            if (ghostAngle > MathHelper.Pi || ghostAngle < -MathHelper.Pi)
            {
                ghostAngle = orintationAngle;
            }
            double angleDifference = ghostAngle - orintationAngle;
            if (Math.Abs(angleDifference) < MathHelper.PiOver4 / 10)
            {
                rotation = Matrix.CreateRotationY((float)orintationAngle);
            }
            else
            {
                if (ghostAngle > 0)
                {
                    if (angleDifference > 0 && angleDifference < MathHelper.Pi)
                        ghostAngle -= turnedAngle;
                    else
                        ghostAngle += turnedAngle;
                }
                else
                {
                    if (angleDifference > -MathHelper.Pi && angleDifference < 0)
                        ghostAngle += turnedAngle;
                    else
                        ghostAngle -= turnedAngle;
                }
                rotation = Matrix.CreateRotationY((float)ghostAngle);
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
            world = Matrix.CreateScale(.08f) * rotation * translation;
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

