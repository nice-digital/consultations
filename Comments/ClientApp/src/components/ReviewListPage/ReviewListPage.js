// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router";
import { StickyContainer, Sticky } from "react-sticky";
import stringifyObject from "stringify-object";

import preload from "../../data/pre-loader";
import { load } from "../../data/loader";
import { saveCommentHandler, deleteCommentHandler, saveAnswerHandler, deleteAnswerHandler } from "../../helpers/editing-and-deleting";
import { queryStringToObject } from "../../helpers/utils";
import { pullFocusById } from "../../helpers/accessibility-helpers";
import { projectInformation } from "../../constants";
import { UserContext } from "../../context/UserContext";

import { Header } from "../Header/Header";
import { PhaseBanner } from "../PhaseBanner/PhaseBanner";
import { BreadCrumbs } from "../Breadcrumbs/Breadcrumbs";
import { FilterPanel } from "../FilterPanel/FilterPanel";
import { withHistory } from "../HistoryContext/HistoryContext";
import { CommentBox } from "../CommentBox/CommentBox";
import { Question } from "../Question/Question";
import { LoginBanner } from "../LoginBanner/LoginBanner";

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
	consultationData: ConsultationDataType,
	commentsData: any, //TODO: any
	userHasSubmitted: boolean,
	validToSubmit: false,
	viewSubmittedComments: boolean,
	path: string,
	hasInitalData: boolean,
	allowComments: boolean,
	comments: Array<CommentType>,
	questions: Array<QuestionType>,
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
			comments: [],
			questions: []
		};

		let preloadedData = {};
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data; //this is data from Configure => SupplyData in Startup.cs. the main thing it contains for this call is the cookie for the current user.
		}
	
		const preloadedCommentsData = preload(
			this.props.staticContext,
			"commentsreview",
			[],
			{ sourceURI: this.props.match.url },
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
			};
		}
	}

	gatherData = async () => {

		const querystring = this.props.history.location.search;
		const path = this.props.basename + this.props.location.pathname + querystring;
		this.setState({
			path,
		});

		const commentsData = load("commentsreview", undefined, [], Object.assign({ sourceURI: this.props.match.url }, queryStringToObject(querystring)))
			.then(response => response.data)
			.catch(err => {
				if (window){
					window.location.assign(path); // Fallback to full page reload if we fail to load data
				} else{
					throw new Error("failed to load comments for review.  " + err);
				}				
			});	

		if (this.state.consultationData === null){

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
			if (data.consultationData !== null){
				this.setState({
					consultationData: data.consultationData,
					commentsData: data.commentsData,
					comments: data.commentsData.commentsAndQuestions.comments,
					questions: data.commentsData.commentsAndQuestions.questions,
					userHasSubmitted: data.consultationData.consultationState.userHasSubmitted,
					validToSubmit: data.consultationData.consultationState.supportsSubmission,
					loading: false,
					allowComments: (data.consultationData.consultationState.consultationIsOpen && !data.consultationData.consultationState.userHasSubmitted),
				});
			} else{
				this.setState({
					commentsData: data.commentsData,
					comments: data.commentsData.commentsAndQuestions.comments,
					questions: data.commentsData.commentsAndQuestions.questions,
					loading: false,
				});
			}			
		})
		.catch(err => {
			throw new Error("gatherData in componentDidMount failed " + err);
		});
	}

	componentDidMount() {
		if (!this.state.hasInitalData){
			// this.loadDataAndUpdateState(); 	
		}
		this.props.history.listen(location => {
			this.loadDataAndUpdateState();
		});
	}

	componentDidUpdate(prevProps: PropsType) {
		const oldQueryString = prevProps.location.search;
		const newQueryString = this.props.location.search;
		if (oldQueryString === newQueryString) return;
		pullFocusById("comments-column");
	}

	getCurrentSourceURI = () => {
		const queryParams = queryStringToObject(this.props.location.search);
		return queryParams.sourceURI;
	};

	submitConsultation = () => {
		const comments = this.state.commentsData.commentsAndQuestions.comments;
		let answersToSubmit = [];

		if (typeof(this.reviewList) !== "undefined"){
			const questions = this.state.commentsData.commentsAndQuestions.questions;
			questions.forEach(function(question){
				if (question.answers != null){
					answersToSubmit = answersToSubmit.concat(question.answers);
				}
			});
		}
		let commentsAndAnswers = {comments: comments, answers: answersToSubmit};

		load("submit", undefined, [], {}, "POST", commentsAndAnswers, true)
			.then(() => {
				this.submittedHandler();
			})
			.catch(err => {
				console.log(err);
				if (err.response) alert(err.response.statusText);
			});
	};

	submittedHandler = () => {
		this.setState({
			userHasSubmitted: true,
			validToSubmit: false,
			viewSubmittedComments: false,
		});
	};

	//this validation handler code is going to have to get a bit more advanced when questions are introduced, as it'll be possible
	//to answer a question on the review page and the submit button should then enable - if the consultation is open + hasn't already been submitted + all the mandatory questions are answered.
	//(plus there's the whole unsaved changes to deal with. what happens there?)
	validationHander = () => {
		const comments = this.reviewList.getComments();
		let hasAnswers = false;
		if (typeof(this.reviewList) !== "undefined"){
			const questions = this.reviewList.getQuestions();

			questions.forEach(function(question){
				if (question.answers !== null && question.answers.length > 0){
					hasAnswers = true;
				}
			});
		}
		this.setState({
			validToSubmit: comments.length > 0 || hasAnswers,
		});
	};

	viewSubmittedCommentsHandler = () => {
		this.setState({
			viewSubmittedComments: true,
		});
	};

	//these handlers are in the helpers/editing-and-deleting.js utility file as they're also used in CommentList.js
	saveCommentHandler = (e: Event, comment: CommentType) => {
		saveCommentHandler(e, comment, this);
	}
	deleteCommentHandler = (e: Event, comment: CommentType) => {
		deleteCommentHandler(e, comment, this);
	}
	saveAnswerHandler = (e: Event, answer: AnswerType) => {
		saveAnswerHandler(e, answer, this);
	}
	deleteAnswerHandler = (e: Event, answer: AnswerType) => {
		deleteAnswerHandler(e, answer, this);
	}	

	render() {
		if (this.state.loading) return <h1>Loading...</h1>;
		const { title, reference } = this.state.consultationData;	
		const commentsToShow = this.state.comments || []; //.filter(comment => !comment.show);
		const questionsToShow = this.state.questions || []; //.filter(question => !question.show);

		return (
			<Fragment>
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
										title={title}
										reference={reference}
										consultationState={this.state.consultationData.consultationState}/>
									<h2 className="mt--0">{this.state.userHasSubmitted ? "Comments submitted" : "Comments for review"}</h2>
									<UserContext.Consumer>
										{ (contextValue: ContextType) => {
											return (
												!contextValue.isAuthorised ?
													<LoginBanner
															signInButton={true}
															currentURL={this.props.match.url}
															signInURL={contextValue.signInURL}
															registerURL={contextValue.registerURL}
														/> :
														(
															(this.state.userHasSubmitted && !this.state.viewSubmittedComments) ?
																<div className="hero">
																	<div className="hero__container">
																		<div className="hero__body">
																			<div className="hero__copy">
																				<p className="hero__intro" data-qa-sel="submitted-text">Thank you, your comments have been submitted.</p>
																				<div className="hero__actions">
																					<button className="btn" data-qa-sel="review-submitted-comments" onClick={this.viewSubmittedCommentsHandler}>Review all submitted comments</button>
																				</div>
																			</div>
																		</div>
																	</div>
																</div>
																: 
															(
																<StickyContainer className="grid">
																	<div data-g="12 md:9 md:push:3">
						
																		{/* <ReviewListWithRouter
																			isVisible={true}
																			isSubmitted={this.state.userHasSubmitted}
																			wrappedComponentRef={component => (this.reviewList = component)}
																			submittedHandler={this.submittedHandler}
																			validationHander={this.validationHander}
																			comments={this.state.commentsData.commentsAndQuestions.comments}
																			questions={this.state.commentsData.commentsAndQuestions.questions}
																			loading={this.state.loading}
																			/> */}
						
																		<div data-qa-sel="comment-list-wrapper">					
																					
																			{questionsToShow.length > 0 &&
																				<div>
																					<p>We would like to hear your views on the draft recommendations presented in the guideline, and any comments you may have on the rationale and impact sections in the guideline and the evidence presented in the evidence reviews documents. We would also welcome views on the Equality Impact Assessment.</p>
																					<p>We would like to hear your views on these questions:</p>
																					<ul className="CommentList list--unstyled">
																						{questionsToShow.map((question) => {
																							return (
																								<Question
																									readOnly={!this.state.allowComments}
																									isVisible={this.props.isVisible}
																									key={question.questionId}
																									unique={`Comment${question.questionId}`}
																									question={question}
																									saveAnswerHandler={this.saveAnswerHandler}
																									deleteAnswerHandler={this.deleteAnswerHandler}
																								/>
																							);
																						})}
																					</ul>
																				</div>						
																			}														
																			{commentsToShow.length === 0 ? <p>No comments yet</p> :
																				<ul className="CommentList list--unstyled">
																					{commentsToShow.map((comment) => {
																						return (
																							<CommentBox
																								readOnly={!this.state.allowComments}
																								isVisible={this.props.isVisible}
																								key={comment.commentId}
																								unique={`Comment${comment.commentId}`}
																								comment={comment}
																								saveHandler={this.saveCommentHandler}
																								deleteHandler={this.deleteCommentHandler}
																							/>
																						);
																					})}
																				</ul>
																			}
																		</div>		
																		
																		{this.state.userHasSubmitted ?
																			<div className="hero">
																				<div className="hero__container">
																					<div className="hero__body">
																						<div className="hero__copy">
																							<p className="hero__intro" data-qa-sel="submitted-text">Thank you, your comments have been submitted.</p>
																							<button className="btn btn--secondary">Download all comments</button>
																						</div>
																					</div>
																				</div>
																			</div>
																			:
																			<div className="hero">
																				<div className="hero__container">
																					<div className="hero__body">
																						<div className="hero__copy">
																							{/* <h1 className="hero__title">Hero title</h1> */}
																							<p className="hero__intro">You are about to submit your final response to NICE</p>
																							<p>After submission you won't be able to:</p>
																							<ul>
																								<li>edit your comments further</li>
																								<li>add any extra comments.</li>
																							</ul>
																							<p>Do you want to continue?</p>
																							<UserContext.Consumer>
																								{contextValue => {
																									if (contextValue.isAuthorised) {
																										return (
																											<Fragment>
																												<h3 className="mt--0">Ready to submit?</h3>
																												<button
																													disabled={!this.state.validToSubmit}
																													className="btn btn--cta"
																													data-qa-sel="submit-comment-button"
																													onClick={this.submitConsultation}
																												>{this.state.userHasSubmitted ? "Comments submitted": "Submit your comments"}
																												</button>
																												<button className="btn btn--secondary">Download all comments</button>
																											</Fragment>
																										);
																									}
																								}}
																							</UserContext.Consumer>
																						</div>
																					</div>
																				</div>
																			</div>
																		}
																	</div>
																	<div data-g="12 md:3 md:pull:9">
																		<Sticky disableHardwareAcceleration>
																			{({ style }) => (
																				<div style={style}>
																					<FilterPanel filters={this.state.commentsData.filters} path={this.state.path} />
																				</div>
																			)}
																		</Sticky>
																	</div>
																</StickyContainer>
															)
														)
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