var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

//Disable the send button until connection is established.
document.getElementById("joinGame").disabled = true;

connection.start().then(function () {
    document.getElementById("joinGame").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("PlayerJoined", function (player) {
    var li = document.createElement("li");
    document.getElementById("joinedPlayers").appendChild(li);
    li.textContent = `${player} joined the game.`;
});

connection.on("PlayerLeft", function (connectionId, player) {
    console.log(player, connectionId);
    var li = document.createElement("li");
    document.getElementById("joinedPlayers").appendChild(li);
    li.textContent = `${player} left the game.`;
    console.log(connectionId);
    var score = document.getElementById(connectionId);
    console.log(score);
    score.parentNode.removeChild(score);
});

document.getElementById("joinGame").addEventListener("click", function (event) {
    var playername = document.getElementById("playerNameInput").value;
    connection.invoke("AddPlayer", playername).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("RestartButton").addEventListener("click", function (event) {
    connection.invoke("RestartGame").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.addEventListener("keydown", function (event) {
    switch (event.key) {
        case "ArrowUp":
        case "ArrowDown":
        case "ArrowLeft":
        case "ArrowRight":
        case " ":
            keyDownHandler(event);
            break;
        default:
            return;
    }
    event.preventDefault();
});

document.addEventListener("keyup", function (event) {
    switch (event.key) {
        case "ArrowUp":
        case "ArrowDown":
        case "ArrowLeft":
        case "ArrowRight":
        case " ":
            keyUpHandler(event);
            break;
        default:
            return;
    }
    event.preventDefault();
});

function keyDownHandler(e) {
    connection.invoke("KeyDown", e.key, e.keyCode).catch(function (err) {
        return console.error(err.toString());
    });
}

function keyUpHandler(e) {
    connection.invoke("KeyUp", e.key, e.keyCode).catch(function (err) {
        return console.error(err.toString());
    });
}