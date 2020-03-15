using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSFML
{
    class Word
    {
        public Word(string s, int p)
        {
            word = s;
            pos = p;
        }
        public string word;
        public int pos;
    }

    enum GameState
    {
        Restart, Play, Win, ViewRecords, DoorOpening, FirstScreen
    }
}
