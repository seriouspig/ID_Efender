using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ID_efender
{   // This is the class for the RED ENEMY hovering directly above the piper and randomly trying to snatch him
    class EnemyPiper
    {
        // Class Variables
        Texture2D m_txr;

        public Rectangle rect;

        public Vector2 m_position;
        public Vector2 m_velocity;

        private Texture2D m_txrMap;
        public Rectangle m_rectMap;
        public Vector2 mapPosition;

        public Rectangle m_animCell;

        // Constructor
        public EnemyPiper(Texture2D txr, Texture2D txrMap, Random RNG,int piperX)
        {
            m_txr = txr;
            m_txrMap = txrMap;

            // Set the initial X position to be aligned with the position of the piper
            m_position = new Vector2(piperX, 0);

            // Set the rectangle to the txr width divided by 7 (as there is 7 frames in the animation) and txr height
            rect = new Rectangle((int)m_position.X, (int)m_position.Y, txr.Width / 7, txr.Height);

            // Set the rectangle for the position of the Enemy on the map
            m_rectMap = new Rectangle((int)mapPosition.X, (int)mapPosition.Y, txrMap.Width, txrMap.Height);

            // Set a random initial Vertical velocity
            m_velocity = new Vector2(0, (float)RNG.NextDouble() * 2 + 0.5f);

            // Set the animation Cell rectangle to the txr width divided by 7 (as there is 7 frames in the animation) and txr height
            m_animCell = new Rectangle(0, 0, m_txr.Width / 7, m_txr.Height);
        }

        public void updateme(Random RNG, int maxY, float horSpeed, int originX, int originY)
        {
            // modify the Y position by the vertical velocity
            // modify the X position by the horizontal speed
            // set collision rectangle to m_position coordinates
            m_position.Y = m_position.Y + m_velocity.Y;
            m_position.X += horSpeed;
            rect.X = (int)m_position.X;
            rect.Y = (int)m_position.Y;

            // Make the spaceship stop once it reaches the landing position (maxY)
            if (m_position.Y > maxY)
            {
                m_velocity.Y = 0;
            }

            // Update the point on the map to reflect the position of the spaceship (math convertion of coordinates in relation to the origin point)
            mapPosition.X = 250 + (m_position.X - originX) / 10;
            mapPosition.Y = 8 + (m_position.Y - originY) / 10;
            m_rectMap.Location = mapPosition.ToPoint();

            // Enemy Animation - change the animation frame of the enemy depending on the Y position on the screen            
            if (m_position.Y < 260)
                m_animCell.X = 0;
            if ((m_position.Y > 260) && (m_position.Y <= 280))
                m_animCell.X = 160;
            if ((m_position.Y > 280) && (m_position.Y <= 300))
                m_animCell.X = 320;
            if ((m_position.Y > 300) && (m_position.Y <= 320))
                m_animCell.X = 480;
            if ((m_position.Y > 320) && (m_position.Y <= 340))
                m_animCell.X = 640;
            if ((m_position.Y > 340) && (m_position.Y <= 360))
                m_animCell.X = 800;
            if (m_position.Y > 360)
                m_animCell.X = 960;
        }

        // draw the main spaceship
        public void drawme(SpriteBatch sb)
        {
            // use the spritebatch "sb" to draw the sprite at "m_position" using the "m_animCell" frame of texture "m_txr" with a white tint
            sb.Draw(m_txr, m_position, m_animCell, Color.White);
        }

        // draw the position of the spaceship on the map
        public void drawMeMap(SpriteBatch sb)
        {
            // use the spritebatch "sb" to draw the sprite at rectangle's "m_rectMap" coordinates using the texture "m_txrMap" with a white tint
            sb.Draw(m_txrMap, m_rectMap, Color.White);
        }
    }
}