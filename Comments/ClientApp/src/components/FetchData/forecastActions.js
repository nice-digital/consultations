import fetchData from "./get_forecasts";

const willGetForecast = () => ({
	type: "WILL_GET_FORECAST"
});

const didGetForecast = data => ({
	type: "DID_GET_FORECAST",
	data
});

const cantGetForecast = data => ({
	type: "CANT_GET_FORECAST",
	data
});

export const fetchForecastData = () => async (dispatch) => {
	dispatch(willGetForecast());
	try {
		const data = await fetchData();
		dispatch(didGetForecast(data));
	} catch (error) {
		dispatch(cantGetForecast(error));
	}
};
