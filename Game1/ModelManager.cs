using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Game1
{
    class ModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //位置
        private string currentPosition;
        private string pickPosition;
        public string CurrentPosition
        {
            get { return currentPosition; }

        }

        public string PickPosition
        {
            get { return pickPosition; }

        }

        

        Ground ground;
        Tank tank;
        public Vector3 tankCurrentPosition { get { return tank.CurrentPosition; } }


        Vector3 maxSpawnLocation = new Vector3(100, 0, -3000);
        int nextSpanwTime = 0;
        int timeSinceLastSpawn = 0;
        float maxRollAngle = MathHelper.PiOver4 / 40;
        int enemyThisLevel = 0;
        int missedThisLevel = 0;
        public int currentLevel = 0;
        protected Tank1 tank1;
        public List<LevelInfo> levelInfoList = new List<LevelInfo>();
        List<BasicModel> models = new List<BasicModel>();

        Game game;
        public List<BasicModel> bullets = new List<BasicModel>();

        List<BasicModel> enemies = new List<BasicModel>();

        public ModelManager(Game game) : base(game) 
        {
            
            //tank1 = new Tank1(Game.Content.Load<Model>(@"Models/Tank/tank"), ((Game1)Game).GraphicsDevice,
            //   ((Game1)Game).camera);
            levelInfoList.Add(new LevelInfo(10,100,5,2,21,10));
            levelInfoList.Add(new LevelInfo(900,2800,10,3,6,9));
            levelInfoList.Add(new LevelInfo(800, 2600, 15, 4, 6, 8));
            levelInfoList.Add(new LevelInfo(700, 2400,20, 5, 7, 7));
            levelInfoList.Add(new LevelInfo(600, 2200, 25, 6, 7, 6));
            levelInfoList.Add(new LevelInfo(500, 2000, 30, 7, 7, 5));
            levelInfoList.Add(new LevelInfo(400, 1800, 35, 8, 7, 4));
            levelInfoList.Add(new LevelInfo(300, 1600, 40, 8, 8, 3));
            levelInfoList.Add(new LevelInfo(200, 1400, 45, 8, 8, 2));
            levelInfoList.Add(new LevelInfo(100, 1200, 55, 8, 9, 1));
            levelInfoList.Add(new LevelInfo(50, 1000, 60, 8, 9, 0));
            levelInfoList.Add(new LevelInfo(50, 800, 65,8, 9, 0));
            levelInfoList.Add(new LevelInfo(50, 600, 70, 8, 10, 0));
            levelInfoList.Add(new LevelInfo(25, 400, 75, 8, 10, 0));
            levelInfoList.Add(new LevelInfo(0, 200, 80, 8, 20, 0));
            this.game = game;
           
           
        }
        public void SetNextSpawnTime()
        {
            nextSpanwTime = ((Game1)Game).rnd.Next(
                levelInfoList[currentLevel].minSpawnTime,
                levelInfoList[currentLevel].maxSpawnTime);
            timeSinceLastSpawn = 0;
        }
        public override void Initialize()
        {
            ground = new Ground(Game.Content.Load<Model>(@"Models/Ground/Ground"));

            models.Add(new SkyBox(
                   Game.Content.Load<Model>(@"Models/Skybox/skybox")));
            tank = new Tank(Game.Content.Load<Model>(@"Models/Tank/tank"), (((Game1)Game).GraphicsDevice), ((Game1)Game).camera);

            
            base.Initialize();
            
        }
        protected override void LoadContent()
        {
            //models.Add(new BasicModel(
               // Game.Content.Load<Model>(@"Models/Ground/Ground")));
            models.Add(ground);
            //models.Add(new SkyBox(Game.Content.Load<Model>(@"Models/SkyBox/skybox")));
            models.Add(tank);
           
            base.LoadContent();
        }
        private void SpawnEnemy()
        {
            Vector3 position = new Vector3(((Game1)Game).rnd.Next(-2000,(int)maxSpawnLocation.X),
                0,
                ((Game1)Game).rnd.Next((int)maxSpawnLocation.Z,-100));
            Vector3 direction = new Vector3(0, 0, tank.CurrentPosition.Z);
               
          //  float rollRotation = (float)(((Game1)Game).rnd.NextDouble()*maxRollAngle - (maxRollAngle/2));
            enemies.Add(new TankEnemy(Game.Content.Load<Model>(@"Models/Tank/tank"), position, tank,levelInfoList[currentLevel].minSpeed));
            ++enemyThisLevel;
            SetNextSpawnTime();
        }
        protected void CheckToSpawnEnemy(GameTime gameTime)
        {
            if (enemyThisLevel < levelInfoList[currentLevel].numberEnemies)
            {
                timeSinceLastSpawn += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastSpawn > nextSpanwTime)
                {
                    SpawnEnemy();
                }
            
            }
        }
        protected void UpdateModels(GameTime gameTime)
        {
            for (int i = 0; i < models.Count; ++i)
            {
                models[i].Update(gameTime);
                if (models[i].world.Translation.Z > ((Game1)Game).camera.cameraPosition.Z + 100)
                {
                    models.RemoveAt(i);
                    --i;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            
            CheckToSpawnEnemy(gameTime);
            UpdateModels(gameTime);

            //Quadtree is use for reduce cpu time in relation to 
            //the collisions between bullets and enemy tanks
            int worldSize = 1200;
            int maxDepth = 7;
            int maxNodeObject = 5;
            Point center = new Point(0, 0);
            Quadtree quadtree_bulletEnemy = new Quadtree (worldSize,maxDepth,maxNodeObject,center);


            foreach (BasicModel model in models)
            {
                model.Update(gameTime);
            }
            currentPosition = tank.CurrentPosition.ToString();
            pickPosition = tank.PickPosition.ToString();



            foreach (BasicModel model in bullets)
            {
                quadtree_bulletEnemy.Add(model);
                model.Update(gameTime);
            }


            foreach (BasicModel model in enemies)
            {
                
                model.Update(gameTime);
            }

            for (int i = 0;i< enemies.Count;i++)
            {
                float x= enemies[i].world.Translation.X;
                float y = enemies[i].world.Translation.Y;

                //Enemies collides with player (player health -)
                if (enemies[i].CollidesWith(tank.model,tank.world))
                {
                    enemies.RemoveAt(i);
                    ((Game1)Game).kill();
                   --i;
                    ((Game1)Game).reduceHealth();
                }

                Quadtree temp = quadtree_bulletEnemy.GetNodeContaining(x, y);
                foreach (BasicModel bullet in temp.models)
                {
                    if (enemies[i].CollidesWith(bullet.model, bullet.world))
                    {
                        ((Game1)Game).soundHit.Play();
                        ((Game1)Game).AddPoints();

                        enemies.RemoveAt(i);
                        bullets.Remove(bullet);

                    }
                }
            }

            updateShots(gameTime);


            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            foreach(BasicModel model in models)
            {
                model.Draw(((Game1)Game).device,((Game1)Game).camera);
            }

            foreach (BasicModel model in bullets)
            {
                model.Draw(((Game1)Game).device, ((Game1)Game).camera);

            }
            foreach (BasicModel model in enemies)
            {
                model.Draw(((Game1)Game).device, ((Game1)Game).camera);

            }
            base.Draw(gameTime);
        }


        public void AddBullets(Vector3 target)
        {
            bullets.Add(new Bullet
                (Game.Content.Load<Model>((@"Models/Tank/tank")),
                tank.world.Translation, target));
        }


        protected void updateShots(GameTime gameTime)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update(gameTime);

                if (bullets[i].world.Translation.X > 5000 || bullets[i].world.Translation.Y > 5000 || bullets[i].world.Translation.Z > 5000||
                    bullets[i].world.Translation.X < -5000 || bullets[i].world.Translation.Y < -100 || bullets[i].world.Translation.X < -5000
                    )
                {
                    bullets.RemoveAt(i);
                    i--;
                }
                //else
                //{
                //    for (int j = 0; j <enemies.Count; j++)
                //    {
                //        if (bullets[i].CollidesWith(enemies[j].model, enemies[j].world))
                //        {
                //            ((Game1)Game).soundHit.Play();
                //            ((Game1)Game).AddPoints();
                    
                //            enemies.RemoveAt(j);
                //            bullets.RemoveAt(i);
                //            break;
                //        }
                //    }
                //}
            }



        }
    }
}
