using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Game1
{
    class Steering
    {

        public float maxSpeed { get; set; }
        public float slowRadius { get; set; }
        Random random = new Random(Environment.TickCount);
        public Steering(float maxspeed, float slowradius)
        {
            maxSpeed = maxspeed;
            slowRadius = slowradius;
        }
        /*  public void update(Vector3 force)
          {
              acceleration = force / mass;
              velocity += acceleration;
              currentPosition += velocity;
          }*/
        public Vector3 seek(Vector3 targetPosition, Vector3 currentPosition, Vector3 velocity)
        {
            float distance = Vector3.Subtract(targetPosition, currentPosition).Length();
            Vector3 desiredVelocity = Vector3.Normalize(Vector3.Subtract(targetPosition, currentPosition));
            if (distance < slowRadius)
            {
                desiredVelocity *= distance / slowRadius * maxSpeed;
            }
            else
                desiredVelocity = desiredVelocity * maxSpeed;
            Vector3 force = Vector3.Subtract(desiredVelocity, velocity);
            return force;
        }
        public Vector3 flee(Vector3 targetPosition, Vector3 currentPosition, Vector3 velocity)
        {
            Vector3 desiredVelocity = Vector3.Normalize(Vector3.Subtract(currentPosition, targetPosition) * maxSpeed);
            Vector3 force = Vector3.Subtract(desiredVelocity, velocity);
            return force;
        }
        public Vector3 pursue(Vector3 evaderPosition, Vector3 evaderVelocity, Vector3 currentPosition, Vector3 velocity)
        {
            float distance = Vector3.Distance(evaderPosition, currentPosition);
            float timeToReachTarget = distance / velocity.Length();
            Vector3 targetPosition = evaderPosition + evaderVelocity * timeToReachTarget;
            return seek(targetPosition, currentPosition, velocity);
        }
        public Vector3 evade(Vector3 evaderPosition, Vector3 evaderVelocity, Vector3 currentPosition, Vector3 velocity)
        {
            float distance = Vector3.Subtract(evaderPosition, currentPosition).Length();
            float timeToReachTarget = distance / velocity.Length();
            Vector3 targetPosition = evaderPosition + evaderVelocity * timeToReachTarget;
            return flee(targetPosition, currentPosition, velocity);
        }
    }
}
