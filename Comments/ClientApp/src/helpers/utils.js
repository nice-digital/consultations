/**
 * Create a query string out of an object
 * @param obj the key-value pairs that you'd like to be turned into a querystring
 * @returns {string}
 */
export function objectToQueryString(obj) {
	let str = [];
	for (let p in obj)
		if (obj.hasOwnProperty(p)) {
			str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
		}
	if (str.length) return "?" + str.join("&");
	return "";
}

/**
 * Resolve a promise on the next tick - used in testing
 * @see https://nodejs.org/en/docs/guides/event-loop-timers-and-nexttick/#process-nexttick
 * @returns {Promise<any>}
 */
export const nextTick = async () => {
	return new Promise((resolve) => {
		setTimeout(resolve, 0);
	});
};

export function replaceFormat(args) {
	return this.replace(/{(\d+)}/g, function(match, number) {
		return typeof args[number] !== "undefined"
			? args[number]
			: match;
	});
}
