// @flow

import React, { Component, Fragment } from "react";
import { AnswerBox } from "../AnswerBox/AnswerBox";

//import stringifyObject from "stringify-object";

type PropsType = {
	staticContext?: any,
	saveAnswerHandler: Function,
	deleteAnswerHandler: Function,
	question: QuestionType,
	readOnly: boolean,
};

type StateType = {
	questionId: number
};

export class Question extends Component<PropsType, StateType> {
	constructor() {
		super();
		this.state = {
			unsavedChanges: false,
		};
	}

	isTextSelection = (question) => question.commentOn && question.commentOn.toLowerCase() === "selection" && question.quote;

	render() {
		if (!this.props.question) return null;

		let answers = this.props.question.answers;
		if (answers === null || answers.length < 1){
			answers = [{
				answerId: -1,
				questionId: this.props.question.questionId,
			}];
		}

		return (

			<li className="CommentBox">
				{!this.isTextSelection(this.props.question) &&
				<Fragment>
					<h1 data-qa-sel="comment-box-title" className="CommentBox__title mt--0 mb--d">
						Question on: <span className="text-lowercase">{this.props.question.commentOn}</span>
						<br/>
						{this.props.question.quote}
					</h1>
				</Fragment>
				}

				{this.isTextSelection(this.props.question) &&
				<Fragment>
					<h1 data-qa-sel="comment-box-title" className="CommentBox__title mt--0 mb--d">
						Question on: <span className="text-lowercase">{this.props.question.commentOn}</span>
					</h1>
					<div className="CommentBox__quote mb--d">{this.props.question.quote}</div>
				</Fragment>
				}
				<p><strong>{this.props.question.questionText}</strong></p>
				{answers.map((answer) => {
					return (
						<AnswerBox
							updateUnsavedIds={this.props.updateUnsavedIds}
							questionId={this.props.question.questionId}
							readOnly={this.props.readOnly}
							isVisible={this.props.isVisible}
							key={answer.answerId}
							unique={`Answer${answer.answerId}`}
							answer={answer}
							saveAnswerHandler={this.props.saveAnswerHandler}
							deleteAnswerHandler={this.props.deleteAnswerHandler}
						/>
					);
				})}

			</li>
		);
	}
}

export default Question;
