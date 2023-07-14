var app = app || {};

app.canvas = {
	width: 0,
	height: 0,
	ctx: null,
	init: function(){
		var canvas = document.getElementById("canvas");
		app.canvas.ctx = canvas.getContext("2d");
		app.canvas.ctx.clearRect(0, 0, canvas.width, canvas.height);
		app.canvas.width = canvas.width;
		app.canvas.height = canvas.height;
	},
	paintJobs: [],
	render: function(){

		var pj = app.canvas.paintJobs.slice().sort(function(a, b) {
			if (a.zIndex < b.zIndex) {
				return -1;
			}
			if (a.zIndex > b.zIndex) {
				return 1;
			}
			return 0;
		});
		
		app.canvas.paintJobs = [];
		
		for(var n = 0; n < pj.length; n++){
		
			app.canvas.ctx.save();

			if(pj[n].draw == "circle"){
				app.canvas.ctx.beginPath();
				app.canvas.ctx.arc(Math.floor(pj[n].pos.x), Math.floor(pj[n].pos.y), pj[n].rad, 0, 2 * Math.PI, false);
				app.canvas.ctx.fillStyle = pj[n].fill;
				app.canvas.ctx.fill();
			}else if(pj[n].draw == "line"){
				if(pj[n].dotted){
					app.canvas.ctx.setLineDash(pj[n].dotted);/*dashes are 5px and spaces are 3px*/
				}
				app.canvas.ctx.beginPath();
				app.canvas.ctx.strokeStyle = pj[n].color;
				app.canvas.ctx.lineWidth = pj[n].thickness;
				app.canvas.ctx.moveTo(Math.floor(pj[n].start.x), Math.floor(pj[n].start.y));
				app.canvas.ctx.lineTo(Math.floor(pj[n].end.x), Math.floor(pj[n].end.y));
				app.canvas.ctx.stroke();

			}else if(pj[n].draw == "text"){
				
				app.canvas.ctx.shadowOffsetX = 1;
				app.canvas.ctx.shadowOffsetY = 1;
				app.canvas.ctx.shadowBlur = 0;
				app.canvas.ctx.shadowColor = "rgba(0,0,0,1)";
				app.canvas.ctx.font = pj[n].font;
				app.canvas.ctx.fillStyle = pj[n].color;
				app.canvas.ctx.fillText(pj[n].value, pj[n].pos.x, pj[n].pos.y);
				
			}else if(pj[n].draw == "square"){
				app.canvas.ctx.strokeStyle = pj[n].color;
				app.canvas.ctx.lineWidth = pj[n].thickness;
				app.canvas.ctx.strokeRect(Math.floor(pj[n].pos.x), Math.floor(pj[n].pos.y), Math.floor(pj[n].size.w), Math.floor(pj[n].size.h));

			}else if(pj[n].draw == "fillSquare"){
				app.canvas.ctx.fillStyle = pj[n].color;
				app.canvas.ctx.lineWidth = 0;
				app.canvas.ctx.fillRect(Math.floor(pj[n].pos.x), Math.floor(pj[n].pos.y), Math.floor(pj[n].size.w), Math.floor(pj[n].size.h));

			}

			app.canvas.ctx.restore();

		}
		
	}
	
}