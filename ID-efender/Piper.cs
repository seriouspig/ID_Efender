using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ID_efender
{   // This is the class for the animated piper. He won´t be moving, but can be lifted up by the enemies
    class Piper
    {
        // Class Variables
        public Vector2 m_position;
        private Texture2D m_txr;
        private Texture2D m_txrMap;
        public Rectangle m_rect;
        public Rectangle m_rectMap;        
        public Vector2 mapPosition;

        public Rectangle m_animCell;

        private float m_frameTimer;
        private float m_fps;

        // Constructor        
        public Piper(Texture2D txr, Texture2D txrMap, int xpos, int fps)
        {
            // Copy the texture "txr" into the "m_txr" class variable , copy the texture "txrMap" into the "m_txrMap" class variable
            m_txr = txr;
            m_txrMap = txrMap;

            // Set the initial X position to xpos, set the intial Y position to 494
            m_position = new Vector2(xpos, 494);
            m_rect = new Rectangle((int)m_position.X, (int)m_position.Y, txr.Width / 2, txr.Height / 2);
            m_rectMap = new Rectangle((int)mapPosition.X, (int)mapPosition.Y, txrMap.Width, txrMap.Height);

            // set the animation cell Width to be devided by the number of frames (2),
            // also the piper has 2 different animations, each one in the seperate row on the spritesheet, 
            // so the animacion Cell Height needs to be divided by 2 as well
            m_animCell = new Rectangle(0, 0, m_txr.Width / 2, m_txr.Height / 2);

            m_frameTimer = 1;
            m_fps = fps;
        }

        public void updateme(int scrollSpeed, int originX, int originY)
        {
            // modify the the X position of the piper by the speed of the background scrolling   
            // update the collision rectangle to the m_position
            m_position.X += scrollSpeed;
            m_rect.Location = m_position.ToPoint();

            // Update the point on the map to reflect the position of the piper (math convertion of coordinates in relation to the origin point)
            mapPosition.X = 250 + (m_position.X - originX) / 10;
            mapPosition.Y = 8 + (m_position.Y - originY) / 10;
            m_rectMap.Location = mapPosition.ToPoint();

            // if the piper is above ground apply gravity
            if (m_position.Y < 494)
            {
                m_position.Y += 3;               
            }
        }

        // draw the main piper
        public void drawMe(SpriteBatch sb, GameTime gt)
        {
            // if the piper is above ground set the animation Cell to the second row (32 pixels) in the sprite sheed, 
            // otherwise play the first row of animation
            if (m_rect.Y < 493)
               m_animCell.Y = 32;
            else m_animCell.Y = 0;

            // move the animation cell by it´s width every time the timer reaches 0 and reset the timer to 10
            if (m_frameTimer <= 0)
            {
                m_animCell.X = (m_animCell.X + m_animCell.Width);
                if (m_animCell.X >= m_txr.Width)
                    m_animCell.X = 0;                              

                m_frameTimer = 10;
            }
            else
            {
                m_frameTimer -= (float)gt.ElapsedGameTime.TotalSeconds * m_fps;
            }

           
            sb.Draw(m_txr, m_position, m_animCell, Color.White);          
        }

        // draw the position of the walker on the map
        public void drawMeMap(SpriteBatch sb)
        {
            // use the spritebatch "sb" to draw the sprite at rectangle's "m_rectMap" coordinates using the texture "m_txrMap" with a white tint
            sb.Draw(m_txrMap, m_rectMap, Color.White);
        }
    }
}



