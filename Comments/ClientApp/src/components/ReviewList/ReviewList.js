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
import { pullFocusById } from "../../helpers/accessibility-helpers";
//import stringifyObject from "stringify-object";
import { saveCommentHandler, deleteCommentHandler, saveAnswerHandler, deleteAnswerHandler } from "../../helpers/editing-and-deleting";

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
	comments: Array<CommentType>,
	questions: Array<QuestionType>,
	loading: boolean,
};

type StateType = {
	// comments: Array<CommentType>,
	// questions: Array<QuestionType>,
	// loading: boolean,
	allowComments: boolean,
};

type ContextType = any;

export class ReviewList extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			// comments: [],
			// questions: [],
			//loading: true,
			allowComments: true,
		};
	}

	getComments = () => {
		return this.state.comments;
	}

	getQuestions = () => {
		return this.state.questions;
	}

	

	render() {
		const commentsToShow = this.props.comments || []; //.filter(comment => !comment.show);
		const questionsToShow = this.props.questions || []; //.filter(question => !question.show);
		return (
			<UserContext.Consumer>
				{ (contextValue: ContextType) => {
					return (
						<div data-qa-sel="comment-list-wrapper">

							{this.props.loading ? <p>Loading...</p> :

								contextValue.isAuthorised ?

									<Fragment>
										{questionsToShow.length > 0 &&
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
																saveAnswerHandler={saveAnswerHandler}
																deleteAnswerHandler={deleteAnswerHandler}
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
															readOnly={!this.state.allowComments || this.props.isSubmitted}
															isVisible={this.props.isVisible}
															key={comment.commentId}
															unique={`Comment${comment.commentId}`}
															comment={comment}
															saveHandler={saveCommentHandler}
															deleteHandler={deleteCommentHandler}
														/>
													);
												})}
											</ul>
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

export default withRouter(ReviewList);