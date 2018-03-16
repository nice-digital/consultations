import { Endpoints } from "./endpoints";
import axios from "axios";
import { objectToQueryString } from "./../helpers/utils";

/**
 * Load data using Axios
 * @see https://github.com/axios/axios
 * @param endpoint - can be either the shortcut of an endpoint as specified in the Endpoints collection, or a URL / file path
 * @param baseUrl - the root url segment that's prepended to the query
 * @param query - the data to be serialised into query string
 * @returns {*|PromiseLike<T>|Promise<T>} - returns a promise with the data
 *
 * Example usage:
 * load("chapter", "/consultations", { consultationId: 1, documentId: 2, chapterSlug: "risk-assessment" });
 * ...would result in...
 * axios("/consultations/api/Chapter?consultationId=1&documentId=2&chapterSlug=risk-assessment").then(res => res.data);
 */

export const generateUrl = (endpoint, baseUrl = "/consultations", query = {}) => {
	if (Endpoints[endpoint]) {
		return baseUrl + Endpoints[endpoint] + objectToQueryString(query);
	}
	return endpoint;
};

export const load = (endpoint, baseUrl = "/consultations", query = {}) => {
	return new Promise((resolve, reject) => {
		axios(generateUrl(endpoint, baseUrl, query))
			.then(response => {
				resolve(response);
			})
			.catch(err => {
				reject(err);
			});
	});
};

export default load;
