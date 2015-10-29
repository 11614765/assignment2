using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Game1
{
    class Wallstone : BasicModel
    {
        Matrix translation = Matrix.Identity;
        Matrix scale;
        public Wallstone(Model model, Vector3 position)
            : base(model)
        {
            translation = Matrix.CreateTranslation(position);
        }
        public override void Update(GameTime gameTime)
                {
                    base.Update(gameTime);
                } 
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            //device.SamplerStates[0] = SamplerState.LinearClamp;
            scale = Matrix.CreateScale(3f);
            base.Draw(device, camera);
        }

        
        protected override Matrix GetWorld()
        {
            return scale * translation;
        }
    }
}