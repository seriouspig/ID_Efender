using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ID_efender
{   // This is the class for all the background elements. Each one of them will have a different horizontal speed to create the parallax effect
    class Background
    {
        //Class variables
        public Vector2 m_position;
        private Texture2D m_txr;
                   
        //Constructor
        public Background (Texture2D txr, int xpos, int ypos)
        {
            // Copy the texture "txr" into the "m_txr" class variable
            // Copy the coordinates "xpos" and "ypos" into the "m_position" class variable
            m_position = new Vector2(xpos, ypos);
            m_txr = txr;            
        }

        public void updateMe(float backgroundSpeed) 
        {
            // modify the the X position by the specified background speed value
            m_position.X -= backgroundSpeed;            
        }    
       
        public void drawMe (SpriteBatch sb)
        {
            // use the spritebatch "sb" to draw the sprite at "m_position" using the texture "m_txr" with a white tint
            sb.Draw(m_txr, m_position, Color.White);
        }

    }
}
