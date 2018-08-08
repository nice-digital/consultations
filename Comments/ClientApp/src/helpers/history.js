import { canUseDOM } from "./utils";

let browserHistory = null;

// Browser history only makes sense on the client
if(canUseDOM()) {
	const createHistory = require("history/createBrowserHistory").default;
	browserHistory = createHistory();
}

export default browserHistory;
