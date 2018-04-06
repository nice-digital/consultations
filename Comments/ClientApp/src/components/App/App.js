// @flow

import React from "react";
import { Route, Switch, Redirect } from "react-router";
import { Helmet } from "react-helmet";

import DocumentWithRouter from "../Document/Document";
import { Drawer } from "../Drawer/Drawer";
import NotFound from "../NotFound/NotFound";

const App = () => (
	<div>
		<Helmet titleTemplate="%s | Consultations | NICE">
			<html lang="en-GB" />
		</Helmet>

		<Switch>
			{/*Home*/}
			<Route exact path="/">
				<Redirect to="/1/1/introduction" />
			</Route>

			{/*Document View*/}
			<Route path="/:consultationId/:documentId/:chapterSlug">
				<div>
					<Drawer />
					<DocumentWithRouter />
				</div>
			</Route>

			{/*404*/}
			<Route component={NotFound} />
		</Switch>
	</div>
);

export default App;
