using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
   
    class SkyBox : BasicModel
    {
       
      
       
        
       public SkyBox (Model model) : base (model)
        {

        }
 
        
        public override void Update(GameTime gameTime)
        {

          base.Update(gameTime);
        }
          
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            device.SamplerStates[0] = SamplerState.LinearClamp;
            Matrix[] skyboxTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(skyboxTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect currenteffect in mesh.Effects)
                {
                    currenteffect.World = mesh.ParentBone.Transform * GetWorld() * Matrix.CreateTranslation(camera.cameraPosition);
                    currenteffect.View = camera.view;
                    currenteffect.Projection = camera.projection;
                    currenteffect.TextureEnabled = true;
                }
                mesh.Draw();
            }
              
        }
        protected override Matrix GetWorld()
        {
            return Matrix.CreateScale(2000f)* Matrix.CreateTranslation(0,-400,0);
           
        }

    }
}
