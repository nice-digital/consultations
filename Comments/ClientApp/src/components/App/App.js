// @flow

import React, { Fragment } from "react";
import { Route, Switch } from "react-router";
import { Helmet } from "react-helmet";

import DocumentViewWithRouter from "../DocumentView/DocumentView";
import NotFound from "../NotFound/NotFound";
//import ReviewPageWithRouter from "../ReviewPage/ReviewPage";
import ReviewListPageWithRouter from "../ReviewListPage/ReviewListPage";
import UserProviderWithRouter from "../../context/UserContext";
import FooterWithRouter from "../Footer/Footer";
import DocumentPreviewWithRouter from "../DocumentPreview/DocumentPreview";
import ErrorBoundary from "../ErrorBoundary/ErrorBoundary";
import {LiveAnnouncer, LiveMessenger} from "react-aria-live";

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
					<ErrorBoundary>
						<LiveAnnouncer>
							<Switch>
								{/*Document View*/}			
								<Route exact
											 path="/:consultationId/:documentId/:chapterSlug">
									<DocumentViewWithRouter/>
								</Route>

								{/*Document (Preview Layout)*/}
								<Route exact
											 path="/preview/:reference/consultation/:consultationId/document/:documentId/chapter/:chapterSlug">
									<DocumentPreviewWithRouter/>
								</Route>

								{/*Review Page*/}
								<Route exact
											 path="/:consultationId/review">
									<LiveMessenger>
										{({announcePolite, announceAssertive}) =>
											<ReviewListPageWithRouter
												announcePolite={announcePolite}
												announceAssertive={announceAssertive}
												basename={this.props.basename}/>
										}
									</LiveMessenger>
								</Route>

								{/*404*/}
								<Route component={NotFound}/>
							</Switch>
						</LiveAnnouncer>
					</ErrorBoundary>
				</UserProviderWithRouter>
				<FooterWithRouter />
			</Fragment>
		);
	}
}

export default App;
