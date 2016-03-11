using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    /// <summary>
    /// An interactive GUI object based upon specified user input
    /// </summary>
    class Button
    {
        // Fields
        private Texture2D buttonTexture; // Texture of background image
        private Rectangle position;
        private Vector2 textLocation;
        private SpriteFont font;
        private string buttonText;
        private bool active, clickable, pressed;
        private Color buttonColor, textColor;

        // Properties
        /// <summary>
        /// Whether this button is activatable
        /// (should do something when clicked)
        /// </summary>
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        /// <summary>
        /// Returns if the button is activatable and pressed down.
        /// Used in determining whether an action should be performed.
        /// </summary>
        public bool Activated
        {
            get { return (active && pressed); }
        }

        /// <summary>
        /// Whether this button should change colors when clicked
        /// </summary>
        public bool Clickable
        {
            get { return clickable; }
            set { clickable = value; }
        }

        /// <summary>
        /// Returns whether this button is currently pressed down
        /// </summary>
        public bool Pressed
        {
            get { return pressed; }
        }

        /// <summary>
        /// The position of the button
        /// </summary>
        public Rectangle Position
        {
            get { return position; }
            set
            {
                textLocation = (textLocation - (
                    new Vector2(position.X, position.Y)))
                    + new Vector2(value.X, value.Y);

                position = value;
            }
        }


        // Constructor
        /// <summary>
        /// An interactive interface based upon user input
        /// </summary>
        /// <param name="texture">Texture of the button's background</param>
        /// <param name="location">Where is the button located</param>
        /// <param name="font">Font of the button text</param>
        /// <param name="text">Button text</param>
        /// <param name="textLocation">Where the text is located RELATIVE to the
        /// button's location</param>
        /// <param name="textColor">The color of the text</param>
        /// <param name="active">Is this button activatable?</param>
        /// <param name="clickable">Is this button clickable? Will it notify the
        /// user when it has been pressed?)</param>
        public Button(Texture2D texture, Rectangle location, SpriteFont font,
            string text, Vector2 textLocation, Color textColor,
            bool active = true, bool clickable = true)
        {
            buttonTexture = texture;
            position = location;
            this.font = font;
            buttonText = text;

            pressed = false;

            this.active = active;
            this.clickable = clickable;

            this.textLocation = new Vector2(position.X, position.Y)
                + textLocation;

            buttonColor = Color.White;
            this.textColor = textColor;
        }


        // Methods
        /// <summary>
        /// Update the button based on input information
        /// </summary>
        /// <param name="mouse">Mouse information</param>
        public void Update(MouseState mouse)
        {
            // Button is selected (Mouse hovering over button)
            if (position.Contains(mouse.X, mouse.Y))
            {
                // Button is pressed down
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (active)
                    {
                        pressed = true;
                    }

                    // Button is clickable and clicked on
                    if (clickable)
                    {
                        buttonColor = Color.DarkGray;
                    }
                }

                // Button is not pressed down
                else
                {
                    pressed = false;

                    // Button is activatable
                    if (active)
                    {
                        // Button is activatable, clickable, and hovered over
                        if (clickable)
                        {
                            buttonColor = Color.LightGray;
                        }

                        // Button is activatable, unclickable, and hovered over
                        else
                        {
                            buttonColor = Color.White;
                        }
                    }

                    // Button is not activatable
                    else
                    {
                        // Not activatable, clickable, and hovered over
                        if (clickable)
                        {
                            buttonColor = Color.Gray;
                        }

                        // Not activatable, unclickable, and hovered over
                        else
                        {
                            buttonColor = Color.DarkGray;
                        }
                    }
                }
            }

            // Mouse not hovering over button
            else
            {
                // Button is activatable
                if (active)
                {
                    buttonColor = Color.White;
                }

                // Button is not activatable
                else
                {
                    // Not activatable but still clickable
                    if (clickable)
                    {
                        buttonColor = Color.DarkGray;
                    }

                    // Not activatable or clickable
                    else
                    {
                        buttonColor = Color.DarkGray;
                    }
                }

                pressed = false;
            }
        }

        /// <summary>
        /// Draw the Button
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw button with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonTexture, position, buttonColor);
            spriteBatch.DrawString(font, buttonText, textLocation, textColor);
        }
    }
}
