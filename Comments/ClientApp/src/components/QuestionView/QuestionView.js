// @flow

import React, { Component, Fragment } from "react";
import { withRouter, Prompt, Redirect } from "react-router-dom";
import Helmet from "react-helmet";

import preload from "../../data/pre-loader";
import { load } from "../../data/loader";
import { queryStringToObject } from "../../helpers/utils";
import { tagManager } from "../../helpers/tag-manager";
import { UserContext } from "../../context/UserContext";
import { Header } from "../Header/Header";
import BreadCrumbsWithRouter from "../Breadcrumbs/Breadcrumbs";
import { ReviewResultsInfo } from "../ReviewResultsInfo/ReviewResultsInfo";
import { withHistory } from "../HistoryContext/HistoryContext";
import { Question } from "../Question/Question";
import { pullFocusByQuerySelector } from "../../helpers/accessibility-helpers";

type PropsType = {
	staticContext?: any,
	match: {
		url: string,
		params: any
	},
	location: {
		pathname: string,
		search: string
	},
	history: HistoryType,
	basename: string,
	announceAssertive: Function,
};

type StateType = {
	consultationData: ConsultationDataType | null,
	commentsData: ReviewPageViewModelType | null,
	submittedDate: Date,
	validToSubmit: false,
	path: string | null,
	hasInitialData: boolean,
	allowComments: boolean,
	comments: Array<CommentType>,
	questions: Array<QuestionType>,
	sort: string,
	supportsDownload: boolean,
	loading: boolean,
	respondingAsOrganisation: boolean,
	organisationName: string,
	hasTobaccoLinks: boolean,
	tobaccoDisclosure: string,
	organisationExpressionOfInterest: boolean | null,
	unsavedIds: Array<number>,
	documentTitles: "undefined" | Array<any>,
	justSubmitted: boolean,
	path: null | string,
};

export class QuestionView extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			loading: true,
			consultationData: null,
			commentsData: null,
			submittedDate: null,
			validToSubmit: false,
			path: null,
			hasInitialData: false,
			allowComments: false,
			comments: [], //this contains all the comments, not just the ones displayed to the user. the show property defines whether the comment is filtered out from view.
			questions: [], //this contains all the questions, not just the ones displayed to the user. the show property defines whether the question is filtered out from view.
			sort: "DocumentAsc",
			supportsDownload: false,
			respondingAsOrganisation: false,
			organisationName: "",
			hasTobaccoLinks: false,
			tobaccoDisclosure: "",
			organisationExpressionOfInterest: null,
			unsavedIds: [],
			documentTitles: [],
			justSubmitted: false,
			
		};

		let preloadedData = {};
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data; //this is data from Configure => SupplyData in Startup.cs. the main thing it contains for this call is the cookie for the current user.
		}

		const querystring = this.props.location.search;

		const preloadedCommentsData = preload(
			this.props.staticContext,
			"commentsreview",
			[],
			Object.assign({relativeURL: this.props.match.url}, queryStringToObject(querystring)),
			preloadedData
		);
		const consultationId = this.props.match.params.consultationId;
		const preloadedConsultationData = preload(
			this.props.staticContext,
			"consultation",
			[],
			{consultationId, isReview: true},
			preloadedData
		);

		if (preloadedCommentsData && preloadedConsultationData) {
			if (this.props.staticContext) {
				this.props.staticContext.analyticsGlobals.gidReference = preloadedConsultationData.reference;
				this.props.staticContext.analyticsGlobals.consultationId = preloadedConsultationData.consultationId;
				this.props.staticContext.analyticsGlobals.consultationTitle = preloadedConsultationData.title;
				this.props.staticContext.analyticsGlobals.stage = preloadedConsultationData.consultationState.submittedDate ? "postsubmission" : "presubmission";
			}
			this.state = {
				path: this.props.basename + this.props.location.pathname,
				commentsData: preloadedCommentsData,
				consultationData: preloadedConsultationData,
				submittedDate: preloadedConsultationData.consultationState.submittedDate,
				validToSubmit: preloadedConsultationData.consultationState.supportsSubmission,
				loading: false,
				hasInitialData: true,
				allowComments: (preloadedConsultationData.consultationState.consultationIsOpen && !preloadedConsultationData.consultationState.submittedDate),
				comments: preloadedCommentsData.commentsAndQuestions.comments,
				questions: preloadedCommentsData.commentsAndQuestions.questions,
				sort: preloadedCommentsData.sort,
				supportsDownload: preloadedConsultationData.consultationState.supportsDownload,
				organisationName: preloadedCommentsData.organisationName || "",
				respondingAsOrganisation: false,
				hasTobaccoLinks: false,
				organisationExpressionOfInterest: null,
				tobaccoDisclosure: "",
				unsavedIds: [],
				documentTitles: this.getListOfDocuments(preloadedCommentsData.filters),
				justSubmitted: false,
			};
		}

		let isSSR = false;
		if (this.props.staticContext) {
			isSSR = true;
		}
		const message = `Review page log hit at ${new Date().toJSON()}. running as ${process.env.NODE_ENV} SSR: ${isSSR}`;
		preload(this.props.staticContext, "logging", [], {logLevel: "Warning"}, null, false, "POST", {}, {message}, true);

	}

	//this is temporary for debug purposes.
	logStuff = () => {

		let isSSR = false;
		if (this.props.staticContext) {
			isSSR = true;
		}
		const message = `Review page log hit at ${new Date().toJSON()}. running as ${process.env.NODE_ENV} SSR: ${isSSR}`;

		load("logging", undefined, [], {logLevel: "Warning"}, "POST", {message}, true)
			.then(response => response.data)
			.catch(err => {
				console.error(err);
			});
	};

	gatherData = async () => {
		const querystring = this.props.history.location.search;
		const path = this.props.basename + this.props.location.pathname + this.props.history.location.search;
		this.setState({
			path,
		});
		const commentsData = load("commentsreview", undefined, [], Object.assign({relativeURL: this.props.match.url}, queryStringToObject(querystring)))
			.then(response => response.data)
			.catch(err => {
				if (window) {
					//window.location.assign(path); // Fallback to full page reload if we fail to load data
				} else {
					throw new Error("failed to load comments for review.  " + err);
				}
			});

		if (this.state.consultationData === null) {

			const consultationId = this.props.match.params.consultationId;
			const consultationData = load("consultation", undefined, [], {
				consultationId, isReview: true,
			})
				.then(response => response.data)
				.catch(err => {
					throw new Error("consultationData " + err);
				});

			return {
				consultationData: await consultationData,
				commentsData: await commentsData,
			};
		}
		return {
			consultationData: null,
			commentsData: await commentsData,
		};
	};

	loadDataAndUpdateState = (callback?: Function) => {
		this.gatherData()
			.then(data => {
				if (data.consultationData !== null) {
					this.setState({
						consultationData: data.consultationData,
						commentsData: data.commentsData,
						comments: data.commentsData.commentsAndQuestions.comments,
						questions: data.commentsData.commentsAndQuestions.questions,
						submittedDate: data.consultationData.consultationState.submittedDate,
						validToSubmit: data.consultationData.consultationState.supportsSubmission,
						loading: false,
						allowComments: (data.consultationData.consultationState.consultationIsOpen && !data.consultationData.consultationState.submittedDate),
						supportsDownload: data.consultationData.consultationState.supportsDownload,
						sort: data.commentsData.sort,
						organisationName: data.commentsData.organisationName || "",
						documentTitles: this.getListOfDocuments(data.commentsData.filters),
					});
				} else {
					this.setState({
						commentsData: data.commentsData,
						comments: data.commentsData.commentsAndQuestions.comments,
						questions: data.commentsData.commentsAndQuestions.questions,
						sort: data.commentsData.sort,
						loading: false,
						organisationName: data.commentsData.organisationName || "",
						documentTitles: this.getListOfDocuments(data.commentsData.filters),
					}, () => {
						tagManager({
							event: "generic",
							category: "Consultation comments page",
							action: "Clicked",
							label: "Review filter",
						});
					});
				}
				if (data.consultationData !== null) {
					tagManager({
						event: "pageview",
						gidReference: this.state.consultationData.reference,
						title: this.getPageTitle(true),
						stage: this.state.consultationData.consultationState.submittedDate ? "postsubmission" : "presubmission",
					});
				}
				if (callback) callback();
			})
			.catch(err => {
				throw new Error("gatherData in componentDidMount failed " + err);
			});
	};

	componentWillUnmount() {
		this.unlisten();
	}

	componentDidMount() {
		if (!this.state.hasInitialData) { // if this statement is true then we know we've come from another page
			this.loadDataAndUpdateState(()=>{
				pullFocusByQuerySelector("#root");
			});
		}
		this.unlisten = this.props.history.listen(() => {
			const path = this.props.basename + this.props.location.pathname + this.props.history.location.search;
			if (!this.state.path || path !== this.state.path) {
				this.loadDataAndUpdateState();
			}
		});
	}

	unlisten = () => {
	};


	issueA11yMessage = (message: string) => {
		const unique = new Date().getTime().toString();
		// announcer requires a unique id so we're able to repeat phrases
		this.props.announceAssertive(message, unique);
	};


	getListOfDocuments = (filters: Array<any>) => {
		if (!filters) return;
		return filters.filter(item => item.id === "Document")[0].options
			.map(item => {
				return {
					id: item.id,
					title: item.label,
				};
			});
	};

	getDocumentTitle = (documentId: string) => {
		if (documentId && documentId !== null) {
			// this catch is here temporarily until we have a way to administer questions (it's possible that questions have been placed on documents that are not set to support questions)
			try {
				return this.state.documentTitles.filter(item => item.id === documentId.toString())[0].title;
			}
			catch (err) {
				return "Consultation document ID " + documentId;
			}

		}
	};

	getAppliedFilters(): ReviewAppliedFilterType[] {
		const mapOptions =
			(group: ReviewFilterGroupType) => group.options
				.filter(opt => opt.isSelected)
				.map(opt => ({
					groupTitle: group.title,
					optionLabel: opt.label,
					groupId: group.id,
					optionId: opt.id,
				}));

		return this.state.commentsData.filters
			.map(mapOptions)
			.reduce((arr, group) => arr.concat(group), []);
	}

	getPageTitle = (isForAnalytics: boolean = false) => {
		if (isForAnalytics) return this.state.consultationData.title;
		return `${this.state.consultationData.title} | Review your response`;
	};

	render() {
		if (this.state.loading) return <h1>Loading...</h1>;
		if (this.state.justSubmitted) return <Redirect push to={"submitted"}/>;
		const {reference} = this.state.consultationData;
		const questionsToShow = this.state.questions.filter(question => question.show) || [];

		return (
			<Fragment>
				<Helmet>
					<title>{this.getPageTitle()}</title>
				</Helmet>
				<Prompt
					when={this.state.unsavedIds.length > 0}
					message={`You have ${this.state.unsavedIds.length} unsaved ${this.state.unsavedIds.length === 1 ? "change" : "changes"}. Continue without saving?`}
				/>
				<div className="container">
					<div className="grid">
						<div data-g="12">
							<BreadCrumbsWithRouter links={this.state.consultationData.breadcrumbs}/>
							<UserContext.Consumer>
								{(contextValue: ContextType) => {
									return (
										
											<main role="main">
												<div className="page-header">
													<Header
														title={`Download questions for the consultation "${this.state.consultationData.title}"`}
														reference={reference}
														consultationState={this.state.consultationData.consultationState}
													/>

													<div className="grid">
														<div data-g="12 md:12">
															<ReviewResultsInfo
																questionCount={questionsToShow.length}
																showQuestionsCount={this.state.consultationData.consultationState.shouldShowQuestionsTab}
																sortOrder={this.state.sort}
																appliedFilters={this.getAppliedFilters()}
																path={this.state.path}
																isLoading={this.state.loading}/>

															<div data-qa-sel="comment-list-wrapper">
																{questionsToShow.length > 0 &&
																<div>
																	<ul className="CommentList list--unstyled">
																		{questionsToShow.map((question) => {
																			return (
																				<Question
																					key={question.questionId}
																					unique={`Comment${question.questionId}`}
																					question={question}
																					showAnswer={contextValue.isAuthorised}
																					readOnly={true}
																					documentTitle={this.getDocumentTitle(question.documentId)}
																				/>
																			);
																		})}
																	</ul>
																</div>
																}
															</div>

														</div>
													</div>
												</div>
											</ main>
									);
								}}
							</UserContext.Consumer>
						</div>
					</div>
				</div>
			</Fragment>
		);
	}
}

export default withRouter(withHistory(QuestionView));
