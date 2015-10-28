using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    class LevelInfo
    {
        public int minSpawnTime { get; protected set; }
        public int maxSpawnTime { get; protected set; }
        public int numberEnemies { get; protected set; }
        public int minSpeed { get; protected set; }
        public int maxSpeed { get; protected set; }
        public int missAllowed { get; protected set; }
        public LevelInfo(int minSpawnTime, int maxSpawnTime, int numberEnemies, int minSpeed, int maxSpeed, int missAllowed) 
        {
            this.minSpawnTime = minSpawnTime;
            this.maxSpawnTime = maxSpawnTime;
            this.numberEnemies = numberEnemies;
            this.minSpeed = minSpeed;
            this.maxSpeed = maxSpeed;
            this.missAllowed = missAllowed;
        }
    }
}
