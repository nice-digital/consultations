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
