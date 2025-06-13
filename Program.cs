using System;
using System.Windows;
using System.Threading;


namespace KingPongClient
{
    class Program
    {

        public static WinThread winThread;
        public static Client client;


        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("KingPong Client");
            Console.WriteLine("Controls:");
            Console.WriteLine("  W - Move paddle up");
            Console.WriteLine("  S - Move paddle down");
            Console.WriteLine("  Q - Quit");
            Console.WriteLine();


            Console.WriteLine("Starte GUI in eigenem Thread...");

            winThread = new WinThread();
            winThread.Start();

            client = new Client();
            client.Connect();





            bool running = true;
            while (running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.W:
                            client.SendPaddleControl(1); // Up
                            break;
                        case ConsoleKey.S:
                            client.SendPaddleControl(-1); // Down
                            break;
                        case ConsoleKey.Q:
                            running = false;
                            break;
                    }
                }
                Thread.Sleep(10);
            }

            client.Disconnect();
        }
    }
} 