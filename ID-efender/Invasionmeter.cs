using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ID_efender
{   // This is the class for invasionmeter bar reflecting the percentage of the invasion 
    class Invasionmeter
    {   
        //Class variables
        public Texture2D m_txr;
        public Vector2 m_position;
        public Rectangle m_animCell;

        //Constructor
        public Invasionmeter(Texture2D txr, int xpos, int ypos)
        {
            // Copy the texture "txr" into the "m_txr" class variable
            // Copy the coordinates "xpos" and "ypos" into the "m_position" class variable
            m_txr = txr;
            m_animCell = new Rectangle(0, 0, txr.Width, txr.Height);
            m_position = new Vector2(xpos, ypos);            
        }

        public void updateme(int invasionmeter)
        {
            //Set the width of the animation cell to the percentage of invasionmeter
            m_animCell.Width = m_txr.Width * invasionmeter/100;
        }
        public void Drawme(SpriteBatch sb)
        {
            // use the spritebatch "sb" to draw the sprite at "m_position" using the "m_animCell" frame of the texture "m_txr" with a white tint
            sb.Draw(m_txr, m_position, m_animCell, Color.White);
        }

    }
}