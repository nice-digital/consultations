import React, { Component, Fragment } from "react";
import Moment from "react-moment";

import { Answer } from "../Answer/Answer";
//import stringifyObject from "stringify-object";

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
	commentOn: string
};

type StatusType = {
	statusId: number,
	name: string
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

type PropsType = {
	staticContext?: any,
	saveAnswerHandler: Function,
	deleteAnswerHandler: Function,
	question: QuestionType,
	readOnly: boolean,
	isVisible: boolean, //doesn't appear to get used.
};

type StateType = {
	questionId: number
};

export class Question extends Component<PropsType, StateType> {
	constructor() {
		super();
		this.state = {
			unsavedChanges: false
		};
	}

	isTextSelection = (question) => question.commentOn && question.commentOn.toLowerCase() === "selection" && question.quote;

	render() {
		if (!this.props.question) return null;		

		let answers = this.props.question.answers;
		if (answers === null || answers.length < 1){
			answers = [{
				answerId: -1,
				questionId: this.props.question.questionId
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
						<Answer
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
