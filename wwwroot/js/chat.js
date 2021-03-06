﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (board) {

    //board is 1d array of json objects, each object will have
    //two items: if the cell is on or off and the color

    //receive the board in json format

    //deserialize board

    //render the board


    //deserilize json
    board = JSON.parse(board);

    //build board
    var container = document.getElementById("container");
    
    container.appendChild(grid(18, 10, 600, board));




});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {


    //board is 1d array of json objects, each object will have
    //two items: if the cell is on or off and the color

    //serialize the board into json

    //send the board to the server:
    connection.invoke("SendMessage", board).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});






//connection.on("ReceiveMessage", function (user, message) {

//    //board is 1d array of json objects, each object will have
//    //two items: if the cell is on or off and the color

//    //receive the board in json format

//    //deserialize board

//    //render the board



//    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
//    var encodedMsg = user + " says " + msg;
//    var li = document.createElement("li");
//    li.textContent = encodedMsg;
//    document.getElementById("messagesList").appendChild(li);
//});

//connection.start().then(function () {
//    document.getElementById("sendButton").disabled = false;
//}).catch(function (err) {
//    return console.error(err.toString());
//});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;

//    //board is 1d array of json objects, each object will have
//    //two items: if the cell is on or off and the color

//    //serialize the board into json

//    //send the board to the server:
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});