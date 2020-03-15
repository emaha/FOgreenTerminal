using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SharpSFML
{
    class Game
    {
        #region Global vars

        RenderWindow window;
        List<Player> list = new List<Player>();

        //Used to time the movement of the objects on screen
        Clock clock = new Clock();
        Time gameTime = new Time();

        //FPS vars
        float timeElapsed = 0;
        int fps = 0;

        
        int WINDOW_HEIGHT;
        int WINDOW_WIDTH;
        
        CircleShape shape = new CircleShape(100.0f);

        #endregion

        public Game(uint windowWidth = 800, uint windowHeight = 600, string title = "SFML APP",
            Styles style = Styles.Close)
        {
            window = new RenderWindow(new VideoMode(windowWidth, windowHeight), title, style);
            window.KeyPressed += window_KeyPressed;
            window.Closed += window_Closed;
            
            WINDOW_WIDTH = (int)windowWidth;
            WINDOW_HEIGHT = (int) windowHeight;
            
            shape.FillColor = Color.Green;
        }

        public void Run()
        {
            window.SetVisible(true);
            SetUpGlobalVars();
            clock.Restart();

            list.Add(new Player());

            while (window.IsOpen)
            {
                window.DispatchEvents();
                Update();
                Draw();
            }
        }

        void SetUpGlobalVars()
        {
        }

        #region Input functions

        void window_KeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Escape:
                    window.Close();
                    break;

                default:
                    break;
            }
        }

        void window_Closed(object sender, EventArgs e)
        {
            window.Close();
        }

        #endregion

        private void Update()
        {
            gameTime = clock.Restart();
            timeElapsed += gameTime.AsSeconds();

            if (timeElapsed > 1f)
            {
                Console.WriteLine("FPS: {0}", fps);
                shape.Radius = fps/100.0f;

                

                fps = 0;
                timeElapsed = 0;
            }

            foreach (var l in list)
            {
                l.Update();
            }

            
            fps++;
        }

        private void Draw()
        {
            window.Clear();
            window.Draw(shape);
            foreach (var l in list)
            {
                l.Draw(window);
            }
            window.Display();
        }

    }


}

