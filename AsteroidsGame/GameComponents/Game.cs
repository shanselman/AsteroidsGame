
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Asteroids.GameComponents
{
    // An enum that defines the possible states of the game
    public enum GameState
    {
        Running,
        Winning,
        Lost,
        End,
        WaitingforStart
    }

    public partial class Game
    {
        public int AsteroidsCount { get; set; } = 5;

        private int _width;
        private int _height;
        private Random _random = new Random();

        public Dictionary<string, Player> Players { get; set; } = new Dictionary<string, Player>();

        public List<Asteroid> Asteroids { get; set; } = new List<Asteroid>();

        public List<Bullet> Bullets { get; set; } = new List<Bullet>();

        public List<Asteroid> Explosions { get; set; } = new List<Asteroid>();

        public GameState State { get; set; } = GameState.WaitingforStart;

        // A constructor that initializes the game status with a list of players and a level
        public Game(int screenWidth, int screenHeight)
        {
            _width = screenWidth;
            _height = screenHeight;
            Asteroids = GenerateAsteroids(AsteroidsCount);
        }

        // A method that generates a list of asteroids with random positions, velocities and size based on a given count
        // Asteroid constructor: Asteroid(float x, float y, float dx, float dy, int size)
        // Asteroid size is 50-90, dx and dy are random between -2 and 2
        public List<Asteroid> GenerateAsteroids(int count)
        {
            List<Asteroid> asteroids = new List<Asteroid>();
            for (int i = 0; i < count; i++)
            {
                float x = _random.Next(0, _width);
                float y = _random.Next(0, _height);
                int size = _random.Next(50, 90);
                float dx = (float)(_random.NextDouble() * 4 - 2);
                float dy = (float)(_random.NextDouble() * 4 - 2);
                asteroids.Add(new Asteroid(x, y, dx, dy, size));
            }
            return asteroids;
        }

        // A method that moves the asteroids based on their dx, dy. Limit the movement to the screen size
        public void MoveAsteroids()
        {
            for (int i = 0; i < Asteroids.Count; i++)
            {
                Asteroids[i].x += Asteroids[i].dx;
                Asteroids[i].y += Asteroids[i].dy;

                if (Asteroids[i].x > _width)
                {
                    Asteroids[i].x = 0 - Asteroids[i].size;
                }
                else if (Asteroids[i].x < 0 - Asteroids[i].size)
                {
                    Asteroids[i].x = _width;
                }

                if (Asteroids[i].y > _height)
                {
                    Asteroids[i].y = 0 - Asteroids[i].size;
                }
                else if (Asteroids[i].y < 0 - Asteroids[i].size)
                {
                    Asteroids[i].y = _height;
                }
            }
        }

        // A method that moves all players based on its speed and angle. Limit the movement to the screen size
        public void MoveSpaceShip()
        {
            foreach (var player in Players.Values)
            {
                float newX = player.x + MathF.Sin(player.angle) * player.speed;
                float newY = player.y - MathF.Cos(player.angle) * player.speed;

                if (newX > _width)
                {
                    newX = 0;
                }
                else if (newX < 0)
                {
                    newX = _width;
                }

                if (newY > _height)
                {
                    newY = 0;
                }
                else if (newY < 0)
                {
                    newY = _height;
                }

                player.x = newX;
                player.y = newY;
            }
        }

        // A method that moves the bullets based on their speed and angle
        public void MoveBullets()
        {
            lock (Bullets)
            {
                if (Bullets is null)
                {
                    return;
                }
                for (int i = 0; i < Bullets.Count; i++)
                {
                    float x = Bullets[i].x + MathF.Sin(Bullets[i].angle) * Bullets[i].speed;
                    float y = Bullets[i].y - MathF.Cos(Bullets[i].angle) * Bullets[i].speed;

                    if (x > _width || x < 0 || y > _height || y < 0)
                    {
                        Bullets.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        Bullets[i].Update(x, y);
                    }
                }
            }
        }

        // A method that detects collision between bullets and asteroids, remove asteroids and bullets if they collide, add score to the player who shot the bullet (using connectionId)
        public void BulletAsteroidsCollisionDetection()
        {
            foreach (var bullet in Bullets)
            {
                for (int j = 0; j < Asteroids.Count; j++)
                {
                    var dx = bullet.x - Asteroids[j].x;
                    var dy = bullet.y - Asteroids[j].y;
                    var distance = MathF.Sqrt(dx * dx + dy * dy);
                    if (distance < Asteroids[j].size / 2)
                    {
                        Explosions.Add(Asteroids[j]);
                        Bullets.Remove(bullet);
                        Asteroids.RemoveAt(j);
                        foreach (var player in Players.Values)
                        {
                            if (player.connectionId == bullet.connectionId)
                            {
                                player.Score += 100;
                            }
                        }
                        break;
                    }
                }
            }
        }

        // A method that detects collision between ship and asteroids
        // Set player.isDead to true if they collide
        public void ShipAsteroidsCollisionDetection()
        {
            foreach (var player in this.Players.Values)
            {
                for (int j = 0; j < Asteroids.Count; j++)
                {
                    var dx = player.x - Asteroids[j].x;
                    var dy = player.y - Asteroids[j].y;
                    var distance = MathF.Sqrt(dx * dx + dy * dy);
                    if (distance < Asteroids[j].size)
                    {
                        //State = GameState.Lost;
                        player.isDead = true;
                        RespawnPlayer(player);
                    }
                }
            }
        }

        public void Move()
        {
            this.Explosions = new List<Asteroid>();

            MoveSpaceShip();
            MoveAsteroids();
            MoveBullets();

            SpawnAsteroids();
        }
    }
}
