using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SemesterProject
{
    /// <summary>
    /// A GUI element representing header and/or body text laid atop of a
    /// background image.
    /// </summary>
    class Menu
    {
        // Fields
        protected Texture2D menuTexture;
        protected Vector2 menuPosition, headPosition, bodyPosition;
        protected Color headColor, bodyColor, menuColor;
        protected SpriteFont headFont, bodyFont;
        protected string headText, bodyText;


        // Properties
        /// <summary>
        /// The Menu's background texture
        /// </summary>
        public Texture2D Texture
        {
            get { return menuTexture; }
            set { menuTexture = value; }
        }

        /// <summary>
        /// The font of the Header
        /// </summary>
        public SpriteFont HeaderFont
        {
            get { return headFont; }
            set { headFont = value; }
        }

        /// <summary>
        /// The font of the Body
        /// </summary>
        public SpriteFont BodyFont
        {
            get { return bodyFont; }
            set { bodyFont = value; }
        }

        /// <summary>
        /// The position of the Menu's background location
        /// </summary>
        public Vector2 MenuPosition
        {
            get { return menuPosition; }
            set
            {
                if (value != null)
                {
                    menuPosition = value;
                }

                else
                {
                    menuPosition = Vector2.Zero;
                }
            }
        }

        /// <summary>
        /// The position of the Header relative to the Menu's location
        /// </summary>
        public Vector2 HeaderPosition
        {
            get { return headPosition - menuPosition; }
            set
            {
                if (value != null)
                {
                    headPosition = menuPosition + value;
                }

                else
                {
                    headPosition = menuPosition;
                }
            }
        }

        /// <summary>
        /// The position of the Body relative to the Menu's location
        /// </summary>
        public Vector2 BodyPosition
        {
            get { return bodyPosition - menuPosition; }
            set
            {
                if (value != null)
                {
                    bodyPosition = menuPosition + value;
                }

                else
                {
                    bodyPosition = menuPosition;
                }
            }
        }

        /// <summary>
        /// The overall location of the Menu.<para>Changing this property will
        /// change the location of both background and texts.</para>
        /// </summary>
        public Vector2 Position
        {
            set
            {
                if (value != null)
                {
                    headPosition += (headPosition - menuPosition) + value;
                    menuPosition = value;
                }

                else
                {
                    headPosition = headPosition - menuPosition;
                    menuPosition = Vector2.Zero;
                }
            }
        }

        /// <summary>
        /// The text of the Header
        /// </summary>
        public string HeaderText
        {
            get { return headText; }
            set { headText = value; }
        }

        /// <summary>
        /// The text of the Body
        /// </summary>
        public string BodyText
        {
            get { return bodyText; }
            set { bodyText = value; }
        }

        /// <summary>
        /// The color of the Menu background
        /// </summary>
        public Color MenuColor
        {
            get { return menuColor; }
            set
            {
                if (value != null)
                    menuColor = value;
            }
        }

        /// <summary>
        /// The color of the Header text
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

        // Constructor
        /// <summary>
        /// A Menu with no text
        /// </summary>
        /// <param name="texture">The Menu's background image</param>
        /// <param name="menuLocation">The location of the background</param>
        /// <param name="menuColor">The Color of the Menu</param>
        public Menu(Texture2D texture, Vector2 menuLocation, Color menuColor)
        {
            menuTexture = texture;
            menuPosition = menuLocation;
            this.menuColor = menuColor;

            headFont = null;
            headPosition = menuPosition;
            headText = "";
            headColor = Color.White;

            bodyFont = null;
            bodyPosition = menuPosition;
            bodyText = "";
            bodyColor = Color.White;
        }

        /// <summary>
        /// A Menu with Header text and no background
        /// </summary>
        /// <param name="headerFont">The font of the Header</param>
        /// <param name="headerText">The text of the Header</param>
        /// <param name="headerLocation">The location of the Header</param>
        /// <param name="headerColor">The color of the Header text</param>
        public Menu(SpriteFont headerFont, string headerText,
            Vector2 headerLocation, Color headerColor)
        {
            menuTexture = null;
            menuPosition = headerLocation;
            menuColor = Color.White;
        
            headFont = headerFont;
            headPosition = headerLocation;
            headText = headerText;
            headColor = headerColor;

            bodyFont = null;
            bodyPosition = headerLocation;
            bodyText = "";
            bodyColor = Color.White;
        }

        /// <summary>
        /// A Menu with Header and Body text with no background
        /// </summary>
        /// <param name="headerFont">The font of the Header</param>
        /// <param name="headerText">The text of the Header</param>
        /// <param name="headerLocation">The location of the Header</param>
        /// <param name="headerColor">The color of the Header text</param>
        /// <param name="bodyFont">The font of the Body</param>
        /// <param name="bodyText">The text of the Body</param>
        /// <param name="bodyLocation">The location of the Body</param>
        /// <param name="bodyColor">The color of the Body text</param>
        public Menu(SpriteFont headerFont, string headerText,
            Vector2 headerLocation, Color headerColor, SpriteFont bodyFont,
            string bodyText, Vector2 bodyLocation, Color bodyColor)
        {
            menuTexture = null;
            menuPosition = headerLocation;
            menuColor = Color.White;

            headFont = headerFont;
            headPosition = headerLocation;
            headText = headerText;
            headColor = headerColor;

            this.bodyFont = bodyFont;
            bodyPosition = headerLocation + bodyLocation;
            this.bodyText = bodyText;
            this.bodyColor = bodyColor;
        }

        /// <summary>
        /// A Menu with a background and Header text
        /// </summary>
        /// <param name="texture">The Menu's background image</param>
        /// <param name="menuLocation">The location of the background</param>
        /// <param name="menuColor">The Color of the Menu</param>
        /// <param name="headerFont">The font of the Header</param>
        /// <param name="headerText">The text of the Header</param>
        /// <param name="headerLocation">The location of the Header RELATIVE to
        /// the location of the menu itself</param>
        /// <param name="headerColor">The color of the Header text</param>
        public Menu(Texture2D texture, Vector2 menuLocation, Color menuColor,
            SpriteFont headerFont, string headerText, Vector2 headerLocation,
            Color headerColor)
        {
            menuTexture = texture;
            menuPosition = menuLocation;
            this.menuColor = menuColor;

            headFont = headerFont;
            headPosition = menuLocation + headerLocation;
            headText = headerText;
            headColor = headerColor;

            bodyFont = null;
            bodyPosition = menuLocation;
            bodyText = "";
            bodyColor = Color.White;
        }

        /// <summary>
        /// A Menu with a background and Header and Body texts
        /// </summary>
        /// <param name="texture">The Menu's background image</param>
        /// <param name="menuLocation">The location of the background</param>
        /// <param name="menuColor">The Color of the Menu</param>
        /// <param name="headerFont">The font of the Header</param>
        /// <param name="headerText">The text of the Header</param>
        /// <param name="headerLocation">The location of the Header RELATIVE to
        /// the location of the menu itself</param>
        /// <param name="headerColor">The color of the Header text</param>
        /// <param name="bodyFont">The font of the Body</param>
        /// <param name="bodyText">The text of the Body</param>
        /// <param name="bodyLocation">The location of the Body RELATIVE to
        /// the location of the menu itself</param>
        /// <param name="bodyColor">The color of the Body text</param>
        public Menu(Texture2D texture, Vector2 menuLocation, Color menuColor,
            SpriteFont headerFont, string headerText, Vector2 headerLocation,
            Color headerColor, SpriteFont bodyFont, string bodyText,
            Vector2 bodyLocation, Color bodyColor)
        {
            menuTexture = texture;
            menuPosition = menuLocation;
            this.menuColor = menuColor;

            headFont = headerFont;
            headPosition = menuLocation + headerLocation;
            headText = headerText;
            headColor = headerColor;

            this.bodyFont = bodyFont;
            bodyPosition = menuLocation + bodyLocation;
            this.bodyText = bodyText;
            this.bodyColor = bodyColor;
        }


        // Methods
        /// <summary>
        /// Draw the Menu.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (menuTexture != null)
            {
                spriteBatch.Draw(
                    menuTexture,
                    menuPosition,
                    menuColor);
            }

            if (headFont != null)
            {
                spriteBatch.DrawString(
                    headFont,
                    headText,
                    headPosition,
                    headColor);
            }

            if (bodyFont != null)
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