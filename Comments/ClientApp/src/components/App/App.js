// @flow

import React from "react";
import { Route, Switch, Redirect } from "react-router";
import { Helmet } from "react-helmet";

import LayoutTwoCol from "../LayoutTwoColumn";
import WeatherForecast from "../WeatherForecast/WeatherForecast";
import DocumentWithRouter from "../Document/Document";
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
					<Redirect to="/1/1/introduction" />
				</Route>

				{/*weather-forecast*/}
				<Route path="/weather-forecast">
					<LayoutTwoCol>
						<WeatherForecast />
					</LayoutTwoCol>
				</Route>

				{/*document*/}
				<Route path="/:consultationId/:documentId/:chapterSlug">
					<DocumentWithRouter />
				</Route>

				{/*404*/}
				<Route component={NotFound} />
			</Switch>
		</div>
	);
};

export default App;
