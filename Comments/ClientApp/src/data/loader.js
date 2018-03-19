import { Endpoints } from "./endpoints";
import axios from "axios";

const load = (endpoint, baseUrl = "/consultations") => {
	// if there's no endpoint for the string that's come in
	if (!Endpoints[endpoint]) {
		return axios(endpoint)
			.then(response => response.data)
			.catch((err) => {
				err.message += `. Error on endpoint ${endpoint}.`;
				throw err;
			});
	}
	return axios(baseUrl + Endpoints[endpoint])
		.then(response => response.data)
		.catch((err) => {
			err.message += `. Error on endpoint ${endpoint}.`;
			throw err;
		});
};

export default load;
