using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ID_efender
{   // This is the class for the text input box for entering the name into the highscore table
    class InputBox
    {
        // Class Variables
        private Texture2D background;
        private SpriteFont displayFont;
        private string text;
        private Boolean tracking;

        private Keys[] oldPressedKeys, currPressedKeys;

        /// <summary>
        /// Input box constructor
        /// </summary>
        /// <param name="bg">The background image to be used</param>
        /// <param name="disp">The font to be used in the textbox</param>
        public InputBox(Texture2D bg, SpriteFont disp)
        {
            // Note: This does not stop the font overflowing from the textbox!
            background = bg;
            displayFont = disp;
            text = "";
            tracking = false;
        }

        /// <summary>
        /// Turn off and on the checking of the keyboard (starts off)
        /// </summary>
        /// <param name="t">Set the mode to true or false</param>
        public void TrackString(Boolean t)
        {
            tracking = t;
        }

        /// <summary>
        /// Whether the keyboard is currently being checked
        /// </summary>
        /// <returns>true if the keyboard is being checked</returns>
        public Boolean AmTracking()
        {
            return tracking;
        }

        /// <summary>
        /// Checks the keyboard and updates the string if the tracking is turned on
        /// </summary>
        /// <returns>the current string being built</returns>
        /// 
        public void clearText ()
        {
             if (text.Length > 0)
                    {
                        text = text.Remove(text.Length - 1, 1);
                    }
        }
        public string UpdateMe()
        {
            // Get the current list of pressed keys
            currPressedKeys = Keyboard.GetState().GetPressedKeys();

            // Stop doing stuff if we are not currently supposed to be tracking the keyboard
            if (!tracking)
                return text;

            //check if any of the previous update's keys are no longer pressed
            if (oldPressedKeys != null)
                foreach (Keys key in oldPressedKeys)
                {
                    if (!currPressedKeys.Contains(key))
                        ProcessKey(key);
                }

            oldPressedKeys = currPressedKeys;
            return text;
        }

        /// <summary>
        /// add the key pressed to the string if it is a valid one
        /// </summary>
        /// <param name="key">The key that has been pressed</param>
        private void ProcessKey(Keys key)
        {
            // I have added a maximum of 18 characters to be entered         

                switch (key)
                {
                    case Keys.Space:
                        if (text.Length < 18)
                        text += " ";
                        break;
                    case Keys.A:
                        if (text.Length < 18)
                        text += "A";
                        break;
                    case Keys.B:
                        if (text.Length < 18)
                        text += "B";
                        break;
                    case Keys.C:
                        if (text.Length < 18)
                        text += "C";
                        break;
                    case Keys.D:
                        if (text.Length < 18)
                        text += "D";
                        break;
                    case Keys.E:
                        if (text.Length < 18)
                        text += "E";
                        break;
                    case Keys.F:
                        if (text.Length < 18)
                        text += "F";
                        break;
                    case Keys.G:
                        if (text.Length < 18)
                        text += "G";
                        break;
                    case Keys.H:
                        if (text.Length < 18)
                        text += "H";
                        break;
                    case Keys.I:
                        if (text.Length < 18)
                        text += "I";
                        break;
                    case Keys.J:
                        if (text.Length < 18)
                        text += "J";
                        break;
                    case Keys.K:
                        if (text.Length < 18)
                        text += "K";
                        break;
                    case Keys.L:
                        if (text.Length < 18)
                        text += "L";
                        break;
                    case Keys.M:
                        if (text.Length < 18)
                        text += "M";
                        break;
                    case Keys.N:
                        if (text.Length < 18)
                        text += "N";
                        break;
                    case Keys.O:
                        if (text.Length < 18)
                        text += "O";
                        break;
                    case Keys.P:
                        if (text.Length < 18)
                        text += "P";
                        break;
                    case Keys.Q:
                        if (text.Length < 18)
                        text += "Q";
                        break;
                    case Keys.R:
                        if (text.Length < 18)
                        text += "R";
                        break;
                    case Keys.S:
                        if (text.Length < 18)
                        text += "S";
                        break;
                    case Keys.T:
                        if (text.Length < 18)
                        text += "T";
                        break;
                    case Keys.U:
                        if (text.Length < 18)
                        text += "U";
                        break;
                    case Keys.V:
                        if (text.Length < 18)
                        text += "V";
                        break;
                    case Keys.W:
                        if (text.Length < 18)
                        text += "W";
                        break;
                    case Keys.X:
                        if (text.Length < 18)
                        text += "X";
                        break;
                    case Keys.Y:
                        if (text.Length < 18)
                        text += "Y";
                        break;
                    case Keys.Z:
                        if (text.Length < 18)
                        text += "Z";
                        break;
                    case Keys.D0:
                        if (text.Length < 18)
                        text += "0";
                        break;
                    case Keys.D1:
                        if (text.Length < 18)
                        text += "1";
                        break;
                    case Keys.D2:
                        if (text.Length < 18)
                        text += "2";
                        break;
                    case Keys.D3:
                        if (text.Length < 18)
                        text += "3";
                        break;
                    case Keys.D4:
                        if (text.Length < 18)
                        text += "4";
                        break;
                    case Keys.D5:
                        if (text.Length < 18)
                        text += "5";
                        break;
                    case Keys.D6:
                        if (text.Length < 18)
                        text += "6";
                        break;
                    case Keys.D7:
                        if (text.Length < 18)
                        text += "7";
                        break;
                    case Keys.D8:
                        if (text.Length < 18)
                        text += "8";
                        break;
                    case Keys.D9:
                        if (text.Length < 18)
                        text += "9";
                        break;
                    case Keys.NumPad0:
                        if (text.Length < 18)
                        text += "0";
                        break;
                    case Keys.NumPad1:
                        if (text.Length < 18)
                        text += "1";
                        break;
                    case Keys.NumPad2:
                        if (text.Length < 18)
                        text += "2";
                        break;
                    case Keys.NumPad3:
                        if (text.Length < 18)
                        text += "3";
                        break;
                    case Keys.NumPad4:
                        if (text.Length < 18)
                        text += "4";
                        break;
                    case Keys.NumPad5:
                        if (text.Length < 18)
                        text += "5";
                        break;
                    case Keys.NumPad6:
                        if (text.Length < 18)
                        text += "6";
                        break;
                    case Keys.NumPad7:
                        if (text.Length < 18)
                        text += "7";
                        break;
                    case Keys.NumPad8:
                        if (text.Length < 18)
                        text += "8";
                        break;
                    case Keys.NumPad9:
                        if (text.Length < 18)
                        text += "9";
                        break;
                    case Keys.OemPeriod:
                        if (text.Length < 18)
                        text += ".";
                        break;

                    // Also allow the backspace to be used to delete the last character in the string
                    case Keys.Back:
                        if (text.Length > 0)
                        {
                            text = text.Remove(text.Length - 1, 1);
                        }
                        break;                
                }
        }

        /// <summary>
        /// Draw the textbox at the given location
        /// </summary>
        /// <param name="sb">The spritebatch currently being used</param>
        /// <param name="pos">The coordinates to display the textbox</param>
        public void DrawMe(SpriteBatch sb, Vector2 pos)
        {
            // If it's tracking, draw it, if not, still draw it but greyed out
            if (tracking)
            {
                sb.Draw(background, pos, Color.White);
                sb.DrawString(displayFont, text, pos + new Vector2(8, 6), Color.Black);
            }
            else
            {
                sb.Draw(background, pos, Color.LightGray);
                sb.DrawString(displayFont, text, pos + new Vector2(8, 6), Color.DarkGray);
            }

        }
    }
}