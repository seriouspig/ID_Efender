using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ID_efender
{   // This is the class for the main player controlled object - the plane
    // It can be controlled by d-pad or keyboard in 8 directions, also has animation depending on his position on the screen
    class spaceship
    {
        // Class Variables
        public Vector2 m_position;
        private Texture2D m_txr;
        private Rectangle m_rect;
        public Rectangle m_animCell;

        private Texture2D m_txrMap;
        public Rectangle m_rectMap;
        public Vector2 mapPosition;

        float velUp, velDown, velRight, velLeft;

        public float horSpeed;
        public float turnSpeed;
        public float shipStopper;

        // Constructor
        public spaceship (Texture2D txr, Texture2D txrMap, int xpos, int ypos, int framecount, float slowdown)
        {
            // Copy the coordinates "xpos" and "ypos" into the "m_position" class variable
            // Copy the texture "txr" into the "m_txr" class variable , copy the texture "txrMap" into the "m_txrMap" class variable
            m_position = new Vector2(xpos, ypos);
            m_txr = txr;
            m_txrMap = txrMap;

            // Set the collision rectange to m_position coordinates and give it txr width and height            
            m_rect = new Rectangle((int)m_position.X, (int)m_position.Y, txr.Width, txr.Height);
            m_rectMap = new Rectangle((int)mapPosition.X, (int)mapPosition.Y, txrMap.Width, txrMap.Height);

            // set the animation cell Width to be devided by the number of frames
            m_animCell = new Rectangle(0, 0, m_txr.Width / framecount, m_txr.Height);

            // set slowdown fot implementing inertia
            shipStopper = slowdown;
        }

        public void updateMe(KeyboardState kb, GamePadState currPad, GamePadState oldPad, int originX, int originY)
        {   
            // Create the vertical movement of the plane
            // It will be a combination of force moving plane up and force moving plane down
            // this way if both up and down keys are pressed at the same time, the movement will be 0
            m_position.Y += (velUp + velDown);
                        
            {
                if ((kb.IsKeyDown(Keys.Down)) || (currPad.DPad.Down == ButtonState.Pressed))
                {
                    if (m_position.Y < 460)     // Set the lower boundry for vertical movement
                    {
                        velDown += 0.5f;        // Gradually increase down speed
                        if (velDown > 8)        // Set the maximum down speed to 8
                        {
                            velDown = 8;
                        }
                    }
                    else velDown = 0;
                }
                else velDown -= 0.1f;           // If Down button is not pressed reduce the down speed gradually

                if ((kb.IsKeyDown(Keys.Up) || (currPad.DPad.Up == ButtonState.Pressed)))
                {
                    if (m_position.Y > 71)      // Set the upper boundry for vertical movement
                    {
                        velUp -= 0.5f;          // Gradually increase the up speed
                        if (velUp < -8)         // Set the maximum up speed to 8
                        {
                            velUp = -8;
                        }
                    }
                    else velUp = 0;
                }
                else velUp += 0.1f;             // If Up button is not pressed reduce the Up speed gradually

                if (velUp > 0)                  // Don´t let the up speed get over 0
                    velUp = 0;

                if (velDown < 0)                // Don´t let the down speed get under 0
                    velDown = 0;

                
                // Constraining the vertical movement to the screen
                if (m_position.Y < 71)
                    m_position.Y = 71;

                if (m_position.Y > 460)
                    m_position.Y = 460;

                // HORIZONTAL SPEED FOR BACKGROUND SCROLLING
                // Create the horizontal movement of the plane
                // It will be a combination of force moving plane right and force moving plane left
                // this way if both right and left keys are pressed at the same time, the movement will be 0
                // The plane itself will not move horizontally (only when turning), 
                // but the horizontal speed value will be used to move the backgrounds and all other objects
                horSpeed = (velRight + velLeft) - turnSpeed;

                if ((kb.IsKeyDown(Keys.Right)) || (currPad.DPad.Right == ButtonState.Pressed))
                {
                    velRight += 1f;             // Gradually increase right direction speed until it reaches 12
                    if (velRight > 12)
                    {
                        velRight = 12;
                    }
                }
                else velRight -= 0.2f;          // if Right key is not pressed, gradually reduce right direction speed

                if ((kb.IsKeyDown(Keys.Left)) || (currPad.DPad.Left == ButtonState.Pressed))
                {
                    velLeft -= 1f;              // Gradually increase left direction speed until it reaches 12
                    if (velLeft < -12)
                    {
                        velLeft = -12;
                    }
                }
                else velLeft += 0.2f;           // if Left key is not pressed, gradually reduce left direction speed

                if (velRight < 0)               // Don´t let the right speed get under 0
                    velRight = 0;

                if (velLeft > 0)                // Don´t let the left speed get over 0
                    velLeft = 0;

                m_position.X += turnSpeed;      // modify the X position of the plane by the turning speed
            }

            
            // horizontal position of the plane will only change once it changes direction of movement
            // it will add the turnspeed to the X position, but only within the constraints of 250 and 610 pixels           
            if ((kb.IsKeyDown(Keys.Left)) || (currPad.DPad.Left == ButtonState.Pressed))
            {
                turnSpeed = 16;             // move the plane to the right until it reaches the point of 610
                if (m_position.X > 610)
                {
                    turnSpeed = 0;
                }
            }            

            if ((kb.IsKeyDown(Keys.Right)) || (currPad.DPad.Right == ButtonState.Pressed))
            {
                turnSpeed = -16;            // move the plane to the left until it reaches the point of 250
                if (m_position.X < 250)
                {
                    turnSpeed = 0;
                }
            }
            
            // prevent the plane from flying away - had to add those lines, otherwise the flyaway bug was happening
            if (m_position.X > 610 && kb.IsKeyUp(Keys.Right) && (currPad.DPad.Right == ButtonState.Released))
                turnSpeed = 0;
            
            if (m_position.X < 250 && kb.IsKeyUp(Keys.Left) && (currPad.DPad.Left == ButtonState.Released))
                turnSpeed = 0;                       

            // set the plane turning animation by playing the right animation frame, depending on the horizontal position of the plane
            if (m_position.X > 249 && m_position.X < 301)
                m_animCell.X = 0;
            if (m_position.X >= 301 && m_position.X < 353)
                m_animCell.X = 100;
            if (m_position.X >= 353 && m_position.X < 405)
                m_animCell.X = 200;
            if (m_position.X >= 405 && m_position.X < 457)
                m_animCell.X = 300;
            if (m_position.X >= 457 && m_position.X < 509)
                m_animCell.X = 400;
            if (m_position.X >= 509 && m_position.X < 561)
                m_animCell.X = 500;
            if (m_position.X >= 561 && m_position.X < 610)
                m_animCell.X = 600;

            // Update the point on the map to reflect the position of the plane (math convertion of coordinates in relation to the origin point)
            mapPosition.X = 250 + (m_position.X - originX) / 10;
            mapPosition.Y = 8 + (m_position.Y - originY) / 10;
            m_rectMap.Location = mapPosition.ToPoint();            
        }

        // draw the main plane
        public void drawMe(SpriteBatch sb)
        {
            sb.Draw(m_txr, m_position, m_animCell, Color.White);
        }

        // draw the position of the plane on the map
        public void drawMeMap(SpriteBatch sb)
        {
            // use the spritebatch "sb" to draw the sprite at rectangle's "m_rectMap" coordinates using the texture "m_txrMap" with a white tint
            sb.Draw(m_txrMap, m_rectMap, Color.White);
        }

    }
}
    
