// @flow

import React, { Component, Fragment } from "react";
import { withRouter, Prompt } from "react-router-dom";
import Helmet from "react-helmet";

import preload from "../../data/pre-loader";
import { load } from "../../data/loader";
import {
	saveCommentHandler,
	deleteCommentHandler,
	saveAnswerHandler,
	deleteAnswerHandler,
} from "../../helpers/editing-and-deleting";
import { queryStringToObject } from "../../helpers/utils";
import { pullFocusById } from "../../helpers/accessibility-helpers";
import { tagManager } from "../../helpers/tag-manager";
import { projectInformation } from "../../constants";
import { UserContext } from "../../context/UserContext";

import { Header } from "../Header/Header";
import { PhaseBanner } from "../PhaseBanner/PhaseBanner";
import { BreadCrumbs } from "../Breadcrumbs/Breadcrumbs";
import { FilterPanel } from "../FilterPanel/FilterPanel";
import { ResultsInfo } from "../ResultsInfo/ResultsInfo";
import { withHistory } from "../HistoryContext/HistoryContext";
import { CommentBox } from "../CommentBox/CommentBox";
import { Question } from "../Question/Question";
import { LoginBanner } from "../LoginBanner/LoginBanner";
import { SubmitResponseDialog } from "../SubmitResponseDialog/SubmitResponseDialog";
import { updateUnsavedIds } from "../../helpers/unsaved-comments";

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
};

type StateType = {
	consultationData: ConsultationDataType | null,
	commentsData: ReviewPageViewModelType | null,
	userHasSubmitted: boolean,
	validToSubmit: false,
	viewSubmittedComments: boolean,
	path: string | null,
	hasInitalData: boolean,
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
	unsavedIds: Array<number>,
	documentTitles: Array<any>,
};

export class ReviewListPage extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			loading: true,
			consultationData: null,
			commentsData: null,
			userHasSubmitted: false,
			viewSubmittedComments: false,
			validToSubmit: false,
			path: null,
			hasInitalData: false,
			allowComments: false,
			comments: [], //this contains all the comments, not just the ones displayed to the user. the show property defines whether the comment is filtered out from view.
			questions: [], //this contains all the questions, not just the ones displayed to the user. the show property defines whether the question is filtered out from view.
			sort: "DocumentAsc",
			supportsDownload: false,
			respondingAsOrganisation: false,
			organisationName: "",
			hasTobaccoLinks: false,
			tobaccoDisclosure: "",
			unsavedIds: [],
			documentTitles: [],
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
				this.props.staticContext.globals.gidReference = preloadedConsultationData.reference;
			}
			this.state = {
				path: this.props.basename + this.props.location.pathname,
				commentsData: preloadedCommentsData,
				consultationData: preloadedConsultationData,
				userHasSubmitted: preloadedConsultationData.consultationState.userHasSubmitted,
				validToSubmit: preloadedConsultationData.consultationState.supportsSubmission,
				loading: false,
				hasInitalData: true,
				allowComments: (preloadedConsultationData.consultationState.consultationIsOpen && !preloadedConsultationData.consultationState.userHasSubmitted),
				comments: preloadedCommentsData.commentsAndQuestions.comments,
				questions: preloadedCommentsData.commentsAndQuestions.questions,
				sort: preloadedCommentsData.sort,
				supportsDownload: preloadedConsultationData.consultationState.supportsDownload,
				viewSubmittedComments: false,
				organisationName: preloadedCommentsData.organisationName || "",
				respondingAsOrganisation: false,
				hasTobaccoLinks: false,
				tobaccoDisclosure: "",
				unsavedIds: [],
				documentTitles: this.getListOfDocuments(preloadedCommentsData.filters),
			};
		}
	}

	gatherData = async () => {
		const querystring = this.props.history.location.search;
		const path = this.props.basename + this.props.location.pathname + querystring;
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

	loadDataAndUpdateState = () => {
		this.gatherData()
			.then(data => {
				if (data.consultationData !== null) {
					this.setState({
						consultationData: data.consultationData,
						commentsData: data.commentsData,
						comments: data.commentsData.commentsAndQuestions.comments,
						questions: data.commentsData.commentsAndQuestions.questions,
						userHasSubmitted: data.consultationData.consultationState.userHasSubmitted,
						validToSubmit: data.consultationData.consultationState.supportsSubmission,
						loading: false,
						allowComments: (data.consultationData.consultationState.consultationIsOpen && !data.consultationData.consultationState.userHasSubmitted),
						supportsDownload: data.consultationData.consultationState.supportsDownload,
						sort: data.commentsData.sort,
						organisationName: data.commentsData.organisationName || "",
						documentTitles: this.getListOfDocuments(data.commentsData.filters),
					},
					() => {
						tagManager({
							event: "pageview",
							gidReference: this.state.consultationData.reference,
							title: this.getPageTitle(),
							stage: this.state.userHasSubmitted ? "postsubmission" : "presubmission",
						});
					}
					);
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
						// todo: this is where we'd track an applied filter
					});
				}
			})
			.catch(err => {
				throw new Error("gatherData in componentDidMount failed " + err);
			});
	};

	componentDidMount() {
		if (!this.state.hasInitalData) { //typically this page is accessed by clicking a link on the document page, so it won't SSR.
			this.loadDataAndUpdateState();
		}
		this.props.history.listen(() => {
			this.loadDataAndUpdateState();
		});
	}

	componentDidUpdate(prevProps: PropsType) {
		const oldQueryString = prevProps.location.search;
		const newQueryString = this.props.location.search;
		if (oldQueryString === newQueryString) return;
		tagManager({
			event: "pageview",
			gidReference: this.state.consultationData.reference,
			title: this.getPageTitle(),
			stage: this.state.userHasSubmitted ? "postsubmission" : "presubmission",
		});
		pullFocusById("comments-column");
	}

	submitConsultation = () => {
		const comments = this.state.comments;
		const questions = this.state.questions;
		const organisationName = this.state.organisationName;
		const tobaccoDisclosure = this.state.tobaccoDisclosure;
		const respondingAsOrganisation = this.state.respondingAsOrganisation === "yes";
		const hasTobaccoLinks = this.state.hasTobaccoLinks === "yes";

		let answersToSubmit = [];
		questions.forEach(function (question) {
			if (question.answers != null) {
				answersToSubmit = answersToSubmit.concat(question.answers);
			}
		});
		let submission = {
			comments,
			answers: answersToSubmit,
			organisationName: respondingAsOrganisation ? organisationName : null,
			tobaccoDisclosure: hasTobaccoLinks ? tobaccoDisclosure : null,
			respondingAsOrganisation,
			hasTobaccoLinks,
		};
		load("submit", undefined, [], {}, "POST", submission, true)
			.then(response => {
				response.data.durationFromFirstCommentOrAnswerSavedUntilSubmissionInSeconds = 123456;
				tagManager({
					event: "generic",
					category: "Consultation comments page",
					action: "Response submitted",
					label: `${response.data.comments ? response.data.comments.length : "0"} comments and ${response.data.answers ? response.data.answers.length : "0"} answers`,
				});
				tagManager({
					event: "generic",
					category: "Consultation comments page",
					action: "Length to submit response",
					label: (response.data.durationFromFirstCommentOrAnswerSavedUntilSubmissionInSeconds / 3600).toString(),
				});
				tagManager({
					event: "pageview",
					gidReference: this.state.consultationData.reference,
					title: this.getPageTitle(),
					stage: "submitted",
				});
				this.setState({
					userHasSubmitted: true,
					validToSubmit: false,
					viewSubmittedComments: false,
					allowComments: false,
				});
			})
			.catch(err => {
				console.log(err);
				if (err.response) alert(err.response.statusText);
			});
	};

	//this validation handler code is going to have to get a bit more advanced when questions are introduced, as it'll be possible
	//to answer a question on the review page and the submit button should then enable - if the consultation is open + hasn't already been submitted + all the mandatory questions are answered.
	//(plus there's the whole unsaved changes to deal with. what happens there?)
	validationHander = () => {
		const comments = this.state.comments;
		const questions = this.state.questions;
		let hasAnswers = false;
		questions.forEach(function (question) {
			if (question.answers !== null && question.answers.length > 0) {
				hasAnswers = true;
			}
		});
		const anyCommentsOrAnswers = comments.length > 0 || hasAnswers;
		this.setState({
			validToSubmit: anyCommentsOrAnswers,
			supportsDownload: anyCommentsOrAnswers,
		});
	};

	viewSubmittedCommentsHandler = () => {
		this.setState({
			viewSubmittedComments: true,
		});
		tagManager({
			event: "generic",
			category: "Consultation comments page",
			action: "Clicked",
			label: "Review your response button",
		});
		tagManager({
			event: "pageview",
			gidReference: this.state.consultationData.reference,
			title: this.getPageTitle(),
			stage: this.state.userHasSubmitted ? "postsubmission" : "presubmission",
		});
	};

	fieldsChangeHandler = (e: SyntheticInputEvent) => {
		this.setState({
			[e.target.name]: e.target.value,
		});
		if (e.target.name === "hasTobaccoLinks" || e.target.name === "respondingAsOrganisation") {
			tagManager({
				event: "generic",
				category: "Consultation comments page",
				action: `Submission mandatory question - ${e.target.name}`,
				label: `${e.target.value}`,
			});
		}
	};

	issueA11yMessage = (message: string) => {
		const unique = new Date().getTime().toString();
		// announcer requires a unique id so we're able to repeat phrases
		this.props.announceAssertive(message, unique);
	};

	//these handlers are in the helpers/editing-and-deleting.js utility file as they're also used in CommentList.js
	saveCommentHandler = (e: Event, comment: CommentType) => {
		saveCommentHandler(e, comment, this);
	};

	deleteCommentHandler = (e: Event, comment: CommentType) => {
		deleteCommentHandler(e, comment, this);
	};

	saveAnswerHandler = (e: Event, answer: AnswerType, questionId: number) => {
		saveAnswerHandler(e, answer, questionId, this);
	};

	deleteAnswerHandler = (e: Event, questionId: number, answerId: number) => {
		deleteAnswerHandler(e, questionId, answerId, this);
	};

	updateUnsavedIds = (commentId: string, dirty: boolean) => {
		updateUnsavedIds(commentId, dirty, this);
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

	getPageTitle = () => {
		return `${this.state.consultationData.title} | Response reviewed pre submission`;
	};

	render() {
		if (this.state.loading) return <h1>Loading...</h1>;
		const {reference} = this.state.consultationData;
		const commentsToShow = this.state.comments.filter(comment => comment.show) || [];
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
							<PhaseBanner
								phase={projectInformation.phase}
								name={projectInformation.name}
								repo={projectInformation.repo}
							/>
							<BreadCrumbs links={this.state.consultationData.breadcrumbs}/>
							<main role="main">
								<div className="page-header">
									<Header
										title={this.state.userHasSubmitted ? "Response submitted" : "Review your response"}
										subtitle1={this.state.userHasSubmitted ? "" : "Review and edit your question responses and comments before you submit them to us."}
										subtitle2={this.state.userHasSubmitted ? "" : "Once they have been submitted you will not be able to edit them further or add any extra comments."}
										reference={reference}
										consultationState={this.state.consultationData.consultationState}
									/>
									<UserContext.Consumer>
										{(contextValue: ContextType) => {
											return (
												!contextValue.isAuthorised ?
													<LoginBanner
														signInButton={true}
														currentURL={this.props.match.url}
														signInURL={contextValue.signInURL}
														registerURL={contextValue.registerURL}
													/> :
													<div className="grid">
														{(this.state.userHasSubmitted && !this.state.viewSubmittedComments) ?
															<div data-g="12">
																<button
																	className="btn btn--cta"
																	data-qa-sel="review-submitted-comments"
																	onClick={this.viewSubmittedCommentsHandler}>Review your response
																</button>
																{this.state.supportsDownload &&
																<a
																	onClick={() => {
																		tagManager({
																			event: "generic",
																			category: "Consultation comments page",
																			action: "Clicked",
																			label: "Download your response button",
																		});
																	}}
																	className="btn btn--secondary"
																	href={`${this.props.basename}/api/exportexternal/${this.props.match.params.consultationId}`}>Download
																	your response</a>
																}
																<h2>What happens next?</h2>
																<p>We will review all the submissions received for this consultation. Our response
																	will
																	be published on the website around the time the guidance is published.</p>
																<h2>Help us improve our online commenting service</h2>
																<p>This is the first time we have used our new online commenting software on a live
																	consultation. We'd really like to hear your feedback so that we can keep improving
																	it.</p>
																<p>Answer our short, anonymous survey (4 questions, 2 minutes).</p>
																<p>
																	<a className="btn btn--cta"
																		 href="https://in.hotjar.com/s?siteId=119167&surveyId=109567" target="_blank"
																		 rel="noopener noreferrer">
																		Answer the survey
																	</a>
																</p>
															</div>
															:
															<Fragment>
																<div data-g="12 md:3" className="sticky">
																	<FilterPanel filters={this.state.commentsData.filters} path={this.state.path}/>
																</div>
																<div data-g="12 md:9">
																	<ResultsInfo commentCount={commentsToShow.length}
																							 showCommentsCount={this.state.consultationData.consultationState.shouldShowCommentsTab}
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
																					const isUnsaved = this.state.unsavedIds.includes(`${question.questionId}q`);
																					return (
																						<Question
																							updateUnsavedIds={this.updateUnsavedIds}
																							isUnsaved={isUnsaved}
																							readOnly={!this.state.allowComments || this.state.userHasSubmitted}
																							key={question.questionId}
																							unique={`Comment${question.questionId}`}
																							question={question}
																							saveAnswerHandler={this.saveAnswerHandler}
																							deleteAnswerHandler={this.deleteAnswerHandler}
																							documentTitle={this.getDocumentTitle(question.documentId)}
																						/>
																					);
																				})}
																			</ul>
																		</div>
																		}
																		{commentsToShow.length === 0 ? <p>{/*No comments yet*/}</p> :
																			<ul className="CommentList list--unstyled">
																				{commentsToShow.map((comment) => {
																					return (
																						<CommentBox
																							readOnly={!this.state.allowComments || this.state.userHasSubmitted}
																							isVisible={this.props.isVisible}
																							key={comment.commentId}
																							unique={`Comment${comment.commentId}`}
																							comment={comment}
																							documentTitle={this.getDocumentTitle(comment.documentId)}
																							saveHandler={this.saveCommentHandler}
																							deleteHandler={this.deleteCommentHandler}
																							updateUnsavedIds={this.updateUnsavedIds}
																						/>
																					);
																				})}
																			</ul>
																		}
																	</div>
																	<SubmitResponseDialog
																		unsavedIds={this.state.unsavedIds}
																		isAuthorised={contextValue.isAuthorised}
																		userHasSubmitted={this.state.userHasSubmitted}
																		validToSubmit={this.state.validToSubmit}
																		submitConsultation={this.submitConsultation}
																		fieldsChangeHandler={this.fieldsChangeHandler}
																		respondingAsOrganisation={this.state.respondingAsOrganisation}
																		organisationName={this.state.organisationName}
																		hasTobaccoLinks={this.state.hasTobaccoLinks}
																		tobaccoDisclosure={this.state.tobaccoDisclosure}
																	/>
																</div>
															</Fragment>
														}
													</div>
											);
										}}
									</UserContext.Consumer>
								</div>
							</ main>
						</div>
					</div>
				</div>
			</Fragment>
		);
	}
}

export default withRouter(withHistory(ReviewListPage));
