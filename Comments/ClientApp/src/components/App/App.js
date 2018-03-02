// @flow

import React, { Component } from "react";
import { Route, Switch } from "react-router-dom";
import { Helmet } from "react-helmet";

import DemoPage from "../DemoPage";
import FullPage from "../FullPage";
import Home from "../Home/Home";
import Counter from "../Counter/Counter";
import FetchData from "../FetchData/FetchData";
import BasicForm from "../BasicForm/BasicForm";
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

					{/*counter*/}
					<Route path="/counter">
						<DemoPage>
							<Counter />
						</DemoPage>
					</Route>

					{/*fetch-data*/}
					<Route path="/fetchdata">
						<DemoPage>
							<FetchData />
						</DemoPage>
					</Route>

					{/*basic-form*/}
					<Route path="/basic-form">
						<DemoPage>
							<BasicForm />
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
