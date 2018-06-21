// @flow

import React, { Fragment } from "react";
import { Route, Switch, Redirect } from "react-router";
import { Helmet } from "react-helmet";
import CommentListWithRouter from "../CommentList/CommentList";
import { DocumentView } from "../DocumentView/DocumentView";
import NotFound from "../NotFound/NotFound";
import ReviewPageWithRouter from "../ReviewPage/ReviewPage";
import UserProviderWithRouter from "../../context/UserContext";
import OnboardingModal from "../OnboardingModal/OnboardingModal";
import { pullFocus } from "../../helpers/accessibility-helpers";

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

				<button className="screenreader-button" onClick={() => pullFocus("main h1")}>Skip to Main Content</button>

				<Switch>
					{/*Home*/}
					<Route exact path="/">
						<Redirect to="/10/5/overview"/>
					</Route>

					{/*Document View*/}
					<Route path="/:consultationId/:documentId/:chapterSlug">
						<DocumentView/>
					</Route>

					{/*Review Page*/}
					<Route path="/:consultationId/review">
						<ReviewPageWithRouter/>
					</Route>

					<Route path="/commentlist">
						<Fragment>
							<CommentListWithRouter/>
						</Fragment>
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
