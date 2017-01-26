let fs = require("fs");
let path = require('path');
let p = path.join(__dirname, '..', 'ips.json');
let ping = require("./ping_tool.js");

let working = false;
let games_list = {
	init() {
		this.games = JSON.parse(fs.readFileSync(p).toString());
	},

	CreateList(object) {
		if (!this.games) this.init();

		for (let i = 0; i < this.games.length; i++) {
			let game = this.games[i];

			let header = this.CreateGameHeader(game.name);
			let region_list = Object.keys(game.ips);

			for (let reg = 0; reg < region_list.length; reg++) {
				for (let ip_idx = 0; ip_idx < game.ips[region_list[reg]].length; ip_idx++) {
					let game_item = this.CreateGameItem(region_list[reg].toLocaleUpperCase() + " Server : " + game.ips[region_list[reg]][ip_idx], game.ips[region_list[reg]][ip_idx]);

					header.append(game_item);
				}
			}

			object.append(header);
		}
	},

	CreateGameHeader(name) {
		let $opt_group = $("<optgroup></optgroup>");

		$opt_group.attr("label", name);

		return $opt_group;
	},

	CreateGameItem(region, ip) {
		let $opt = $("<option></option>");

		$opt.attr("value", JSON.stringify({
			"ip": ip,
			"region": region
		}));
		$opt.html(region);

		return $opt;
	},

	handleGame(value, index) {
		if (value.length <= 0) return;

		if(working) return;
		$("ul.data_output").empty();

		working = true;
		value = JSON.parse(value);

		ping.init(value.ip, 10, value.region, $("ul.data_output"));
		ping.Start().then(function(data){
			console.log(data);
			working = false;
		});
	}
}

module.exports = games_list;
