using System.Net.Sockets;
using System.Text;
using KingPongClient.Network;
using KingPongServer;
using MessagePack;

namespace KingPongClient
{
    public class Client
    {
        private const string KEY_PHRASE = "better atom task thank dynamic audit mixture onion fog";
        private const string SERVER_IP = "pong.geffert.xyz";
        private const int SERVER_PORT = 80;

        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        private bool isRunning;
        public int playerId;
        public int score1;
        public int score2;
        public int gameState;
        public int countdown;

        // Game state
        public double paddle1X, paddle1Y, paddle1VelX, paddle1VelY;
        public double paddle2X, paddle2Y, paddle2VelX, paddle2VelY;
        public double ballX, ballY, ballVelX, ballVelY;

        private readonly object renderLock = new object();

        public Client()
        {
            isRunning = false;
            score1 = score2 = 0;
            gameState = 0; // WAITING
        }

        public void Connect()
        {
            try
            {
                client = new TcpClient();
                client.Connect(SERVER_IP, SERVER_PORT);
                stream = client.GetStream();

                // Send auth packet
                byte[] keyPhraseBytes = Encoding.UTF8.GetBytes(KEY_PHRASE);
                stream.Write(keyPhraseBytes, 0, keyPhraseBytes.Length);

                isRunning = true;
                receiveThread = new Thread(ReceiveLoop);
                receiveThread.Start();

                Console.WriteLine("Connected to server!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect: {ex.Message}");
            }
        }

        public void SendPaddleControl(short direction)
        {
            if (!isRunning) return;

            try
            {
                IPacket packet = new PaddleControlPacket(direction);
                byte[] data = MessagePackSerializer.Serialize(packet);
                System.Console.WriteLine($"packet len: {data.Length}");
                stream.Write(BitConverter.GetBytes((short)data.Length), 0, 2);
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending paddle control: {ex.Message}");
                Disconnect();
            }
        }

        private void ReceiveLoop()
        {
            while (isRunning)
            {
                try
                {
                    if (stream.DataAvailable)
                    {
                        // Read packet length
                        byte[] lengthBuffer = new byte[2];
                        int lengthBytesRead = stream.Read(lengthBuffer, 0, 2);
                        
                        if (lengthBytesRead == 0)
                        {
                            break;
                        }

                        int packetLength = BitConverter.ToInt16(lengthBuffer, 0);
                        
                        // Read packet data
                        byte[] buffer = new byte[packetLength];
                        int bytesRead = stream.Read(buffer, 0, packetLength);
                        
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        IPacket packet = MessagePackSerializer.Deserialize<IPacket>(buffer);
                        HandlePacket(packet);
                    }
                    Thread.Sleep(5);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in receive loop: {ex.Message}");
                    break;
                }
            }
            Disconnect();
        }

        private void HandlePacket(IPacket packet)
        {
            switch (packet)
            {
                case GameStatePacket gameStatePacket:
                    gameState = gameStatePacket.State;
                    countdown = gameStatePacket.Countdown;
                    playerId = gameStatePacket.playerId;
                    Console.WriteLine($"Game state: {gameState}, Countdown: {countdown}, Player ID: {playerId}");
                    break;

                case ScorePacket scorePacket:
                    score1 = scorePacket.PlayerOneScore;
                    score2 = scorePacket.PlayerTwoScore;
                    Console.WriteLine($"Score - Player 1: {score1}, Player 2: {score2}");
                    break;

                case PaddlePositionPacket paddlePacket:
                    paddle1X = paddlePacket.PaddleOneX;
                    paddle1Y = paddlePacket.PaddleOneY;
                    paddle1VelX = paddlePacket.PaddleOneVelocityX;
                    paddle1VelY = paddlePacket.PaddleOneVelocityY;
                    paddle2X = paddlePacket.PaddleTwoX;
                    paddle2Y = paddlePacket.PaddleTwoY;
                    paddle2VelX = paddlePacket.PaddleTwoVelocityX;
                    paddle2VelY = paddlePacket.PaddleTwoVelocityY;
                    ballX = paddlePacket.BallX;
                    ballY = paddlePacket.BallY;
                    ballVelX = paddlePacket.BallVelocityX;
                    ballVelY = paddlePacket.BallVelocityY;
                    //System.Console.WriteLine($"Ball X: {ballX}, Ball Y: {ballY}");
                    RenderGame();
                    break;

                case PingPacket pingPacket:
                    // Echo ping packet back
                    byte[] data = MessagePackSerializer.Serialize(new PingPacket(pingPacket.Id));
                    stream.Write(BitConverter.GetBytes((short)data.Length), 0, 2);
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                    break;
            }
        }

        private void RenderGame()
        {


         
            
        }

        public void Disconnect()
        {
            isRunning = false;
            try
            {
                client?.Close();
            }
            catch { }
        }
    }
} 