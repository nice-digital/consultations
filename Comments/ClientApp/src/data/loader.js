import Endpoints from "./endpoints";
import axios from "axios";

const load = (endpoint, baseUrl = "/consultations") => {
	return axios(baseUrl + Endpoints[endpoint])
		.then(response => response.data);
};

export default load;
