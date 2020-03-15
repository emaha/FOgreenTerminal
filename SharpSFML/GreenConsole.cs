using System;
using System.Collections.Generic;
using System.Threading;
using SFML.Graphics;
using SFML.System;

namespace SharpSFML
{
    class GreenConsole
    {
        private Text sfText = new Text();
        private Font font = new Font("Terminus.ttf");
        private RectangleShape shape = new RectangleShape();
        
        private readonly Vector2f charSize = new Vector2f(15,35);
        private List<String> stringList = new List<string>();

        public GreenConsole()
        {
            sfText.Font = font;
        }

        public void PrintLine(string t)
        {
            stringList.Add(t);
        }



        public void DrawChar(RenderTarget t, char chr, Vector2f pos, bool isInvert)
        {
            string s = new string(chr,1);
            sfText.DisplayedString = s;
            sfText.Position = pos;
            if (isInvert)
            {
                sfText.Color = Color.Black;
                shape.Position = pos;
                shape.Size = charSize;
                shape.FillColor = Color.Green;
                t.Draw(shape);
            }
            else
            {
                sfText.Color = Color.Green;
            }

            t.Draw(sfText);
        }

        public void DrawString(RenderTarget t, string s, Vector2f pos, bool isInvert)
        {
            sfText.DisplayedString = s;
            sfText.Position = pos;
            if (isInvert)
            {
                sfText.Color = Color.Black;
                shape.Position = pos;
                shape.Size = new Vector2f(charSize.X * s.Length,charSize.Y);
                shape.FillColor = Color.Green;
                t.Draw(shape);
            }
            else
            {
                sfText.Color = Color.Green;
            }

            t.Draw(sfText);
        }

        public void DrawList(RenderTarget t, List<string> list, Vector2f pos, uint fontSize)
        {
            Text tx = new Text();
            tx.Font = font;



            foreach (string s in list)
            {
                tx.Color = Color.Green;
                tx.Position = pos;
                tx.DisplayedString = s;
                tx.CharacterSize = fontSize;
                t.Draw(tx);
                pos.Y += fontSize;
            }

        }

        public void Update()
        {
            
        }

        public void Draw(RenderTarget target)
        {
            /*  обчный вывод текста типа консоль, но в данном проекте не нужно.
            int st = 0;
            if (list.Count > maxRows)
            {
                st = list.Count - maxRows;
            }

            for (int i = st; i < listHeader.Count; i++)
            {
                sfText.Position = new Vector2f(leftMargin, topMargin + (i - st) * offsetY);
                sfText.DisplayedString = listHeader[i];


                target.Draw(sfText);
            }
            */
        }
    }
}

