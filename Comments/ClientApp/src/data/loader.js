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

const load = (endpoint, baseUrl = "/consultations", query = {}) => {
	// see if you're passing an endpoint that's in our Endpoints list of shortcuts
	if (Endpoints[endpoint]) {
		return axios(baseUrl + Endpoints[endpoint] + objectToQueryString(query))
			.then(response => response.data);
	} else {
		// if not, pass the string through as is
		return axios(endpoint).then(response => response.data);
	}
};

export default load;
