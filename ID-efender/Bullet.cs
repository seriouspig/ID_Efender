using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ID_efender
{   // This is the class for the bullet created at the plane position once the player presses the fire button
    class Bullet
    {
        // Class Variables
        public Vector2 m_position;
        private Texture2D m_txr;
        public Rectangle m_rect;        
        private Vector2 m_velocity;        
        public float horSpeed;        
        int speed;

        // Constructor
        public Bullet (Texture2D txr, int xpos, int ypos, int bulletSpeed)
        {
            // Copy the coordinates "xpos" and "ypos" into the "m_position" class variable
            // Copy the texture "txr" into the "m_txr" class variable
            // Assign bulletspeed to "speed" class variable            
            m_position = new Vector2(xpos, ypos);
            m_txr = txr;
            speed = bulletSpeed;
            m_rect = new Rectangle((int)m_position.X, (int)m_position.Y, txr.Width, txr.Height);
        }

        public void updateme ()
        {
            // modify the the X position by the bullet speed value
            // update the collision rectangle to the m_position
            m_position.X += speed;
            m_rect.Location = m_position.ToPoint();
        }

        public void drawMe(SpriteBatch sb)
        {
            // use the spritebatch "sb" to draw the sprite at "m_position" using the texture "m_txr" with a white tint
            sb.Draw(m_txr, m_position, Color.White);
        }
    }
}
