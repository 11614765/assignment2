using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class MousePicking
    {
        GraphicsDevice graphicDevice;
        Camera camera;

        public MousePicking(GraphicsDevice graphicDevice, Camera camera)
        {
            this.graphicDevice = graphicDevice;
            this.camera = camera;
        }

        public Vector3? GetCollisionPosition()
        {
            MouseState mousestate = Mouse.GetState();
            Vector3 near = new Vector3(mousestate.X,mousestate.Y,0f);
            Vector3 far = new Vector3(mousestate.X,mousestate.Y,1f);

            Vector3 nearPoint = graphicDevice.Viewport.Unproject(near,camera.projection,camera.view,Matrix.Identity);
            Vector3 farPoint = graphicDevice.Viewport.Unproject(far,camera.projection,camera.view,Matrix.Identity);

            Vector3 Direction = farPoint - nearPoint;
            Direction.Normalize();

            Ray pickRay = new Ray(nearPoint,Direction);
            Nullable<float> result = pickRay.Intersects(new Plane(Vector3.Up, 0f));

            Vector3? ResultVector = Direction * result;
            Vector3? Collision = ResultVector + nearPoint;

            return Collision;
        
        }

    }
}
