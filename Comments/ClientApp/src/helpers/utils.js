import queryString from "query-string";
//import stringifyObject from "stringify-object";

/**
 * Create a query string out of an object
 * @param obj the key-value pairs that you'd like to be turned into a querystring
 * @returns {string}
 */
export function objectToQueryString(obj) {
	let str = [];
	for (let p in obj)
		if (obj.hasOwnProperty(p)) {
			if (Array.isArray(obj[p])){
				for (let index in obj[p]){
					str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p][index]));	
				}
			} else{
				str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
			}
		}
	if (str.length) return "?" + str.join("&");
	return "";
}

/**
 * Creates an object out of a querys tring
 * 	@param {string} the querystring you'd like to be turned into an object
 *  @returns obj
 */
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

// Remove querystring parameters from a given URL
export const removeQueryParameter =
	(url: string, parameter: string, value: ?string = null): string => {
		const urlParts = url.split("?");

		if (urlParts.length < 2)
			return url;

		let urlParams = urlParts[1].split(/[&;]/g);

		for (var i = urlParams.length; i-- > 0;) {
			const parParts = urlParams[i].split("=");

			if (parParts[0].toLowerCase() === parameter.toLowerCase()
				&& (!value || (value && value.toLowerCase() === (parParts[1] || "").toLowerCase()))) {
				urlParams.splice(i, 1);
			}
		}

		const queryString = (urlParams.length > 0 ? "?" + urlParams.join("&") : "");
		return urlParts[0] + queryString;
	};

export const appendQueryParameter =
	(url: string, parameter: string, value: string): string => `${url}${url.indexOf("?") === -1 ? "?" : "&"}${parameter}=${value}`;

/// Determines if the execution environment is client or server
/// See https://stackoverflow.com/a/32598826/486434
export const canUseDOM = (): boolean => (
	!!(
		typeof window !== "undefined" &&
		window.document &&
		window.document.createElement
	));

function whichChild(elem){
	let  i= 0;
	while ((elem=elem.previousSibling)!=null) ++i;
	return i;
}
	
//this function returns a long dotted decimal
export const getElementPositionWithinDocument = (elem) => {
	let curindex = "";
	if (elem.offsetParent) {
		do {
			curindex += this.whichChild(elem).toString() + ".";
		} while (elem = elem.offsetParent && elem.id !== "root"); //assignment is correct. should not be conditional.
	}
	return curindex.trim().replace(new RegExp(/\s/, "g"), ".");
};