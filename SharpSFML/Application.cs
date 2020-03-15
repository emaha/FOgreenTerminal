using System;
using System.Threading;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SharpSFML
{
    class Application
    {
        #region Global vars

        RenderWindow window;
        int WINDOW_HEIGHT;
        int WINDOW_WIDTH;

        Clock clock = new Clock();
        Time gameTime;

        GameManager gameManager;

        float timeElapsed = 0;
        #endregion

        public Application(uint windowWidth = 800, uint windowHeight = 600, string title = "HXX",
            Styles style = Styles.Close)
        {
            window = new RenderWindow(new VideoMode(windowWidth, windowHeight), title, style);
            window.KeyPressed += window_KeyPressed;
            window.Closed += window_Closed;
            WINDOW_HEIGHT = (int) windowHeight;
            WINDOW_WIDTH = (int) windowWidth;
            window.SetMouseCursorVisible(false);
            gameManager = new GameManager(WINDOW_WIDTH, WINDOW_HEIGHT);
        }

        public void Run()
        {
            window.SetVisible(true);
            SetUpGlobalVars();
            clock.Restart();

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
            gameManager.PressKey(e.Code);
            switch (e.Code)
            {
                case Keyboard.Key.F10:
                    if (e.Control)
                    {
                        window.Close();
                    }
                    break;

                case Keyboard.Key.Escape:
                    //window.Close();
                    break;

                default:
                    break;
            }
            Thread.Sleep(100);
        }

        void window_Closed(object sender, EventArgs e)
        {
            //window.Close();
        }

        #endregion

        private void Update()
        {
            gameTime = clock.Restart();
            timeElapsed += gameTime.AsSeconds();

            gameManager.Update(gameTime);

            if (timeElapsed > 1)
            {

                timeElapsed = 0;
            }
        }

        private void Draw()
        {
            window.Clear();
            gameManager.Draw(window);
            window.Display();
        }

    }


}

