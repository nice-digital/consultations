// @flow

import React, { Fragment } from "react";
import { Route, Switch, Redirect } from "react-router";
import { Helmet } from "react-helmet";

import DocumentViewWithRouter from "../DocumentView/DocumentView";
import NotFound from "../NotFound/NotFound";
import ReviewWithRouter from "../Review/Review";
import SubmittedWithRouter from "../Submitted/Submitted";
import UserProviderWithRouter from "../../context/UserContext";
import FooterWithRouter from "../Footer/Footer";
import DocumentPreviewWithRouter from "../DocumentPreview/DocumentPreview";
import QuestionsWithRouter from "../Questions/Questions";
import { ErrorBoundary } from "../ErrorBoundary/ErrorBoundary";
import { LiveAnnouncer, LiveMessenger } from "react-aria-live";
import { projectInformation } from "../../constants";
import { PhaseBanner } from "../PhaseBanner/PhaseBanner";

type PropsType = any;

type StateType = {
	onboarded: boolean
}

class App extends React.Component<PropsType, StateType> {
	render() {
		return (
			<Fragment>
				<div className="container">
					<div className="grid">
						<div data-g="12">
							<PhaseBanner
								phase={projectInformation.phase}
								name={projectInformation.name}
							/>
						</div>
					</div>
				</div>
				<UserProviderWithRouter>
					<Helmet titleTemplate="%s | Consultations | NICE">
						<html lang="en-GB"/>
					</Helmet>
					<ErrorBoundary>
						<LiveAnnouncer>
							<Switch>

								{/*Admin Routes*/}
								{/*Download*/}
								<Route exact path="/admin">
									<Redirect to={"/admin/download"}/>
								</Route>

								{/*Questions*/}
								<Route exact path="/admin/questions/:consultationId">
									<QuestionsWithRouter/>
								</Route>

								{/*Questions*/}
								<Route exact path="/admin/questions/:consultationId/:documentId">
									<QuestionsWithRouter/>
								</Route>

								{/*Document View*/}
								<Route exact path="/:consultationId/:documentId/:chapterSlug">
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
											<ReviewWithRouter
												announcePolite={announcePolite}
												announceAssertive={announceAssertive}
												basename={this.props.basename}/>
										}
									</LiveMessenger>
								</Route>

								{/*Submit Page*/}
								<Route exact
											 path="/:consultationId/submitted">
									<LiveMessenger>
										{({announcePolite, announceAssertive}) =>
											<SubmittedWithRouter
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
				<FooterWithRouter/>
			</Fragment>
		);
	}
}

export default App;
