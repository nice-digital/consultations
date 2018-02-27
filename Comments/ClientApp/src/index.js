import React from "react";
import ReactDOM from "react-dom";
import { BrowserRouter } from "react-router-dom";
import { Provider } from "react-redux";

import registerServiceWorker from "./registerServiceWorker";
import configureStore from "./store/configureStore";

import App from "./App";

const baseUrl = document.getElementsByTagName("base")[0].getAttribute("href");
const rootElement = document.getElementById("root");

const store = configureStore(); // this is where you would pass initial state for SSR

ReactDOM.hydrate(
	<Provider store={store}>
		<BrowserRouter basename={baseUrl}>
			<App />
		</BrowserRouter>
	</Provider>,
	rootElement
);

registerServiceWorker();
