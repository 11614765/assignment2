using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    #region Map Tile Type Enum
    public enum MapTileType
    {
        Empty,
        Barrier,
        Start,
        Exit
    }
    #endregion
    public class Map
    {
        // Map data
        //private List<MapData> maps;
        public List<Point> barrierList;
        public MapTileType[,] mapTiles;
        //private int currentMap;
        public int numberColumns;
        public int numberRows;
        public const float tileSize = 120f;
        public Vector3 tileCenter;

        public Map()
        {
            numberColumns = 20;
            numberRows = 20;
            barrierList = new List<Point>();
            barrierList.Add(new Point(5, 8));
            barrierList.Add(new Point(8, 8));
            barrierList.Add(new Point(7, 8));
            barrierList.Add(new Point(9, 8));
            barrierList.Add(new Point(10, 8));
            barrierList.Add(new Point(11, 8));
            barrierList.Add(new Point(3, 5));
            barrierList.Add(new Point(7, 7));
            barrierList.Add(new Point(13, 13));
            tileCenter = new Vector3(tileSize / 2, 0, tileSize / 2);
            mapTiles = new MapTileType[20, 20];
            int x = 0, y = 0;
            for (int i = 0; i < barrierList.Count; i++)
            {
                x = barrierList[i].X;
                y = barrierList[i].Y;
                mapTiles[x, y] = MapTileType.Barrier;
            }
        }
        /// <summary>
        /// Translates a map tile location into a 3D world position
        /// </summary>
        /// <param name="column">column position(x)</param>
        /// <param name="row">row position(y)</param>
        /// <param name="centered">true: return the location of the center of the tile
        /// false: return the position of the upper-left corner of the tile</param>
        /// <returns>screen position</returns> 
        public Vector3 MapToWorld(int column, int row, bool centered)
        {
            Vector3 groundPosition = new Vector3();

            if (InMap(column, row))
            {
                groundPosition.X = column * tileSize - 1200;
                groundPosition.Y = 0;
                groundPosition.Z = row * tileSize - 1200;
                //if (centered)
                //{
                //    groundPosition += tileCenter;
                //}
            }
            else
            {
                groundPosition = new Vector3(-1200, 0, -1200);
            }
            return groundPosition;
        }

        /// <summary>
        /// Translates a map tile location into a 3D world position
        /// </summary>
        /// <param name="location">map location</param>
        /// <param name="centered">true: return the location of the center of the tile
        /// false: return the position of the upper-left corner of the tile</param>
        /// <returns>screen position</returns>
        public Vector3 MapToWorld(Point location, bool centered)
        {
            Vector3 groundPosition = new Vector3();

            if (InMap(location.X, location.Y))
            {
                groundPosition.X = location.X * tileSize - 1200;
                groundPosition.Y = 0;
                groundPosition.Z = location.Y * tileSize - 1200;
                if (centered)
                {
                    groundPosition += tileCenter;
                }
            }
            else
            {
                groundPosition = new Vector3(-1200, 0, -1200);
            }
            return groundPosition;
        }

        public static Vector3 MapToWorld(Point point)
        {
            Vector3 worldPosition = new Vector3();
            worldPosition.X = tileSize * point.X - 1200;
            worldPosition.Y = 0;
            worldPosition.Z = tileSize * point.Y - 1200;
            worldPosition += new Vector3(60, 0, 60);
            return worldPosition;
        }

        public static Point WorldToMap(Vector3 position)
        {
            Point point;
            point.X = (int)((position.X + 1200) / tileSize);
            point.Y = (int)((position.Z + 1200) / tileSize);
            return point;
        }

        /// <summary>
        /// Returns true if the given map location exists
        /// </summary>
        /// <param name="column">column position(x)</param>
        /// <param name="row">row position(y)</param>
        public bool InMap(int column, int row)
        {
            return (row >= 0 && row < numberRows &&
                column >= 0 && column < numberColumns);
        }
    }
}
