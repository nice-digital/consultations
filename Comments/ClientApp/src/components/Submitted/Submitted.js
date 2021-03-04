// @flow

import React, { Component } from "react";
import { Link, withRouter } from "react-router-dom";
import { tagManager } from "../../helpers/tag-manager";
import Helmet from "react-helmet";
import BreadCrumbsWithRouter from "../Breadcrumbs/Breadcrumbs";
import { Header } from "../Header/Header";
import { UserContext } from "../../context/UserContext";
import LoginBannerWithRouter from "../LoginBanner/LoginBanner";
import preload from "../../data/pre-loader";
import { load } from "../../data/loader";
import Moment from "react-moment";

type PropsType = any;

type StateType = {
	consultationData: ConsultationStateType | null,
	loading: boolean,
	hasInitialData: boolean,
	error: {
		hasError: boolean,
		message: string,
	},

}

export class Submitted extends Component<PropsType, StateType> {

	constructor(props: PropsType) {
		super(props);

		this.state = {
			consultationData: null,
			loading: true,
			hasInitialData: false,
			error: {
				hasError: false,
				message: "",
			},
		};

		if (this.props) {

			let preloadedConsultation;

			let preloadedData = {};
			if (this.props.staticContext && this.props.staticContext.preload) {
				preloadedData = this.props.staticContext.preload.data; //this is data from Configure => SupplyData in Startup.cs. the main thing it contains for this call is the cookie for the current user.
			}

			preloadedConsultation = preload(
				this.props.staticContext,
				"consultation",
				[],
				{consultationId: this.props.match.params.consultationId, isReview: false},
				preloadedData,
			);

			if (preloadedConsultation) {
				if (this.props.staticContext) {
					this.props.staticContext.analyticsGlobals.gidReference = preloadedConsultation.reference;
					this.props.staticContext.analyticsGlobals.consultationTitle = preloadedConsultation.title;
					this.props.staticContext.analyticsGlobals.stage = "submit";
				}
				this.state = {
					consultationData: preloadedConsultation,
					loading: false,
					hasInitialData: true,
					error: {
						hasError: false,
						message: "",
					},
				};
			}
		}
	}

	gatherData = async () => {
		const {consultationId} = this.props.match.params;

		const consultationData = load("consultation", undefined, [], {
			consultationId,
			isReview: false,
		})
			.then(response => response.data)
			.catch(err => {
				console.log(err);
			});

		return {
			consultationData: await consultationData,
		};
	};

	componentDidMount() {
		if (!this.state.hasInitialData) {
			this.gatherData()
				.then(data => {
					this.setState({
						consultationData: data.consultationData,
						loading: false,
						hasInitialData: true,
					}, () => {
						tagManager({
							event: "pageview",
							gidReference: this.state.consultationData.reference,
							title: this.getPageTitle(true),
							stage: "submit",
						});
					});
				})
				.catch(err => {
					this.setState({
						error: {
							hasError: true,
							message: "gatherData in componentDidMount failed " + err,
						},
					});
				});
		}
	}

	getPageTitle = (isForAnalytics: boolean = false) => {
		if (isForAnalytics) return this.state.consultationData.title;
		return `${this.state.consultationData.title} | Response submitted`;
	};

	render() {
		if (!this.state.hasInitialData) return <h1>Loading...</h1>;
		return (
			<>
				<Helmet>
					<title>{this.getPageTitle()}</title>
				</Helmet>
				<div className="container">
					<div className="grid">
						<div data-g="12">
							<BreadCrumbsWithRouter links={this.state.consultationData.breadcrumbs}/>
							<main>
								<div className="page-header">
									<Header
										title="Response submitted"
										consultationState={this.state.consultationData.consultationState}
									/>
									<UserContext.Consumer>
										{(contextValue: ContextType) => {
											return (
												!contextValue.isAuthorised ?
													<LoginBannerWithRouter
														signInButton={true}
														currentURL={this.props.match.url}
														signInURL={contextValue.signInURL}
														registerURL={contextValue.registerURL}
														allowOrganisationCodeLogin={false}
														orgFieldName="submitted"
													/> :
													<>

														<h2>Your response was submitted {contextValue.isOrganisationCommenter  && !contextValue.isLead && `to ${contextValue.organisationName}`} on <Moment format="D MMMM YYYY" date={this.state.consultationData.consultationState.submittedDate}/>.</h2>
														
														<ul className="list list--unstyled">
															<li>
																<Link
																	to={`/${this.props.match.params.consultationId}/review`}
																	data-qa-sel="review-submitted-comments">
																	Review your response
																</Link>
															</li>
															{this.state.consultationData.consultationState.supportsDownload &&
															<li>
																<a
																	onClick={() => {
																		tagManager({
																			event: "generic",
																			category: "Consultation comments page",
																			action: "Clicked",
																			label: "Download your response button",
																		});
																	}}
																	href={`${this.props.basename}/api/exportexternal/${this.props.match.params.consultationId}`}>
																	Download submitted response (Excel)</a>
															</li>
															}
															{contextValue.isLead && this.state.consultationData.consultationState.leadHasBeenSentResponse &&
															<li>
																<a
																	onClick={() => {
																		tagManager({
																			event: "generic",
																			category: "Consultation comments page",
																			action: "Clicked",
																			label: "Download comments submitted to lead button",
																		});
																	}}
																	href={`${this.props.basename}/api/exportlead/${this.props.match.params.consultationId}`}>
																Download all responses from your organisation (Excel)</a>
															</li>
															}
														</ul>

														<h2>What happens next?</h2>
														{contextValue.isOrganisationCommenter && !contextValue.isLead ? (
															<>
																<p>{`${contextValue.organisationName}`} will review all the submissions received for this consultation.</p>
																<p>NICE's response to all the submissions received will be published on the website around the time the final guidance is published.</p>
															</>
														) : (
															<p>We will review all the submissions received for this consultation. Our response
															will be published on the website around the time the final guidance is published.</p>
														)}

														<h2>Help us improve our online commenting service</h2>
														<p>This is the first time we have used our new online commenting software. We'd really like to hear your feedback so that we can keep improving
															it.</p>
														<p>Answer our short, anonymous survey (4 questions, 2 minutes).</p>
														<p>
															<a className="btn btn--cta"
																 href="https://in.hotjar.com/s?siteId=119167&surveyId=109567" target="_blank"
																 rel="noopener noreferrer">
																Answer the survey
															</a>
														</p>
													</>
											);
										}}
									</UserContext.Consumer>
								</div>
							</ main>
						</div>
					</div>
				</div>
			</>

		);
	}
}

export default withRouter(Submitted);
