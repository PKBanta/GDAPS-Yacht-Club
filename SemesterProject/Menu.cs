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
        private List<Button> menuButtons;


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

        /// <summary>
        /// The Button in the list of buttons for this menu.
        /// </summary>
        /// <param name="index">Index of button to access</param>
        /// <returns>Button in this index</returns>
        public Button this[int index]
        {
            get
            {
                if (index >= 0 && index < menuButtons.Count)
                {
                    return menuButtons[index];
                }

                else
                {
                    return default(Button);
                }
            }
        }


        // Constructors
        /// <summary>
        /// An object representing a menu background with no additional text
        /// </summary>
        /// <param name="texture">Texture of the menu's background</param>
        /// <param name="menuLocation">Where the menu's background is located
        /// </param>
        /// <param name="buttons">The list of buttons for this menu</param>
        public Menu(Texture2D texture, Vector2 menuLocation,List<Button> buttons)
        {
            menuTexture = texture;
            menuPosition = menuLocation;

            headFont = default(SpriteFont);
            headPosition = Vector2.Zero;
            headColor = Color.White;

            bodyFont = default(SpriteFont);
            bodyPosition = Vector2.Zero;
            bodyColor = Color.White;

            menuButtons = buttons;
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
        /// <param name="buttons">The list of buttons for this menu</param>
        public Menu(Texture2D texture, Vector2 menuLocation,
            SpriteFont font, string header, Vector2 headerLocation,
            Color headerColor, List<Button> buttons)
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

            menuButtons = buttons;
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
        ///// <param name="buttons">The list of buttons for this menu</param>
        public Menu(Texture2D texture, Vector2 menuLocation,
            SpriteFont headerFont, string header, Vector2 headerLocation,
            Color headerColor, SpriteFont captionFont, string caption,
            Vector2 captionLocation, Color captionColor, List<Button> buttons)
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

            menuButtons = buttons;
        }

        
        // Methods
        /// <summary>
        /// Remove the header text from the menu. (Sets text to null)
        /// </summary>
        public void RemoveHeader()
        {
            headText = null;
        }

        /// <summary>
        /// Remove the body text from the menu. (Sets text to null)
        /// </summary>
        public void RemoveBody()
        {
            bodyText = null;
        }

        /// <summary>
        /// Update the menu's buttons
        /// </summary>
        /// <param name="mouse">Mouse information</param>
        public void Update(MouseState mouse)
        {
            for (int i = 0; i < menuButtons.Count; i++)
            {
                menuButtons[i].Update(mouse);
            }
        }

        /// <summary>
        /// Draw the menu & its buttons
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw menu with</param>
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

            // Draw the menu's buttons
            foreach (Button b in menuButtons)
            {
                b.Draw(spriteBatch);
            }
        }
    }
}
