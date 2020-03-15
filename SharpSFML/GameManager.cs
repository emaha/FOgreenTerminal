using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Media;
using SFML.Graphics;

using SFML.System;
using SFML.Window;

namespace SharpSFML
{
    class GameManager
    {
        char[] array = new char[320]; //набор символов 16*10*2

        GameState gameState;
        GreenConsole greenConsole = new GreenConsole(); // Тот кто занимается рисованием текста

        List<Word> generatedWordList = new List<Word>(); 
        List<string> numLinesList = new List<string>();
        List<string> PossibleWords = new List<string>();
        List<string> results = new List<string>();

        string CorrectWord;
        int TriesRemaining = 4;
        int selectedItem = 0;
        
        int HEIGHT;
        int WIDTH;
        int offsetY = 35; //интервал между строк
        int offsetX = 15; //ширина буквы
        int topMargin = 10; //отступы от края экрана
        int leftMargin = 30;
        private int numberOfWords = 10;

        float doorOpeningTimer = 0; //просто переменная чтобы при открытии двери была хоть какая-то анимация
        private float winTimer = 0; //для того чтобы не проигрывался звук по 10 раз

        int restartTime = 30; //КД на игру
        float timeElapsed;

        Random random = new Random();

        public GameManager(int w, int h)
        {
            HEIGHT = h;
            WIDTH = w;

            if (Directory.Exists("WordLists"))
            {
                foreach (string wordsListFile in Directory.EnumerateFiles("WordLists"))
                {
                    //int name = Convert.ToInt32(wordsListFile.Split('\\')[1].Split('.')[0]);
                    foreach (string word in File.ReadAllText(wordsListFile).Split('\n')) PossibleWords.Add(word.Replace("\r", "").ToUpper());
                }
            }
            else
            {
                Console.WriteLine("No word lists able to be found, exiting");
                Thread.Sleep(3000);
                Environment.Exit(1);
            }
            
            Initialization();

            gameState = GameState.Play;
        }

        public void Update(Time time)
        {
            switch (gameState)
            {
                case GameState.Restart:
                    timeElapsed += time.AsSeconds();

                    if (timeElapsed > restartTime)
                    {
                        Initialization();
                        gameState = GameState.Play;
                        timeElapsed = 0;
                    }
                break;
                case GameState.DoorOpening:    //данный пункт отключен, да и по игре не предпологается.
                    doorOpeningTimer += time.AsSeconds();
                    break;
                case GameState.Win:
                    winTimer -= time.AsSeconds();
                    break;

            }

        }

        public void Initialization()
        {
            CorrectWord = "";
            results.Clear();
            numLinesList.Clear();
            generatedWordList.Clear();
            TriesRemaining = 4;
            selectedItem = 0;
            doorOpeningTimer = 0;
            gameState = GameState.Play;

            //заполняем массив
            string chars = "!@#$%^&*()/\\\";:?_=+-`~";
            for (int i = 0; i < 320; i++)
            {
                array[i] = chars[random.Next(0, chars.Length)];
            }

            //расставляем 10 слов случайно
            for (int i = 0; i < 10; i++)
            {
                string randomWord = PossibleWords[random.Next(0, PossibleWords.Count)];
                while(generatedWordList.Count(a => a.word.Contains(randomWord))>0)
                {
                    randomWord = PossibleWords[random.Next(0, PossibleWords.Count)];
                }
                generatedWordList.Add(new Word(randomWord, random.Next(i*31, i*31+25)));
            }

            //вставляем эти слова в массив символов
            foreach (Word t in generatedWordList)
            {
                string s = t.word;
                int r = t.pos;
                for (int j = r; j <  r+s.Length; j++)
                {
                    array[j] = s[j-r];
                }
            }

            int startNum = 64004; // по заданию будем начинать с этой цифры
            for (int j = 0; j < 32; j++)
            {
                numLinesList.Add("0x" + Convert.ToString(startNum+=24,16).ToUpper());
            }
            
        }
        
        public void PressKey(Keyboard.Key key)
        {
            if (gameState == GameState.Win)
            {
                switch (key)
                {
                    case Keyboard.Key.Up:
                        selectedItem = 0;
                        break;
                    case Keyboard.Key.Down:
                        selectedItem = 1;
                        break;
                    case Keyboard.Key.Return:
                        WinScreenCheck(selectedItem);
                        break;
                }
                return;
            }

            if (gameState == GameState.FirstScreen)
            {
                switch (key)
                {
                    case Keyboard.Key.Up:
                        if (selectedItem > 0)
                        {
                            selectedItem--;
                        }
                        break;
                    case Keyboard.Key.Down:
                        if (selectedItem < 15) //пока пунктов такое количество
                        {
                            selectedItem++;
                        }
                        break;
                    case Keyboard.Key.Return:
                        {
                            if (selectedItem == 6) //Верный для запуска вариант
                            {
                                gameState = GameState.Win;
                                selectedItem = 0;
                            }
                        }
                        return;
                }

            }

            if (gameState == GameState.DoorOpening)
            {
                if (key == Keyboard.Key.F1)
                {
                    Initialization();
                }
                return;
            }

            if (gameState == GameState.Play)
            {
                switch (key)
                {
                    case Keyboard.Key.Up:
                        if (selectedItem > 0)
                        {
                            selectedItem--;
                        }
                        break;
                    case Keyboard.Key.Down:
                        if (selectedItem < numberOfWords-1)
                        {
                            selectedItem++;
                        }
                        break;
                    case Keyboard.Key.Left:
                        if (selectedItem > numberOfWords/2 - 1)
                        {
                            selectedItem -= numberOfWords/2;
                        }
                        break;
                    case Keyboard.Key.Right:
                        if (selectedItem < numberOfWords/2)
                        {
                            selectedItem += numberOfWords/2;
                        }
                        break;
                    
                    case Keyboard.Key.F1:
                        //Initialization();
                        break;
                    
                    case Keyboard.Key.Return:
                        PlayScreenCheck();
                        break;
                }
                return;
            }
            
            if (gameState == GameState.ViewRecords)
            {
                switch (key)
                {
                    case Keyboard.Key.Return:
                        gameState = GameState.Win;
                        selectedItem = 0;
                        break;
                }
            }
        }

        public void WinScreenCheck(int option)
        {
            switch (option)
            {
                case 0:
                    //Console.Beep();
                    if (winTimer <= 0)
                    {
                        SoundPlayer simpleSound = new SoundPlayer(@"win.wav");
                        simpleSound.Play();
                        winTimer = 5.0f;
                    }
                    
                    
                    //gameState = GameState.DoorOpening;

                    break;
                case 1:
                    gameState = GameState.ViewRecords;
                    break;
            }
        }

        public void PlayScreenCheck()
        {
            //чтобы игра не казалась угадайкой, устанавливаем ответ только после первого выбора
            if (CorrectWord == "")
            {
                while (generatedWordList[selectedItem].word ==
                       (CorrectWord = generatedWordList[random.Next(generatedWordList.Count)].word))
                {}
            }

            Console.WriteLine("Correct word = {0}",CorrectWord);

            if (CorrectWord.Equals(generatedWordList[selectedItem].word))
            {
                gameState = GameState.Win;
                selectedItem = 0;
            }
            else
            {
                int matchCount = CalcMatches(CorrectWord, generatedWordList[selectedItem].word);
                results.Add(generatedWordList[selectedItem].word);
                results.Add("Вход запрещен");
                results.Add("Сходства=" + matchCount.ToString());

                if (--TriesRemaining == 0)
                {
                    gameState = GameState.Restart;
                }
            }
        }

        public int CalcMatches(string s, string d)
        {
            return s.Where((t, i) => t == d[i]).Count();
        }

        public void DrawNumLines(RenderTarget target)
        {
            int x = 30;
            int y = 150;
            for (int i = 0; i < numLinesList.Count; i++)
            {
                greenConsole.DrawString(target, numLinesList[i], new Vector2f(x,y), false);
                y += offsetY;
                if ((i + 1) % 16 == 0)
                {
                    x += 350;   y = 150;
                }
            }
        }

        public void DrawChars(RenderTarget target)
        {
            int wordIndex=0;
            string selectedString="";

            if (selectedItem >= 0)
            {
                selectedString = generatedWordList[selectedItem].word;
                string str = new string(array);

                //ищем позицию вхождения слова
                wordIndex = str.IndexOf(selectedString);
            }


            int x = 140;
            int y = 150;
            for (int k = 0; k < 320; k++)
            {
                greenConsole.DrawChar(target, array[k], new Vector2f(x, y), k >= wordIndex && k < wordIndex + selectedString.Length);
                
                if ((k+1)%10 == 0)
                {
                    x -= offsetX*10;   y += offsetY;
                }
                if ((k+1) % 160 == 0) // следующие 160 символов буду во 2м столбике
                {
                    x += 350;   y = 150;
                }
                x += offsetX;
            }
        }

        public void DrawHeaderAndResult(RenderTarget target)
        {
            List<string> list = new List<string>();
            list.Add("Добро пожаловать в сеть \"РОБКО Индастриз(ТМ)\"");
            list.Add("Требуется пароль");

            StringBuilder sb = new StringBuilder();
            sb.Append("Осталось попыток:      ");
            for (int i = 0; i < TriesRemaining; i++)
            {
                sb.Append("█ ");
            }
            list.Add(sb.ToString());
            list.Add("");

            int x = leftMargin;
            int y = topMargin;
            greenConsole.DrawList(target, list, new Vector2f(x,y),30);

            //вывод. Производится в правой части экрана в обратно порядке
            x = 700;
            y = 675;
            for (int i = results.Count - 1; i >= 0; i--)
            {
                greenConsole.DrawString(target,results[i],new Vector2f(x,y), false);
                y -= offsetY;
            }
        }

        public void DrawRestartScreen(RenderTarget target)
        {
            greenConsole.DrawString(target, "ПРОГРАММА ВЗЛОМА ПАРОЛЯ ПЕРЕЗАГРУЖАЕТСЯ.", new Vector2f(WIDTH / 5, HEIGHT / 2 - 20), false);
            greenConsole.DrawString(target, "ПОЖАЛУЙСТА, ПОДОЖДИТЕ!", new Vector2f(WIDTH / 2 - 200, HEIGHT / 2 + 20), false);
            greenConsole.DrawString(target, (Math.Round(restartTime - timeElapsed)).ToString(), new Vector2f(WIDTH / 2 - 50, HEIGHT / 2 + 50), false);
        }

        public void DrawWinScreen(RenderTarget target)
        {
            List<string> title = new List<string>();
            title.Add("ROBCO INDUSTRIES UNIFYED OPERATING SYSTEM COPYRIGHT 2075-2077");
            title.Add("-Server 10-");
            title.Add("\"Волт-Тек\" - адмистративная система");
            title.Add("База данных 923-А");
            title.Add("****СЕТЬ ВЫКЛЮЧЕНА****");
            title.Add("Пожалуйста, проверьте локальные соединения");
            title.Add("____________________________________________________________________");

            int x = leftMargin;
            int y = topMargin;
            greenConsole.DrawList(target, title, new Vector2f(x, y),30);

            List<String> options = new List<string>();
            options.Add(">Открыть дверь");
            options.Add(">Просмотреть записи");

            x = leftMargin;
            y = 300;

            for(int i=0;i< options.Count;i++)
            {
                greenConsole.DrawString(target, options[i], new Vector2f(x, y), i == selectedItem);
                y += offsetY;
            }
        }

        public void DrawViewRecordsScreen(RenderTarget target)
        {
            int x = leftMargin;
            int y = topMargin;
            List<String> recordsList = new List<string>();

            recordsList.Add("Исследование воздействия вируса ВРЭ на человеческий организм." );
            recordsList.Add("Военная база Марипоза, Калифорния, США.");
            recordsList.Add("1. После проведения успешных испытаний на животных, " );
            recordsList.Add("   было решено провести первые испытания на людях.(8)");
            recordsList.Add("2. Испытуемыми выступают в основном добровольцы из числа " );
            recordsList.Add("   заключенных с пожизненным сроком заключения. (6)");
            recordsList.Add("3. Вирус Рукотворной Эволюции ускоряет различные процессы в организме, " );
            recordsList.Add("   резко увеличивает рост тканей, изменяет функции мозга. (0)");
            recordsList.Add("4. Преимущества данного способа в том, что есть возможность контролировать " );
            recordsList.Add("   эти изменения, внося изменения в сам геном вируса.");
            recordsList.Add("   Так мы можем вносить необходимые изменения даже после 1 эксперимента. (1)");
            recordsList.Add("5. К сожалению, не все испытуемые выживают после инъекций вируса. " );
            recordsList.Add("   На данный момент выживаемость около 1%, что уже выше ожиданий. (4)");
            recordsList.Add("6. На данный момент мы провели 242 эксперимента. Средняя длительность " );
            recordsList.Add("   эксперимента 3 минуты 18 секунд. (2)");
            recordsList.Add("7. На данный момент, пережило эксперимент только 4 подопытных, " );
            recordsList.Add("   2 из которых скончались менее чем через сутки после эксперимента. (7)");
            recordsList.Add("8. Один из выживших не реагирует на внешние раздражители, " );
            recordsList.Add("   однако сердцебиение и дыхание присутствуют. ");
            recordsList.Add("   Прощупать пульс не представляется возможным. (9)");
            recordsList.Add("9. У второго выжившего значительно увеличилась мышечная масса, ");
            recordsList.Add("   однако он не способен двигаться. (3)");
            recordsList.Add("");
            recordsList.Add("В дальнейших экспериментах планируется увеличить процент выживаемости, ");
            recordsList.Add("а так же изменить качественные характеристики получившихся объектов.");
            recordsList.Add("");
            recordsList.Add("Д-р. А. Кляйнер.");

            greenConsole.DrawList(target, recordsList, new Vector2f(leftMargin, topMargin),25);

            y = 700;
            greenConsole.DrawString(target, ">Назад", new Vector2f(x, y), true);
            
        }

        public void DrawDoorOpeningScreen(RenderTarget target)
        {
            int x = leftMargin;
            int y = topMargin;

            StringBuilder sb = new StringBuilder();
            sb.Append("Открытие двери");
            for(int i=0;i<doorOpeningTimer;i++)
            {
                sb.Append(" .");
            }
            greenConsole.DrawString(target, sb.ToString(), new Vector2f(x, y), false);
        }

        public void DrawFirstScreen(RenderTarget target)
        {
            //Тут у нас будет много бесполезных пункотов, только нажав на 1 правильный начнется игра
            List<String> options = new List<string>();
            options.Add("> Нет");
            options.Add("> Нет");
            options.Add("> Нет");
            options.Add("> Нет");
            options.Add("> Нет");
            options.Add("> Нет");
            options.Add("> Да");
            options.Add("> Нет");
            options.Add("> Нет");
            options.Add("> Нет");
            options.Add("> Нет");
            options.Add("> Нет");
            options.Add("> Нет");
            options.Add("> Нет");
            options.Add("> Нет");
            options.Add("> Нет");

            int x = leftMargin;
            int y = 100;

            for (int i = 0; i < options.Count; i++)
            {
                greenConsole.DrawString(target, options[i], new Vector2f(x, y), i == selectedItem);
                y += offsetY;
            }
        }

        public void Draw(RenderTarget target)
        {
            switch (gameState)
            {
                case GameState.Play:
                    DrawHeaderAndResult(target);
                    DrawChars(target);
                    DrawNumLines(target);
                    break;
                case GameState.Restart:
                    DrawRestartScreen(target);
                    break;
                case GameState.Win:
                    DrawWinScreen(target);
                    break;
                case GameState.ViewRecords:
                    DrawViewRecordsScreen(target);
                    break;
                case GameState.DoorOpening:
                    DrawDoorOpeningScreen(target);
                    break;
                case GameState.FirstScreen:
                    DrawFirstScreen(target);
                    break;
            }


        }
    }
}
