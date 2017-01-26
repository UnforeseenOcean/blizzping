let path = require("path");

let gameHandler = require("./core/games_list.js");

$(window).on("DOMContentLoaded", function () {

	gameHandler.CreateList($("select.region_selector"));
    
	$(".selecter_1").selecter({
		cover: true,
		label: "Select Game/Region",
		callback: gameHandler.handleGame
	});

});
