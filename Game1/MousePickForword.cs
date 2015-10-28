using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class MousePickForward
    {

        GraphicsDevice device;
        Camera camera;
        Vector3 forward;

        public MousePickForward(GraphicsDevice device, Camera camera)
        {
            this.device = device;
            this.camera = camera;
    
        }

        public Vector3? PickedPosition()
        {
            MouseState mouseState = Mouse.GetState();
            Vector3 nearSource = new Vector3(mouseState.X, mouseState.Y, 0f);
            Vector3 farSource = new Vector3(mouseState.X, mouseState.Y, 1f);


            Vector3 nearPoint = device.Viewport.Unproject(nearSource, camera.projection, camera.view, Matrix.Identity);
            Vector3 farPoint = device.Viewport.Unproject(farSource, camera.projection, camera.view, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            Ray pickRay = new Ray(nearPoint, direction);
            Nullable<float> result = pickRay.Intersects(new Plane(camera.view.Forward, -999f));

            Vector3? resultVector = direction * result;
            Vector3? collisionPoint = resultVector + nearPoint;

            return collisionPoint;


        }

    }
}