using System.Data;

namespace Asteroids.GameComponents
{
    // Define a class Player that has the following properties: connectionId, name, float x, float y, float angle, float speed, float acceleration, int size = 50, uint Score=0, bool isDead=false
    // Define a constructor that takes ConnectionId, Name, x, y as parameters
    // Define a method Update(x, y, angle, speed, acceleration)
    // Define a method Update(x, y)
    // all functions and properties are public
    public class Player
    {
        public string connectionId { get; set; }
        public string name { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float angle { get; set; }
        public float speed { get; set; }
        public float acceleration { get; set; }
        public int size { get; set; } = 50;
        public uint Score { get; set; } = 0;
        public bool isDead { get; set; } = false;

        public Player(string ConnectionId, string Name, float x, float y)
        {
            connectionId = ConnectionId;
            name = Name;
            this.x = x;
            this.y = y;
        }

        public void Update(float x, float y, float angle, float speed, float acceleration)
        {
            this.x = x;
            this.y = y;
            this.angle = angle;
            this.speed = speed;
            this.acceleration = acceleration;
        }

        public void Update(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }


    // Define a Bullet class that has the following properties: connectionId, float x, float y, float angle, float speed = 10, int size = 30
    // Define a constructor that takes ConnectionId, x, y, angle as parameters
    // Define a method Update(x, y)
    // all functions and properties are public
    public class Bullet
    {
        public string connectionId { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float angle { get; set; }
        public float speed { get; set; } = 10;
        public int size { get; set; } = 30;

        public Bullet(string ConnectionId, float x, float y, float angle)
        {
            connectionId = ConnectionId;
            this.x = x;
            this.y = y;
            this.angle = angle;
        }

        public void Update(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }


    // Define a class Asteroid that has the following properties: float x, float y, float dx, float dy, int size
    // Define a constructor that takes x, y, dx, dy, size as parameters
    // Define a method Update(x, y)
    // all functions and properties are public
    public class Asteroid
    {
        public float x { get; set; }
        public float y { get; set; }
        public float dx { get; set; }
        public float dy { get; set; }
        public int size { get; set; }

        public Asteroid(float x, float y, float dx, float dy, int size)
        {
            this.x = x;
            this.y = y;
            this.dx = dx;
            this.dy = dy;
            this.size = size;
        }

        public void Update(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

}

