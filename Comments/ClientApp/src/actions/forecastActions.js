import fetchData from "./../components/get_forecasts";

const willGetForecast = () => ({
	type: "WILL_GET_FORECAST"
});

const didGetForecast = data => ({
	type: "DID_GET_FORECAST",
	data
});

export const fetchForecastData = () => async (dispatch) => {
	dispatch(willGetForecast());
	try {
		const data = await fetchData();
		dispatch(didGetForecast(data));
	} catch (error) {
		throw new Error("arrrrgghhhhh");
	}
};
