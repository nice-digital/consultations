import {Endpoints, BaseUrl} from "./endpoints";
import axios from "axios";
import {objectToQueryString, replaceFormat} from "./../helpers/utils";

/**
 * Load data using Axios
 * @see https://github.com/axios/axios
 * @param endpointName - can be either the shortcut of an endpoint as
 * specified in the Endpoints collection, or a URL / file path
 * @param baseUrl - the root url segment that's prepended to the query
 * @param query - the data to be serialised into query string
 * @returns {*|PromiseLike<T>|Promise<T>} - returns a promise with the data
 *
 * Example usage:
 * load("chapter", "/consultations", { consultationId: 1, documentId: 2, chapterSlug: "risk-assessment" });
 * ...would result in...
 * axios("/consultations/api/Chapter?consultationId=1&documentId=2&chapterSlug=risk-assessment").then(res => res.data);
 */

export const generateUrl = (endpointName, baseUrl = BaseUrl, urlParameters = [], query = {}) => {
	var endpoint = Endpoints[endpointName];
	if (!endpoint) return endpointName;

	return baseUrl + replaceFormat(endpoint, urlParameters) + objectToQueryString(query);
};

export const load = (endpoint, baseUrl = BaseUrl, urlParameters = [],  query = {}, method = "GET", data = {}, isJson = false) => {
	return new Promise((resolve, reject) => {
		const url = generateUrl(endpoint, baseUrl, urlParameters, query);
		const headers = isJson ? { "Content-Type" : "application/json"} : {};
		console.log(`loader.js: ${url} method: ${method}`);
		axios({url, data, method, headers})
			.then(response => {
				resolve(response);
			})
			// .then((response )=>{
			// 	if (endpoint === "comment"){
			// 		console.log(`load endpoint ${endpoint} response:${stringifyObject(response)}`);
			// 	}
			// })
			.catch(err => {
				reject(err);
			});
	});
};

export default load;
