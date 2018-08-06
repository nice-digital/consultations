// @flow

import React, { Component, Fragment } from "react";
import { withRouter, Link } from "react-router-dom";
//import stringifyObject from "stringify-object";

import preload from "../../data/pre-loader";
import { load } from "../../data/loader";
import { queryStringToObject, replaceFormat } from "../../helpers/utils";
import { saveCommentHandler, deleteCommentHandler, saveAnswerHandler, deleteAnswerHandler } from "../../helpers/editing-and-deleting";
import { pullFocusById } from "../../helpers/accessibility-helpers";

import { CommentBox } from "../CommentBox/CommentBox";
import { Question } from "../Question/Question";
import { LoginBanner } from "../LoginBanner/LoginBanner";
import { UserContext } from "../../context/UserContext";

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
	initialDataLoaded: boolean
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
			initialDataLoaded: false,
		};
		let preloadedData = {};
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data; //this is data from Configure => SupplyData in Startup.cs. the main thing it contains for this call is the cookie for the current user.
		}

		const preloadedCommentsData = preload(
			this.props.staticContext,
			"comments",
			[],
			{ sourceURI: this.props.match.url },
			preloadedData
		);

		if (preloadedCommentsData) {
			let allowComments = preloadedCommentsData.consultationState.consultationIsOpen && !preloadedCommentsData.consultationState.userHasSubmitted;
			this.state = {
				comments: preloadedCommentsData.comments,
				questions: preloadedCommentsData.questions,
				loading: false,
				allowComments: allowComments,
				error: "",
				initialDataLoaded: true,
			};
		}
	}

	loadComments() {
		load("comments", undefined, [], { sourceURI: this.props.match.url }).then(
			response => {
				let allowComments = response.data.consultationState.consultationIsOpen && !response.data.consultationState.userHasSubmitted;
				this.setState({
					comments: response.data.comments,
					questions: response.data.questions,
					loading: false,
					allowComments: allowComments,
				});
			})
			.catch(err => console.log("load comments in commentlist " + err));
	}

	componentDidMount() {
		if (!this.state.initialDataLoaded){
			this.loadComments();
		}
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

	//these handlers have moved to the helpers/editing-and-deleting.js utility file as they're also used in ReviewList.js
	saveCommentHandler = (e: Event, comment: CommentType) => {
		saveCommentHandler(e, comment, this);
	}
	deleteCommentHandler = (e: Event, comment: CommentType) => {
		deleteCommentHandler(e, comment, this);
	}
	saveAnswerHandler = (e: Event, answer: AnswerType) => {
		saveAnswerHandler(e, answer, this);
	}
	deleteAnswerHandler = (e: Event,  questionId: number, answerId: number) => {
		deleteAnswerHandler(e, questionId, answerId, this);
	}

	render() {
		const commentsToShow = this.state.comments;
		const questionsToShow = this.state.questions;
		return (
			<UserContext.Consumer>
				{ (contextValue: ContextType) => {
					return (
						<div data-qa-sel="comment-list-wrapper">

							<div className="grid">
								<h1 data-g="6" id="commenting-panel" className="p">
									{this.props.viewComments ? "Comments" : "Questions"} panel
								</h1>
								{contextValue.isAuthorised ?
									<p data-g="6">
										<Link 	to={`/${this.props.match.params.consultationId}/review`}
												data-qa-sel="review-all-comments"
												className="right">Review all {this.props.viewComments ? "comments" : "questions"}</Link>
									</p> : null
								}
							</div> 

							{this.state.error != "" ? 
								<div className="errorBox">
									<p>We couldn't {this.state.error} your comment. Please try again in a few minutes.</p>
									<p>If the problem continues please <a href="/get-involved/contact-us">contact us</a>.</p>
								</div>
								: null }

							{this.state.loading ? <p>Loading...</p> :

								contextValue.isAuthorised ?

									<Fragment>
										{this.props.viewComments ? (
											this.state.comments.length === 0 ? <p>No comments yet</p> :
											<ul className="CommentList list--unstyled">
												{this.state.comments.map((comment) => {
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
										) : ( 
											<div>
												<p>We would like to hear your views on the draft recommendations presented in the guideline, and any comments you may have on the rationale and impact sections in the guideline and the evidence presented in the evidence reviews documents. We would also welcome views on the Equality Impact Assessment.</p>
												<p>We would like to hear your views on these questions:</p>
												<ul className="CommentList list--unstyled">
													{this.state.questions.map((question) => {
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
										)}	
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