using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    class Quadtree
    {
        static int childCount = 4;
        int maxObjectCount = 7;
        int maxDepth= 5;

        private Quadtree nodeParent;
        private Quadtree[] childNodes;

        public List<BasicModel> models = new List<BasicModel>();

        private int currentDepth = 0;

        private Point nodeCenter;
        private Rectangle nodeBounds = new Rectangle();
        private float nodeSize = 0f;

        public Quadtree(int worldSize, int maxNodeDepth, int maxNodeObjects, Point center) : this(worldSize, 0, center, null)
        {
            maxDepth = maxNodeDepth;
            maxObjectCount = maxNodeObjects;
        }

        private Quadtree(int size, int depth, Point center, Quadtree parent)
        {
            this.nodeSize = size;
            this.currentDepth = depth;
            this.nodeCenter = center;
            this.nodeParent = parent;


            //first squre
            if (this.currentDepth == 0)
            {
                this.nodeBounds = new Rectangle((int)center.X - size, (int)center.Y - size, size * 2, size * 2);
            }
            //depth >0 means it is divided by previous squre and its size is only 1/4
            else
            {
                this.nodeBounds = new Rectangle((int)center.X - size / 2, (int)center.Y - size / 2, size, size);
            }

        }

        public bool Add(BasicModel obj)
        {
            //If object fall into the area a sub-world, it will be added in 
            if (this.nodeBounds.Contains(obj.world.Translation.X, obj.world.Translation.Z))
            {
                return this.Add(obj, new Point((int)obj.world.Translation.X, (int)obj.world.Translation.Z)) != null;
            }
            return false;
        }


        public Quadtree Add(BasicModel obj, Point objCenter)
        {
            //This world has been divided into sub-worlds
            //Find the sub-world it belongs to.

            //--------
            //| 0|1 |
            //-------
            //| 2|3 |
            //-------
            if (this.childNodes != null)
            {
                int index = (objCenter.X < this.nodeCenter.X ? 0 : 1)
                            + (objCenter.Y < this.nodeCenter.Y ? 0 : 2);
                return this.childNodes[index].Add(obj, objCenter);
            }

            //Create more sub-world if there is too much object in the same world
            //and the level of worlds did not reach the maximun

            if (this.currentDepth < maxDepth && this.models.Count + 1 > maxObjectCount)
            {
                Split((int)nodeSize);
                List<BasicModel> tempModel = new List<BasicModel>();
                foreach (BasicModel nodeModel in models)
                {
                    tempModel.Add(nodeModel);
                    
                }
                this.models.Clear();
                foreach (BasicModel tempmodels asdsadsad in tempModel)
                {
                    Add(tempmodels);
                }
               
                return Add(obj, objCenter);
            }

            //just add object to this world as other criteria dose not match 
            else
            {
                this.models.Add(obj);
            }
            return this;

        }


        //split current world into 4 sub-worlds
        private void Split(int parentSize)
        {
            this.childNodes = new Quadtree[Quadtree.childCount];
            int depth = this.currentDepth + 1;
            int quarter = parentSize / 4;
            this.childNodes[0] = new Quadtree(parentSize / 2, depth, this.nodeCenter + new Point(-quarter, -quarter), this);
            this.childNodes[1] = new Quadtree(parentSize / 2, depth, this.nodeCenter + new Point(quarter, -quarter), this);
            this.childNodes[2] = new Quadtree(parentSize / 2, depth, this.nodeCenter + new Point(-quarter, quarter), this);
            this.childNodes[3] = new Quadtree(parentSize / 2, depth, this.nodeCenter + new Point(quarter, quarter), this);
        }


        public Quadtree GetNodeContaining(float x, float y)
        {
            if (this.childNodes != null)
            {
                // Find the index of the child that contains the center of the object
                int index = (x < this.nodeCenter.X ? 0 : 1)
                          + (y < this.nodeCenter.Y ? 0 : 2);

                return this.childNodes[index].GetNodeContaining(x, y);
            }
            else
            {
                return this;
            }
        }
    }
}
