using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SemesterProject
{
    /// <summary>
    /// Game Screen representing an interface that displays text atop a
    /// specified background image
    /// </summary>
    class Menu
    {
        // Fields
        private Texture2D menuTexture;
        private SpriteFont headFont, bodyFont;
        private Vector2 menuPosition, headPosition, bodyPosition;
        private string headText, bodyText;
        private Color headColor, bodyColor;


        // Properties
        /// <summary>
        /// The font of the header
        /// </summary>
        public SpriteFont HeaderFont
        {
            get { return headFont; }
            set
            {
                if (value != null)
                    headFont = value;
            }
        }

        /// <summary>
        /// The font of the body
        /// </summary>
        public SpriteFont BodyFont
        {
            get { return bodyFont; }
            set
            {
                if (value != null)
                    bodyFont = value;
            }
        }

        /// <summary>
        /// The text of the header
        /// </summary>
        public string HeaderText
        {
            get { return headText; }
            set { headText = value; }
        }

        /// <summary>
        /// The text of the body
        /// </summary>
        public string BodyText
        {
            get { return bodyText; }
            set { bodyText = value; }
        }

        /// <summary>
        /// The position of menu
        /// </summary>
        public Vector2 MenuPosition
        {
            get { return menuPosition; }
            set
            {
                if (value != null)
                    menuPosition = value;
            }
        }

        /// <summary>
        /// The position of the header relative to the menu's location
        /// </summary>
        public Vector2 HeaderPosition
        {
            get { return headPosition - menuPosition; }
            set
            {
                if (value != null)
                    headPosition = menuPosition + value;
            }
        }

        /// <summary>
        /// The position of the body relative to the menu's location
        /// </summary>
        public Vector2 BodyPosition
        {
            get { return bodyPosition - menuPosition; }
            set
            {
                if (value != null)
                    bodyPosition = menuPosition + value;
            }
        }

        /// <summary>
        /// The color of the header text
        /// </summary>
        public Color HeaderColor
        {
            get { return headColor; }
            set
            {
                if (value != null)
                    headColor = value;
            }
        }

        /// <summary>
        /// The color of the body text
        /// </summary>
        public Color BodyColor
        {
            get { return bodyColor; }
            set
            {
                if (value != null)
                    bodyColor = value;
            }
        }


        // Constructors
        /// <summary>
        /// An object representing a menu background with no additional text
        /// </summary>
        /// <param name="texture">Texture of the menu's background</param>
        /// <param name="menuLocation">Where the menu's background is located
        /// </param>
        public Menu(Texture2D texture, Vector2 menuLocation)
        {
            menuTexture = texture;
            menuPosition = menuLocation;

            headFont = default(SpriteFont);
            headPosition = Vector2.Zero;
            headColor = Color.White;

            bodyFont = default(SpriteFont);
            bodyPosition = Vector2.Zero;
            bodyColor = Color.White;
        }

        /// <summary>
        /// An object representing a menu that displays header text
        /// </summary>
        /// <param name="texture">Texture of the menu's background</param>
        /// <param name="menuLocation">Where the menu's background is located
        /// </param>
        /// <param name="font">Font of the header</param>
        /// <param name="header">Header text</param>
        /// <param name="headerLocation">The location of the header relative to
        /// the location of the menu itself</param>
        /// <param name="headerColor">Color of header text</param>
        public Menu(Texture2D texture, Vector2 menuLocation,
            SpriteFont font, string header, Vector2 headerLocation,
            Color headerColor)
        {
            menuTexture = texture;
            menuPosition = menuLocation;

            headFont = font;
            headText = header;
            headPosition = menuLocation + headerLocation;
            headColor = headerColor;

            bodyFont = default(SpriteFont);
            bodyPosition = Vector2.Zero;
            bodyColor = Color.White;
        }

        /// <summary>
        /// An object representing a menu that displays header and body text
        /// </summary>
        /// <param name="texture">Texture of the menu's background</param>
        /// <param name="menuLocation">Where the menu's background is located
        /// </param>
        /// <param name="headerFont">Font of the header</param>
        /// <param name="header">Header text</param>
        /// <param name="headerLocation">The location of the header relative to
        /// the location of the menu itself</param>
        /// <param name="headerColor">Color of header text</param>
        /// <param name="captionFont">Font of the body</param>
        /// <param name="caption">Body text</param>
        /// <param name="captionLocation">The location of the body relative
        /// to the location of the menu itself</param>
        /// <param name="captionColor">Color of body text</param>
        public Menu(Texture2D texture, Vector2 menuLocation,
            SpriteFont headerFont, string header, Vector2 headerLocation,
            Color headerColor, SpriteFont captionFont, string caption,
            Vector2 captionLocation, Color captionColor)
        {
            menuTexture = texture;
            menuPosition = menuLocation;

            headFont = headerFont;
            headText = header;
            headPosition = menuLocation + headerLocation;
            headColor = headerColor;

            bodyFont = captionFont;
            bodyText = caption;
            bodyPosition = menuLocation + captionLocation;
            bodyColor = captionColor;
        }

        
        // Methods
        public void RemoveHeader()
        {
            headText = null;
        }

        public void RemoveBody()
        {
            bodyText = null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the menu's background
            spriteBatch.Draw(
                menuTexture,
                menuPosition,
                Color.White);

            // Draw the menu header
            if (headText != null)
            {
                spriteBatch.DrawString(
                    headFont,
                    headText,
                    headPosition,
                    headColor);
            }

            // Draw the menu body text
            if (bodyText != null)
            {
                spriteBatch.DrawString(
                    bodyFont,
                    bodyText,
                    bodyPosition,
                    bodyColor);
            }
        }

    }
}
