using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SemesterProject
{
    /// <summary>
    /// An object that represents the physical representation of a measured
    /// quantity with a maximum value
    /// </summary>
    class StatusBar
    {
        // Fields
        private Texture2D baseTexture, currentTexture;
        private Rectangle baseBar, currentBar;
        private Color currentColor, baseColor, textColor, shadowColor;
        private float maxValue, currentValue;
        private SpriteFont font;
        private bool drawText;
        private Vector2 textLocation, shadowOffset;
        private const int Y_OFFSET = 2;


        // Properties
        /// <summary>
        /// The maximum value this Status Bar can measure
        /// </summary>
        public float MaxValue
        {
            get { return maxValue; }
            set
            {
                if (maxValue > 0)
                {
                    maxValue = value;

                    if (currentValue > maxValue)
                    {
                        currentValue = maxValue;
                    }

                    currentBar.Width = (int)(Math.Round(
                        (currentValue / maxValue) * (baseBar.Width
                        - (currentBar.X * 2))));
                }
            }
        }

        /// <summary>
        /// The current value of the status bar (the unit being measured by the
        /// bar)
        /// </summary>
        public float CurrentValue
        {
            get { return currentValue; }
            set
            {
                if (value > maxValue)
                {
                    value = maxValue;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                currentValue = value;

                currentBar.Width = (int)(Math.Round(
                    (currentValue / maxValue) * (baseBar.Width
                    - ((currentBar.X - baseBar.X) * 2))));
            }
        }

        /// <summary>
        /// The Rectangle of the base bar
        /// </summary>
        public Rectangle BaseBar
        {
            get { return baseBar; }
        }

        /// <summary>
        /// The Rectangle of the overlayed bar/current value bar
        /// </summary>
        public Rectangle CurrentBar
        {
            get { return currentBar; }
        }

        /// <summary>
        /// The width of the BaseBar of this status bar. Changing this will
        /// dynamically change the width of the overlayed bar.
        /// </summary>
        public int BaseWidth
        {
            get { return baseBar.Width; }
            set
            {
                if (value > 4)
                {
                    baseBar.Width = value;
                    currentBar.Width = (int)(Math.Round(
                        (currentValue / maxValue) * (baseBar.Width
                        - ((currentBar.X - baseBar.X) * 2))));

                    textLocation = font.MeasureString(currentValue.ToString()
                        + " / " + maxValue);
                    textLocation.X = baseBar.X + baseBar.Width / 2
                        - textLocation.X / 2;
                    textLocation.Y = baseBar.Y + baseBar.Height / 2
                        - textLocation.Y / 2 + Y_OFFSET;
                }
            }
        }

        /// <summary>
        /// The X position of this StatusBar
        /// </summary>
        public int X
        {
            get { return baseBar.X; }
            set
            {
                currentBar.X += value - baseBar.X;
                baseBar.X = value;
                textLocation = font.MeasureString(currentValue.ToString()
                + " / " + maxValue);
                textLocation.X = baseBar.X + baseBar.Width / 2
                    - textLocation.X / 2;
                textLocation.Y = baseBar.Y + baseBar.Height / 2
                    - textLocation.Y / 2 + Y_OFFSET;
            }
        }

        /// <summary>
        /// The Y position of this StatusBar
        /// </summary>
        public int Y
        {
            get { return baseBar.Y; }
            set
            {
                currentBar.Y += value - baseBar.Y;
                baseBar.Y = value;

                textLocation = font.MeasureString(currentValue.ToString()
                + " / " + maxValue);
                textLocation.X = baseBar.X + baseBar.Width / 2
                    - textLocation.X / 2;
                textLocation.Y = baseBar.Y + baseBar.Height / 2
                    - textLocation.Y / 2 + Y_OFFSET;
            }
        }

        /// <summary>
        /// The locaion of this StatusBar
        /// </summary>
        public Vector2 Location
        {
            get { return new Vector2(baseBar.X, baseBar.Y); }
            set
            {
                currentBar.X += (int)value.X - baseBar.X;
                currentBar.Y += (int)value.Y - baseBar.Y;
                baseBar.X = (int)value.X;
                baseBar.Y = (int)value.Y;

                textLocation = font.MeasureString(currentValue.ToString()
                + " / " + maxValue);
                textLocation.X = baseBar.X + baseBar.Width / 2
                    - textLocation.X / 2;
                textLocation.Y = baseBar.Y + baseBar.Height / 2
                    - textLocation.Y / 2 + Y_OFFSET;
            }
        }

        /// <summary>
        /// The font of the text on this StatusBar
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        /// <summary>
        /// Whether or nor text should be drawn on top of the StatusBar
        /// </summary>
        public bool DrawText
        {
            get { return drawText; }
            set { drawText = value; }
        }

        /// <summary>
        /// The color of the comparative overlay bar
        /// </summary>
        public Color CurrentColor
        {
            get { return currentColor; }
            set { currentColor = value; }
        }

        /// <summary>
        /// The color of the base bar
        /// </summary>
        public Color BaseColor
        {
            get { return baseColor; }
            set { baseColor = value; }
        }

        /// <summary>
        /// The color of the text for the StatusBar
        /// </summary>
        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        /// <summary>
        /// The color of the text for the StatusBar
        /// </summary>
        public Color TextShadowColor
        {
            get { return textColor; }
            set { textColor = value; }
        }


        // Constructor
        /// <summary>
        /// An object that represents the physical representation of a measured
        /// quantity with a maximum value
        /// </summary>
        /// <param name="baseTexture">The texture of the base background bar
        /// </param>
        /// <param name="currentTexture">The texture of the overlayed bar which
        /// represents the current measured value.</param>
        /// <param name="barRectangle">The location and size of the base Status
        /// Bar - Should be at least 5 units tall/wide, as the overlayed bar
        /// will be displayed </param>
        /// <param name="currentBarRectangle">The location and size of the
        /// overlayed StatusBar, which represents the current measured value.
        /// Location is RELATIVE to the base barRectangle. There must be at
        /// least 2 units of padding space on each side of the currentBarLoc
        /// between the edge of the barRectangle.</param>
        /// <param name="font">The font to draw the text on</param>
        /// <param name="max">The maximum quantity that this status bar measures
        /// </param>
        public StatusBar(Texture2D baseTexture, Texture2D currentTexture,
            Rectangle barRectangle, Rectangle currentBarRectangle,
            SpriteFont font, float max)
        {
            this.baseTexture = baseTexture;
            this.currentTexture = currentTexture;
            this.font = font;
            drawText = true;

            // Ensure that the base background bar's positioning and size is physically possible to draw
            if (barRectangle.Height > 4 && barRectangle.Width > 4)
            {
                baseBar = barRectangle;
            }
            else
            {
                barRectangle = new Rectangle(0, 0, 104, 13);
            }

            // Ensure that the overlayed bar's positioning and size is physically possible to draw
            if (currentBarRectangle.Y > baseBar.Height / 2 || currentBarRectangle.Y < 2)
            {
                currentBarRectangle.Y = baseBar.Height / 2;
            }

            if (currentBarRectangle.X > baseBar.Width / 2 || currentBarRectangle.X < 2)
            {
                currentBarRectangle.X = baseBar.Width / 2;
            }

            currentBar.Height = baseBar.Y + baseBar.Height - currentBarRectangle.Y - 2;

            if (baseBar.Width - currentBarRectangle.Width - currentBarRectangle.X < 2)
            {
                currentBarRectangle.Width = 1;
            }

            // Create the bars
            currentBar = new Rectangle(
                currentBarRectangle.X + baseBar.X,
                currentBarRectangle.Y + baseBar.Y,
                currentBarRectangle.Width,
                currentBarRectangle.Height);

            currentColor = Color.LawnGreen;
            baseColor = Color.White;
            textColor = new Color(237, 234, 227);
            shadowColor = new Color(42, 41, 61);

            maxValue = max;
            currentValue = max;

            textLocation = font.MeasureString(currentValue.ToString()
                + " / " + maxValue);
            textLocation.X = baseBar.X + baseBar.Width / 2
                - textLocation.X / 2;
            textLocation.Y = baseBar.Y + baseBar.Height / 2
                - textLocation.Y / 2 + Y_OFFSET;

            shadowOffset = new Vector2(-1, 1);
        }


        // Methods
        /// <summary>
        /// Draw this StatusBar to the screen
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                baseTexture,
                baseBar,
                baseColor);

            spriteBatch.Draw(
                currentTexture,
                currentBar,
                currentColor);

            if (drawText)
            {
                spriteBatch.DrawString(
                    font,
                    currentValue.ToString() + " / " + maxValue,
                    textLocation + shadowOffset,
                    shadowColor);
                spriteBatch.DrawString(
                    font,
                    currentValue.ToString() + " / " + maxValue,
                    textLocation,
                    textColor);
            }
        }
    }
}
