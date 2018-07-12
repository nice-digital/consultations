import React, { Component, Fragment } from "react";
import Moment from "react-moment";

import { Answer } from "../Answer/Answer";

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
	answers: Array<AnswerType>
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
			// question: {
			// 	questionText: ""
			// },
			unsavedChanges: false
		};
	}

	componentDidMount() {
		// this.setState({
		// 	question: this.props.question
		// });
	}

	// static getDerivedStateFromProps(nextProps, prevState) {
	// 	const prevTimestamp = prevState.comment.lastModifiedDate;
	// 	const nextTimestamp = nextProps.comment.lastModifiedDate;
	// 	const hasCommentBeenUpdated = () => prevTimestamp !== nextTimestamp;
	// 	if (hasCommentBeenUpdated()) {
	// 		return {
	// 			comment: nextProps.comment,
	// 			unsavedChanges: false
	// 		};
	// 	}
	// 	return null;
	// }

	// isTextSelection = (comment) => comment.commentOn && comment.commentOn.toLowerCase() === "selection" && comment.quote;

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
				<p>{this.props.question.questionText}</p>

				{answers.map((answer) => {
					return (
						<Answer
							readOnly={this.props.readonly}
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
