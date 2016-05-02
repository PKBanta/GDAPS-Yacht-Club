using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    /// <summary>
    /// A game screen representing an interactive interface that displays text
    /// atop a specified background image
    /// </summary>
    class ListMenu : Menu
    {
        // Fields
        private List<Button> menuButtons;
        private bool wrapping;
        private int selectedButton, count, initialSelect;
        private Keys prevKey, nextKey, pressKey;

        // Properties
        /// <summary>
        /// How many buttons are included in this menu
        /// </summary>
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// The Button in the list of buttons for this menu.
        /// </summary>
        /// <param name="index">Index of button to access</param>
        /// <returns>Button at this index</returns>
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
        /// A Menu object that consists of a List of Button objects which can be
        /// selected and accessed in a sequential order via specified keyboard
        /// controls.
        /// </summary>
        /// <param name="menu">The base Menu object for this ListMenu</param>
        /// <param name="buttons">Sequential list of user-interactable Buttons
        /// that, when activated, should execute a set of specified functions
        /// assigned to the Button</param>
        /// <param name="backKey">Keyboard key that should be used to navigate
        /// selection to the previous Button in the <paramref name="buttons"/>
        /// list<para>(Cannot be equal to <paramref name="nextKey"/> or
        /// <paramref name="confirmKey"/> unless value is Keys.None)</para>
        /// </param>
        /// <param name="nextKey">Keyboard key that should be used to navigate
        /// selection to the previous Button in the <paramref name="buttons"/>
        /// list<para>(Cannot be equal to <paramref name="backKey"/> or
        /// <paramref name="confirmKey"/> unless value is Keys.None)</para>
        /// </param>
        /// <param name="confirmKey">Keyboard key that should be used to Press
        /// the currently selected Button in the <paramref name="buttons"/> List
        /// <para>(Cannot be equal to <paramref name="backKey"/> or <paramref
        /// name="nextKey"/> unless value is Keys.None)</para></param>
        /// <param name="initSelect">When the ListMenu is first loaded, the
        /// index of this Button in the buttons List should be the default
        /// Selected Button</param>
        /// <param name="wrapSelection">When the first or last Buttons in this
        /// ListMenu's <paramref name="buttons"/> List are Selected, can
        /// <paramref name="backKey"/> or <paramref name="nextKey"/> be used to
        /// traverse to the opposite end of the <paramref name="buttons"/> List?
        /// </param>
        public ListMenu(Menu menu, List<Button> buttons, Keys backKey,
            Keys nextKey, Keys confirmKey, int initSelect, bool wrapSelection)
            : base(menu.Texture, menu.MenuPosition, menu.MenuColor,
                  menu.HeaderFont, menu.HeaderText, menu.HeaderPosition,
                  menu.HeaderColor, menu.BodyFont, menu.BodyText,
                  menu.BodyPosition, menu.BodyColor)
        {
            menuButtons = new List<Button>();
            wrapping = wrapSelection;

            prevKey = backKey;
            this.nextKey = nextKey;
            pressKey = confirmKey;

            count = 0;

            if (buttons != null)
            {
                if (initSelect < -1 || initSelect >= buttons.Count)
                {
                    initSelect = -1;
                }
                selectedButton = initSelect;

                if ((backKey != Keys.None && (backKey == nextKey
                    || backKey == confirmKey)) || (nextKey != Keys.None
                    && (nextKey == backKey || nextKey == confirmKey))
                    || (confirmKey != Keys.None && (confirmKey == backKey
                    || confirmKey == nextKey)))
                {
                    throw new ArgumentException(
                        "ListMenu navigation keys cannot be the same.");
                }

                foreach (Button b in buttons)
                {
                    if (b != null)
                    {
                        if (count == selectedButton - 1)
                        {
                            if (!b.Selected)
                            {
                                b.Selected = true;
                            }
                        }

                        else if (b.Selected)
                        {
                            b.Selected = false;
                        }

                        b.ButtonSelectionEvent += SelectSingleButton;

                        menuButtons.Add(b);
                        count++;
                    }
                }
            }

            else
            {
                selectedButton = -1;
            }

            initialSelect = selectedButton;
        }

        /// <summary>
        /// A Menu object that consists of a List of Button objects which can be
        /// selected and accessed in a sequential order via specified keyboard
        /// controls.
        /// </summary>
        /// <param name="texture">The Texture of this ListMenu's background
        /// </param>
        /// <param name="menuLocation">The location of the ListMenu's base Menu
        /// object</param>
        /// <param name="menuColor">The Color of this ListMenu's background
        /// texture</param>
        /// <param name="buttons">Sequential list of user-interactable Buttons
        /// that, when activated, should execute a set of specified functions
        /// assigned to the Button</param>
        /// <param name="backKey">Keyboard key that should be used to navigate
        /// selection to the previous Button in the <paramref name="buttons"/>
        /// list<para>(Cannot be equal to <paramref name="nextKey"/> or
        /// <paramref name="confirmKey"/> unless value is Keys.None)</para>
        /// </param>
        /// <param name="nextKey">Keyboard key that should be used to navigate
        /// selection to the previous Button in the <paramref name="buttons"/>
        /// list<para>(Cannot be equal to <paramref name="backKey"/> or
        /// <paramref name="confirmKey"/> unless value is Keys.None)</para>
        /// </param>
        /// <param name="confirmKey">Keyboard key that should be used to Press
        /// the currently selected Button in the <paramref name="buttons"/> List
        /// <para>(Cannot be equal to <paramref name="backKey"/> or <paramref
        /// name="nextKey"/> unless value is Keys.None)</para></param>
        /// <param name="initSelect">When the ListMenu is first loaded, the
        /// index of this Button in the buttons List should be the default
        /// Selected Button</param>
        /// <param name="wrapSelection">When the first or last Buttons in this
        /// ListMenu's <paramref name="buttons"/> List are Selected, can
        /// <paramref name="backKey"/> or <paramref name="nextKey"/> be used to
        /// traverse to the opposite end of the <paramref name="buttons"/> List?
        /// </param>
        public ListMenu(Texture2D texture, Vector2 menuLocation,
            Color menuColor, List<Button> buttons, Keys backKey, Keys nextKey,
            Keys confirmKey, int initSelect, bool wrapSelection)
            : base(texture, menuLocation, menuColor)
        {
            menuButtons = new List<Button>();
            wrapping = wrapSelection;

            prevKey = backKey;
            this.nextKey = nextKey;
            pressKey = confirmKey;
            
            count = 0;

            if (buttons != null)
            {
                if (initSelect < -1 || initSelect >= buttons.Count)
                {
                    initSelect = -1;
                }
                selectedButton = initSelect;

                if((backKey != Keys.None && (backKey == nextKey
                    || backKey == confirmKey)) || (nextKey != Keys.None
                    && (nextKey == backKey || nextKey == confirmKey))
                    || (confirmKey != Keys.None && (confirmKey == backKey
                    || confirmKey == nextKey)))
                {
                    throw new ArgumentException(
                        "ListMenu navigation keys cannot be the same.");
                }

                foreach (Button b in buttons)
                {
                    if (b != null)
                    {
                        if (count == selectedButton - 1)
                        {
                            if (!b.Selected)
                            {
                                b.Selected = true;
                            }
                        }

                        else if (b.Selected)
                        {
                            b.Selected = false;
                        }

                        b.ButtonSelectionEvent += SelectSingleButton;

                        menuButtons.Add(b);
                        count++;
                    }
                }
            }

            else
            {
                selectedButton = -1;
            }

            initialSelect = selectedButton;
        }

        /// <summary>
        /// A Menu object that consists of a List of Button objects which can be
        /// selected and accessed in a sequential order via specified keyboard
        /// controls.
        /// </summary>
        /// <param name="headerFont">The font of the Header</param>
        /// <param name="headerText">The text of the Header</param>
        /// <param name="headerLocation">The location of the Header</param>
        /// <param name="headerColor">The color of the Header text</param>
        /// <param name="buttons">Sequential list of user-interactable Buttons
        /// that, when activated, should execute a set of specified functions
        /// assigned to the Button</param>
        /// <param name="backKey">Keyboard key that should be used to navigate
        /// selection to the previous Button in the <paramref name="buttons"/>
        /// list<para>(Cannot be equal to <paramref name="nextKey"/> or
        /// <paramref name="confirmKey"/> unless value is Keys.None)</para>
        /// </param>
        /// <param name="nextKey">Keyboard key that should be used to navigate
        /// selection to the previous Button in the <paramref name="buttons"/>
        /// list<para>(Cannot be equal to <paramref name="backKey"/> or
        /// <paramref name="confirmKey"/> unless value is Keys.None)</para>
        /// </param>
        /// <param name="confirmKey">Keyboard key that should be used to Press
        /// the currently selected Button in the <paramref name="buttons"/> List
        /// <para>(Cannot be equal to <paramref name="backKey"/> or <paramref
        /// name="nextKey"/> unless value is Keys.None)</para></param>
        /// <param name="initSelect">When the ListMenu is first loaded, the
        /// index of this Button in the buttons List should be the default
        /// Selected Button</param>
        /// <param name="wrapSelection">When the first or last Buttons in this
        /// ListMenu's <paramref name="buttons"/> List are Selected, can
        /// <paramref name="backKey"/> or <paramref name="nextKey"/> be used to
        /// traverse to the opposite end of the <paramref name="buttons"/> List?
        /// </param>
        public ListMenu(SpriteFont headerFont, string headerText,
            Vector2 headerLocation, Color headerColor, List<Button> buttons,
            Keys backKey, Keys nextKey, Keys confirmKey, int initSelect,
            bool wrapSelection)
            : base(headerFont, headerText, headerLocation, headerColor)
        {
            menuButtons = new List<Button>();
            wrapping = wrapSelection;

            prevKey = backKey;
            this.nextKey = nextKey;
            pressKey = confirmKey;
            
            count = 0;

            if (buttons != null)
            {
                if (initSelect < -1 || initSelect >= buttons.Count)
                {
                    initSelect = -1;
                }
                selectedButton = initSelect;

                if ((backKey != Keys.None && (backKey == nextKey
                    || backKey == confirmKey)) || (nextKey != Keys.None
                    && (nextKey == backKey || nextKey == confirmKey))
                    || (confirmKey != Keys.None && (confirmKey == backKey
                    || confirmKey == nextKey)))
                {
                    throw new ArgumentException(
                        "ListMenu navigation keys cannot be the same.");
                }

                foreach (Button b in buttons)
                {
                    if (b != null)
                    {
                        if (count == selectedButton - 1)
                        {
                            if (!b.Selected)
                            {
                                b.Selected = true;
                            }
                        }

                        else if (b.Selected)
                        {
                            b.Selected = false;
                        }

                        b.ButtonSelectionEvent += SelectSingleButton;

                        menuButtons.Add(b);
                        count++;
                    }
                }
            }

            else
            {
                selectedButton = -1;
            }

            initialSelect = selectedButton;
        }

        /// <summary>
        /// A Menu object that consists of a List of Button objects which can be
        /// selected and accessed in a sequential order via specified keyboard
        /// controls.
        /// </summary>
        /// <param name="headerFont">The font of the Header</param>
        /// <param name="headerText">The text of the Header</param>
        /// <param name="headerLocation">The location of the Header</param>
        /// <param name="headerColor">The color of the Header text</param>
        /// <param name="bodyFont">The font of the Body</param>
        /// <param name="bodyText">The text of the Body</param>
        /// <param name="bodyLocation">The location of the Body</param>
        /// <param name="bodyColor">The color of the Body text</param>
        /// <param name="buttons">Sequential list of user-interactable Buttons
        /// that, when activated, should execute a set of specified functions
        /// assigned to the Button</param>
        /// <param name="backKey">Keyboard key that should be used to navigate
        /// selection to the previous Button in the <paramref name="buttons"/>
        /// list<para>(Cannot be equal to <paramref name="nextKey"/> or
        /// <paramref name="confirmKey"/> unless value is Keys.None)</para>
        /// </param>
        /// <param name="nextKey">Keyboard key that should be used to navigate
        /// selection to the previous Button in the <paramref name="buttons"/>
        /// list<para>(Cannot be equal to <paramref name="backKey"/> or
        /// <paramref name="confirmKey"/> unless value is Keys.None)</para>
        /// </param>
        /// <param name="confirmKey">Keyboard key that should be used to Press
        /// the currently selected Button in the <paramref name="buttons"/> List
        /// <para>(Cannot be equal to <paramref name="backKey"/> or <paramref
        /// name="nextKey"/> unless value is Keys.None)</para></param>
        /// <param name="initSelect">When the ListMenu is first loaded, the
        /// index of this Button in the buttons List should be the default
        /// Selected Button</param>
        /// <param name="wrapSelection">When the first or last Buttons in this
        /// ListMenu's <paramref name="buttons"/> List are Selected, can
        /// <paramref name="backKey"/> or <paramref name="nextKey"/> be used to
        /// traverse to the opposite end of the <paramref name="buttons"/> List?
        /// </param>
        public ListMenu(SpriteFont headerFont, string headerText,
            Vector2 headerLocation, Color headerColor, SpriteFont bodyFont,
            string bodyText, Vector2 bodyLocation, Color bodyColor,
            List<Button> buttons, Keys backKey, Keys nextKey, Keys confirmKey,
            int initSelect, bool wrapSelection)
            : base(headerFont, headerText, headerLocation, headerColor,
                  bodyFont, bodyText, bodyLocation, bodyColor)
        {
            menuButtons = new List<Button>();
            wrapping = wrapSelection;

            prevKey = backKey;
            this.nextKey = nextKey;
            pressKey = confirmKey;
            
            count = 0;

            if (buttons != null)
            {
                if (initSelect < -1 || initSelect >= buttons.Count)
                {
                    initSelect = -1;
                }
                selectedButton = initSelect;

                if ((backKey != Keys.None && (backKey == nextKey
                    || backKey == confirmKey)) || (nextKey != Keys.None
                    && (nextKey == backKey || nextKey == confirmKey))
                    || (confirmKey != Keys.None && (confirmKey == backKey
                    || confirmKey == nextKey)))
                {
                    throw new ArgumentException(
                        "ListMenu navigation keys cannot be the same.");
                }

                foreach (Button b in buttons)
                {
                    if (b != null)
                    {
                        if (count == selectedButton - 1)
                        {
                            if (!b.Selected)
                            {
                                b.Selected = true;
                            }
                        }

                        else if (b.Selected)
                        {
                            b.Selected = false;
                        }

                        b.ButtonSelectionEvent += SelectSingleButton;

                        menuButtons.Add(b);
                        count++;
                    }
                }
            }

            else
            {
                selectedButton = -1;
            }

            initialSelect = selectedButton;
        }

        /// <summary>
        /// A Menu object that consists of a List of Button objects which can be
        /// selected and accessed in a sequential order via specified keyboard
        /// controls.
        /// </summary>
        /// <param name="texture">The Texture of this ListMenu's background
        /// </param>
        /// <param name="menuLocation">The location of the ListMenu's base Menu
        /// object</param>
        /// <param name="menuColor">The Color of this ListMenu's background
        /// texture</param>
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
        /// <param name="buttons">Sequential list of user-interactable Buttons
        /// that, when activated, should execute a set of specified functions
        /// assigned to the Button</param>
        /// <param name="backKey">Keyboard key that should be used to navigate
        /// selection to the previous Button in the <paramref name="buttons"/>
        /// list<para>(Cannot be equal to <paramref name="nextKey"/> or
        /// <paramref name="confirmKey"/> unless value is Keys.None)</para>
        /// </param>
        /// <param name="nextKey">Keyboard key that should be used to navigate
        /// selection to the previous Button in the <paramref name="buttons"/>
        /// list<para>(Cannot be equal to <paramref name="backKey"/> or
        /// <paramref name="confirmKey"/> unless value is Keys.None)</para>
        /// </param>
        /// <param name="confirmKey">Keyboard key that should be used to Press
        /// the currently selected Button in the <paramref name="buttons"/> List
        /// <para>(Cannot be equal to <paramref name="backKey"/> or <paramref
        /// name="nextKey"/> unless value is Keys.None)</para></param>
        /// <param name="initSelect">When the ListMenu is first loaded, the
        /// index of this Button in the buttons List should be the default
        /// Selected Button</param>
        /// <param name="wrapSelection">When the first or last Buttons in this
        /// ListMenu's <paramref name="buttons"/> List are Selected, can
        /// <paramref name="backKey"/> or <paramref name="nextKey"/> be used to
        /// traverse to the opposite end of the <paramref name="buttons"/> List?
        /// </param>
        public ListMenu(Texture2D texture, Vector2 menuLocation,
            Color menuColor, SpriteFont headerFont, string headerText,
            Vector2 headerLocation, Color headerColor, SpriteFont bodyFont,
            string bodyText, Vector2 bodyLocation, Color bodyColor,
            List<Button> buttons, Keys backKey, Keys nextKey, Keys confirmKey,
            int initSelect, bool wrapSelection)
            : base(texture, menuLocation, menuColor, headerFont, headerText,
                  headerLocation, headerColor, bodyFont, bodyText, bodyLocation,
                  bodyColor)
        {
            menuButtons = new List<Button>();
            wrapping = wrapSelection;

            prevKey = backKey;
            this.nextKey = nextKey;
            pressKey = confirmKey;

            count = 0;

            if (buttons != null)
            {
                if (initSelect < -1 || initSelect >= buttons.Count)
                {
                    initSelect = -1;
                }
                selectedButton = initSelect;

                if ((backKey != Keys.None && (backKey == nextKey
                    || backKey == confirmKey)) || (nextKey != Keys.None
                    && (nextKey == backKey || nextKey == confirmKey))
                    || (confirmKey != Keys.None && (confirmKey == backKey
                    || confirmKey == nextKey)))
                {
                    throw new ArgumentException(
                        "ListMenu navigation keys cannot be the same.");
                }

                foreach (Button b in buttons)
                {
                    if (b != null)
                    {
                        if (count == selectedButton - 1)
                        {
                            if (!b.Selected)
                            {
                                b.Selected = true;
                            }
                        }

                        else if (b.Selected)
                        {
                            b.Selected = false;
                        }

                        b.ButtonSelectionEvent += SelectSingleButton;

                        menuButtons.Add(b);
                        count++;
                    }
                }
            }

            else
            {
                selectedButton = -1;
            }

            initialSelect = selectedButton;
        }

        /// <summary>
        /// A Menu object that consists of a List of Button objects which can be
        /// selected and accessed in a sequential order via specified keyboard
        /// controls.
        /// </summary>
        /// <param name="texture">The Texture of this ListMenu's background
        /// </param>
        /// <param name="menuLocation">The location of the ListMenu's base Menu
        /// object</param>
        /// <param name="menuColor">The Color of this ListMenu's background
        /// texture</param>
        /// <param name="headerFont">The font of the Header</param>
        /// <param name="headerText">The text of the Header</param>
        /// <param name="headerLocation">The location of the Header RELATIVE to
        /// the location of the menu itself</param>
        /// <param name="headerColor">The color of the Header text</param>
        /// <param name="buttons">Sequential list of user-interactable Buttons
        /// that, when activated, should execute a set of specified functions
        /// assigned to the Button</param>
        /// <param name="backKey">Keyboard key that should be used to navigate
        /// selection to the previous Button in the <paramref name="buttons"/>
        /// list<para>(Cannot be equal to <paramref name="nextKey"/> or
        /// <paramref name="confirmKey"/> unless value is Keys.None)</para>
        /// </param>
        /// <param name="nextKey">Keyboard key that should be used to navigate
        /// selection to the previous Button in the <paramref name="buttons"/>
        /// list<para>(Cannot be equal to <paramref name="backKey"/> or
        /// <paramref name="confirmKey"/> unless value is Keys.None)</para>
        /// </param>
        /// <param name="confirmKey">Keyboard key that should be used to Press
        /// the currently selected Button in the <paramref name="buttons"/> List
        /// <para>(Cannot be equal to <paramref name="backKey"/> or <paramref
        /// name="nextKey"/> unless value is Keys.None)</para></param>
        /// <param name="initSelect">When the ListMenu is first loaded, the
        /// index of this Button in the buttons List should be the default
        /// Selected Button</param>
        /// <param name="wrapSelection">When the first or last Buttons in this
        /// ListMenu's <paramref name="buttons"/> List are Selected, can
        /// <paramref name="backKey"/> or <paramref name="nextKey"/> be used to
        /// traverse to the opposite end of the <paramref name="buttons"/> List?
        /// </param>
        public ListMenu(Texture2D texture, Vector2 menuLocation,
            Color menuColor, SpriteFont headerFont, string headerText,
            Vector2 headerLocation, Color headerColor, List<Button> buttons,
            Keys backKey, Keys nextKey, Keys confirmKey, int initSelect,
            bool wrapSelection)
            : base(texture, menuLocation, menuColor, headerFont, headerText,
                  headerLocation, headerColor)
        {
            menuButtons = new List<Button>();
            wrapping = wrapSelection;

            prevKey = backKey;
            this.nextKey = nextKey;
            pressKey = confirmKey;
            
            count = 0;

            if (buttons != null)
            {
                if (initSelect < -1 || initSelect >= buttons.Count)
                {
                    initSelect = -1;
                }
                selectedButton = initSelect;

                if ((backKey != Keys.None && (backKey == nextKey
                    || backKey == confirmKey)) || (nextKey != Keys.None
                    && (nextKey == backKey || nextKey == confirmKey))
                    || (confirmKey != Keys.None && (confirmKey == backKey
                    || confirmKey == nextKey)))
                {
                    throw new ArgumentException(
                        "ListMenu navigation keys cannot be the same.");
                }

                foreach (Button b in buttons)
                {
                    if (b != null)
                    {
                        if (count == selectedButton - 1)
                        {
                            if (!b.Selected)
                            {
                                b.Selected = true;
                            }
                        }

                        else if (b.Selected)
                        {
                            b.Selected = false;
                        }

                        b.ButtonSelectionEvent += SelectSingleButton;

                        menuButtons.Add(b);
                        count++;
                    }
                }
            }

            else
            {
                selectedButton = -1;
            }

            initialSelect = selectedButton;
        }


        // Methods
        /// <summary>
        /// Update the menu's buttons
        /// </summary>
        /// <param name="mouse">Mouse information</param>
        /// <param name="prevMouse">The previous mouse information</param>
        public void Update(MouseState mouse, MouseState prevMouse,
            KeyboardState kbState, KeyboardState prevKBState)
        {
            NavigationInput(kbState, prevKBState);
            ConfirmationPress(kbState, prevKBState);
            ConfirmationRelease(kbState, prevKBState);

            for (int i = 0; i < count; i++)
            {
                menuButtons[i].Update(mouse, prevMouse);
            }
            
        }

        /// <summary>
        /// Draw the menu & its buttons
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw menu with</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            
            // Draw the menu's buttons
            if (menuButtons != null)
            {
                foreach (Button b in menuButtons)
                {
                    b.Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Turn off all other Buttons except for one, and turn that one button
        /// on (if it exists within the menu).
        /// </summary>
        /// <param name="keepOn">Button to keep on</param>
        private void SelectSingleButton(Button keepOn)
        {
            if (keepOn == null)
            {
                selectedButton = -1;
            }

            for (int i = 0; i < Count; i++)
            {
                if (menuButtons[i] != keepOn)
                {
                    menuButtons[i].Selected = false;
                }
                else
                {
                    menuButtons[i].Selected = true;
                    selectedButton = i;
                }
            }
        }

        /// <summary>
        /// When the Previous or Next keys are pressed, change the selected
        /// button.
        /// </summary>
        /// <param name="kbState">The current keyboard state</param>
        /// <param name="prevKBState">The previous keyboard state</param>
        private void NavigationInput(KeyboardState kbState,
            KeyboardState prevKBState)
        {
            if (menuButtons != null && menuButtons.Count > 0)
            {
                // Navigate back a button
                if (kbState.IsKeyDown(prevKey) && prevKBState.IsKeyUp(prevKey)
                    && kbState.IsKeyUp(nextKey))
                {
                    // No button is currently selected
                    if (selectedButton == -1)
                    {
                        selectedButton = count - 1;
                        menuButtons[selectedButton].Selected = true;
                    }

                    // A button is currently selected
                    else
                    {
                        if (wrapping && selectedButton == 0)
                        {
                            selectedButton = count - 1;
                        }
                        else if (selectedButton > 0)
                        {
                            --selectedButton;
                        }
                    }
                }

                // Navigate back a button
                else if (kbState.IsKeyDown(nextKey)
                    && prevKBState.IsKeyUp(nextKey) && kbState.IsKeyUp(prevKey))
                {
                    // No button is currently selected
                    if (selectedButton == -1)
                    {
                        selectedButton = 0;
                        menuButtons[selectedButton].Selected = true;
                    }

                    // A button is currently selected
                    else
                    {
                        if (wrapping && selectedButton == count - 1)
                        {
                            selectedButton = 0;
                        }
                        else if (selectedButton < count - 1)
                        {
                            ++selectedButton;
                        }
                    }
                }
                
                if (selectedButton != -1)
                {
                    menuButtons[selectedButton].Selected = true;
                }
            }
        }

        /// <summary>
        /// Test to see if this menu's Confirmation key has been pressed this
        /// frame.
        /// </summary>
        /// <param name="kbState">The current keyboard state</param>
        /// <param name="prevKBState">The previous keyboard state</param>
        private void ConfirmationPress(KeyboardState kbState,
            KeyboardState prevKBState)
        {
            if (selectedButton != -1 && kbState.IsKeyDown(pressKey)
                && prevKBState.IsKeyUp(pressKey)
                && menuButtons[selectedButton].Selected)
            {
                menuButtons[selectedButton].Pressed = true;
            }
        }

        /// <summary>
        /// Test to see if this menu's Confirmation key has been released this
        /// frame.
        /// </summary>
        /// <param name="kbState">The current keyboard state</param>
        /// <param name="prevKBState">The previous keyboard state</param>
        private void ConfirmationRelease(KeyboardState kbState,
            KeyboardState prevKBState)
        {
            if (selectedButton != -1 && kbState.IsKeyUp(pressKey)
                && prevKBState.IsKeyDown(pressKey)
                && menuButtons[selectedButton].Selected)
            {
                menuButtons[selectedButton].Pressed = false;
            }
        }

        /// <summary>
        /// Reset this ListMenu to its default, initialized settings.
        /// </summary>
        public void Reset()
        {
            if (initialSelect != -1)
            {
                SelectSingleButton(menuButtons[initialSelect]);
                //menuButtons[initialSelect].Selected = true;
            }

            else
            {
                SelectSingleButton(null);
            }
        }

        public void Remove(int index)
        {
            menuButtons.RemoveAt(index);
            count--;
        }

    }
}