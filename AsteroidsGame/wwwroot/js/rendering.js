var connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .build();

var gameframe;

connection.on("updateFrame", (frame) => {
    // Do something with the frame object
    gameframe = frame;
    render(frame);
});

connection.start()
    .then(() => console.log("SignalR connection started"))
    .catch((err) => console.error(err));

const canvas = document.getElementById("canvas");
const ctx = canvas.getContext("2d");
const scoreboard = document.getElementById("scoreboard");

const images = {};
const imageNames = ["ship", "asteroid", "bullet", "explosion_1", "background"];
imageNames.forEach((name) => {
    const img = new Image();
    img.src = `${name}.png`;
    images[name] = img;
});

function render(frame) {
    // Clear the canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Draw the background
    ctx.drawImage(images.background, 0, 0, canvas.width, canvas.height);

    // Draw the asteroids
    frame.asteroids.forEach((asteroid) => {
        ctx.drawImage(images.asteroid, asteroid.x, asteroid.y, asteroid.size, asteroid.size);
    });

    // Draw the bullets
    frame.bullets.forEach((bullet) => {
        ctx.drawImage(images.bullet, bullet.x, bullet.y, 10, 10);
    });

    // Draw the explosions
    frame.explosions.forEach((explosion) => {
        const spriteWidth = 64;
        const spriteHeight = 64;
        const spriteX = Math.floor(explosion.frame / 4) * spriteWidth;
        const spriteY = (explosion.type - 1) * spriteHeight;
        ctx.drawImage(images.explosion_1, spriteX, spriteY, spriteWidth, spriteHeight, explosion.x, explosion.y, spriteWidth, spriteHeight);
    });

    // Draw the spaceships
    frame.players.forEach((player) => {
        const img = images.ship;
        const x = player.x - player.size / 2;
        const y = player.y - player.size / 2;
        const angle = player.angle;
        const alpha = player.isDead ? 0.5 : 1.0;
        ctx.save();
        ctx.translate(x + player.size / 2, y + player.size / 2);
        ctx.rotate(angle);
        ctx.globalAlpha = alpha;
        ctx.drawImage(img, -player.size / 2, -player.size / 2, player.size, player.size);
        ctx.restore();

        // Draw the player name
        ctx.fillStyle = player.isDead ? "red" : "white";
        ctx.font = "16px Arial";
        ctx.textAlign = "center";
        ctx.fillText(player.name, player.x, player.y + player.size / 2 + 20);
    });

    // Update the scoreboard
    scoreboard.innerHTML = "";
    frame.players.forEach((player) => {
        const div = document.createElement("div");
        div.innerHTML = `${player.name}: ${player.score}`;
        scoreboard.appendChild(div);
    });
}