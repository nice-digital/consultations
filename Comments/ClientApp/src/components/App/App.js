// @flow

import React, { Fragment } from "react";
import { Route, Switch, Redirect } from "react-router";
import { Helmet } from "react-helmet";

import DocumentWithRouter from "../Document/Document";
import { Drawer } from "../Drawer/Drawer";
import CommentListWithRouter from "../CommentList/CommentList";
import NotFound from "../NotFound/NotFound";

const App = () => (
	<Fragment>
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
				<Fragment>
					<Drawer />
					<DocumentWithRouter />
				</Fragment>
			</Route>

			<Route path="/commentlist">
				<Fragment>
					<CommentListWithRouter />
				</Fragment>
			</Route>

			{/*404*/}
			<Route component={NotFound} />
		</Switch>
	</Fragment>
);

export default App;
