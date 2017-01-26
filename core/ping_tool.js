let ping = require('pinger');
let promise = require("promise");
let ms_averages = new Array();
let messages = new Array();

let amount = 0;
let failed = 0;
let lag_spikes = 0;
let WorkAmount = 0;
let region = false;
let ul_region = undefined;

function GetPercentage(quantity, percent) {
	return quantity / percent * 100;
}

function GetMSAverage(ms_s) {
	let total = 0;
	for (let i = 0; i < ms_s.length; i++) {
		total += ms_s[i];
	}
	let avg = total / ms_s.length;

	return Math.floor(avg);
}

let ping_tool = {
	init(ip, amt, reg, ul) {
		document.title = "BlizzPing - " + reg.split(" : ")[1].trim() + " - Avg: N/A";
		ul_region = ul;
		ms_averages = new Array();
		messages = new Array();

		amount = amt;
		failed = 0;
		lag_spikes = 0;
		WorkAmount = amt;
		region = reg;

		this.ip = ip;
	},

	Start() {
		return new Promise(function (fulfil, deny) {
			ping_tool.Work(function () {
				document.title = "BlizzPing - " + region.split(" : ")[1].trim() + " - Avg: " + GetMSAverage(ms_averages) + " ms";
				fulfil(messages);
			});
		});
	},

	Work(cb) {
		let DoPing = function () {
			return new Promise(function (fulfil, fail) {
				ping(ping_tool.ip, function (err, ms) {
					if (err) {
						fail(err);
						return;
					}

					fulfil(parseInt(ms));
				});
			});
		};

		let task = function (current) {
			if (current > amount) {
				cb();
				return;
			}

			DoPing(this.ip).then(function (data) {
				let msg = region + " : " + data;
				let lag_spike = false;
				if (data > 250) {
					msg += " - Lag Spike";
					lag_spike = true;
				}

				ping_tool.AddLogItem({
					"region": region,
					"ms": data,
					"lag_spike": lag_spike
				});
				messages.push(msg);
				ms_averages.push(data);
				current += 1;
				task(current);
			}).catch(function (err) {
				messages.push(region + " : " + err.message);
				ping_tool.AddLogItem({
					"region": region,
					"error": err.message
				});
				current += 1;
				failed += 1;
				task(current);
			});
		};

		task(1);
	},

	AddLogItem(msg) {
		let $li = $("<li></li>");
		$li.addClass("list-group-item");

		let $badge = $("<span></span>");
		$badge.addClass("badge");

		if (msg.error) {
			$badge.html(msg.error);
			$li.addClass("list-group-item-danger");
			$li.css("color", "white");
		} else {
			$badge.html(msg.ms + " ms");
		}

		if (msg.lag_spike) {
			$li.addClass("list-group-item-warning");
			$li.css("color", "white");
		}
		$li.append($badge);
		$li.append(msg.region);
		ul_region.append($li);
	}
}

module.exports = ping_tool;
