using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ID_efender
{   // This is the class for the GAME OVER sign that will be displayed once the invasionmeter reaches 100%
    class GameOverSign
    {
        //Class variables
        public Texture2D m_txr;
        public Vector2 m_position;              
         
        // Constructor
        public GameOverSign (Texture2D txr, int xpos, int ypos)
        {
            m_txr = txr;
            m_position = new Vector2(xpos, ypos);                     
        }
        
        // Just need to draw it on screen, no update needed
        public void Drawme(SpriteBatch sb)
        {                                   
            sb.Draw(m_txr, m_position, Color.White);
        }
    }
}