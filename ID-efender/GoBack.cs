using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ID_efender
{   // This is the class for the GO BACK sign that will be displayed flashing when the plane reaches the boarder of the scene
    class GoBack
    {   
        //Class variables
        public Texture2D m_SpriteSheet;
        public Vector2 m_position;

        public Rectangle m_animCell;

        private float m_frameTimer;
        private float m_fps;

        public Rectangle CollisionRect;

        // Constructor
        public GoBack(Texture2D spriteSheet, int xpos, int ypos, int frameCount, int fps)
        {
            // Copy the texture "spriteSheet" into the "m_SpriteSheet" class variable  
            m_SpriteSheet = spriteSheet;

            // Set the animation Cell rectangle to the txr width divided by 2 (number of animation frames)
            m_animCell = new Rectangle(0, 0, spriteSheet.Width / frameCount, spriteSheet.Height / 2);

            // Copy the coordinates "xpos" and "ypos" into the "m_position" class variable
            m_position = new Vector2(xpos, ypos);

            m_frameTimer = 2;
            m_fps = fps;

            // Set the collision rectangle to the txr width divided by the number of animation frames
            CollisionRect = new Rectangle(xpos, ypos, spriteSheet.Width / frameCount, spriteSheet.Height);
        }

        public void Drawme(SpriteBatch sb, GameTime gt, int position)
        {
            // move the animation cell by it´s width every time the timer reaches 0 and reset the timer to 2
            if (m_frameTimer <= 0)
            {
                m_animCell.X = (m_animCell.X + m_animCell.Width);
                if (m_animCell.X >= m_SpriteSheet.Width)
                    m_animCell.X = 0;

                m_frameTimer = 2;
            }
            else
            {
                m_frameTimer -= (float)gt.ElapsedGameTime.TotalSeconds * m_fps;
            }

            m_animCell.Y = position;

            // use the spritebatch "sb" to draw the sprite at "m_position" using the "m_animCell" frame of the sprite sheet "m_SpriteSheet" with a white tint
            sb.Draw(m_SpriteSheet, m_position, m_animCell, Color.White);
        }
    }
}
