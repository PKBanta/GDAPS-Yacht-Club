using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemesterProject
{
    class Level
    {
        // Fields
        private LinkedList<Room> rooms; // This level's sequential list of rooms


        // Properties
        /// <summary>
        /// Get the Room object at the specified index of the level's room
        /// sequence
        /// </summary>
        /// <param name="index">Index in room sequence</param>
        /// <returns>Room at <paramref name="index"/> in level sequence
        /// </returns>
        public Room this[int index]
        {
            get { return rooms.ElementAt(index); }
        }


        // Constructors
        public Level(List<Room> levelRooms)
        {
            foreach (Room r in levelRooms)
            {
                rooms.AddLast(r);
            }
        }


        // Methods
    }
}
