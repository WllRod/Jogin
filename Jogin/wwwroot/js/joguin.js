"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/joguin").build();
var mousePos;

var myID = ""
var container = document.querySelector('.containers')
container.addEventListener('mousemove', function (event) {
    var dot, eventDoc, doc, body, pageX, pageY;

    event = event || window.event; // IE-ism

    // If pageX/Y aren't available and clientX/Y are,
    // calculate pageX/Y - logic taken from jQuery.
    // (This is to support old IE)
    if (event.pageX == null && event.clientX != null) {
        eventDoc = (event.target && event.target.ownerDocument) || document;
        doc = eventDoc.documentElement;
        body = eventDoc.body;

        event.pageX = event.clientX +
            (doc && doc.scrollLeft || body && body.scrollLeft || 0) -
            (doc && doc.clientLeft || body && body.clientLeft || 0);
        event.pageY = event.clientY +
            (doc && doc.scrollTop || body && body.scrollTop || 0) -
            (doc && doc.clientTop || body && body.clientTop || 0);
    }

    mousePos = {
        x: event.pageX,
        y: event.pageY
    };


    var player = document.querySelector(`.player[player-id="${myID}"]`)

    player.style.left = `${mousePos.x}px`
    player.style.top = `${mousePos.y}px`

    connection.invoke("PlayerPos", myID, mousePos.x, mousePos.y)

    var players = document.querySelectorAll('.player')

    players.forEach(item => {
        var connectionID = item.getAttribute("player-id")

        if (connectionID !== myID) {
            if (isCollide(player, item)) {
                connection.invoke("EliminatePlayer", myID, connectionID)

                var size = parseInt(player.style.width)+5
                player.style.width = `${size}px`
                player.style.height = `${size}px`

                container.removeChild(item)
            }
        }
    })
})

//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;



connection.start().then(function (e) {
    
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessage", function (e) {
    var data = JSON.parse(e)

    for (var i = 0; i < data.length; i++) {
        var player = document.createElement('div')

        player.style.left = `${data[i].posX}px`
        player.style.top = `${data[i].posY}px`
        player.style.width = `${data[i].size}px`
        player.style.height = `${data[i].size}px`


        player.classList.add('player')

        if (myID !== data[i].Name) {
            player.setAttribute("player-id", data[i].Name)
        }
        else {
            player.setAttribute("player-id", myID)
        }
        
        container.appendChild(player)
    }
});


connection.on("MyId", function (e) {
    myID = e;
});

connection.on("EnemyPos", function (e) {
    var data = JSON.parse(e)

    for (var i = 0; i < data.length; i++) {
        var player = document.querySelector(`.player[player-id="${data[i].Name}"]`)

        player.style.left = `${data[i].posX}px`
        player.style.top = `${data[i].posY}px`
        player.style.height = `${data[i].size}px`
        player.style.width = `${data[i].size}px`


        
    }
});

connection.on("ILost", function (e) {
   
    window.location.href = 'https://google.com'
});

function isCollide(a, b) {
    var aRect = a.getBoundingClientRect();
    var bRect = b.getBoundingClientRect();

    return !(
        ((aRect.top + aRect.height) < (bRect.top)) ||
        (aRect.top > (bRect.top + bRect.height)) ||
        ((aRect.left + aRect.width) < bRect.left) ||
        (aRect.left > (bRect.left + bRect.width))
    );
}