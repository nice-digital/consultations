// @flow

import React from "react";
import { Route, Switch, Redirect } from "react-router";
import { Helmet } from "react-helmet";
import { DocumentView } from "../DocumentView/DocumentView";
import DocumentPreviewWithRouter from "../DocumentPreview/DocumentPreview";
import NotFound from "../NotFound/NotFound";
import DocumentPreviewRedirectWithRouter from "../DocumentPreview/DocumentPreviewRedirect";
import ReviewPageWithRouter from "../ReviewPage/ReviewPage";
import UserProviderWithRouter from "../../context/UserContext";
import OnboardingModal from "../OnboardingModal/OnboardingModal";

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
						<Redirect to="/10/5/overview"/>
					</Route>

					{/*Standard Document View*/}
					<Route exact path="/:consultationId/:documentId/:chapterSlug">
						<DocumentView/>
					</Route>

					{/*Preview View*/}
					{/*Comes from indev - e.g. http://dev.nice.org.uk/preview/consultation/1/document/1*/}
					<Route exact path="/preview/consultation/:consultationId/document/:documentId/chapter/:chapterSlug">
						<DocumentPreviewWithRouter/>
					</Route>

					{/*If we hit this we're coming in *without* a chapter slug,
					so we need to get the first chapter of the current document and pass its slug into the URL
					so it matches the route above*/}
					<Route exact path="/preview/consultation/:consultationId/document/:documentId">
						<DocumentPreviewRedirectWithRouter/>
					</Route>

					{/*Review Page*/}
					<Route exact path="/:consultationId/review">
						<ReviewPageWithRouter/>
					</Route>

					{/*404*/}
					<Route component={NotFound}/>
				</Switch>
				<OnboardingModal/>
			</UserProviderWithRouter>
		);
	}
}

export default App;
