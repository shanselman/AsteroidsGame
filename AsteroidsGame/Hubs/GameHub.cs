using Microsoft.AspNetCore.SignalR;
using Asteroids.GameComponents;

namespace Asteroids.Hubs
{
    public class GameHub : Hub
    {
        private static GameManager _game;
        private FrameStreamer _frameStreamer;

        public GameHub(GameManager game, FrameStreamer frameStreamer)
        {
            _game = game;
            _frameStreamer = frameStreamer;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            _frameStreamer.StartStreamingAsync(); // start the streaming task
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var player = _game.GetPlayer(Context.ConnectionId);
            
            if (player != null)
            {
                var connectionId = Context.ConnectionId;
                await _game.RemovePlayer(Context.ConnectionId);
                await Clients.All.SendAsync("PlayerLeft", connectionId, player.name);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task AddPlayer(string name)
        {
            _game.AddPlayer(Context.ConnectionId, name);

            await Clients.All.SendAsync("PlayerJoined", name);
        }

        public async Task RestartGame()
        {
            _game.Initialzie();
        }

        public async Task KeyDown(string key, int keyCode)
        {
            if (!_game.isRunning())
            {
                return;
            }
            Player player = _game.GetPlayer(Context.ConnectionId);
            if (player != null)
            {
                float acceleration = player.acceleration;
                float angle = player.angle;
                float speed = player.speed;
                if (key == "Up" || key == "ArrowUp")
                {
                    acceleration = 1;
                    speed = player.speed + 0.2f;
                }
                else if (key == "Down" || key == "ArrowDown")
                {
                    speed = player.speed - 0.2f;
                }
                else if (key == "Right" || key == "ArrowRight")
                {
                    angle = player.angle + 0.1f;
                }
                else if (key == "left" || key == "ArrowLeft")
                {
                    angle = player.angle - 0.1f;
                }
                else if (keyCode == 32 && _game.GetPlayerBulletCount(Context.ConnectionId) < 5)
                {
                    _game.AddBullet(Context.ConnectionId, player.x, player.y, angle);
                }
                player.Update(player.x, player.y, angle, speed, acceleration);
            }
        }

        public async Task KeyUp(string key, int keyCode)
        {
            if (!_game.isRunning())
            {
                return;
            }
            Player player = _game.GetPlayer(Context.ConnectionId);
            if (player is not null)
            {
                float acceleration = player.acceleration;
                float angle = player.angle;
                float speed = player.speed;
                if (key == "Up" || key == "ArrowUp")
                {
                    acceleration = 0;
                    speed = player.speed - 0.2f;
                }
                else if (key == "Down" || key == "ArrowDown")
                {
                }
                player.Update(player.x, player.y, angle, speed, acceleration);
            }
        }

    }
}
