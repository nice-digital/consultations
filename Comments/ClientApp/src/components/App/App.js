import React, { Component } from "react";
import { Route, Switch } from "react-router-dom";
import { Helmet } from "react-helmet";

import Layout from "../Layout";
import Home from "../Home/Home";
import FetchData from "../FetchData/FetchData";
import Counter from "../Counter/Counter";
import BasicForm from "../BasicForm/BasicForm";
import NotFound from "../NotFound/NotFound";

export default class App extends Component {
	render() {
		return (
			<Layout>
				<Helmet titleTemplate="%s | Consultations | NICE">
					<html lang="en-GB" />
				</Helmet>
				<Switch>
					<Route exact path="/" component={Home} />
					<Route path="/counter" component={Counter} />
					<Route path="/fetchdata" component={FetchData} />
					<Route path="/basic-form" component={BasicForm} />
					<Route component={NotFound} />
				</Switch>
			</Layout>
		);
	}
}
