using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ID_efender
{   // This is the class for the animated people walking at the bottom of the screen
    class Walker
    {
        // Class Variables
        public Vector2 m_position;
        private Texture2D m_txr;
        private Texture2D m_txrMap;
        public Rectangle m_rect;
        public Rectangle m_rectMap;
        public int walkerSpeed;
        public Vector2 mapPosition;

        public Rectangle m_animCell;

        private float m_frameTimer;
        private float m_fps;

        // Constructor
        public Walker(Texture2D txr, Texture2D txrMap, Random RNG, int minX, int maxX, int minSpeed, int maxSpeed, int frameCount, int fps, int topSprite, int bottomSprite )
        {
            // Copy the texture "txr" into the "m_txr" class variable , copy the texture "txrMap" into the "m_txrMap" class variable
            m_txr = txr;
            m_txrMap = txrMap;           

            // Set the initial X position to random between minX and maxX, set the intial Y position to 500
            m_position = new Vector2(RNG.Next(minX, maxX), 500);
            
            m_rect = new Rectangle((int)m_position.X, (int)m_position.Y, txr.Width, txr.Height);
            m_rectMap = new Rectangle((int)mapPosition.X, (int)mapPosition.Y, txrMap.Width, txrMap.Height);

            // Set the intial horizontal speed to random between minSpeed and maxSpeed
            walkerSpeed = (RNG.Next(minSpeed, maxSpeed));

            // set the animation cell to randomly choose one of 3 rows from the spritesheet (each row is 23 pixels high), 
            // also set the animation cell Width to be devided by the number of frames
            m_animCell = new Rectangle(0, 23 * (int) RNG.Next(topSprite, bottomSprite), m_txr.Width / frameCount, m_txr.Height / 3);

            m_frameTimer = 2;
            m_fps = fps;

            // if the random initial horizontal speed picked is 0, then set it to maxSpeed, we don´t want to have any people just standing
            if (walkerSpeed == 0)
                walkerSpeed = maxSpeed;            
        }

        public void updateme(int scrollSpeed, int originX, int originY)
        {
            // modify the the X position of the walker by the speed of the walker and also by the speed of the background scrolling   
            // update the collision rectangle to the m_position
            m_position.X += walkerSpeed + scrollSpeed;         
            m_rect.Location = m_position.ToPoint();

            // Update the point on the map to reflect the position of the walker (math convertion of coordinates in relation to the origin point)
            mapPosition.X = 250 + (m_position.X - originX) / 10;
            mapPosition.Y = 8 + (m_position.Y - originY) / 10;
            m_rectMap.Location = mapPosition.ToPoint();
            
            // if the walker is above ground apply gravity and also stop him moving horizontally
            if (m_position.Y < 500)
            {
                m_position.Y += 3;
                walkerSpeed = 0;
            }                             
        }

        // draw the main walker
        public void drawMe(SpriteBatch sb, GameTime gt)
        {
            // move the animation cell by it´s width every time the timer reaches 0 and reset the timer to 2
            if (m_frameTimer <= 0)
            {
                m_animCell.X = (m_animCell.X + m_animCell.Width);
                if (m_animCell.X >= m_txr.Width)
                    m_animCell.X = 0;

                m_frameTimer = 2;
            }
            else
            {
                m_frameTimer -= (float)gt.ElapsedGameTime.TotalSeconds * m_fps;
            }

            // if walker is moving right, face the sprit right, if he is moving left, flip the sprite horizontally
            if (walkerSpeed > 0)
                sb.Draw(m_txr, m_position, m_animCell,  Color.White);
            else 
                sb.Draw(m_txr, m_position, m_animCell, Color.White, 0, Vector2.Zero, 1,  SpriteEffects.FlipHorizontally, 0);
        }

        // draw the position of the walker on the map
        public void drawMeMap(SpriteBatch sb)
        {
            // use the spritebatch "sb" to draw the sprite at rectangle's "m_rectMap" coordinates using the texture "m_txrMap" with a white tint
            sb.Draw(m_txrMap, m_rectMap, Color.White);
        }
    }
}



