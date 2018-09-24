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
		const { documentTitle } = this.props;
		const { commentOn, quote, questionText } = this.props.question;
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
					<h1 className="CommentBox__title mt--0 mb--0">{documentTitle}</h1>
					<h2 data-qa-sel="comment-box-title" className="CommentBox__title mt--0 mb--0">
						Question on <span className="text-lowercase">{commentOn}</span>
					</h2>
				</Fragment>
				}

				{this.isTextSelection(this.props.question) &&
				<Fragment>
					<h1 className="CommentBox__title mt--0 mb--0">{documentTitle}</h1>
					<h2 data-qa-sel="comment-box-title" className="CommentBox__title mt--0 mb--0">
						Question on: <span className="text-lowercase">{commentOn}</span>
					</h2>
					<div className="CommentBox__quote mb--d">{quote}</div>
				</Fragment>
				}
				<p><strong>{questionText}</strong></p>
				{answers.map((answer) => {
					return (
						<AnswerBox
							readOnly={this.props.readOnly}
							isVisible={this.props.isVisible}
							key={answer.answerId}
							unique={`Answer${answer.answerId}`}
							answer={answer}
							saveHandler={this.props.saveAnswerHandler}
							deleteHandler={this.props.deleteAnswerHandler}
						/>
					);
				})}

			</li>
		);
	}
}

export default Question;
