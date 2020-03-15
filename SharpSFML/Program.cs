using System.IO;
using System.Runtime.InteropServices;
using SFML.Window;


namespace SharpSFML
{
    class Program
    {
        static void Main(string[] args)
        {
            Styles style = Styles.None;
            #if !DEBUG
                style = Styles.Fullscreen;
            #endif



            

            Application app = new Application(1024,768,"GreenTerminal", style);
            app.Run();
        }
    }
}
