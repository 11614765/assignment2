using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml.Linq;

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

        public string PickPositionString
        {
            get { return pickPosition; }

        }

        
        //model define
        Ground ground;
        Tank tank;
        PursuitEnemy pursuitenemy;
        List<Wallstone> pathtrack = new List<Wallstone>();
        Wallstone[] wallstone;

        Map map;
        Pathfinder pathfinder;
        public Vector3 tankCurrentPosition { get { return tank.CurrentPosition; } }

        //debug parameter
        public int count;
        public string cur;
        public string pos;

        Vector3 maxSpawnLocation = new Vector3(100, 0, -3000);
        int nextSpanwTime = 0;
        int timeSinceLastSpawn = 0;
        float maxRollAngle = MathHelper.PiOver4 / 40;
        public int enemyThisLevel = 0;
        int missedThisLevel = 0;
        public int currentLevel = 0;

        public List<LevelInfo> levelInfoList = new List<LevelInfo>();
        List<BasicModel> models = new List<BasicModel>();
        List<BasicModel> obstacles = new List<BasicModel>();
        List<BasicModel> enemies = new List<BasicModel>();
        public List<BasicModel> bullets = new List<BasicModel>();

        Game game;


        public ModelManager(Game game) : base(game) 
        {
            map = new Map();
            pathfinder = new Pathfinder(map);
            wallstone = new Wallstone[map.barrierList.Count];
            XElement level = XElement.Load(@"Content/config/levelsetting.xml");
            //tank1 = new Tank1(Game.Content.Load<Model>(@"Models/Tank/tank"), ((Game1)Game).GraphicsDevice,
            //   ((Game1)Game).camera);
            foreach (XElement levelnumber in level.Elements()) { 
            foreach (XElement parameters in levelnumber.Elements())
            {
                levelInfoList.Add(new LevelInfo(int.Parse(parameters.Attribute("minSpawnTime").Value),
                                int.Parse(parameters.Attribute("maxSpawnTime").Value),
                                int.Parse(parameters.Attribute("numberEnemies").Value),
                                int.Parse(parameters.Attribute("minspeed").Value),
                                int.Parse(parameters.Attribute("maxSpeed").Value),
                                int.Parse(parameters.Attribute("missAllowed").Value),
                                int.Parse(parameters.Attribute("numHuman").Value)
                  ));
             }
            }
        /*    levelInfoList.Add(new LevelInfo(10,100,5,2,21,10));
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
         */   
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
            //pursuitenemy = new PursuitEnemy(Game.Content.Load<Model>(@"Models/Tank/tank"), (((Game1)Game).GraphicsDevice), ((Game1)Game).camera);
            for (int i = 0; i < wallstone.Length; i++)
            {
                Vector3 stoneposition = map.MapToWorld(map.barrierList[i], true);
                wallstone[i] = new Wallstone(Game.Content.Load<Model>(@"Models/Obstacle/stone"), stoneposition);
                obstacles.Add(wallstone[i]);
            }
            
            //for(int x=0; x<21; x++)
            //{
            //    for (int y = 0; y < 21; y++)
            //    {
            //        if (y == 0 ||x==0 || y==20||x==20)
            //        { 
            //            AddWall(-1200 + y * 120, -1200 + x * 120);
            //        }


            //    }
            //}
            for (int x = -1 * 1200; x <= 1200; x += 120)
            {
                AddWall(x, 1200);
                AddWall(x, -1200);
                AddWall(1200, x);
                AddWall(-1200, x);


            }


            base.Initialize();
            
        }
        protected override void LoadContent()
        {
            //models.Add(new BasicModel(
               // Game.Content.Load<Model>(@"Models/Ground/Ground")));
            models.Add(ground);
            //models.Add(new SkyBox(Game.Content.Load<Model>(@"Models/SkyBox/skybox")));
            models.Add(tank);
            //models.Add(pursuitenemy);
            //pursuitenemy.TargetPlayer(tank);

            //foreach (BasicModel wallstonemodel in wallstone)
            //{
            //    models.Add(wallstonemodel);
            //}
            base.LoadContent();
        }
        private void SpawnEnemy()
        {
            Vector3 position = new Vector3(0, 0, -1100);
            Random random = new Random();
            int spwanIndex = random.Next(3);
            if (spwanIndex == 0)
            {
                position = new Vector3(-900, 0, -900);
            }
            if (spwanIndex == 1)
            {
                position = new Vector3(0, 0, -1100);
            }
            if (spwanIndex == 2)
            {
                position = new Vector3(900, 0, -900);
            }


            //Vector3 position = new Vector3(((Game1)Game).rnd.Next(-2000,(int)maxSpawnLocation.X),
            //    0,
            //    ((Game1)Game).rnd.Next((int)maxSpawnLocation.Z,-100));
            //Vector3 direction = new Vector3(0, 0, tank.CurrentPosition.Z);
               
          //  float rollRotation = (float)(((Game1)Game).rnd.NextDouble()*maxRollAngle - (maxRollAngle/2));
            //enemies.Add(new TankEnemy(Game.Content.Load<Model>(@"Models/Tank/tank"), position, tank,levelInfoList[currentLevel].minSpeed));
           
            //pursuitenemy.TargetPlayer(tank);
            enemies.Add(new PursuitEnemy(Game.Content.Load<Model>(@"Models/Tank/tank"), position, (((Game1)Game).GraphicsDevice), ((Game1)Game).camera,tank));
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
                    if (enemyThisLevel == 0)
                    {
                        for (int i=0; i<levelInfoList[currentLevel].numHuman; i++)
                        {
                            Vector3 humPosition = new Vector3(((Game1)Game).rnd.Next(600, 1000),
                                0,
                                ((Game1)Game).rnd.Next(-1000, 1000));
                            enemies.Add(new Human(Game.Content.Load<Model>(@"Models/Tank/anna"), humPosition, tank, 3));

                        }
                    }
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
            //UpdateModels(gameTime);

            //Quadtree is use for reduce cpu time in relation to 
            //the collisions between bullets and enemy tanks
            int worldSize = 2400;
            int maxDepth = 7;
            int maxNodeObject = 5;
            Point center = new Point(0, 0);
            Quadtree quadtree_Enemy = new Quadtree (worldSize,maxDepth,maxNodeObject,center);
            Quadtree quadtree_obstacles = new Quadtree(worldSize, maxDepth, maxNodeObject, center);


            //if (pursuitenemy.pathdebug != null)
            //{
            //    foreach (Vector3 track in pursuitenemy.pathdebug)
            //    {
            //        pathtrack.Add(new Wallstone(Game.Content.Load<Model>(@"Models/Obstacle/stone"), track));

            //    }
            //    pursuitenemy.pathdebug = null;
            //    foreach (BasicModel track in pathtrack)
            //    {
            //        models.Add(track);
            //    }
            //    pathtrack.Clear();

            //}


            foreach (BasicModel model in models)
            {
                model.Update(gameTime);
            }
            currentPosition = tank.CurrentPosition.ToString();
            pickPosition = tank.PickPosition.ToString();



            foreach (BasicModel model in bullets)
            {
                
                model.Update(gameTime);
            }
                

            foreach (BasicModel model in obstacles)
            {
                quadtree_obstacles.Add(model);
                model.Update(gameTime);
            }

            foreach (BasicModel model in enemies)
            {
                quadtree_Enemy.Add(model);
                model.Update(gameTime);
            }


            for(int i = 0; i<enemies.Count;i ++)
            {
                if (enemies[i].CollidesWith(tank.model, tank.world))
                {
                    if (enemies[i] is Human)
                    {
                        enemies.RemoveAt(i);
                        
                        --i;
                        ((Game1)Game).reduceHealth();
                        ((Game1)Game).DeductPoints();
                        break;
                    }
                    else
                    {
                    enemies.RemoveAt(i);
                    ((Game1)Game).kill();
                    --i;
                    ((Game1)Game).reduceHealth();
                    break;
                }
            }
            }

            for (int i = 0; i< bullets.Count;i++)
            {
                float x = bullets[i].world.Translation.X;
                float y = bullets[i].world.Translation.Z;
                Quadtree nearWalls = quadtree_obstacles.GetNodeContaining(x, y);

                foreach (BasicModel walls in obstacles)
                {
                    if (bullets[i].CollidesWith(walls.model, walls.world))
                    {
                        bullets.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }



            for (int i = 0;i< bullets.Count;i++)
            {
                float x= bullets[i].world.Translation.X;
                float y = bullets[i].world.Translation.Z;
                //Enemies collides with player (player health -)
                Quadtree nearEnemies = quadtree_Enemy.GetNodeContaining(x, y);


                foreach (BasicModel enemy in nearEnemies.models)
                {
                    if (bullets[i].CollidesWith(enemy.model, enemy.world))
                    {
                        bullets.RemoveAt(i);
                        ((Game1)Game).soundHit.Play();
                        if (enemy is Human)
                        {
                            ((Game1)Game).DeductPoints();
                             ((Game1)Game).reduceHealth();
                        }
                        else
                        {
                        ((Game1)Game).AddPoints();
                        }
                        enemies.Remove(enemy);
                        
                        --i;
                        break;
                    }
                }
            }


            Quadtree nearObstacles = quadtree_obstacles.GetNodeContaining(tank.translation.Translation.X, tank.translation.Translation.Z);

            foreach (BasicModel model in nearObstacles.models )
            {

                if (model.CollidesWith(tank.model,tank.world))
                {


                    tank.CurrentPosition = tank.CurrentPosition - tank.velocity*(gameTime.ElapsedGameTime.Milliseconds) /100;
                    tank.velocity = Vector3.Zero;
                    //tank.tankDirection = -tank.tankDirection;
                    //tank.PickPosition = tank.CurrentPosition;

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
            foreach (BasicModel model in obstacles)
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
        public void AddWall(int x, int y)
        {
            obstacles.Add(new Wallstone(Game.Content.Load<Model>(@"Models/Obstacle/stone"), new Vector3(x,0,y)));
        }


        protected void updateShots(GameTime gameTime)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update(gameTime);

                if (bullets[i].world.Translation.X > 2000 || bullets[i].world.Translation.Z > 2000||
                    bullets[i].world.Translation.X < -2000 || bullets[i].world.Translation.X < -2000)
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
