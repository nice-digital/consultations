import "core-js/es6/map";
import "core-js/es6/set";
import "core-js/fn/array/includes";
import "core-js/es6/regexp";
import "raf/polyfill";
import "classlist-polyfill";
import "ie9-oninput-polyfill";
import React from "react";
import ReactDOM from "react-dom";
import { BrowserRouter } from "react-router-dom";
import App from "./components/App/App";

if (process.env.NODE_ENV === "development") {
	console.log("Attaching react-perf-devtool...");
	const { registerObserver } = require("react-perf-devtool");
	registerObserver();
}

const baseUrl = document.getElementsByTagName("base")[0].getAttribute("href").replace(/\/$/, "");

let rootElement = document.getElementById("root");

if (!rootElement){ //element not found. we'll assume (for now) that we're commenting on something that's not a consultation. 

	const currentlyExecutingScriptTagInPage = document.querySelector("script[src*='static/js/bundle.js']");

	let rootNode = document.createElement("div");
	rootNode.setAttribute("id", "comments-root");

	rootElement = currentlyExecutingScriptTagInPage.parentElement.insertBefore(rootNode, currentlyExecutingScriptTagInPage);

	document.querySelector("body").addEventListener("mouseup", function(event){
		window.dispatchEvent(new CustomEvent("comment", { detail: {eventRaised: event}}));
	});

	loadStyle("/consultations/styles/main.css");	
}

//stuff to insert stuff here:
//<link rel="stylesheet" href="/consultations/styles/main.css" />
//<div id="commentablecontent" onmouseup="window.dispatchEvent(new CustomEvent('comment', { detail: {eventRaised: event}}));">
// [content]
//</div>
//<div id="root"></div>

ReactDOM.hydrate(
	<BrowserRouter basename={baseUrl}>
		<App basename={baseUrl}/>
	</BrowserRouter>,
	rootElement
);

function loadStyle(href, callback){
	// avoid duplicates
	for(var i = 0; i < document.styleSheets.length; i++){
		if(document.styleSheets[i].href == href){
			return;
		}
	}
	var head  = document.getElementsByTagName("head")[0];
	var link  = document.createElement("link");
	link.rel  = "stylesheet";
	link.type = "text/css";
	link.href = href;
	if (callback) { link.onload = function() { callback(); }; }
	head.appendChild(link);
}