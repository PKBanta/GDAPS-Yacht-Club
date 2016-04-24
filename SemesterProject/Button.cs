using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    delegate void SelectionFunction(Button selection);
    delegate void ActivationFunction();

    /// <summary>
    /// An interactive GUI object based upon specified user input
    /// </summary>
    class Button
    {
        // Fields
        private Texture2D buttonTexture;
        private Rectangle position;
        private Color buttonColor, textColor;
        private static Color inactivePressColor = new Color(100, 100, 100, 255);

        private Vector2 textLocation;
        private SpriteFont font;
        private string buttonText;

        private bool highlightable, clickable, linger;
        private GUIButtonState buttonState;
        
        /// <summary>
        /// Triggers all functions that should happen when this Button is
        /// Selected.
        /// </summary>
        public event SelectionFunction ButtonSelectionEvent;

        /// <summary>
        /// Triggers all functions that should happen when this Button is
        /// Activated (Active and Pressed).
        /// </summary>
        public event ActivationFunction ButtonActivationEvent;

        /// <summary>
        /// Flags for determining and manipulating the Button state
        /// </summary>
        private enum GUIButtonState
        {
            Standby = 1,
            Active = 2,
            Selected = 4,
            Pressed = 8,
        }

        // Properties
        /// <summary>
        /// Whether this button is activatable
        /// (should do something when clicked)
        /// </summary>
        public bool Active
        {
            get { return buttonState.HasFlag(GUIButtonState.Active); }
            set
            {
                if (value == false && Active)
                {
                    if (Selected && highlightable)
                    {
                        buttonColor = Color.Gray;
                    }
                    else
                    {
                        buttonColor = Color.DarkGray;
                    }

                    buttonState ^= GUIButtonState.Active;
                }

                else if (value == true && !Active)
                {
                    if (Selected && highlightable)
                    {
                        buttonColor = Color.LightGray;
                    }
                    else
                    {
                        buttonColor = Color.White;
                    }
                    
                    buttonState |= GUIButtonState.Active;
                }
            }
        }

        /// <summary>
        /// Whether this button is currently selected
        /// </summary>
        public bool Selected
        {
            get { return buttonState.HasFlag(GUIButtonState.Selected); }
            set
            {
                if (value == false && Selected)
                {
                    buttonState |= GUIButtonState.Standby;
                    buttonState ^= GUIButtonState.Selected;

                    if (buttonState.HasFlag(GUIButtonState.Pressed))
                    {
                        buttonState ^= GUIButtonState.Pressed;
                    }

                    if (Active)
                    {
                        buttonColor = Color.White;
                    }
                    else
                    {
                        buttonColor = Color.DarkGray;
                    }
                }

                else if (value == true && !Selected)
                {
                    buttonState ^= GUIButtonState.Standby;
                    buttonState |= GUIButtonState.Selected;

                    if (Active)
                    {
                        if (highlightable)
                        {
                            buttonColor = Color.LightGray;
                        }
                        else
                        {
                            buttonColor = Color.White;
                        }
                    }

                    else
                    {
                        if (highlightable)
                        {
                            buttonColor = Color.Gray;
                        }
                        else
                        {
                            buttonColor = Color.DarkGray;
                        }
                    }

                    if (ButtonSelectionEvent != null)
                    {
                        ButtonSelectionEvent(this);
                    }
                }
            }
        }

        /// <summary>
        /// Returns whether this button is currently pressed down
        /// </summary>
        public bool Pressed
        {
            get { return buttonState.HasFlag(GUIButtonState.Pressed); }
            set
            {
                if (value == false
                    && buttonState.HasFlag(GUIButtonState.Pressed))
                {
                    buttonState ^= GUIButtonState.Pressed;

                    if (Active && highlightable)
                    {
                        buttonColor = Color.LightGray;
                        
                        if (ButtonActivationEvent != null)
                        {
                            ButtonActivationEvent();
                        }
                    }
                    else if (Active)
                    {
                        buttonColor = Color.White;
                    }

                    else
                    {
                        buttonColor = Color.Gray;
                    }
                }

                else if (value == true
                    && !buttonState.HasFlag(GUIButtonState.Pressed))
                {
                    buttonState |= GUIButtonState.Pressed;

                    if (Active && highlightable)
                    {
                        buttonColor = Color.DarkGray;
                    }
                    else if (Active)
                    {
                        buttonColor = Color.White;

                        if (ButtonActivationEvent != null
                            && buttonState.HasFlag(GUIButtonState.Selected))
                        {
                            ButtonActivationEvent();
                        }
                    }

                    else if (highlightable)
                    {
                        buttonColor = inactivePressColor;
                    }
                    else if (!Active)
                    {
                        buttonColor = Color.Gray;
                    }
                }
            }
        }

        /// <summary>
        /// Whether this button should change colors when selected/pressed to
        /// indicate that something is happening
        /// </summary>
        public bool Highlightable
        {
            get { return highlightable; }
            set
            {
                highlightable = value;

                if (value == true)
                {
                    if (Pressed)
                    {
                        if (Active)
                        {
                            buttonColor = Color.DarkGray;
                        }
                        else
                        {
                            buttonColor = inactivePressColor;
                        }
                    }

                    else if (Selected)
                    {
                        if (Active)
                        {
                            buttonColor = Color.LightGray;
                        }
                        else
                        {
                            buttonColor = Color.Gray;
                        }
                    }
                }

                else
                {
                    if (Active)
                    {
                        buttonColor = Color.White;
                    }
                    else
                    {
                        buttonColor = Color.DarkGray;
                    }
                }
            }
        }

        /// <summary>
        /// Whether this button can be clicked with the mouse
        /// </summary>
        public bool Clickable
        {
            get { return clickable; }
            set { clickable = value; }
        }

        /// <summary>
        /// Whether this button becomes de-Selected if the mouse moves off of it
        /// </summary>
        public bool Lingering
        {
            get { return linger; }
            set { linger = value; }
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

        /// <summary>
        /// The color of this button's text
        /// </summary>
        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        /// <summary>
        /// The string text of this Button
        /// </summary>
        public String Text
        {
            get { return buttonText; }
            set { buttonText = value; }
        }

        /// <summary>
        /// The texture of this Button
        /// </summary>
        public Texture2D Texture
        {
            get { return buttonTexture; }
            set { buttonTexture = value; }
        }

        /// <summary>
        /// The font of this Button
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }


        // Constructors
        /// <summary>
        /// An interactive GUI element that can produce output based upon user
        /// input.
        /// </summary>
        /// <param name="texture">The texture of the Button's background</param>
        /// <param name="location">Where the Button is located</param>
        /// <param name="activationFunction">A void function/method that should
        /// be executed when the button is Activated</param>
        /// <param name="active">Whether prssing the Button should execute
        /// functions from an activation event</param>
        /// <param name="highlightable">Highlight the button when it's either
        /// Selected or Pressed</param>
        /// <param name="clickable">Can the button be Pressed by a left mouse
        /// click?</param>
        /// <param name="linger">Will the button stay selected if the mouse
        /// moves off of the button while the Button is clickable</param>
        /// <param name="selected">Whether the Button be Selected when it's
        /// first loaded</param>
        public Button(Texture2D texture, Rectangle location,
            ActivationFunction activationFunction, bool active,
            bool highlightable, bool clickable, bool linger,
            bool selected = false)
        {
            buttonTexture = texture;
            position = location;

            font = null;
            buttonText = "";
            textColor = Color.White;
            textLocation = Vector2.Zero;

            if (activationFunction != null)
            {
                ButtonActivationEvent += activationFunction;
            }

            this.highlightable = highlightable;
            this.linger = linger;
            this.clickable = clickable;

            if (active)
            {
                buttonState = GUIButtonState.Active;
                if (highlightable && selected)
                {
                    buttonColor = Color.LightGray;
                }
                else
                {
                    buttonColor = Color.White;
                }
            }

            else
            {
                if (highlightable && selected)
                {
                    buttonColor = Color.Gray;
                }
                else
                {
                    buttonColor = Color.DarkGray;
                }
            }

            buttonState |=
                (selected ? GUIButtonState.Selected : GUIButtonState.Standby);
        }

        /// <summary>
        /// An interactive interface based upon user input
        /// </summary>
        /// <param name="texture">Texture of the button's background</param>
        /// <param name="location">Where is the button located</param>
        /// <param name="activationFunction">A void function/method that should
        /// be executed when the button is Activated</param>
        /// <param name="font">Font of the button text</param>
        /// <param name="text">Button text</param>
        /// <param name="textLocation">Where the text is located RELATIVE to the
        /// button's location</param>
        /// <param name="textColor">The color of the text</param>
        /// <param name="active">Whether prssing the Button should execute
        /// functions from an activation event</param>
        /// <param name="highlightable">Highlight the button when it's either
        /// Selected or Pressed</param>
        /// <param name="clickable">Can the button be Pressed by a left mouse
        /// click?</param>
        /// <param name="linger">Will the button stay selected if the mouse
        /// moves off of the button while the Button is clickable</param>
        /// <param name="selected">Whether the Button be Selected when it's
        /// first loaded</param>
        public Button(Texture2D texture, Rectangle location,
            ActivationFunction activationFunction, SpriteFont font, string text,
            Vector2 textLocation, Color textColor, bool active,
            bool highlightable, bool clickable, bool linger,
            bool selected = false)
        {
            buttonTexture = texture;
            position = location;

            this.font = font;
            buttonText = text;
            this.textColor = textColor;
            this.textLocation = new Vector2(position.X, position.Y)
                + textLocation;

            if (activationFunction != null)
            {
                ButtonActivationEvent += activationFunction;
            }

            this.highlightable = highlightable;
            this.linger = linger;
            this.clickable = clickable;

            if (active)
            {
                buttonState = GUIButtonState.Active;
                if (highlightable && selected)
                {
                    buttonColor = Color.LightGray;
                }
                else
                {
                    buttonColor = Color.White;
                }
            }

            else
            {
                if (highlightable && selected)
                {
                    buttonColor = Color.Gray;
                }
                else
                {
                    buttonColor = Color.DarkGray;
                }
            }

            buttonState |=
                (selected ? GUIButtonState.Selected : GUIButtonState.Standby);
        }

        /// <summary>
        /// An interactive GUI element that can produce output based upon user
        /// input.
        /// </summary>
        /// <param name="texture">The texture of the Button's background</param>
        /// <param name="location">Where the Button is located</param>
        /// <param name="activationEvents">A List of void functions/methods that
        /// should be executed when the button is Activated</param>
        /// <param name="active">Whether prssing the Button should execute
        /// functions from an activation event</param>
        /// <param name="highlightable">Highlight the button when it's either
        /// Selected or Pressed</param>
        /// <param name="clickable">Can the button be Pressed by a left mouse
        /// click?</param>
        /// <param name="linger">Will the button stay selected if the mouse
        /// moves off of the button while the Button is clickable</param>
        /// <param name="selected">Whether the Button be Selected when it's
        /// first loaded</param>
        public Button(Texture2D texture, Rectangle location,
            List<ActivationFunction> activationEvents, bool active,
            bool highlightable, bool clickable, bool linger,
            bool selected = false)
        {
            buttonTexture = texture;
            position = location;

            font = null;
            buttonText = "";
            textColor = Color.White;
            textLocation = Vector2.Zero;
            
            foreach (ActivationFunction func in activationEvents)
            {
                if (func != null)
                {
                    ButtonActivationEvent += func;
                }
            }

            this.highlightable = highlightable;
            this.linger = linger;
            this.clickable = clickable;

            if (active)
            {
                buttonState = GUIButtonState.Active;
                if (highlightable && selected)
                {
                    buttonColor = Color.LightGray;
                }
                else
                {
                    buttonColor = Color.White;
                }
            }

            else
            {
                if (highlightable && selected)
                {
                    buttonColor = Color.Gray;
                }
                else
                {
                    buttonColor = Color.DarkGray;
                }
            }

            buttonState |=
                (selected ? GUIButtonState.Selected : GUIButtonState.Standby);
        }

        /// <summary>
        /// An interactive interface based upon user input
        /// </summary>
        /// <param name="texture">Texture of the button's background</param>
        /// <param name="location">Where is the button located</param>
        /// <param name="activationEvents">A List of void functions/methods that
        /// should be executed when the button is Activated</param>
        /// <param name="font">Font of the button text</param>
        /// <param name="text">Button text</param>
        /// <param name="textLocation">Where the text is located RELATIVE to the
        /// button's location</param>
        /// <param name="textColor">The color of the text</param>
        /// <param name="active">Whether prssing the Button should execute
        /// functions from an activation event</param>
        /// <param name="highlightable">Highlight the button when it's either
        /// Selected or Pressed</param>
        /// <param name="clickable">Can the button be Pressed by a left mouse
        /// click?</param>
        /// <param name="linger">Will the button stay selected if the mouse
        /// moves off of the button while the Button is clickable</param>
        /// <param name="selected">Whether the Button be Selected when it's
        /// first loaded</param>
        public Button(Texture2D texture, Rectangle location,
            List<ActivationFunction> activationEvents, SpriteFont font,
            string text, Vector2 textLocation, Color textColor, bool active,
            bool highlightable, bool clickable, bool linger,
            bool selected = false)
        {
            buttonTexture = texture;
            position = location;

            this.font = font;
            buttonText = text;
            this.textColor = textColor;
            this.textLocation = new Vector2(position.X, position.Y)
                + textLocation;

            foreach (ActivationFunction func in activationEvents)
            {
                if (func != null)
                {
                    ButtonActivationEvent += func;
                }
            }

            this.highlightable = highlightable;
            this.linger = linger;
            this.clickable = clickable;

            if (active)
            {
                buttonState = GUIButtonState.Active;
                if (highlightable && selected)
                {
                    buttonColor = Color.LightGray;
                }
                else
                {
                    buttonColor = Color.White;
                }
            }

            else
            {
                if (highlightable && selected)
                {
                    buttonColor = Color.Gray;
                }
                else
                {
                    buttonColor = Color.DarkGray;
                }
            }

            buttonState |=
                (selected ? GUIButtonState.Selected : GUIButtonState.Standby);
        }


        // Methods
        /// <summary>
        /// Update the button based on input information
        /// </summary>
        /// <param name="mouse">Current mouse information</param>
        /// <param name="prevMouse">Last frame's mouse information</param>
        public void Update(MouseState mouse, MouseState prevMouse)
        {
            switch (buttonState)
            {
                // Button is Active and not Selected
                case  (GUIButtonState.Active | GUIButtonState.Standby):
                    
                    // Button is Clickable
                    if (clickable && position.Contains(mouse.Position)
                        && prevMouse.Position != mouse.Position)
                    {
                        Selected = true;
                        ButtonSelected();
                    }

                    break;

                
                // Button is Active and Selected
                case (GUIButtonState.Active | GUIButtonState.Selected):
                    
                    // Button is Clickable
                    if (clickable)
                    {
                        // Not lingering
                        if (!linger && !(position.Contains(mouse.Position))
                            && (mouse.Position != prevMouse.Position))
                        {
                            Selected = false;
                            if (ButtonSelectionEvent != null)
                            {
                                ButtonSelectionEvent(null);
                            }
                            break;
                        }

                        // Button becomes Pressed
                        if (position.Contains(mouse.Position)
                            && SingleMouseClick(mouse, prevMouse))
                        {
                            Pressed = true;
                        }
                    }
                    
                    break;

                // Button is Active and Pressed
                case (GUIButtonState.Active | GUIButtonState.Selected
                | GUIButtonState.Pressed):

                    if (clickable)
                    {
                        if (mouse.LeftButton == ButtonState.Released
                        && prevMouse.LeftButton == ButtonState.Pressed
                        && position.Contains(mouse.Position))
                        {
                            Pressed = false;
                        }
                        
                        else if (!linger
                            && (mouse.Position != prevMouse.Position)
                            && !position.Contains(mouse.Position))
                        {
                            Selected = false;
                        }
                    }
                    
                    break;

                // Button is not Active
                case (GUIButtonState.Standby):
                    
                    // Button is Clickable
                    if (clickable && position.Contains(mouse.Position)
                        && prevMouse.Position != mouse.Position)
                    {
                        Selected = true;
                        ButtonSelected();
                    }

                    break;
                
                // Button is not Active and is Selected
                case (GUIButtonState.Selected):

                    // Button is clickable
                    if (clickable)
                    {
                        // Not lingering
                        if (!linger && !(position.Contains(mouse.Position))
                            && (mouse.Position != prevMouse.Position))
                        {
                            Selected = false;
                            if (ButtonSelectionEvent != null)
                            {
                                ButtonSelectionEvent(null);
                            }

                            break;
                        }

                        // Button 
                        else if (position.Contains(mouse.Position)
                            && SingleMouseClick(mouse, prevMouse))
                        {
                            Pressed = true;
                        }
                    }

                    break;

                // Button is not Active but Pressed
                case (GUIButtonState.Selected | GUIButtonState.Pressed):

                    if (clickable)
                    {
                        if (mouse.LeftButton == ButtonState.Released
                            && prevMouse.LeftButton == ButtonState.Pressed)
                        {
                            Pressed = false;
                        }

                        else if (!linger && (mouse.Position
                            != prevMouse.Position)
                            && !position.Contains(mouse.Position))
                        {
                            Selected = false;
                        }
                    }
                    
                    break;
            }
        }

        /// <summary>
        /// Draw the Button
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw button with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (buttonTexture != null)
            {
                spriteBatch.Draw(buttonTexture, position, buttonColor);
            }
            
            if (font != null)
                spriteBatch.DrawString(
                    font, buttonText, textLocation, textColor);
        }

        /// <summary>
        /// Check to see if the left mouse button was pressed just this frame
        /// </summary>
        /// <param name="mouseState">The current mouse state</param>
        /// <param name="prevMouseState">The last frame's mouse state</param>
        /// <returns>True if pressed this frame</returns>
        private bool SingleMouseClick(MouseState mouseState,
            MouseState prevMouseState)
        {
            return (mouseState.LeftButton == ButtonState.Pressed
                && prevMouseState.LeftButton == ButtonState.Released);
        }

        /// <summary>
        /// Call all of this Button's ButtonSelectionEvent events with this
        /// Button
        /// </summary>
        private void ButtonSelected()
        {
            if (ButtonSelectionEvent != null)
            {
                ButtonSelectionEvent(this);
            }
        }
    }
}