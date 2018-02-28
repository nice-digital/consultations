import { createStore, applyMiddleware, combineReducers } from "redux";
import thunk from "redux-thunk";

// Bring in the reducers from the various components
import comments from "../components/BasicForm/formReducer";
import forecast from "./../components/FetchData/forecastReducer";

const rootReducer = combineReducers({
	comments,
	forecast
});

// export createStore to be called from index.js
export default function configureStore(initialState) {
	return createStore(
		rootReducer, initialState, applyMiddleware(thunk)
	);
}
