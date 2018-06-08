// @flow

import React, { Fragment } from "react";
import { Route, Switch, Redirect } from "react-router";
import { Helmet } from "react-helmet";
import CommentListWithRouter from "../CommentList/CommentList";
import { DocumentView } from "../DocumentView/DocumentView";
import NotFound from "../NotFound/NotFound";
import UserProviderWithRouter, { UserContext } from "../../context/UserContext";
import ReviewPageWithRouter from "../ReviewPage/ReviewPage";
import CommentImg from "../Document/img/comment-icon-helper.png";
import SelectionImg from "../Document/img/selection-helper.png";

type PropsType = any;

type StateType = {
	onboarded: boolean
}

class App extends React.Component<PropsType, StateType> {
	constructor() {
		super();
		this.state = {
			onboarded: false
		};
	}

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

				<UserContext.Consumer>
					{contextValue => contextValue.isAuthorised ?
						<div style={{ "display": this.state.onboarded ? "none" : "block" }}
							 className="onboarding"
						>
							<div className="onboarding__modal">
								<button
									className="onboarding__closeButton"
									onClick={() => this.setState({ onboarded: true })}>
									&times;
								</button>
								<h1>How to make comments</h1>
								<div className="grid">
									<div data-g="6">
										<p><b>Click on the icon next to a heading to comment on the chapter or section</b></p>
										<img src={CommentImg}/>
									</div>
									<div data-g="6">
										<p><b>Highlight some text to make a comment on it</b></p>
										<img src={SelectionImg}/>
									</div>
								</div>
							</div>
						</div>
						:
						null}
				</UserContext.Consumer>
			</UserProviderWithRouter>
		);
	}
}

export default App;
