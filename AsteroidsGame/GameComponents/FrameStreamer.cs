using Microsoft.AspNetCore.SignalR;
using Asteroids.Hubs;
using System.Collections.Concurrent;

namespace Asteroids.GameComponents
{
    class Frame
    {
        public int Id { get; set; }

        public int AsteroidsCount { get; set; } = 5;

        public List<Player> Players { get; set; } = new List<Player>();

        public List<Asteroid> Asteroids { get; set; } = new List<Asteroid>();

        public List<Bullet> Bullets { get; set; } = new List<Bullet>();

        public List<Asteroid> Explosions { get; set; } = new List<Asteroid>();

        public GameState State { get; set; } = GameState.WaitingforStart;
    }

    public class FrameStreamer
    {
        private int currentFrameId;

        private ConcurrentQueue<object> frameQueue;

        private Timer frameTimer;

        private CancellationTokenSource cancellationTokenSource;

        private IHubContext<GameHub> hubContext;

        private GameManager _gameManager;

        private const int FrameInterval = 33;

        public FrameStreamer(IHubContext<GameHub> hubContext, GameManager game)
        {
            this.hubContext = hubContext;
            this.frameQueue = new ConcurrentQueue<object>();
            this.cancellationTokenSource = new CancellationTokenSource();
            this.frameTimer = new Timer(EnqueueFrame, null, 0, FrameInterval);
            this._gameManager = game;
        }

        private void EnqueueFrame(object state)
        {
            _gameManager.RefreshGame();
            var CurrentGame = _gameManager.GetGame();
            if (CurrentGame.State == GameState.Running)
            {
                var frame = new Frame
                {
                    Id = Interlocked.Increment(ref currentFrameId),
                    Asteroids = CurrentGame.Asteroids,
                    Players = CurrentGame.Players.Select(kv => kv.Value).ToList(),
                    Bullets = CurrentGame.Bullets,
                    State = CurrentGame.State,
                    Explosions = CurrentGame.Explosions,
                };
                frameQueue.Enqueue(frame);
            }
        }

        /// <summary>
        /// Starts streaming frames to multiple clients using SignalR.
        /// </summary>
        public async Task StartStreamingAsync()
        {
            var cancellationToken = cancellationTokenSource.Token;

            // Keep running until a cancellation request is received.
            while (!cancellationToken.IsCancellationRequested)
            {
                // Check if a frame is available in the frame queue.
                if (frameQueue.TryDequeue(out var frame))
                {
                    // Send the frame to all connected clients.
                    await hubContext.Clients.All.SendAsync("updateFrame", frame);
                }
                else
                {
                    // Delay for 20ms to prevent overloading the system.
                    await Task.Delay(20);
                }
            }
        }

        public void StopStreaming()
        {
            cancellationTokenSource.Cancel();
            frameTimer.Dispose();
            frameQueue.Clear();
        }
    }
}