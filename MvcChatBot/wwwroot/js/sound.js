"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


connection.on("SoundMessage", function (user, message) {
    playSound(message);
});

connection.start().then(function () {

}).catch(function (err) {
    return console.error(err.toString());
});

function playSound(message) {
    let clip = '';

    switch (message) {
        case 'balls':
            clip = 'layla_balls.mp3';
            break;
       
        default:
            clip = null;
    }

    if (clip !== null) {

        var audio = new Audio(clip);
        audio.play();
    }

}
