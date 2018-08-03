// @flow

import React, { Component, Fragment } from "react";
import { withRouter, Link } from "react-router-dom";
import { load } from "../../data/loader";
import preload from "../../data/pre-loader";
import { CommentBox } from "../CommentBox/CommentBox";
import { Question } from "../Question/Question";
import { LoginBanner } from "../LoginBanner/LoginBanner";
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
	//filterByDocument: number,
	isSubmitted: boolean,
	submittedHandler: Function,
	validationHander: Function,
	viewComments: boolean //when false, we view questions.
};

type StateType = {
	comments: Array<CommentType>,
	questions: Array<QuestionType>,
	loading: boolean,
	allowComments: boolean,
};

type ContextType = any;

export class CommentList extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			comments: [],
			questions: [],
			loading: true,
			allowComments: true,
			error: "",
		};
		let preloadedData = {};
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data; //this is data from Configure => SupplyData in Startup.cs. the main thing it contains for this call is the cookie for the current user.
		}

		const preloaded = preload(
			this.props.staticContext,
			"comments",
			[],
			{ sourceURI: this.props.match.url },
			preloadedData
		);

		if (preloaded) {

			//console.log(`preloaded 90: ${stringifyObject(preloaded)}`);

			let allowComments = !preloaded.consultationState.consultationIsOpen && !preloaded.consultationState.userHasSubmitted;
			this.state = {
				loading: false,
				comments: preloaded.comments,
				filteredComments: [],
				questions: preloaded.questions,
				allowComments: allowComments,
				error: ""
			};
		}
	}

	loadComments() {
		load("comments", undefined, [], { sourceURI: this.props.match.url }).then(
			res => {
				this.setCommentListState(res);
			})
			.catch(err => console.log("load comments in commentlist " + err));
	}

	setCommentListState = (response: any) =>
	{
		let allowComments = response.data.consultationState.consultationIsOpen && !response.data.consultationState.userHasSubmitted;
		// const comments = this.filterComments(this.props.location.search, response.data.comments );
		// const questions = this.filterQuestions(this.props.location.search, response.data.questions );
		this.setState({
			comments: response.data.comments,
			questions: response.data.questions,
			loading: false,
			allowComments: allowComments,
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

	getComments = () => {
		return this.state.comments;
	}

	getQuestions = () => {
		return this.state.questions;
	}	

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
		let error = "";

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
						comments,
						error
					});
					if (typeof this.props.validationHander === "function") {
						this.props.validationHander();
					}
				}
			})
			.catch(err => {
				console.log(err);
				if (err.response) {
					error = "save";
					this.setState({
						error
					});
					
				}

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
					if (err.response) {
						const error = "delete";
						this.setState({
							error
						});
						
					}
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
					if (typeof this.props.validationHander === "function") {
						this.props.validationHander();
					}
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
		const error = "";
		comments = comments.filter(comment => comment.commentId !== commentId);
		this.setState({ comments, error });
		if ((comments.length === 0) && (typeof this.props.validationHander === "function")) {
			this.props.validationHander();
		}
	};

	removeAnswerFromState = (questionId: number, answerId: number) => {
		let questions = this.state.questions;
		let questionToUpdate = questions.find(question => question.questionId === questionId);
		questionToUpdate.answers = questionToUpdate.answers.filter(answer => answer.answerId !== answerId);
		this.setState({ questions });
		if (typeof this.props.validationHander === "function") {
			this.props.validationHander();
		}
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

							{this.state.error != "" ? 
								<div className="errorBox">
									<p>
										We couldn't {this.state.error} your comment. Please try again in a few minutes.
									</p>
									<p>
										If the problem continues please <a href="/get-involved/contact-us">contact us</a>.
									</p>
								</div>
								: null }

							{this.state.loading ? <p>Loading...</p> :

								contextValue.isAuthorised ?

									<Fragment>
										{(this.props.isReviewPage || !this.props.viewComments) && 
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
										}										
										{(this.props.isReviewPage || this.props.viewComments) && (
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
											)
										}
									</Fragment>
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