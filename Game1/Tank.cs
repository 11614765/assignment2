using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class Tank : BasicModel
    {
        public Vector3 CurrentPosition { get { return tankPosition; } }
        //Vector3 currentPosition;
        public Vector3 PickPosition { get { return pickPosition; } }
        Vector3 pickPosition;
        Camera camera;

        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        MousePicking mousePick;
        ModelBone turretBone;
        ModelBone lbackwheelBone;
        ModelBone rbackwheelBone;
        ModelBone lfrontwheelBone;
        ModelBone rfrontwheelBone;
        ModelBone lsteergeoBone;
        ModelBone rsteergeoBone;
        ModelBone hatchgeo;
        ModelBone canonBone;
        
        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;
        Matrix leftSteerTransform;
        Matrix rightSteerTransform;
        Matrix turretTransform;
        Matrix canonTransform;
        Matrix hatchTransform;

        float wheelRotationValue;
        //float steerRotationValue;
        float hatchRotationValue;
        float canonRotationValue;
        float turretRotationValue;


        //键盘控制方向（新增）
        float modelRotation = MathHelper.Pi;
        double speedx, speedz;


        //float speedx, speedz;
        //Vector3 v = Vector3.Forward;
        //结束

        Vector3 tankPosition = Vector3.Zero;
        Vector3 tankDirection;
        double threshold;
        Double orientation = 0;
        ButtonState prevMouseleftState = Mouse.GetState().LeftButton;
        public Tank(Model model, GraphicsDevice device, Camera camera)
            : base(model)
        {
            this.camera = camera; 
            mousePick = new MousePicking(device, camera);
            //载入模型
            turretBone = model.Bones["turret_geo"];
            canonBone = model.Bones["canon_geo"];
            lbackwheelBone = model.Bones["l_back_wheel_geo"];
            rbackwheelBone = model.Bones["r_back_wheel_geo"];
            lfrontwheelBone = model.Bones["l_front_wheel_geo"];
            rfrontwheelBone = model.Bones["r_front_wheel_geo"];
            lsteergeoBone = model.Bones["l_steer_geo"];
            rsteergeoBone = model.Bones["r_steer_geo"];
            hatchgeo = model.Bones["hatch_geo"];

            //赋值给每个部件的变换
            leftBackWheelTransform = lbackwheelBone.Transform;
            rightBackWheelTransform = rbackwheelBone.Transform;
            leftFrontWheelTransform = lfrontwheelBone.Transform;
            rightFrontWheelTransform = rfrontwheelBone.Transform;
            leftSteerTransform = lsteergeoBone.Transform;
            rightSteerTransform = rsteergeoBone.Transform;
            turretTransform = turretBone.Transform;
            hatchTransform = hatchgeo.Transform;
            canonTransform = canonBone.Transform;
            rotation = Matrix.CreateRotationY(MathHelper.Pi);
        }

        public override void Update(GameTime gameTime)
        {
            //Vector3? pickPosition = mousePick.GetCollisionPostion();
            turretRotationValue = (float)Math.Atan2(camera.cameraDirection.X, camera.cameraDirection.Z) + MathHelper.Pi;
            canonRotationValue = (float)Math.Atan2(camera.cameraDirection.Y,camera.cameraDirection.Z ) + MathHelper.Pi - MathHelper.Pi/180 *20;
            
            float speed = 10;
            float time = (gameTime.ElapsedGameTime.Milliseconds)/1000f;
            
            //键盘移动
            if (modelRotation >= (MathHelper.Pi * 2))
            {
                modelRotation = 0.0f;
            }
            if (modelRotation < 0)
            {
                modelRotation = (MathHelper.Pi * 2);
            }

            speedx = (Math.Sin(modelRotation)) * 3;
            speedz = (Math.Cos(modelRotation)) * 3;
            float speedxdouble, speedzdouble;
            speedxdouble = (float)speedx;
            speedzdouble = (float)speedz;



            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                tankPosition += Vector3.Forward * speedzdouble/4;
                tankPosition += Vector3.Left * speedxdouble/4;

                //translation *= Matrix.CreateTranslation(v*10);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                tankPosition += Vector3.Backward * speedzdouble/4;
                tankPosition += Vector3.Right * speedxdouble/4;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                //speedx =(float) Math.Sin(MathHelper.PiOver4 / 180);
                //speedz= (float) Math.Cos(MathHelper.PiOver4 / 180);
                //v = new Vector3(speedx, 0f,speedz);
                //v.Normalize();
                //rotation *= Matrix.CreateRotationY(MathHelper.PiOver4 / 180);
                modelRotation += 10 * MathHelper.ToRadians(0.1f);
                rotation = Matrix.CreateRotationY(modelRotation);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                modelRotation -= 10 * MathHelper.ToRadians(0.1f);
                rotation = Matrix.CreateRotationY(modelRotation);

            }

            //鼠标移动
            //if (prevMouseleftState == ButtonState.Pressed && mousePick.GetCollisionPosition().HasValue == true)
            //{


            //    pickPosition = mousePick.GetCollisionPosition().Value;

            //    tankDirection = mousePick.GetCollisionPosition().Value - tankPosition;
            //    tankDirection.Normalize();


            //    //translation = Matrix.CreateTranslation(pickPosition.Value);
            //    //translation.Translation = pickPosition.Value;
            //}
            threshold = Math.Pow((pickPosition.Z - tankPosition.Z),2)+Math.Pow((pickPosition.X - tankPosition.X),2);
            if ((threshold>1) && (Mouse.GetState().LeftButton == ButtonState.Released))
            {
                //tankPosition += speed * tankDirection * time;移动代码，放在旋转后面
                //translation = Matrix.CreateTranslation(tankPosition);

                //lbackwheelBone.Transform = Matrix.CreateTranslation(0, lbackwheelBone.Parent.ModelTransform.Translation.Y, lbackwheelBone.Parent.ModelTransform.Translation.Z);

                //this.rotateWheel();
                Double newOrientation = Math.Atan2(tankDirection.X, tankDirection.Z);
                    float rotateDirection;
                    float rotationalSpeed = MathHelper.PiOver4;
                    
                    wheelRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * speed;
                    //canonRotationValue = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 0.25f) * 0.333f - 0.333f;
                    hatchRotationValue = MathHelper.Clamp((float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 2) * 2, -1, 0);
                    if (newOrientation > orientation)
                        rotateDirection = 1;
                    else
                        rotateDirection = -1;

                    float rotationalVelocity = rotationalSpeed * rotateDirection;
                    float rotateAngel = rotationalVelocity * time;
                    
                    Double orientationThreshold = MathHelper.PiOver4 / 45;
                    if (Math.Abs(newOrientation - orientation) > orientationThreshold)
                    {
                        orientation += rotateAngel;
                        rotation *= Matrix.CreateRotationY(rotateAngel);

                    }
                    else
                    {
                    
                        tankPosition += speed * tankDirection * time;
                        translation = Matrix.CreateTranslation(tankPosition);
                    }
                    
            }

            //turretBone.Transform *= Matrix.CreateRotationY(MathHelper.PiOver4/100);
            //cannonBone.Transform *= Matrix.CreateFromAxisAngle(new Vector3(0,0,1),MathHelper.PiOver4 / 100);


            prevMouseleftState = Mouse.GetState().LeftButton;
            base.Update(gameTime);
        }

        public override void Draw(GraphicsDevice device, Camera camera)
        
        {
            //rbackwheelBone.Transform = Matrix.CreateRotationX(MathHelper.PiOver4 / 4) * rbackwheelBone.Transform;


            Matrix wheelRotation = Matrix.CreateRotationX(wheelRotationValue);
            //Matrix steerRotation = Matrix.CreateRotationY(steerRotationValue);
            Matrix turretRotation = Matrix.CreateRotationY(turretRotationValue);
            Matrix canonRotation = Matrix.CreateRotationX(canonRotationValue);
            Matrix hatchRotation = Matrix.CreateRotationX(hatchRotationValue);
            lbackwheelBone.Transform = wheelRotation * leftBackWheelTransform;
            rbackwheelBone.Transform = wheelRotation * rightBackWheelTransform;
            lfrontwheelBone.Transform = wheelRotation * leftFrontWheelTransform;
            rfrontwheelBone.Transform = wheelRotation * rightFrontWheelTransform;
            //leftSteerTransform = steerRotation * leftSteerTransform;
            //rightSteerTransform = steerRotation * rightSteerTransform;
            hatchgeo.Transform = hatchRotation * hatchTransform;
            turretBone.Transform = turretRotation * turretTransform;
            canonBone.Transform = canonRotation * canonTransform;
                base.Draw(device, camera);
        }
        

        protected override Matrix GetWorld()
        {
            world = Matrix.CreateScale(.1f) * rotation * translation;
            return world;
        }

        //public void rotateWheel()
        //{
            //var lbwheelrotationCenter = new Vector3(0,-58.93,-234.27f);

            //Matrix lbwheeltransformation = Matrix.CreateTranslation(-lbwheelrotationCenter);
                
            //   lbwheeltransformation *= Matrix.CreateRotationX(MathHelper.PiOver4/10);


            //lbackwheelBone.Transform *= Matrix.CreateFromAxisAngle(lbwheelrotationCenter,MathHelper.PiOver4 / 100);
        //    lbackwheelBone.Transform *= Matrix.CreateRotationX(MathHelper.PiOver4 );
            
        //}

    }
}
