import Endpoints from "./endpoints";

const load = (endpoint) => {
	return fetch("http://localhost:52679/" + Endpoints[endpoint])
		.then(response => response.json());
};

export default load;
