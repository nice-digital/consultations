// @flow

import React from "react";
import { Route, Switch } from "react-router-dom";
import { Helmet } from "react-helmet";

import LayoutTwoCol from "../LayoutTwoColumn";
import Home from "../Home/Home";
import WeatherForecast from "../WeatherForecast/WeatherForecast";
import { Document } from "../Document/Document";
import NotFound from "../NotFound/NotFound";

const App = () => {
	return (
		<div>
			<Helmet titleTemplate="%s | Consultations | NICE">
				<html lang="en-GB" />
			</Helmet>

			<Switch>
				{/*home*/}
				<Route exact path="/">
					<LayoutTwoCol>
						<Home />
					</LayoutTwoCol>
				</Route>

				{/*fetch-data*/}
				<Route path="/weather-forecast">
					<LayoutTwoCol>
						<WeatherForecast />
					</LayoutTwoCol>
				</Route>

				{/*document*/}
				<Route path="/:consulationID/:documentID/:chapterSlug">
					<Document />
				</Route>

				{/*404*/}
				<Route component={NotFound} />
			</Switch>
		</div>
	);
};

export default App;
