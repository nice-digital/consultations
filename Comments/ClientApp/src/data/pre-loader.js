import load from "./loader";

// Returns data if it's available or a promise that resolves with the data
// when it's loaded async.
// This assumes the app will be rendered twice on the server: once for
// requests to fire and once when all the data has loaded
const preload = (staticContext, endpoint) => {
	let data = null;

	// Client - get data from global var
	if (typeof window !== "undefined") {
		console.info(
			`Found preloaded data on the client with an endpoint key of ${endpoint}`
		);
		if (!window.__PRELOADED__) return null;
		data = window.__PRELOADED__[endpoint];
		delete window.__PRELOADED__[endpoint];
		return data;
	}

	// There should always be a static context on the server but check anyway
	if (!staticContext) {
		console.warn("Static context was null on the server");
		return data;
	}

	// Data with that key already preloaded on the server
	if (staticContext.preload.data[endpoint]) {
		console.log(
			`Data with key '${endpoint}' has already been preloaded on the server`
		);
		return staticContext.preload.data[endpoint];
	}

	// Load fresh data on the server
	console.log(`Data with key '${endpoint}' isn't loaded, making request`);

	var promise = load(endpoint, staticContext.baseUrl)
		.then(response => {
			console.log(`Data with key '${endpoint}' loaded async from server`);
			staticContext.preload.data[endpoint] = response;
			return response;
		});

	console.log(`Data with key '${endpoint}' loaded async from server`);
	staticContext.preload.data[endpoint] = data;

	// Track promises on the static context so that we can wait for all of them to resolve before
	staticContext.preload.loaders.push(promise);
	return null;
};

export default preload;
