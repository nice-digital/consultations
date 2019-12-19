import { canUseDOM } from "./utils";
import { createBrowserHistory } from "history";

let browserHistory = null;

// Browser history only makes sense on the client
if(canUseDOM()) {
	browserHistory = createBrowserHistory();
}

export default browserHistory;
