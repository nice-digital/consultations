// @flow

import React from "react";
import { Route, Switch, Redirect } from "react-router";
import { Helmet } from "react-helmet";
import DocumentViewWithRouter from "../DocumentView/DocumentView";
import NotFound from "../NotFound/NotFound";
import DocumentPreviewRedirectWithRouter from "../Document/DocumentPreviewRedirect";
import ReviewPageWithRouter from "../ReviewPage/ReviewPage";
import UserProviderWithRouter from "../../context/UserContext";

type PropsType = any;

type StateType = {
	onboarded: boolean
}

class App extends React.Component<PropsType, StateType> {
	
	render() {
		return (
			<UserProviderWithRouter>
				<Helmet titleTemplate="%s | Consultations | NICE">
					<html lang="en-GB"/>
				</Helmet>

				<Switch>
					{/*Home*/}
					<Route exact path="/">
						<Redirect to="/17/1/introduction"/>
					</Route>

					{/*Document (Comment Layout)*/}
					<Route exact path="/:consultationId/:documentId/:chapterSlug">
						<DocumentViewWithRouter/>
					</Route>

					<Switch>
						{/*Document (Preview Layout)*/}
						<Route path="/preview/:reference/consultation/:consultationId/document/:documentId/chapter/:chapterSlug">
							<DocumentView/>
						</Route>

						{/*	If we hit this we're coming in *without* a chapter slug,
						so we need to get the first chapter of the current document
						and pass its slug into the URL
						so it matches the route above */}
						<Route path="/preview/:reference/consultation/:consultationId/document/:documentId">
							<DocumentPreviewRedirectWithRouter/>
							 {/* This component only redirects to the above route */}
						</Route>
					</Switch>

					{/*Review Page*/}
					<Route exact path="/:consultationId/review">
						<ReviewPageWithRouter/>
					</Route>

					{/*404*/}
					<Route component={NotFound}/>
				</Switch>
			</UserProviderWithRouter>
		);
	}
}

export default App;
