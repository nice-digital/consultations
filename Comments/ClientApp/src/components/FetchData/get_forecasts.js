// @flow

import { fetch } from "isomorphic-fetch";
import Promise from "es6-promise";

Promise.polyfill();

const fetchData = async () => {
	// const response = await fetch("/api/SampleData/WeatherForecasts/");
	const response = await fetch("/api/SampleData/WeatherForecasts/");
	console.log({ response });
	return await response.json();
};

export default fetchData;
