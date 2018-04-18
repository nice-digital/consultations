import { load } from "./loader";
import stringifyObject from "stringify-object";

// Returns data if it's available or a promise that resolves with the data
// when it's loaded async.
// This assumes the app will be rendered twice on the server: once for
// requests to fire and once when all the data has loaded
const preload = (staticContext, endpoint,  urlParameters = [], query = {}) => {
	let data = null;
	// console.log("in preload endpoint:" + endpoint);
	// Client - get data from global var
	if (typeof window !== "undefined") {
		if (!window.__PRELOADED__) return null;
		data = window.__PRELOADED__[endpoint];
		delete window.__PRELOADED__[endpoint];
		// console.log("returning from preload endpoint:" + endpoint);
		return data;
	}
	// console.log("in preload 2 endpoint:" + endpoint);
	// There should always be a static context on the server but check anyway
	if (!staticContext) {
		console.warn("Static context was null on the server");
		return data;
	}
	// console.log("in preload 3 endpoint:" + endpoint);
	// Data with that key already preloaded on the server
	if (staticContext.preload.data[endpoint]) {
		return staticContext.preload.data[endpoint];
	}
	// console.log("in preload 4 endpoint:" + endpoint);
	// Load fresh data on the server
	var promise = load(endpoint, staticContext.baseUrl, urlParameters, query)
		.then(response => {
			// console.log('hit preload endpoint. received:');
			// console.log(response);

			if (endpoint === "comment"){
				console.log("in preload 5 promise data:" + stringifyObject(response.data));
			}

			staticContext.preload.data[endpoint] = response.data;
			return response.data;
		});

	if (data != null){
		console.log("john's surprised this is hit:" + data);
	}

	//staticContext.preload.data[endpoint] = data;

	if (endpoint === "comment"){
	 	console.log("in preload 5 data:" + data);
	}
	// Track promises on the static context so that we can wait for all of them to resolve before
	staticContext.preload.loaders.push(promise);
	return null;
};

export default preload;
