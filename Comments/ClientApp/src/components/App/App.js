// @flow

import React, { Fragment } from "react";
import { Route, Switch, Redirect } from "react-router";
import { Helmet } from "react-helmet";
import CommentListWithRouter from "../CommentList/CommentList";
import DocumentViewWithRouter from "../DocumentView/DocumentView";
import NotFound from "../NotFound/NotFound";
import ReviewPageWithRouter from "../ReviewPage/ReviewPage";
import UserProviderWithRouter from "../../context/UserContext";
import FooterWithRouter from "../Footer/Footer";

type PropsType = any;

type StateType = {
	onboarded: boolean
}

class App extends React.Component<PropsType, StateType> {
	render() {
		return (
			<Fragment>
				<UserProviderWithRouter>
					<Helmet titleTemplate="%s | Consultations | NICE">
						<html lang="en-GB"/>
					</Helmet>

					<Switch>
						{/*Home*/}
						<Route exact path="/">
							<Redirect to="/17/1/introduction"/>
						</Route>

						{/*Document View*/}
						<Route path="/:consultationId/:documentId/:chapterSlug">
							<DocumentViewWithRouter/>
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
				</UserProviderWithRouter>
				<FooterWithRouter />
			</Fragment>
		);
	}
}

export default App;
