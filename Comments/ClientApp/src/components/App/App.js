// @flow

import React, { Component } from "react";
import { Route, Switch } from "react-router-dom";
import { Helmet } from "react-helmet";

import DemoPage from "../DemoPage";
import FullPage from "../FullPage";
import Home from "../Home/Home";
import FetchData from "../FetchData/FetchData";
import Document from "../Document/Document";
import NotFound from "../NotFound/NotFound";

export default class App extends Component {
	render() {
		return (
			<div>
				<Helmet titleTemplate="%s | Consultations | NICE">
					<html lang="en-GB" />
				</Helmet>
				<Switch>
					{/*home*/}
					<Route exact path="/">
						<DemoPage>
							<Home />
						</DemoPage>
					</Route>

					{/*fetch-data*/}
					<Route path="/fetchdata">
						<DemoPage>
							<FetchData />
						</DemoPage>
					</Route>

					{/*document*/}
					<Route path="/document">
						<FullPage>
							<Document />
						</FullPage>
					</Route>

					{/*404*/}
					<Route component={NotFound} />
				</Switch>
			</div>
		);
	}
}
