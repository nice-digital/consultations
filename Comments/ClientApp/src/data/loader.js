import {Endpoints, BaseUrl} from "./endpoints";
import axios from "axios";
import https from "https";

import {objectToQueryString, replaceFormat} from "./../helpers/utils";
//import stringifyObject from "stringify-object";

/**
 * Load data using Axios
 * @see https://github.com/axios/axios
 * @param endpointName - can be either the shortcut of an endpoint as
 * specified in the Endpoints collection, or a URL / file path
 * @param baseUrl - the root url segment that's prepended to the query
 * @param urlParameters - an array of url segment numbers e.g. [1, 2] -> "/1/2"
 * @param query - the data to be serialised into query string
 * @returns {*|PromiseLike<T>|Promise<T>} - returns a promise with the data
 *
 * Example usage:
 * load("chapter", "/consultations", { consultationId: 1, documentId: 2, chapterSlug: "risk-assessment" });
 * ...would result in...
 * axios("/consultations/api/Chapter?consultationId=1&documentId=2&chapterSlug=risk-assessment").then(res => res.data);
 */

export const generateUrl = (endpointName, baseUrl = BaseUrl, urlParameters = [], query = {}) => {
	const endpoint = Endpoints[endpointName];
	if (!endpoint) return endpointName;

	return baseUrl + replaceFormat(endpoint, urlParameters) + objectToQueryString(query);
};

export const load = (endpoint, baseUrl = BaseUrl, urlParameters = [],  query = {}, method = "GET", data = {}, isJson = false, cookie = "") => {
	return new Promise((resolve, reject) => {
		const url = generateUrl(endpoint, baseUrl, urlParameters, query);
		let headers = isJson ? { "Content-Type": "application/json"} : {}; 
		//let headers = isJson ? { "Accept": "text/html" } : { "Accept": "text/html"};
		if (cookie !== ""){
		 	headers = Object.assign({"Cookie": cookie}, headers);
		}
		const httpsAgent = (baseUrl.indexOf("https") !== -1) ? new https.Agent({ rejectUnauthorized: false }) : {};

		console.log(`about to hit url:${url}`);
		
		axios({
			url,
			data,
			method,
			headers,
			httpsAgent//,
			//maxRedirects: 0
		}) //withCredentials: true
		.then(response => {
			resolve(response);
		})
		.catch(err => {
			reject(err);
		});
	});
};

export default load;
