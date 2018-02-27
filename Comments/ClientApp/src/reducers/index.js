import { combineReducers } from "redux";
import course from "./courseReducer";

const rootReducer = combineReducers({
	// this is what you'll be calling from your state
	course
});

export default rootReducer;
