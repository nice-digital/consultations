import queryString from "query-string";

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

export function queryStringToObject(qryString) {
	return queryString.parse(qryString);
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

/**
 * Format a string by replacing {0} with arguments passed in
 * @see https://stackoverflow.com/a/4673436
 * @returns {String}
 */
export function replaceFormat(stringToReplace, args) {
	if (typeof(stringToReplace) === undefined || stringToReplace === null)
		return stringToReplace;

	return stringToReplace.replace(/{(\d+)}/g, function(match, number) {
		return typeof args[number] !== undefined
			? args[number]
			: match
		;
	});
}

/**
 * @param String The string in which you want to check for the presence of "http"
 * Detects link that starts with "http"
 * @returns {Boolean}
 */
export const isHttpLink = link => link.indexOf("http") === 0;
