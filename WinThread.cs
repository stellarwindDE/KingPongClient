using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KingPongClient
{
    public class WinThread
    {
        private Thread _thread;
        private DispatcherTimer _timer;

        static public Rectangle player2;
        static public Rectangle player1;
        static public Rectangle ballR;
        public static Ellipse ball;
        public static TextBlock infoText;
        public static TextBlock scooreText;


        public void Start()
        {
            _thread = new Thread(() =>
            {
            
                var app = new Application();

            var window = new Window
            {
                Title = "KingPong",
                Width = 800,
                Height = 600
            };
                //window.SizeToContent = SizeToContent.Height;
                

                var canvas = new Canvas();
                window.Content = canvas;

                // Hintergrund zeichnen
                var rect = new Rectangle
                {
                    Width = 800,
                    Height = 600,
                    Fill = Brushes.Black
                };
                Canvas.SetLeft(rect, 0);
                Canvas.SetTop(rect, 0);
                canvas.Children.Add(rect);


                player1 = new Rectangle
                {
                    Width = 20,
                    Height = 240,
                    Fill = Brushes.White
                };
                Canvas.SetLeft(player1, 5);
                Canvas.SetTop(player1, Program.client.paddle1Y);
                canvas.Children.Add(player1);


                player2 = new Rectangle
                {
                    Width = 20,
                    Height = 240,
                    Fill = Brushes.White
                };
                Canvas.SetLeft(player2, 760);
                Canvas.SetTop(player2, Program.client.paddle2Y);
                canvas.Children.Add(player2);


                // Beispiel: blaue Ellipse
                /*var ellipse = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = Brushes.White
                };
                Canvas.SetLeft(ellipse, Program.client.ballX);
                Canvas.SetTop(ellipse, Program.client.ballY);
                canvas.Children.Add(ellipse);*/

                ballR = new Rectangle
                {
                    Width = 20,
                    Height = 20,
                    Fill = Brushes.White
                };
                Canvas.SetLeft(ballR, Program.client.ballX);
                Canvas.SetTop(ballR, Program.client.ballY);
                canvas.Children.Add(ballR);


                infoText = new TextBlock
                {
                    Text = $"Ball X: {Program.client.ballX}, Ball Y: {Program.client.ballY}",
                    Foreground = Brushes.White,
                    FontSize = 14
                };
                Canvas.SetLeft(infoText, 30);
                Canvas.SetTop(infoText, 580);
                canvas.Children.Add(infoText);


                scooreText = new TextBlock
                {
                    Text = $"Player1: {Program.client.score1}, Player2: {Program.client.score2}",
                    Foreground = Brushes.White,
                    FontSize = 24
                };
                Canvas.SetLeft(scooreText, 100);
                Canvas.SetTop(scooreText, 5);
                canvas.Children.Add(scooreText);


                infoText = new TextBlock
                {
                    Text = $"Ball X: {Program.client.ballX}, Ball Y: {Program.client.ballY}",
                    Foreground = Brushes.White,
                    FontSize = 14
                };
                Canvas.SetLeft(infoText, 30);
                Canvas.SetTop(infoText, 580);
                canvas.Children.Add(infoText);




                // Timer für Zeichenschleife
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(10); // ca. 60 FPS
                _timer.Tick += (s, e) =>
                {
                    // Position des Paddles aktualisieren
                    Canvas.SetTop(player1, Program.client.paddle1Y);
                    Canvas.SetTop(player2, Program.client.paddle2Y);
                    Canvas.SetLeft(ballR, Program.client.ballX);
                    Canvas.SetTop(ballR, Program.client.ballY);
                    infoText.Text = $"Ball X: {Program.client.ballX}, Ball Y: {Program.client.ballY}, GameState: { Program.client.gameState}, Counter: {Program.client.countdown}";
                    scooreText.Text = $"Player1: {Program.client.score1}, Player2: {Program.client.score2}";


                    // Hier kannst du auch Ballposition, Gegner etc. updaten
                };
                _timer.Start();

                window.KeyDown += (sender, e) =>
                {
                    switch (e.Key)
                    {
                        case Key.W:
                            Program.client.SendPaddleControl(1);
                            break;
                        case Key.S:
                            Program.client.SendPaddleControl(-1);
                            break;
                        case Key.Q:
                            Application.Current.Shutdown();
                            break;
                    }
                };

                window.KeyUp += (sender, e) =>
                {
                    if(e.Key == Key.W || e.Key == Key.S)
                    {
                        Program.client.SendPaddleControl(0);
                    }
                };

                window.Loaded += (s, e) => window.Focus(); // Fenster erhält Fokus


                app.Run(window);
            });

        

            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();


        }
    }
}
