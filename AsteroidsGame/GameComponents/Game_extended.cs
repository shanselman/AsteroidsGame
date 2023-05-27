
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Asteroids.GameComponents
{
    public partial class Game
    {

        // Collision detection between ship and ship
        public void ShipShipCollisionDetection()
        {
            if (this.Players.Count < 2) { return; }

            for (int i = 0; i < Players.Count; i++)
            {
                for (var j = i + 1; j < Players.Count; j++)
                {
                    var playerA = Players.ElementAt(i).Value;
                    var playerB = Players.ElementAt(j).Value;
                    var dx = playerA.x - playerB.x;
                    var dy = playerA.y - playerB.y;
                    var distance = MathF.Sqrt(dx * dx + dy * dy);
                    if (distance < playerA.size)
                    {
                        playerA.isDead = true;
                        playerB.isDead = true;
                        RespawnPlayer(playerA);
                        RespawnPlayer(playerB);
                    }
                }
            }
        }

        private void SpawnAsteroids()
        {
            if (this.Players.Count == 0 && this.Asteroids.Count >= AsteroidsCount)
            {
                return;
            }

            int required = AsteroidsCount - this.Asteroids.Count;

            for (int i = 0; i < required; ++i)
            {
                bool retry;
                Asteroid newAsteriod = null;

                do
                {
                    retry = false;

                    float x = (float)(_random.NextDouble() * _width);
                    float y = (float)(_random.NextDouble() * _height);
                    float dx = (float)(_random.NextDouble() * 2 - 1);
                    float dy = (float)(_random.NextDouble() * 2 - 1);
                    int size = _random.Next(40) + 50;

                    switch (_random.Next() % 4)
                    {
                        case 0:
                            x = 50;
                            break;
                        case 1:
                            x = _width - 50;
                            break;
                        case 2:
                            y = 50;
                            break;
                        case 3:
                            y = _height - 50;
                            break;
                    }

                    newAsteriod = new Asteroid(x, y, dx, dy, size);

                    foreach (var player in this.Players.Values)
                    {
                        var distance = MathF.Sqrt(MathF.Pow(player.x - newAsteriod.x, 2) + MathF.Pow(player.y - newAsteriod.y, 2));
                        if (distance < newAsteriod.size + 100)
                        {
                            retry = true;
                            break;
                        }
                    }
                } while (retry);

                if (newAsteriod != null)
                {
                    this.Asteroids.Add(newAsteriod);
                }
            }
        }

        public void AddPlayer(string connectionId, string name, float x = 0, float y = 0)
        {
            Player player = new Player(connectionId, name, x, y);
            lock (Players)
            {
                Players.Add(connectionId, player);
            }
        }

        public void AddBullet(string connectionId, float x, float y, float angle)
        {
            if (Players[connectionId].isDead) return;
            Bullet bullet = new Bullet(connectionId, x, y, angle);
            lock (Bullets)
            {
                Bullets.Add(bullet);
            }
        }

        public async Task RespawnPlayer(Player player)
        {
            await Task.Delay(5000);
            player.isDead = false;
        }

    }
}
