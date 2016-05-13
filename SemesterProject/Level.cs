using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemesterProject
{
    internal enum LevelType
    {
        street,
        sewer,
        skyscraper
    }

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
            get
            {
                if (index >= 0 && index < rooms.Count)
                {
                    return rooms.ElementAt(index);
                }

                else
                {
                    return default(Room);
                }
            }
        }


        // Constructors
        public Level(List<Room> levelRooms)
        {
            rooms = new LinkedList<Room>();

            foreach (Room r in levelRooms)
            {
                rooms.AddLast(r);
            }
        }


        // Methods
    }
}
