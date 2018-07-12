// @flow

import React, { Component } from "react";
import { withRouter, Link } from "react-router-dom";
import { load } from "./../../data/loader";
import preload from "../../data/pre-loader";
import { CommentBox } from "../CommentBox/CommentBox";
import { Question } from "../Question/Question";
import { LoginBanner } from "./../LoginBanner/LoginBanner";
import { UserContext } from "../../context/UserContext";
import { queryStringToObject, replaceFormat } from "../../helpers/utils";
import {pullFocusById} from "../../helpers/accessibility-helpers";
//import stringifyObject from "stringify-object";

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
	isVisible: boolean,
	isReviewPage: boolean,
	filterByDocument: number,
	isSubmitted: boolean,
	submittedHandler: Function,
	validationHander: Function,
	viewComments: boolean //when false, we view questions.
};

type StatusType = {
	statusId: number,
	name: string
};

type CommentType = {
	commentId: number,
	lastModifiedDate: Date,
	lastModifiedByUserId: string,
	commentText: string,
	locationId: number,
	sourceURI: string,
	htmlElementID: string,
	rangeStart: string,
	rangeStartOffset: string,
	rangeEnd: string,
	rangeEndOffset: string,
	quote: string,
	commentOn: string,
	show: boolean,
	status: StatusType
};

type QuestionTypeType = {
	description: string,
	hasTextAnswer: boolean,
	hasBooleanAnswer: boolean
};

type QuestionType = {
	questionId: number,
	questionText: string,
	questionTypeId: number,
	questionOrder: number,
	lastModifiedDate: Date,
	lastModifiedByUserId: string,
	questionType: QuestionTypeType,
	answers: Array<AnswerType>,
	show: boolean,
	commentOn: string,
	sourceURI: string,
};

type AnswerType = {
	answerId: number,
	answerText: string,
	answerBoolean: boolean,
	questionId: number,
	lastModifiedDate: Date,
	lastModifiedByUserId: string,
	statusId: number,
	status: StatusType
};

type StateType = {
	comments: Array<CommentType>,
	questions: Array<QuestionType>,
	loading: boolean,
	allowComments: boolean
};

type ContextType = any;

export class CommentList extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			comments: [],
			questions: [],
			loading: true,
			allowComments: true
		};
		let preloadedData = {};
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data;
		}

		// if (this.props.isReviewPage){
		// 	const preloaded = preload(
		// 		this.props.staticContext,
		// 		"review",
		// 		[],
		// 		{ sourceURI: this.props.match.url },
		// 		preloadedData
		// 	);
		// } else{
		const preloaded = preload(
			this.props.staticContext,
			"comments",
			[],
			{ sourceURI: this.props.match.url },
			preloadedData
		);
		// }

		if (preloaded) {

			//console.log(`preloaded 90: ${stringifyObject(preloaded)}`);

			let allowComments = !preloaded.consultationState.consultationIsOpen && !preloaded.consultationState.userHasSubmitted;
			this.state = {
				loading: false,
				comments: preloaded.comments,
				filteredComments: [],
				questions: preloaded.questions,
				allowComments: allowComments
			};
		}
	}

	loadComments() {
		// if (this.props.isReviewPage){
		// 	load("review", undefined, [this.props.match.params.consultationId], {}) // todo: maybe this should us source URI instead of id... need to change feed to do this
		//  	.then(
		//  	 	res => {
		//  	 		this.setCommentListState(res);
		// 			});
		// } else{
		//  	load("comments", undefined, [], { sourceURI: this.props.match.url }).then(
		//  		res => {
		//  			this.setCommentListState(res);
		//  		});
		// }

		let sourceURI = this.props.match.url;
		if (this.props.isReviewPage)
		{
			sourceURI = replaceFormat("/{0}/{1}/{2}", [this.props.match.params.consultationId, 0, "Review"]);

		}
		// console.log(sourceURI);

		load("comments", undefined, [], { sourceURI: sourceURI, isReview: this.props.isReviewPage }).then(
			res => {
				this.setCommentListState(res);
			})
			.catch(err => console.log("load comments in commentlist " + err));
	}

	setCommentListState = (response: any) =>
	{
		let allowComments = response.data.consultationState.consultationIsOpen && !response.data.consultationState.userHasSubmitted;
		const comments = this.filterComments(this.props.location.search, response.data.comments );
		const questions = this.filterQuestions(this.props.location.search, response.data.questions );
		this.setState({
			comments,
			questions,
			loading: false,
			allowComments: allowComments
		});
	};

	componentDidMount() {
		this.loadComments();
	}

	componentDidUpdate(prevProps: PropsType, prevState: any, nextContent: any) {
		const oldRoute = prevProps.location.pathname + prevProps.location.search;
		const newRoute = this.props.location.pathname + this.props.location.search;
		if (oldRoute !== newRoute) {
			this.setState({
				loading: true
			});
			this.loadComments();
		}
	}

	submitComments = () => {

		const answersToSubmit = [];
		this.state.questions.forEach(function(question){
			if (question.answers != null && question.answers.length > 0){
				question.answers.forEach(function(answer){
					answersToSubmit.push(answer);
				});
			}			
		});

		let commentsAndAnswers = {comments: this.state.comments, answers: answersToSubmit};

		load("submit", undefined, [], {}, "POST", commentsAndAnswers, true)
			.then(res => {
				this.props.submittedHandler();
			})
			.catch(err => {
				console.log(err);
				if (err.response) alert(err.response.statusText);
			});		
	}

	filterComments = (newSourceURIToFilterBy: string, comments: Array<CommentType>): Array<CommentType> => {
		let filterBy = queryStringToObject(newSourceURIToFilterBy);
		if (filterBy.sourceURI == null) filterBy = { sourceURI: "" };
		const idsOfFilteredComments = comments.filter(comment => comment.sourceURI.indexOf(filterBy.sourceURI) !== -1).map(comment => comment.commentId);

		return comments.map(comment => {
			comment.show = !idsOfFilteredComments.includes(comment.commentId);
			return comment;
		});
	};

	filterQuestions = (newSourceURIToFilterBy: string, questions: Array<QuestionType>): Array<QuestionType> => {
		let filterBy = queryStringToObject(newSourceURIToFilterBy);
		if (filterBy.sourceURI == null) filterBy = { sourceURI: "" };
		const idsOfFilteredComments = questions.filter(question => question.sourceURI.indexOf(filterBy.sourceURI) !== -1).map(question => question.questionId);

		return questions.map(question => {
			question.show = !idsOfFilteredComments.includes(question.questionId);
			return question;
		});
	};

	newComment(newComment: CommentType) {
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
			show: false
		});
		comments.unshift(generatedComment);
		this.setState({ comments });
		setTimeout(() => {
			pullFocusById(`Comment${idToUseForNewBox}`);
		}, 0);
	}

	saveCommentHandler = (e: Event, comment: CommentType) => {
		e.preventDefault();

		const isANewComment = comment.commentId < 0;
		const method = isANewComment ? "POST" : "PUT";
		const urlParameters = isANewComment ? [] : [comment.commentId];
		const endpointName = isANewComment ? "newcomment" : "editcomment";

		load(endpointName, undefined, urlParameters, {}, method, comment, true)
			.then(res => {
				if (res.status === 201 || res.status === 200) {
					const index = this.state.comments
						.map(function(comment) {
							return comment.commentId;
						})
						.indexOf(comment.commentId);
					const comments = this.state.comments;
					comments[index] = res.data;
					this.setState({
						comments
					});
				}
			})
			.catch(err => {
				console.log(err);
				if (err.response) alert(err.response.statusText);
			});
	};

	deleteCommentHandler = (e: Event, commentId: number) => {
		e.preventDefault();
		if (commentId < 0) {
			this.removeCommentFromState(commentId);
		} else {
			load("editcomment", undefined, [commentId], {}, "DELETE")
				.then(res => {
					if (res.status === 200) {
						this.removeCommentFromState(commentId);
					}
				})
				.catch(err => {
					console.log(err);
					if (err.response) alert(err.response.statusText);
				});
		}
	};

	saveAnswerHandler = (e: Event, answer: AnswerType) => {
		e.preventDefault();
		//todo: post or put the answer to the api, then on success use answer.questionId to get the question in the this.state.questions array and update the state.
		//console.log(stringifyObject(answer));
		const isANewAnswer = answer.answerId < 0;
		const method = isANewAnswer ? "POST" : "PUT";
		const urlParameters = isANewAnswer ? [] : [answer.answerId];
		const endpointName = isANewAnswer ? "newanswer" : "editanswer";

		load(endpointName, undefined, urlParameters, {}, method, answer, true)
			.then(res => {
				if (res.status === 201 || res.status === 200) {
					const questionIndex = this.state.questions
						.map(function(question) {
							return question.questionId;
						})
						.indexOf(answer.questionId);
					const questions = this.state.questions;

					if (questions[questionIndex].answers === null || questions[questionIndex].answers.length < 1){
						questions[questionIndex].answers = [res.data];
					} else{					
						const answerIndex = questions[questionIndex].answers
							.map(function(answer) {
								return answer.answerId;
							}).indexOf(answer.answerId);
						
						const answers = questions[questionIndex].answers;
						answers[answerIndex] = res.data;	
						questions[questionIndex].answers = answers;
					}					
					this.setState({
						questions
					});
				}
			})
			.catch(err => {
				console.log(err);
				if (err.response) alert(err.response.statusText);
			});
	};

	deleteAnswerHandler = (e: Event, questionId: number, answerId: number) => {
		e.preventDefault();
		//todo: call the delete answer api, then update the state on success.
		if (answerId < 0) {
			this.removeAnswerFromState(questionId, answerId);
		} else {
			load("editanswer", undefined, [answerId], {}, "DELETE")
				.then(res => {
					if (res.status === 200) {
						this.removeAnswerFromState(questionId, answerId);
					}
				})
				.catch(err => {
					console.log(err);
					if (err.response) alert(err.response.statusText);
				});
		}
	};

	removeCommentFromState = (commentId: number) => {
		let comments = this.state.comments;
		comments = comments.filter(comment => comment.commentId !== commentId);
		this.setState({ comments });
		if ((comments.length === 0) && (typeof this.props.validationHander === "function")) {
			this.props.validationHander(false);
		}
	};

	removeAnswerFromState = (questionId: number, answerId: number) => {
		let questions = this.state.questions;
		let questionToUpdate = questions.find(question => question.questionId === questionId);
		questionToUpdate.answers = questionToUpdate.answers.filter(answer => answer.answerId != answerId);
		this.setState({ questions });
	};

	render() {
		const commentsToShow = this.state.comments.filter(comment => !comment.show);
		const questionsToShow = this.state.questions.filter(question => !question.show);
		return (
			<UserContext.Consumer>
				{ (contextValue: ContextType) => {
					return (
						<div data-qa-sel="comment-list-wrapper">

							{!this.props.isReviewPage ?
								<div className="grid">
									<h1 data-g="6" id="commenting-panel" className="p">
										{this.props.viewComments ? "Comments" : "Questions"} panel
									</h1>
									{contextValue.isAuthorised ?
										<p data-g="6">
											<Link
												to={`/${this.props.match.params.consultationId}/review`}
												data-qa-sel="review-all-comments"
												className="right">Review all {this.props.viewComments ? "comments" : "questions"}</Link>
										</p> : null
									}
								</div> : null
							}

							{this.state.loading ? <p>Loading...</p> :

								contextValue.isAuthorised ?

									this.props.viewComments ? 
									
										commentsToShow.length === 0 ? <p>No comments yet</p> :

											<ul className="CommentList list--unstyled">
												{commentsToShow.map((comment) => {
													return (
														<CommentBox
															readOnly={!this.state.allowComments || this.props.isSubmitted}
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
										:
										<div>
											<p>We would like to hear your views on the draft recommendations presented in the guideline, and any comments you may have on the rationale and impact sections in the guideline and the evidence presented in the evidence reviews documents. We would also welcome views on the Equality Impact Assessment.</p>
											<p>We would like to hear your views on these questions:</p>
											<ul className="CommentList list--unstyled">
												{questionsToShow.map((question) => {
													return (
														<Question
															readOnly={!this.state.allowComments || this.props.isSubmitted}
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
									:
									<LoginBanner
										signInButton={true}
										currentURL={this.props.match.url}
										signInURL={contextValue.signInURL}
										registerURL={contextValue.registerURL}
									/>
							}
						</div>
					);
				}}
			</UserContext.Consumer>
		);
	}
}

export default withRouter(CommentList);
