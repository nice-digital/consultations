import axios from "axios";

const fetchData = async () => {
	try {
		const response = await axios("/api/SampleData/WeatherForecasts/");
		return response.data;
	} catch (err) {
		throw new Error(err);
	}
};

export default fetchData;
