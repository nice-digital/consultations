import * as packagejson from "../../package.json";

export const BaseUrl  = packagejson.appSettings.baseUrl;

export const Endpoints = {
	weather: "api/SampleData/WeatherForecasts",
	sample: "sample.json",
	consultation: "api/Consultation",
	consultations: "api/Consultations",
	documents: "api/Documents",
	chapter: "api/Chapter",
};
