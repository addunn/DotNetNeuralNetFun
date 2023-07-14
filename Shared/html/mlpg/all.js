function log(o){
	console.log(o);
}

var app = app || {};


app.selectedProjectItem = null;
app.projects = [];
app.els = {
	body: null,
	networksList: null
};
app.consts = {
	largeNegative: -9999999999,
	largePositive: 9999999999
};
app.state = {
	showNetwork: false,
	showStats: true
};
app.layout = {
	network: {
							
	},
	computeLayout: function(){
		
		var w = app.canvas.width;// * (2 / 3);
		var h = app.canvas.height;// * (5 / 6);

		app.layout.network = {
			width: w,
			height: h,
			x: app.canvas.width - w,
			y: app.canvas.height - h
		};
		
	},
	refreshCanvas: function(){
		
		var el1 = $("#selected-network-panel");
		
		var h1 = el1.height();
		
		var h2 = el1.find(".panel-title").outerHeight();
		
		var h3 = 0;
		el1.find(".panel-section").each(function(n){
			if(n < 1){
				h3 += $(this).outerHeight(true);
			}
		});
		var canv = document.getElementById("canvas");
		var w = $("#selected-network-panel .panel-section").width();
		var h = ((h1 - (h2 + h3)) - 10);
		canv.width = w;
		canv.height = h;
		app.canvas.width = w;
		app.canvas.height = h;
		$("#canvas").css({"height": h + "px", "width": w + "px"});
		
		
	}
};
app.ui = {
	// builds the containers for the projects
	buildProjects: function(){
		// create all the panels and nodes
		
		var el = $("#projects-networks-lists");
		
		el.empty();
		
		
		for(var n = 0; n < app.projects.length; n++){
			var div = $(document.createElement("div"));
			div.addClass("panel-section");
			div.addClass("project-networks-lists-inner");
			
			var label = $(document.createElement("label"));
			label.html(app.projects[n].name);
			
			
			
			var ul = $(document.createElement("ul"));
			ul.addClass("shrink4");
			ul.addClass("networks-list");
			ul.data("project-index", n);
			ul.attr("id", "project" + app.projects[n].index + "-network-list");
			
			
			div.append(label);
			div.append(ul);
			el.append(div);
			
			
		}
		
		
		
		
		// add project index data to UL
		$(".projects-count").html(app.projects.length);
	},
	// just deletes and adds the LIs
	refreshNetworks: function(projectIndex){
		
		var ul = $("#project" + projectIndex + "-network-list");
				
		ul.empty();
				
		if(app.projects[projectIndex] != null){
			
			var id = "";
			
			if (app.selectedProjectItem != null && app.selectedProjectItem.hasOwnProperty("id")) {
				id = app.selectedProjectItem.id;
			}

			for(var n = 0; n < app.projects[projectIndex].projectItems.length; n++){
				var el = $(document.createElement("li"))
				
				el.data({"id":app.projects[projectIndex].projectItems[n].id,"index":n,"name":app.projects[projectIndex].projectItems[n].name});
				el.addClass("network-node");
				el.attr("id", "node-" + app.projects[projectIndex].projectItems[n].id);
				
				if(app.projects[projectIndex].projectItems[n].enabled){
					el.addClass("training-enabled");
				}
				if(app.projects[projectIndex].projectItems[n].id == id){
					el.addClass("selected");
				}
				ul.append(el);
			}
		}
	},
	drawCanvasData: function(){
		
		app.layout.refreshCanvas();

		app.layout.computeLayout();
		
		
		app.graphs.data = app.selectedProjectItem.stats;
		app.nn.network = app.selectedProjectItem.network;
		
		if(app.state.showNetwork){
			app.nn.buildPaintJobs();
		} else {
			app.graphs.computeGraphs();
			app.graphs.buildPaintJobs();
		}
		

		app.canvas.render();
	},
	currentProjectItemTraining: function(index, id){
		var ul = $("#project" + index + "-network-list");
		ul.find(".network-node.training").removeClass("training");
		$("#node-" + id).addClass("training");
	},
	logMessage: function(obj){
		var el = $("#log-panel");
		el.append("<p>" + obj + "</p>");
		el.scrollTop = el.scrollHeight;
	},
	refreshSelectedNetwork: function(){
		if(app.selectedProjectItem != null){
			if (app.selectedProjectItem.hasOwnProperty("id")) {
				
				$(".selected-network-name").html(app.projects[app.selectedProjectItem.projectIndex].name + " - " + app.selectedProjectItem.name);
				
				app.ui.drawCanvasData();
				
					

				
				$("#selected-network-panel .panel-section").removeClass("hide");
				$(".selected-network-loading").addClass("display-none");
			}
		}
	}
};
app.events = {
	networkNodeMouseEnter: function(e){
		log("enter");

		var t = $(this);
		var ul = t.closest("ul");
		var projectIndex = ul.data("project-index");
		
		var nodeIndex  = t.data("index");
		
		var projItem = app.projects[projectIndex].projectItems[nodeIndex];
		
		var topHtml = "<div class=\"data-columns\"><div class=\"left-column\">";
		var middleHtml = "</div><div class=\"right-column\">";
		var bottomHtml = "</div></div>";
		
		var leftColumnHtml = "";
		var rightColumnHtml = "";

		leftColumnHtml += "<div class=\"column-item\">name:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.name + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">id:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.id + "</div>";

		leftColumnHtml += "<div class=\"column-item\">cost:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["performance"]["cost"] + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">percCorrect:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["performance"]["percCorrect"] + "</div>";

		leftColumnHtml += "<div class=\"column-item\">learningRate:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["hyperParameters"]["learningRate"] + "</div>";

		leftColumnHtml += "<div class=\"column-item\">iterations:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.iterations + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">iterationStopwatch:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["training"]["iterationStopwatch"] + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">useMomentum:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["training"]["useMomentum"] + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">useAdaGrad:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["training"]["useAdaGrad"] + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">useRMSProp:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["training"]["useRMSProp"] + "</div>";
		
		
		leftColumnHtml += "<div class=\"column-item\">useAdam:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["training"]["useAdam"] + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">useDynamicDropOut:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["training"]["useDynamicDropOut"] + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">useRegularDropOut:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["training"]["useRegularDropOut"] + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">dynamicDropoutLayerNodeCountDivisor:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["hyperParameters"]["dynamicDropoutLayerNodeCountDivisor"] + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">regularDropoutProbability:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["hyperParameters"]["regularDropoutProbability"] + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">averageDisabledNodes:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["network"]["averageDisabledNodes"] + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">totalLayers:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["network"]["totalLayers"] + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">totalNodes:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["network"]["totalNodes"] + "</div>";
		
		leftColumnHtml += "<div class=\"column-item\">backPropCallsCount:</div>";
		rightColumnHtml += "<div class=\"column-item\">" + projItem.stats["training"]["backPropCallsCount"] + "</div>";

		var html = topHtml + leftColumnHtml + middleHtml + rightColumnHtml + bottomHtml;
		app.tooltip.showTip($(this), "right", html);
	},
	networkNodeMouseLeave: function(e){
		log("leave");
		app.tooltip.hideTip();
		
	},
	windowResized: function(){
		app.layout.refreshCanvas();
	},
	networkNodeClick: function(e){

		if(app.projects.length > 0){
			
			var id = "";

			
				
			if (app.selectedProjectItem != null && app.selectedProjectItem.hasOwnProperty("id")) {
				id = app.selectedProjectItem.id;
			}
			
			
			if($(this).data("id") != id){
				var ul = $(this).closest("ul");
				var projectIndex = ul.data("project-index");
				app.formSend.uiCommand("set-selected-projectitem:" + projectIndex + ":" + $(this).data("id") + ":-1");
				// looks like the user picked one that isn't already selected
				$(".network-node.selected").removeClass("selected");
				$(this).addClass("selected");
				$(".selected-network-name").html(app.projects[projectIndex].name + " - " + $(this).data("name"));
				$("#selected-network-panel .panel-section").addClass("hide");
				$(".selected-network-loading").removeClass("display-none");
			}
			
		}

	},
	actionStartTraining: function(e){
		$(".start-training").addClass("disabled");
		$(".stop-training").removeClass("disabled");
		app.formSend.uiCommand("start-training");
	},
	actionShowStats: function(e){
		$(".show-stats").addClass("disabled");
		$(".show-network").removeClass("disabled");
		app.state.showStats = true;
		app.state.showNetwork = false;
		app.ui.drawCanvasData();
	},
	actionShowNetwork: function(e){
		$(".show-network").addClass("disabled");
		$(".show-stats").removeClass("disabled");
		app.state.showStats = false;
		app.state.showNetwork = true;
		app.ui.drawCanvasData();
	},
	actionLoadProject: function(e){
		
	},
	actionSaveProject: function(e){
		
	}
};
app.graphs = {
	config: {
		colors: {
			graphPlot: 0,
			graphBorder: 0,
			graphBackground: 0,
			graphAxis: 0,
			graphText: 0
		}
	},
	data: {
		
	},
	getPaintJobsForGraph: function(g){
		
		var pjs = [];

		var c = {
			borderThickness: 4
		};
		
		var innerWidth = app.uf.roundNumber(g.width - (c.borderThickness * 2), 0);
		var innerHeight = app.uf.roundNumber(g.height - (c.borderThickness * 2), 0);
		
		// compute min and max of the windowed values
		
		var min = app.consts.largePositive;
		var max = app.consts.largeNegative;
		//for(var n = g.data.length - 1; g.data.length - n < innerWidth && n >= 0; n--){
			
		var nStart = app.uf.roundNumber(g.data.length - innerWidth, 0);
		
		if(nStart < 0){
			nStart = 0;
		}
		
		for(var n = nStart; n < g.data.length; n++){
			if(g.data[n] > max){
				max = g.data[n];
			}
			if(g.data[n] < min){
				min = g.data[n];
			}
		}
		pjs.push({
			draw: "fillSquare",
			color: app.graphs.config.colors.graphBackground,
			pos: { x: g.x, y: g.y },
			size: { w: g.width, h: g.height },
			zIndex: 200
		});
		
		pjs.push({
			draw: "square",
			color: app.graphs.config.colors.graphBorder,
			thickness: c.borderThickness,
			pos: { x: g.x, y: g.y },
			size: { w: g.width, h: g.height },
			zIndex: 1000
		});
	
		pjs.push({
			draw: "text",
			value: g.label + ": " + app.uf.roundNumber(g.data[g.data.length - 1], 6),
			font: "12px Saira Semi Condensed",
			color: app.graphs.config.colors.graphText,
			pos: {x: g.x + c.borderThickness + 4, y: g.y + c.borderThickness + 13},
			zIndex: 5000
		});
		
		pjs.push({
			draw: "text",
			value: app.uf.roundNumber(min, 6),
			font: "12px Saira Semi Condensed",
			color: app.graphs.config.colors.graphText,
			pos: {x: g.x + c.borderThickness + 4, y: g.y + c.borderThickness + innerHeight - 4},
			zIndex: 5000
		});

		var diff = max - min;
		
		var lineYHalf = (innerHeight / 2) + g.y + c.borderThickness;

		pjs.push({
			draw: "line",
			color: app.graphs.config.colors.graphAxis,
			start: {x: g.x + c.borderThickness, y: lineYHalf},
			end: {x: g.x + c.borderThickness + innerWidth, y: lineYHalf},
			thickness: 1,
			dotted: [1,4],
			zIndex: 300
		});
		
		pjs.push({
			draw: "text",
			value: app.uf.roundNumber(max - (diff * 0.5), 6),
			font: "12px Saira Semi Condensed",
			color: app.graphs.config.colors.graphText,
			pos: {x: g.x + c.borderThickness + 4, y: lineYHalf + 4},
			zIndex: 5000
		});

		var forward = (innerWidth > g.data.length);
		
		
		if(forward){
		
			for(var n = 0; n < g.data.length; n++){

				var y = ((g.data[n] - min) / diff) * innerHeight;

				y = g.y + c.borderThickness + innerHeight - y;

				pjs.push({
					draw: "square",
					color: app.graphs.config.colors.graphPlot,
					pos: { x: g.x + c.borderThickness + n, y: y },
					size: { w: 1, h: 1 },
					zIndex: 1000
				});

			}
		}else{

			var m = 0;

			
			for(var n = g.data.length - innerWidth; n < g.data.length; n++){

				var y = ((g.data[n] - min) / diff) * innerHeight;

				y = g.y + c.borderThickness + innerHeight - y;
				
				pjs.push({
					draw: "square",
					color: app.graphs.config.colors.graphPlot,
					pos: { x: g.x + c.borderThickness + m, y: y },
					size: { w: 1, h: 1 },
					zIndex: 1000
				});
				
				m++;

			}				
		}

		return pjs;
	},
	buildPaintJobs: function(){
		
		
		var topBarGraphs = 0;
		var sideBarGraphs = 0;
		
		var columns = 3;
		var totalGraphs = 0;
		
		for (var key1 in app.graphs.graphsEnabled) {
			for (var n = 0; n < app.graphs.graphsEnabled[key1].length; n++) {
				totalGraphs++;
			}
		}


		var totalRows = Math.ceil(totalGraphs / columns);


		var count = 0;
		
		var currentColumn = 0;
		var currentRow = 0;

		for (var key1 in app.graphs.graphsEnabled) {
			
			for (var n = 0; n < app.graphs.graphsEnabled[key1].length; n++) {

				var height = app.canvas.height / totalRows;
				var width = app.canvas.width / columns;
				
				currentColumn = Math.floor(count / totalRows);
				currentRow =  count % totalRows;
				

				var x = currentColumn * width;
				var y = currentRow * height;
				

				var obj = {
					width: width,
					height: height,
					x: x,
					y: y,
					label: app.graphs.graphsEnabled[key1][n],
					data: app.graphs.data[key1][app.graphs.graphsEnabled[key1][n]],
					min: app.graphs.graphsData[key1][app.graphs.graphsEnabled[key1][n]].min,
					max: app.graphs.graphsData[key1][app.graphs.graphsEnabled[key1][n]].max
				};
				

				var pjs = app.graphs.getPaintJobsForGraph(obj);

				for(var n1 = 0; n1 < pjs.length; n1++){
					app.canvas.paintJobs.push(pjs[n1]);
				}
				count++;


			}
			
		}

	},

	graphsEnabled: {
		performance: ["cost","percCorrect"],
		network: ["totalWeightValue", "averageWeightValue","minBiasValue", "maxBiasValue", "minWeightValue", "maxWeightValue", "averageBiasValue"],
		hyperParameters: [],
		training: ["iterationStopwatch"]
	},
	graphsData: {
		performance: {},
		network: {},
		hyperParameters: {},
		training: {}
	},
	computeGraphs: function(){

		var allKeys = ["performance","network","hyperParameters","training"]

		for(var k = 0; k < allKeys.length; k++){
			
			var mainKey = allKeys[k];
			app.graphs.graphsData[mainKey] = [];
			
			for(var n = 0; n < app.graphs.graphsEnabled[mainKey].length; n++){
			
				var min = app.consts.largePositive;
				var max = app.consts.largeNegative;
					
				var key = app.graphs.graphsEnabled[mainKey][n];
				
				app.graphs.graphsData[mainKey][key] = {
					min: min,
					min: max
				};

				for(var m = 0; m < app.graphs.data[mainKey][key].length; m++){

					var val = app.graphs.data[mainKey][key][m];
					
					if(val < min){
						min = val;
					}
					if(val > max){
						max = val;
					}
					
				}
				
				app.graphs.graphsData[mainKey][key].max = max;
				app.graphs.graphsData[mainKey][key].min = min;
			}
		}

	},
	init: function(){
		app.graphs.config.colors.graphPlot = $("#graph-plot").css("background-color");
		app.graphs.config.colors.graphBorder = $("#graph-border").css("background-color");
		app.graphs.config.colors.graphBackground = $("#graph-background").css("background-color");
		app.graphs.config.colors.graphAxis = $("#graph-axis").css("background-color");
		app.graphs.config.colors.graphText = $("#graph-text").css("background-color");
	}
};
app.formSend = {
	uiCommand: function(cmd){
		boundAsync.uiCommand(cmd);
	}
};
app.formReceive = {
	networkListRefresh: function(obj){
		log("networkListRefresh");
		// replace the items with the new list
		var o = JSON.parse(obj);
		var index = o.projectIndex;
		app.projects[index].projectItems = o.items;
		app.ui.refreshNetworks(index);
	},
	selectedProjectItemRefresh: function(obj){
		log("selectedProjectItemRefresh");
		// replace the items with the new list
		app.selectedProjectItem = JSON.parse(obj);
		// app.project.selectedProjectItem = JSON.parse(obj);
		app.ui.refreshSelectedNetwork();
	},
	buildAllProjects: function(){
		log("buildAllProjects");
		app.ui.buildProjects();
		app.ui.refreshNetworks();
		app.ui.refreshSelectedNetwork();
	},
	projectAdd: function(obj){
		log("projectRefresh");
		var p = JSON.parse(obj);
		app.projects[p.index] = p;
		
	},
	logMessage: function(obj){
		app.ui.logMessage(obj);
	},
	currentProjectItemTrainingRefresh: function(index, id){
		app.ui.currentProjectItemTraining(index, id);
	}
};
app.initEvents = function(){
	
	$(document).on("click", ".start-training:not(.disabled)", app.events.actionStartTraining);
	$(document).on("click", ".show-stats:not(.disabled)", app.events.actionShowStats);
	$(document).on("click", ".show-network:not(.disabled)", app.events.actionShowNetwork);
	$(document).on("click", ".network-node:not(.disabled)", app.events.networkNodeClick);
	
	
	$(document).on("mouseenter", ".network-node", app.events.networkNodeMouseEnter);
	$(document).on("mouseleave", ".network-node", app.events.networkNodeMouseLeave);
	
	$(window).resize(function() {
		app.events.windowResized();
	});
	 
	
};
app.initLoad = function(){
	app.els.body.removeClass("hide");
	
	app.layout.computeLayout();
	
	app.graphs.init();
};
app.initReady = function(){
	
	// cache elements that are queried a lot
	app.els.body = $("#body");

	app.els.networksList = $("#networks-list");

	app.canvas.init();
	app.tooltip.init();
	
	

	app.initEvents();
	
	// set up communication with form
	if(window.hasOwnProperty('CefSharp')){
		(async function() {
			await CefSharp.BindObjectAsync("boundAsync", "bound");
		})()
	}
};
app.nn = {
	network: {},
	createDummyNetwork: function(){
		// clear it
		app.nn.network = {
			input: [],
			outputLabels: ["Zero", "One", "Two", "Three"],
			layers: []
		};
		
		var layersCount = 4; // app.uf.getRandomInt(4, 10);
		var inputCount = 10; // app.uf.getRandomInt(20, 100);
		
		// create the input nodes
		for(var n = 0; n < inputCount; n++){
			app.nn.network.input.push(Math.random());
		}
		
		var previousNodeCount = inputCount;
		
		for(var n = 0; n < layersCount; n++){
			
			var nodeCount = app.uf.getRandomInt(20,40);
			
			if(n == layersCount - 1){
				nodeCount = 4;
			}

			var layer = [];
			for(var m = 0; m < nodeCount; m++){
			
				var weights = [];
				for(var w = 0; w < previousNodeCount; w++){
					weights.push(Math.random() * 2 - 1);
				}
				var rn = Math.random() * 6 - 3;
				// sigmoid
				layer.push({ 
					weights: weights.slice(),
					bias: 0,
					value: rn,
					activationValue: app.uf.sigmoid(rn)
				});
			}
			
			app.nn.network.layers.push(layer.slice());
			
			previousNodeCount = nodeCount;
		}
		
	},
	buildPaintJobs: function(config){
	
		// default config
		config = config || {
			nodeSize: 10,
			connectionsThickness: 0.2,
			positiveWeightsColor: {r: 255, g: 255, b: 255},
			negativeWeightsColor: {r: 0, g: 0, b: 0},
			topActivationColor: {r: 255, g: 255, b: 255},
			rightPadding: 128
		};

		// remove all tooltips
		const elTooltips = document.getElementsByClassName("tooltip");

		while (elTooltips.length > 0) elTooltips[0].remove();
		
		const elWeightips = document.getElementsByClassName("weightip");

		while (elWeightips.length > 0) elWeightips[0].remove();
		
		var pj = app.canvas.paintJobs;
		
		// find highest and lowest weights
		var highestWeight = Number.MIN_SAFE_INTEGER;
		var lowestWeight = Number.MAX_SAFE_INTEGER;
		
		var net = app.nn.network;
		
		// find largest and smallest weight
		for(var n = 0; n < net.layers.length; n++){
			for(var m = 0; m < net.layers[n].length; m++){
				for(var w = 0; w < net.layers[n][m].weights.length; w++){
					if(net.layers[n][m].weights[w] > highestWeight){
						highestWeight = net.layers[n][m].weights[w];
					}
					if(net.layers[n][m].weights[w] < lowestWeight){
						lowestWeight = net.layers[n][m].weights[w];
					}
				}
			}
		}

		app.canvas.ctx.beginPath();
		// app.canvas.ctx.rect(app.layout.network.x, app.layout.network.y, app.layout.network.width, app.layout.network.height);
		app.canvas.ctx.stroke();

		// FIRST DO THE INPUT NODES
		var firstLayerStartingY = app.layout.network.y; //(app.layout.network.height / (net.input.length + 1)) - (config.inputNodesSize / 2) + app.layout.network.y;
		var firstLayerNodeSpacing = (app.layout.network.height / (net.input.length - 1)) - config.nodeSize - (config.nodeSize / (net.input.length - 1));
		var layerStartingX = app.layout.network.x; //((app.layout.network.width) / (net.layers.length + 2)) + app.layout.network.x;

		var inputNodesCords = [];

		for(var n = 0; n < net.input.length; n++){
			var x = layerStartingX + (config.nodeSize / 2); // config.canvasPadding + (config.inputNodesSize / 2);
			var y = firstLayerStartingY + (n * (firstLayerNodeSpacing + config.nodeSize)) + (config.nodeSize / 2);
			inputNodesCords.push({x: x, y: y});
			pj.push({
				draw: "circle",
				fill: app.uf.getAdjustedColor(config.topActivationColor, app.uf.roundNumber(net.input[n],1)),
				pos: {x: x, y: y},
				rad: config.nodeSize / 2,
				zIndex: 50 + (100 * net.input[n])
			});


			pj.push({
				draw: "circle",
				fill: app.uf.getAdjustedColor(config.topActivationColor, app.uf.roundNumber(net.input[n],1)),
				//color: app.uf.getAdjustedColor(config.topActivationColor, app.uf.roundNumber(net.input[n],1)),
				pos: { x: (app.layout.network.width - 56) + ((n % 28) * 2), y: ((app.layout.network.height / 2) - 56) + ((n / 28) * 2) },
				// size: { w: 1, h: 1 },
				rad: 1,
				zIndex: 20000
			});
		}

		// JUST OTHER LAYERS IN THIS LOOP
		for(var n = 0; n < net.layers.length; n++){
		
			var nodeSize = config.nodeSize;

			var layerStartingY = app.layout.network.y; //((app.layout.network.height / (net.layers[n].length + 1)) - (nodeSize / 2)) + app.layout.network.y;
			var layerNodeSpacing = (app.layout.network.height / (net.layers[n].length - 1)) - nodeSize - (nodeSize / (net.layers[n].length - 1));

			var layerSpacing = ((app.layout.network.width - config.rightPadding) / (net.layers.length)) - nodeSize - (nodeSize / (net.layers.length + 3));		

			var x = layerStartingX + nodeSize + ((n + 1) * layerSpacing) + (nodeSize / 2);

			// go through each node in the layer
			for(var m = 0; m < net.layers[n].length; m++){
				
				var y = layerStartingY + (nodeSize / 2) + (m * (nodeSize + layerNodeSpacing));
				
				// store these values so we can make weight connections
				net.layers[n][m].pos = {
					x: x, 
					y: y
				};

				pj.push({
					draw: "circle",
					fill: app.uf.getAdjustedColor(config.topActivationColor, app.uf.roundNumber(net.layers[n][m].activationValue, 1)),
					pos: {x: x, y: y},
					rad: nodeSize / 2,
					zIndex: 500 + (100 * net.layers[n][m].activationValue)
				});

				// if output layer
				if(n == net.layers.length - 1){
					// draw text
					var t1 = x + (nodeSize / 2) + 10;
					var t2 = y + (nodeSize / 2) - 1;
					
					
					if(m == 0){
						// first node, nudge down
						t2 += 7;
					}
					else if(m == net.layers[n].length - 1){
						// last node, nudge up
						t2 -= 7;
					}
					pj.push({
						draw: "text",
						value: net.outputLabels[m] + " = " + app.uf.roundNumber(net.layers[n][m].activationValue, 2),
						font: "14px Saira Semi Condensed",
						color: app.uf.getAdjustedColor(config.topActivationColor, app.uf.roundNumber(net.layers[n][m].activationValue, 1)),
						pos: {x: t1, y: t2},
						zIndex: 1000
					});
				}
				
				
				// draw each weight...
				for(var w = 0; w < net.layers[n][m].weights.length; w++){
					// if this needs to be connected to the input nodes...
					
					var destX = 0;
					var destY = 0;
					
					if(n == 0){
						destX = inputNodesCords[w].x;
						destY = inputNodesCords[w].y;
					} else {
						destX = net.layers[n - 1][w].pos.x;
						destY = net.layers[n - 1][w].pos.y;
					}
					
					var color = (net.layers[n][m].weights[w] >= 0 ? config.positiveWeightsColor : config.negativeWeightsColor);
					
					
					
					// stroke thickness based on weight
					//var thickness = 0.1; //Math.round(((highestWeight - net.layers[n][m].weights[w]) / (highestWeight - lowestWeight)) * 10)
					var opacity = net.layers[n][m].weights[w] >= 0 ? (net.layers[n][m].weights[w] / highestWeight) : (net.layers[n][m].weights[w] / lowestWeight);
					
					pj.push({
						draw: "line",
						color: "rgba(" + color.r + "," + color.g + "," + color.b + "," + app.uf.roundNumber(opacity, 1) + ")",
						start: {x: x, y: y},
						end: {x: destX, y: destY},
						thickness: config.connectionsThickness,
						zIndex: 1 + net.layers[n][m].weights[w]
					});
					
					var labelHeight = 17;
					var totalHeight = net.layers[n][m].weights.length * labelHeight;
					
					var nodeX = document.body.clientWidth - (x) + 10;
					
					var nodeY = (y - (totalHeight / 2)) + (w * labelHeight);
				}

			}

		}

	}
};


