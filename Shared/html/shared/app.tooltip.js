var app = app || {};

app.tooltip = {
	tipEl: null,
	init: function(){
		app.tooltip.tipEl = $("#tooltip1");
	},
	hideTip: function(){
		app.tooltip.tipEl.css({
			"top": "-1000px",
			"left": "-1000px"
		});
	},
	showTip: function(el, pos, html){
		
		var spacing = 10;
		
		app.tooltip.tipEl.addClass("hide");
		
		app.tooltip.tipEl.html(html);

		var tipWidth = app.tooltip.tipEl.outerWidth();
		var tipHeight = app.tooltip.tipEl.outerHeight();

		var elWidth = el.outerWidth();
		var elHeight = el.outerHeight();
		
		var elOs = el.offset();
		
		var x = 0;
		var y = 0;
		
		var viewport = $("body");
		
		var viewportHeight = viewport.outerHeight();
		var viewportWidth = viewport.outerWidth();
		
		if(pos == "right" || pos == "left"){
			y = (elOs.top + (elHeight / 2)) - (tipHeight / 2);
			if(y + tipHeight > viewportHeight - spacing){
				y = viewportHeight - spacing - tipHeight;
			}
			if(y < spacing){
				y = spacing;
			}
			if(pos == "right"){
				x = elOs.left + elWidth + spacing;
				if(x + tipWidth > viewportWidth - spacing){
					x = elOs.left - tipWidth - spacing;
				}
			}
			else if(pos == "left"){
				x = elOs.left - tipWidth - spacing;
				if(x < spacing){
					x = elOs.left + elWidth + spacing;
				}
			}
		}else if (pos == "top" || pos == "bottom"){
			
			x = (elOs.left + (elWidth / 2)) - (tipWidth / 2);
			if(x + tipWidth > viewportWidth - spacing){
				x = viewportWidth - spacing - tipWidth;
			}
			if(x < spacing){
				x = spacing;
			}

			if(pos == "bottom"){
				y = elOs.top + elHeight + spacing;
				if(y + tipHeight > viewportHeight - spacing){
					y = elOs.top - tipHeight - spacing;
				}
			}
			else if(pos == "top"){
				y = elOs.top - tipHeight - spacing;
				if(y < spacing){
					y = elOs.top + elHeight + spacing;
				}
			}
			
		}
		
		app.tooltip.tipEl.css({
			"top": y + "px",
			"left": x + "px"
		});
		
		app.tooltip.tipEl.removeClass("hide");
	}
};