// @flow

import React, { Component, Fragment } from "react";
import { withRouter, Link, Prompt } from "react-router-dom";
//import stringifyObject from "stringify-object";
import { LiveMessage } from "react-aria-live";

import preload from "../../data/pre-loader";
import { load } from "../../data/loader";
import {
	saveCommentHandler,
	deleteCommentHandler,
	saveAnswerHandler,
	deleteAnswerHandler,
} from "../../helpers/editing-and-deleting";
import { pullFocusByQuerySelector } from "../../helpers/accessibility-helpers";
import { mobileWidth } from "../../constants";
import { getElementPositionWithinDocument, getSectionTitle , canUseDOM } from "../../helpers/utils";
import { updateUnsavedIds } from "../../helpers/unsaved-comments";
import { tagManager } from "../../helpers/tag-manager";

import { CommentBox } from "../CommentBox/CommentBox";
import { Question } from "../Question/Question";
import LoginBannerWithRouter from "../LoginBanner/LoginBanner";
import { UserContext } from "../../context/UserContext";

import { createQuestionPdf } from "../QuestionView/QuestionViewDocument";

import { Alert } from "@nice-digital/nds-alert";

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
	announceAssertive: Function,
	getTitleFunction: Function,
};

type StateType = {
	comments: Array<CommentType>,
	questions: Array<QuestionType>,
	loading: boolean,
	allowComments: boolean,
	initialDataLoaded: boolean,
	drawerOpen: boolean,
	drawerMobile: boolean,
	viewComments: boolean,
	shouldShowDrawer: boolean,
	shouldShowCommentsTab: boolean,
	shouldShowQuestionsTab: boolean,
	error: string,
	unsavedIds: Array<number>,
	endDate: string,
	allowOrganisationCodeLogin: Boolean,
	isAuthorised: boolean,
	otherUsersComments: Array<CommentType>,
	otherUsersQuestions: Array<QuestionType>,
};

export class CommentList extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			comments: [],
			questions: [],
			loading: true,
			allowComments: true,
			error: "",
			initialDataLoaded: false,
			drawerOpen: false,
			drawerMobile: false,
			viewComments: true,
			shouldShowDrawer: false,
			shouldShowCommentsTab: false,
			shouldShowQuestionsTab: false,
			unsavedIds: [],
			endDate: "",
			allowOrganisationCodeLogin: false,
			isAuthorised: false,
			otherUsersComments: [],
			otherUsersQuestions: [],
		};

		let preloadedData = {};

		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data; //this is data from Configure => SupplyData in Startup.cs. the main thing it contains for this call is the cookie for the current user.
		}

		const enableOrganisationalCommentingFeature = ((preloadedData && preloadedData.organisationalCommentingFeature) || (canUseDOM() && window.__PRELOADED__ && window.__PRELOADED__["organisationalCommentingFeature"]));

		const preloadedCommentsForCurrentUser = preload(
			this.props.staticContext,
			"comments",
			[],
			{sourceURI: this.props.match.url},
			preloadedData,
		);

		if (preloadedCommentsForCurrentUser) {
			let allowComments = preloadedCommentsForCurrentUser.consultationState.consultationIsOpen && !preloadedCommentsForCurrentUser.consultationState.submittedDate;

			this.state = {
				comments: preloadedCommentsForCurrentUser.comments,
				questions: preloadedCommentsForCurrentUser.questions,
				loading: false,
				allowComments: allowComments,
				error: "",
				initialDataLoaded: true,
				viewComments: preloadedCommentsForCurrentUser.consultationState.shouldShowCommentsTab,
				shouldShowDrawer: preloadedCommentsForCurrentUser.consultationState.shouldShowDrawer,
				shouldShowCommentsTab: preloadedCommentsForCurrentUser.consultationState.shouldShowCommentsTab,
				shouldShowQuestionsTab: preloadedCommentsForCurrentUser.consultationState.shouldShowQuestionsTab,
				drawerOpen: false,
				drawerMobile: false,
				unsavedIds: [],
				endDate: preloadedCommentsForCurrentUser.consultationState.endDate,
				enableOrganisationalCommentingFeature,
				allowOrganisationCodeLogin: preloadedCommentsForCurrentUser.consultationState.consultationIsOpen,
				otherUsersComments: [],
				otherUsersQuestions: [],
			};
		}

		const preloadedCommentsFromOtherCodeUsers = preload(
			this.props.staticContext,
			"commentsForOtherOrgCommenters",
			[],
			{sourceURI: this.props.match.url},
			preloadedData,
		);

		if (preloadedCommentsFromOtherCodeUsers) {
			this.state.otherUsersComments = preloadedCommentsFromOtherCodeUsers.comments;
			this.state.otherUsersQuestions = preloadedCommentsFromOtherCodeUsers.questions;
		}
	}

	loadCommentsFromOtherCodeUsers = () => {
		load("commentsForOtherOrgCommenters", undefined, [], {sourceURI: this.props.match.url}).then(
			function(response) {
				const otherUsersComments = response.data.comments;
				const otherUsersQuestions = response.data.questions;

				this.setState({
					otherUsersComments,
					otherUsersQuestions,
					loading: false,
				});
			}.bind(this))
			.catch(err => console.log("load comments in commentlist " + err));
	};

	loadCommentsForCurrentUser = () => {
		const isOrganisationCommenter = this.context.isOrganisationCommenter;

		load("comments", undefined, [], {sourceURI: this.props.match.url}).then(
			function(response) {
				let allowComments = response.data.consultationState.consultationIsOpen && !response.data.consultationState.submittedDate;

				this.setState({
					comments: response.data.comments,
					questions: response.data.questions,
					loading: (isOrganisationCommenter ? true : false),
					allowComments: allowComments,
					shouldShowDrawer: response.data.consultationState.shouldShowDrawer,
					shouldShowCommentsTab: response.data.consultationState.shouldShowCommentsTab,
					shouldShowQuestionsTab: response.data.consultationState.shouldShowQuestionsTab,
					endDate: response.data.consultationState.endDate,
					allowOrganisationCodeLogin: response.data.consultationState.consultationIsOpen,
				});

				if (isOrganisationCommenter) {
					this.loadCommentsFromOtherCodeUsers();
				}
			}.bind(this))
			.catch(err => console.log("load comments in commentlist " + err));
	};

	componentDidMount() {
		const isAuthorised = this.context.isAuthorised;

		if (!this.state.initialDataLoaded) {
			this.loadCommentsForCurrentUser();
		}

		// We can't prerender whether we're on mobile cos SSR doesn't have a window
		// sets isAuthorised from context
		this.setState({
			drawerMobile: this.isMobile(),
			isAuthorised,
		});
	}

	componentDidUpdate(prevProps: PropsType) {
		const routeChanged = (prevProps.location.pathname + prevProps.location.search) !== (this.props.location.pathname + this.props.location.search);
		const authorisationChanged = this.state.isAuthorised !== this.context.isAuthorised;

		if (routeChanged || authorisationChanged) {
			this.setState({
				loading: true,
				unsavedIds: [],
				isAuthorised: this.context.isAuthorised,
			});
			this.loadCommentsForCurrentUser();
		}
	}

	issueA11yMessage = (message: string) => {
		const unique = new Date().getTime().toString();
		// announcer requires a unique id so we're able to repeat phrases
		this.props.announceAssertive(message, unique);
	};

	newComment = (e: Event, newComment: CommentType) => {

		if ((typeof(newComment.order) === "undefined" || (newComment.order === null)) && e !== null) {
			///these values are already set when user has selected text. when they've clicked a button though they'll be unset.
			newComment.order = getElementPositionWithinDocument(e.currentTarget);
			newComment.sectionHeader = getSectionTitle(e.currentTarget);
		}

		this.setState({
			drawerOpen: true,
			viewComments: true,
		});
		let comments = this.state.comments;
		//negative ids are unsaved / new comments
		let idToUseForNewBox = -1;
		if (comments && comments.length) {
			const existingIds = comments.map(c => c.commentId);
			const lowestExistingId = Math.min.apply(Math, existingIds);
			idToUseForNewBox = lowestExistingId >= 0 ? -1 : lowestExistingId - 1;
		} else {
			comments = [];
		}
		const generatedComment = Object.assign({}, newComment, {
			commentId: idToUseForNewBox,
			show: false,
		});
		comments.unshift(generatedComment);
		this.setState({comments});
		setTimeout(() => {
			pullFocusByQuerySelector(`#Comment${idToUseForNewBox}`, false, "#comments-panel");
		}, 0);
	};

	//these handlers have moved to the helpers/editing-and-deleting.js utility file as they're also used in ReviewList.js
	saveCommentHandler = (e: Event, comment: CommentType) => {
		saveCommentHandler(e, comment, this);
	};

	deleteCommentHandler = (e: Event, commentId: number) => {
		deleteCommentHandler(e, commentId, this);
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

	//old drawer code:
	isMobile = () => {
		if (typeof document !== "undefined") {
			return (
				document.getElementsByTagName("body")[0].offsetWidth <= mobileWidth
			);
		}
		return false;
	};

	drawerClassnames = () => {
		const open = this.state.drawerOpen ? "Drawer--open" : "";
		const mobile = this.state.drawerMobile ? "Drawer--mobile" : "";
		return `Drawer ${open} ${mobile}`;
	};

	handleClick = (event: string) => {

		switch (event) {
			case "toggleOpenComments":
				this.setState(prevState => {
					const drawerOpen = prevState.drawerOpen && prevState.viewComments ? !prevState.drawerOpen : true;
					tagManager({
						event: "generic",
						category: "Consultation comments page",
						action: "Clicked",
						label: `${drawerOpen ? "Open" : "Close"} comments panel button`,
					});
					return (
						{
							drawerOpen,
							viewComments: true,
						}
					);
				});
				pullFocusByQuerySelector("#comments-panel");
				break;

			case "toggleOpenQuestions":
				this.setState(prevState => {
					const drawerOpen = prevState.drawerOpen && !prevState.viewComments ? !prevState.drawerOpen : true;
					tagManager({
						event: "generic",
						category: "Consultation comments page",
						action: "Clicked",
						label: `${drawerOpen ? "Open" : "Close"} questions panel button`,
					});
					return (
						{
							drawerOpen,
							viewComments: false,
						}
					);
				});
				pullFocusByQuerySelector("#comments-panel");
				break;

			case "createQuestionPDF":
				const questionsForPDF = this.state.questions;
				const titleForPDF = this.props.getTitleFunction();
				const endDate = this.state.endDate;
				createQuestionPdf(questionsForPDF, titleForPDF, endDate);
				break;

			default:
				return;
		}
	};

	render() {
		if (!this.state.shouldShowDrawer) {
			return null;
		}

		const a11yMessage = () => {
			if (!this.state.drawerOpen) {
				return "Comments and questions panel closed";
			} else {
				if (this.state.viewComments) {
					return "Comments panel open";
				} else {
					return "Questions panel open";
				}
			}
		};

		return (
			<Fragment>
				<Prompt
					when={this.state.unsavedIds.length > 0}
					message={`You have ${this.state.unsavedIds.length} unsaved ${this.state.unsavedIds.length === 1 ? "change" : "changes"}. Continue without saving?`}
				/>
				<LiveMessage message={a11yMessage()} aria-live="assertive"/>
				<section aria-label="Commenting panel"
								 className={this.drawerClassnames()}>
					<div className="Drawer__controls">
						{this.state.shouldShowCommentsTab &&
						<button
							data-qa-sel="open-commenting-panel"
							id="js-drawer-toggleopen-comments"
							className={`Drawer__control Drawer__control--comments ${(this.state.viewComments ? "active" : "active")}`}
							onClick={() => this.handleClick("toggleOpenComments")}
							aria-controls="comments-panel"
							aria-haspopup="true"
							aria-label={this.state.drawerOpen ? "Close the commenting panel" : "Open the commenting panel"}
							tabIndex="0">
							{!this.state.drawerMobile ?
								<span>{(this.state.drawerOpen && this.state.viewComments ? "Close comments" : "Open comments")}</span>
								:
								<span
									className={`icon ${
										this.state.drawerOpen
											? "icon--chevron-right"
											: "icon--chevron-left"}`}
									aria-hidden="true"
									data-qa-sel="close-commenting-panel"
								/>
							}
						</button>
						}
						{this.state.shouldShowQuestionsTab &&
						<button
							data-qa-sel="open-questions-panel"
							id="js-drawer-toggleopen-questions"
							className={`Drawer__control Drawer__control--questions ${(this.state.viewComments ? "active" : "active")}`}
							onClick={() => this.handleClick("toggleOpenQuestions")}
							aria-controls="questions-panel"
							aria-haspopup="true"
							aria-label={this.state.drawerOpen ? "Close the questions panel" : "Open the questions panel"}
							tabIndex="0">
							{!this.state.drawerMobile ?
								<span>{(this.state.drawerOpen && !this.state.viewComments ? "Close questions" : "Open questions")}</span>
								:
								<span
									className={`icon ${
										this.state.drawerOpen
											? "icon--chevron-right"
											: "icon--chevron-left"}`}
									aria-hidden="true"
									data-qa-sel="close-questions-panel"
								/>
							}
						</button>
						}
					</div>
					<div
						aria-disabled={!this.state.drawerOpen && (this.state.shouldShowQuestionsTab || this.state.shouldShowCommentsTab)}
						data-qa-sel="comment-panel"
						id="comments-panel"
						className={`Drawer__main ${this.state.drawerOpen ? "Drawer__main--open" : "Drawer__main--closed"}`}
					>
						<UserContext.Consumer>
							{(contextValue: ContextType) => {
								return (
									<div data-qa-sel="comment-list-wrapper">

										{contextValue.isAuthorised &&
												<Link
													to={`/${this.props.match.params.consultationId}/review`}
													data-qa-sel="review-all-comments"
													className="btn btn--cta mt--c">
													Review and submit your response &nbsp;&nbsp;
													<span className="icon icon--chevron-right" aria-hidden="true" />
												</Link>
										}

										{contextValue.isOrganisationCommenter && !contextValue.isLead &&
											<Alert type="info" role="status" aria-live="polite">
												<p>You are commenting on behalf of {contextValue.organisationName}.</p>
												<p>When you submit your response it will be submitted to the organisational lead at {contextValue.organisationName}. <strong>On submission your email address and responses will be visible to other members or associates of your organisation who are using the same commenting code.</strong></p>
											</Alert>
										}

										{this.state.error !== "" ?
											<div className="errorBox">
												<p>We couldn{"'"}t {this.state.error} your comment. Please try again in a few minutes.</p>
												<p>If the problem continues please <a href={"/get-involved/contact-us"}>contact us</a>.</p>
											</div>
											: null}

										{this.state.loading ? <p>Loading...</p> : (
											<Fragment>
												{contextValue.isAuthorised ? (
													<div className={`${this.state.viewComments ? "show" : "hide"}`}>
														{(this.state.comments.length === 0 && this.state.otherUsersComments.length === 0) ? <p>No comments yet</p> :
															<ul className="CommentList list--unstyled mt--0">
																{this.state.comments.map((comment) => {
																	return (
																		<CommentBox
																			updateUnsavedIds={this.updateUnsavedIds}
																			readOnly={!this.state.allowComments}
																			key={comment.commentId}
																			unique={`Comment${comment.commentId}`}
																			comment={comment}
																			saveHandler={this.saveCommentHandler}
																			deleteHandler={this.deleteCommentHandler}
																		/>
																	);
																})}
																{!contextValue.isLead &&
																	this.state.otherUsersComments.map((otherUsersComment) => {
																		return (
																			<CommentBox
																				updateUnsavedIds={this.updateUnsavedIds}
																				readOnly={true}
																				key={otherUsersComment.commentId}
																				unique={`Comment${otherUsersComment.commentId}`}
																				comment={otherUsersComment}
																				saveHandler={this.saveCommentHandler}
																				deleteHandler={this.deleteCommentHandler}
																			/>
																		);
																	})
																}
															</ul>
														}
													</div>
												) : (
													<LoginBannerWithRouter
														signInButton={true}
														currentURL={this.props.match.url}
														signInURL={contextValue.signInURL}
														registerURL={contextValue.registerURL}
														allowOrganisationCodeLogin={(this.state.allowOrganisationCodeLogin && contextValue.organisationalCommentingFeature)}
													/>
												)}

												<div className={`${this.state.viewComments ? "hide" : "show"}`}>

													<button
														data-qa-sel="create-question-pdf"
														id="js-create-question-pdf"
														className="btn btn--primary"
														onClick={() => this.handleClick("createQuestionPDF")}>
													Download questions (PDF)
													</button>
													{contextValue.isAuthorised ?
														<p className="mt--0">Please answer the following questions</p>
														:
														<p className="CommentBox__validationMessage">You must be signed in to answer questions</p>
													}

													<ul className={`CommentList list--unstyled ${contextValue.isAuthorised ? "mt--0" : ""}`}>
														{this.state.questions.map((question) => {
															const isUnsaved = this.state.unsavedIds.includes(`${question.questionId}q`);

															let matchingQuestion = this.state.otherUsersQuestions.find((otherUsersQuestion) => {
																return otherUsersQuestion.questionId === question.questionId;
															});

															const otherUsersAnswers = matchingQuestion ? matchingQuestion.answers : [];

															return (
																<Question
																	isUnsaved={isUnsaved}
																	updateUnsavedIds={this.updateUnsavedIds}
																	readOnly={!this.state.allowComments}
																	key={question.questionId}
																	unique={`Comment${question.questionId}`}
																	question={question}
																	otherUsersAnswers={otherUsersAnswers}
																	saveAnswerHandler={this.saveAnswerHandler}
																	deleteAnswerHandler={this.deleteAnswerHandler}
																	showAnswer={contextValue.isAuthorised}
																/>
															);
														})}
													</ul>

												</div>
											</Fragment>
										)}
									</div>
								);
							}}
						</UserContext.Consumer>
					</div>
				</section>
			</Fragment>
		);
	}
}

export default withRouter(CommentList);
CommentList.contextType = UserContext;
