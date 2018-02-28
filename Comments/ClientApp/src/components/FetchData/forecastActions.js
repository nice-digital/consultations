import fetchData from "./get_forecasts";

const willGetForecast = () => ({
	type: "WILL_GET_FORECAST"
});

const didGetForecast = data => ({
	type: "DID_GET_FORECAST",
	data
});

export const fetchForecastData = () => async (dispatch) => {
	dispatch(willGetForecast());
	// try {
		const data = await fetchData(); // go and get the data
		dispatch(didGetForecast(data)); // dispatch an action with the data inside it
	// } catch (error) {
	// 	throw new Error(error);
	// }
};
