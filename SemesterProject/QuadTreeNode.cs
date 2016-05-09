using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace SemesterProject
{
    class QuadTreeNode
    {
        #region Constants
        // The maximum number of objects in a quad
        // before a subdivision occurs
        private const int MAX_OBJECTS_BEFORE_SUBDIVIDE = 10;
        #endregion

        #region Variables
        // The game objects held at this level of the tree
        private List<MapObject> _objects;

        // This quad's rectangle area
        private Rectangle _rect;

        // This quad's divisions
        private QuadTreeNode[] _divisions;
        #endregion

        #region Properties
        /// <summary>
        /// The divisions of this quad
        /// </summary>
        public QuadTreeNode[] Divisions { get { return _divisions; } }

        /// <summary>
        /// This quad's rectangle
        /// </summary>
        public Rectangle Rectangle { get { return _rect; } }

        /// <summary>
        /// The game objects inside this quad
        /// </summary>
        public List<MapObject> MapObjects { get { return _objects; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Quad Tree
        /// </summary>
        /// <param name="x">This quad's x position</param>
        /// <param name="y">This quad's y position</param>
        /// <param name="width">This quad's width</param>
        /// <param name="height">This quad's height</param>
        public QuadTreeNode(int x, int y, int width, int height)
        {
            // Save the rectangle
            _rect = new Rectangle(x, y, width, height);

            // Create the object list
            _objects = new List<MapObject>();

            // No divisions yet
            _divisions = null;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a game object to the quad.  If the quad has too many
        /// objects in it, and hasn't been divided already, it should
        /// be divided
        /// </summary>
        /// <param name="gameObj">The object to add</param>
        public void AddObject(MapObject mapObj)
        {
            if (_rect.Contains(mapObj.Rect))
            {
                if (this._divisions != null)
                {
                    foreach (QuadTreeNode n in this._divisions)
                    {
                        if (n._rect.Contains(mapObj.Rect))
                        {

                            n.AddObject(mapObj);
                        }
                    }
                }
                else
                {

                    _objects.Add(mapObj);
                    if (_objects.Count >= MAX_OBJECTS_BEFORE_SUBDIVIDE)
                    {

                        this.Divide();
                    }
                }
            }
        }

        /// <summary>
        /// Divides this quad into 4 smaller quads.  Moves any game objects
        /// that are completely contained within the new smaller quads into
        /// those quads and removes them from this one.
        /// </summary>
        public void Divide()
        {
            if (this._divisions == null)
            {
                int newX = this.Rectangle.X;
                int newY = this.Rectangle.Y;
                int newWidth = this.Rectangle.Width / 2;
                int newHeight = this.Rectangle.Height / 2;

                QuadTreeNode node1 = new QuadTreeNode(newX, newY, newWidth, newHeight);
                QuadTreeNode node2 = new QuadTreeNode(newX + newWidth, newY, newWidth, newHeight);
                QuadTreeNode node3 = new QuadTreeNode(newX, newY + newHeight, newWidth, newHeight);
                QuadTreeNode node4 = new QuadTreeNode(newX + newWidth, newY + newHeight, newWidth, newHeight);

                _divisions = new QuadTreeNode[4];
                _divisions[0] = node1;
                _divisions[1] = node2;
                _divisions[2] = node3;
                _divisions[3] = node4;

                for (int j = 0; j < _divisions.Length; j++)
                {

                    for (int n = 0; n < _objects.Count; n++)
                    {

                        if (_divisions[j].Rectangle.Contains(_objects[n].Rect))
                        {
                            _divisions[j]._objects.Add(_objects[n]);
                            this._objects.Remove(_objects[n]);
                        }
                    }
                }
            }


        }

        /// <summary>
        /// Recursively populates a list with all of the rectangles in this
        /// quad and any subdivision quads.  Use the "AddRange" method of
        /// the list class to add the elements from one list to another.
        /// </summary>
        /// <returns>A list of rectangles</returns>
        public List<Rectangle> GetAllRectangles()
        {
            List<Rectangle> rects = new List<Rectangle>();
            //int index = 0;

            if (Divisions != null)
            {
                foreach (QuadTreeNode n in this._divisions)
                {
                    rects.AddRange(n.GetAllRectangles());
                }
            }
            if (Divisions == null)
            {
                rects.Add(this.Rectangle);

            }

            return rects;
        }

        /// <summary>
        /// A possibly recursive method that returns the
        /// smallest quad that contains the specified rectangle
        /// </summary>
        /// <param name="rect">The rectangle to check</param>
        /// <returns>The smallest quad that contains the rectangle</returns>
        public QuadTreeNode GetContainingQuad(Rectangle rect)
        {
            if (!this.Rectangle.Contains(rect))
            {
                return null;
            }
            else
            {
                if (this._divisions == null)
                {
                    return this;
                }

                else
                {
                    for (int n = 0; n < 4; n++)
                    {
                        if (_divisions[n].Rectangle.Contains(rect))
                        {
                            return _divisions[n].GetContainingQuad(rect);
                        }
                    }
                    return this;


                }

            }

        }
        #endregion
    }
}