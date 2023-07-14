var app = app || {};

// USEFUL FUNCTIONS

app.uf = {
	sigmoid: function(t) {
		return 1 / (1 + Math.pow(Math.E, -t));
	},
	// Returns a random integer between min (inclusive) and max (inclusive).
	getRandomInt: function(min, max) {
		min = Math.ceil(min);
		max = Math.floor(max);
		return Math.floor(Math.random() * (max - min + 1)) + min;
	},
	getAdjustedColor: function(c, perc){
		perc = perc > 1 ? 1 : (perc < 0 ? 0 : perc);
		return "rgb(" + app.uf.roundNumber((c.r * perc),0) + "," + app.uf.roundNumber((c.g * perc),0) + "," + app.uf.roundNumber((c.b * perc),0) + ")";
	},
	roundNumber: function(n, digits) {
		try{
			return Number((n).toFixed(digits));
		}catch(e){
			
		}
		return n;
	},
};