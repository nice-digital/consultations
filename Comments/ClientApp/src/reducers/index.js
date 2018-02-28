import { combineReducers } from "redux";
// whatever you import your reducer as, is how that state will be referenced everywhere
import course from "./courseReducer";
import forecast from "./forecastReducer";

const rootReducer = combineReducers({
	// this is what you'll be calling from your state
	course,
	forecast
});

export default rootReducer;
