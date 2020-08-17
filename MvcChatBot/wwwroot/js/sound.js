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
        case 'getyourcake':
            clip = 'get_your_cake_in.mp3';
            break;
        case 'codecake':
            clip = 'code_cake.mp3';
            break;
        case 'stopcoming':
            clip = 'stop_coming_out.mp3';
            break;
        case 'thatcode':
            clip = 'thats_code_cake.mp3';
            break;
        default:
            clip = null;
    }

    if (clip !== null) {

        var audio = new Audio(clip);
        audio.play();
    }

}
