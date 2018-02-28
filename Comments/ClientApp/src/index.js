import React from "react";
import ReactDOM from "react-dom";
import { BrowserRouter } from "react-router-dom";
import { Provider } from "react-redux";

import registerServiceWorker from "./registerServiceWorker";

import configureStore from "./store/configureStore";

import App from "./components/App/App";

const baseUrl = document.getElementsByTagName("base")[0].getAttribute("href");
const rootElement = document.getElementById("root");

// Possible entry point for SSR
const store = configureStore();

ReactDOM.hydrate(
	<Provider store={store}>
		<BrowserRouter basename={baseUrl}>
			<App />
		</BrowserRouter>
	</Provider>,
	rootElement
);

registerServiceWorker();
