import Endpoints from "./endpoints";
import axios from "axios";

const load = (endpoint, root = "") => {
	return axios(root + Endpoints[endpoint])
		.then(response => response.data);
};

export default load;
