export default function forecastReducer(state = { status: "idle" }, action) {
	switch (action.type) {
		case "WILL_GET_FORECAST":
			return { status: "loading" };
		case "DID_GET_FORECAST":
			return {
				status: "complete",
				data: action.data
			};
		case "CANT_GET_FORECAST":
			return {
				status: "failed",
				data: action.data
			};
		default:
			return state;
	}
}
