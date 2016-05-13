using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    /// <summary>
    /// An object that stores an event. The event's functions are executed when
    /// a specified action is executed during a timed window of opportunity.
    /// </summary>
    class QuickTimeEvent
    {
        // Fields
        public event ActivationFunction quickTimeEvent;
        private Keys activationKey;
        private double totalTime, windowLength;
        private bool windowOpen;


        // Properties
        /// <summary>
        /// The key to press during the window of opportunity to activate the
        /// quicktime event
        /// </summary>
        public Keys ActivationKey
        {
            get { return activationKey; }
            set
            {
                if (value != Keys.None)
                {
                    activationKey = value;
                }
            }
        }

        /// <summary>
        /// The length of time that the window of opportunity for this quicktime
        /// event is open
        /// </summary>
        public double WindowLength
        {
            get { return windowLength; }
            set
            {
                if (value > 0)
                {
                    windowLength = value;
                }
            }
        }

        /// <summary>
        /// Whether the window of opportunity is currently open for this
        /// quicktime event
        /// </summary>
        public bool WindowOpen
        {
            get { return windowOpen; }
            set
            {
                if (value == false)
                {
                    totalTime = 0;
                }

                windowOpen = value;
            }
        }

        // Constructors
        /// <summary>
        /// Create a new quicktime event activated with keyboard input
        /// </summary>
        /// <param name="function">Function to execute when the quicktime action
        /// is executed within the window of opportunity for the event</param>
        /// <param name="activationKey">Key to press during the window of
        /// opportunity to activate the quicktime event (Cannot be Keys.None)
        /// </param>
        /// <param name="windowLength">The length of time that the window of
        /// opportunity will be open for, in seconds</param>
        public QuickTimeEvent(ActivationFunction function, Keys activationKey,
            double windowLength)
        {
            // Set up opportunity window fields
            totalTime = 0;
            windowOpen = false;
            if (windowLength > 0)
            {
                this.windowLength = windowLength;
            }
            else
            {
                windowLength = 1;
            }

            // Add the function to the event
            quickTimeEvent += function;

            // Ensure the activation key is valid and add it
            if (activationKey == Keys.None)
            {
                this.activationKey = Keys.Enter;
            }
            else
            {
                this.activationKey = activationKey;
            }
        }

        /// <summary>
        /// Create a new quicktime event activated with keyboard input
        /// </summary>
        /// <param name="functions">Functions to execute when the quicktime
        /// action is executed within the window of opportunity for the event
        /// </param>
        /// <param name="activationKey">Key to press during the window of
        /// opportunity to activate the quicktime event (Cannot be Keys.None)
        /// </param>
        /// <param name="windowLength">The length of time that the window of
        /// opportunity will be open for</param>
        public QuickTimeEvent(List<ActivationFunction> functions,
            Keys activationKey, double windowLength)
        {
            // Set up opportunity window fields
            totalTime = 0;
            windowOpen = false;
            if (windowLength > 0)
            {
                this.windowLength = windowLength;
            }
            else
            {
                windowLength = 1;
            }

            // Add all the functions to the event
            foreach (ActivationFunction function in functions)
            {
                quickTimeEvent += function;
            }

            // Ensure the activation key is valid and add it
            if (activationKey == Keys.None)
            {
                this.activationKey = Keys.Enter;
            }
            else
            {
                this.activationKey = activationKey;
            }
        }


        // Methods
        /// <summary>
        /// Update this QuickTimeEvent by determining whether the activation key
        /// was pressed during this frame
        /// </summary>
        /// <param name="elapsedTime">The time passed since the last frame
        /// </param>
        /// <param name="keyState">The current keyboard state</param>
        /// <param name="previousKeyState">The previous keyboard state</param>
        public void Update(double elapsedTime, KeyboardState keyState,
            KeyboardState previousKeyState)
        {
            // If the window of opportunity is open
            if (windowOpen)
            {
                // If the total time that has passed exceeds the window of
                // opportunity for this QuickTimeEvent, close the window and
                // reset the totalTime timer
                if (totalTime >= windowLength)
                {
                    windowOpen = false;
                    totalTime = 0;
                    return;
                }

                // If the window is still open, keep counting the total time
                // that has passed
                totalTime += elapsedTime;

                // Also determine if the QuickTimeEvent conditions have been met
                if (keyState.IsKeyDown(activationKey)
                    && previousKeyState.IsKeyUp(activationKey))
                {
                    // If it has been, close the window, restart the timer, and
                    // invoke the QuickTimeEvent function(s)
                    windowOpen = false;
                    totalTime = 0;
                    quickTimeEvent();
                }
            }
            
        }

        /// <summary>
        /// Activate the quicktime event function regardless of window values
        /// </summary>
        public void Invoke()
        {
            quickTimeEvent();
        }

        /// <summary>
        /// Open the window of opportunity to activate this quicktime event with
        /// a defined period of time to activate the QuickTimeEvent
        /// </summary>
        /// <param name="windowLength">The period of time that the window of
        /// opportunity will be open for</param>
        public void OpenWindow(double windowLength)
        {
            totalTime = 0;
            WindowLength = windowLength;
            windowOpen = true;
        }

        /// <summary>
        /// Open the window of opportunity to activate this quicktime event with
        /// the currently defined period of time to activate the QuickTimeEvent
        /// </summary>
        public void OpenWindow()
        {
            totalTime = 0;
            windowOpen = true;
        }
    }
}