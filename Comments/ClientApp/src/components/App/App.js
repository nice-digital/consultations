// @flow

import React, { Fragment, Component } from "react";
import { Route, Switch, Redirect } from "react-router";
import { Helmet } from "react-helmet";
import CommentListWithRouter from "../CommentList/CommentList";
import { DocumentView } from "../DocumentView/DocumentView";
import NotFound from "../NotFound/NotFound";
import UserProviderWithRouter from "../../context/UserContext";
import { ReviewPage } from "../ReviewPage/ReviewPage";

const App = () => {
	return (
		<UserProviderWithRouter>
			<Helmet titleTemplate="%s | Consultations | NICE">
				<html lang="en-GB"/>
			</Helmet>

			<Switch>
				{/*Home*/}
				<Route exact path="/">
					<Redirect to="/1/1/introduction"/>
				</Route>

				{/*Document View*/}
				<Route path="/:consultationId/:documentId/:chapterSlug">
					<DocumentView/>
				</Route>

				{/*Review Page*/}
				<Route path="/:consultationId/review">
					<ReviewPage/>
				</Route>

				<Route path="/commentlist">
					<Fragment>
						<CommentListWithRouter/>
					</Fragment>
				</Route>

				{/*404*/}
				<Route component={NotFound}/>
			</Switch>
		</UserProviderWithRouter>
	);
};

export default App;
