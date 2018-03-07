import Endpoints from "./endpoints";
import axios from "axios";

const load = (endpoint) => {
	return axios("http://localhost:52679/" + Endpoints[endpoint])
		.then(response => response.data);
};

export default load;
