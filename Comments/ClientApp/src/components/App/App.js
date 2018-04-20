// @flow

import React, { Fragment } from "react";
import { Route, Switch, Redirect } from "react-router";
import { Helmet } from "react-helmet";
import CommentListWithRouter from "../CommentList/CommentList";
import { DocumentView } from "../DocumentView/DocumentView";
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
				<DocumentView />
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
