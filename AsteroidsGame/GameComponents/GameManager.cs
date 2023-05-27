using Asteroids.Hubs;
using Microsoft.AspNetCore.SignalR;
using Timer = System.Timers.Timer;

namespace Asteroids.GameComponents
{
    public class GameManager
    {
        private readonly IHubContext<GameHub> _hubContext;

        private readonly SemaphoreSlim _gameLock = new SemaphoreSlim(1, 1);

        private Game _game;

        private int _width = 1910;
        private int _height = 800;

        // An enum that defines the possible states of the game
        // A constructor that initializes the game status with a list of players and a level
        public GameManager(IHubContext<GameHub> gameHub)
        {
            _hubContext = gameHub;
            Initialzie();
        }

        public void Initialzie()
        {
            _game = new Game(_width, _height);
            _game.State = GameState.Running;
        }

        public void AddPlayer(string connectionId, string name)
        {
            try
            {
                Random rand = new Random();
                var x = rand.Next(50, _width - 50);
                var y = rand.Next(50, _height - 50);
                _game.AddPlayer(connectionId, name, x, y);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public async Task RemovePlayer(string connectionId)
        {
            await _gameLock.WaitAsync();
            try
            {
                _game.Players.Remove(connectionId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                _gameLock.Release();
            }
        }

        public void AddBullet(string connectionId, float x, float y, float angle)
        {
            _game.AddBullet(connectionId, x, y, angle);
        }

        public void RefreshGame()
        {
            if (_game.State == GameState.Running)
            {
                Move();
                _game.ShipAsteroidsCollisionDetection();
                _game.ShipShipCollisionDetection();
                CollisionDetection();
            }
        }
        public void CollisionDetection()
        {
            _gameLock.Wait();
            try
            {
                _game.BulletAsteroidsCollisionDetection();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                _gameLock.Release();
            }
        }

        public void Move()
        {
            try
            {
                _game.Move();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public Game GetGame()
        {
            return _game;
        }

        public bool isRunning()
        {
            if (_game.State == GameState.Running)
            {
                return true;
            }
            else { return false; }
        }

        public Player GetPlayer(string contextId)
        {
            Player player = null;
            _game.Players.TryGetValue(contextId, out player);
            return player;
        }

        public int GetPlayerBulletCount(string contextId)
        {
            int count = 0;
            foreach (var bullet in _game.Bullets)
            {
                if (bullet.connectionId == contextId)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
