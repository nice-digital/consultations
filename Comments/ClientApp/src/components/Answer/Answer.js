import React, { Component, Fragment } from "react";
import Moment from "react-moment";

import stringifyObject from "stringify-object";

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
	isVisible: boolean,
	answer: AnswerType,
	readOnly: boolean,
	saveHandler: Function,
	deleteHandler: Function,
	unique: string
};

type StateType = {
	answerId: number,
	answerText: string,
	answer: AnswerType
};

export class Answer extends Component<PropsType, StateType> {
	constructor() {
		super();
		this.state = {
			answer: {
				answerText: ""
			},
			unsavedChanges: false
		};
	}

	componentDidMount() {
		this.setState({
			answer: this.props.answer
		});
	}

	textareaChangeHandler = e => {
		const answer = this.state.answer;
		answer.answerText = e.target.value;
		this.setState({
			answer,
			unsavedChanges: true
		});
	};

	static getDerivedStateFromProps(nextProps, prevState) {
		 console.log(`answer nextProps: ${stringifyObject(nextProps)}`);
		 console.log(`answer prevState: ${stringifyObject(prevState)}`);
		console.log('get derived thing in answer');
		const prevTimestamp = prevState.answer.lastModifiedDate;
		const nextTimestamp = nextProps.answer.lastModifiedDate;
		const hasAnswerBeenUpdated = () => prevTimestamp !== nextTimestamp;
		if (hasAnswerBeenUpdated()) {
			return {
				answer: nextProps.answer,
				unsavedChanges: false
			};
		}
		return null;
	}

	render() {
		if (!this.state.answer) return null;
		const {
			answerText,
			lastModifiedDate,
			answerId,
			questionId
		} = this.state.answer;
		const unsavedChanges = this.state.unsavedChanges;
		const answer = this.state.answer;
		const readOnly = this.props.readOnly;
		const moment = require("moment");

		return (

			<Fragment>
				<section role="form">

					{lastModifiedDate ? (
						<div className="CommentBox__datestamp mb--d font-weight-bold">
							Last Modified:{" "}
							<Moment format="D/M/YYYY - h:mma" date={moment.utc(lastModifiedDate).toDate()}/>
						</div>
					) : null}
					<form onSubmit={e => this.props.saveHandler(e, answer)}>
						<div className="form__group form__group--textarea mb--0">
							<textarea
								data-qa-sel="Comment-text-area"
								disabled={readOnly}
								id={this.props.unique}
								className="form__input form__input--textarea"
								onChange={this.textareaChangeHandler}
								placeholder="Enter your answer here"
								value={answerText}/>
						</div>
						{!readOnly && answerText && answerText.length > 0 && (
							<input
								data-qa-sel="submit-button"
								className="btn ml--0"
								type="submit"
								value={unsavedChanges ? "Save answer" : "Saved"}
								disabled={!unsavedChanges}/>

						)}
						{!readOnly &&
						<button
							data-qa-sel="delete-comment-button"
							className="btn mr--0 right"
							onClick={e => this.props.deleteHandler(e, questionId, answerId)}>
							<span className="visually-hidden">Delete this answer</span>
							<span className="icon icon--trash" aria-hidden="true"/>
						</button>
						}
					</form>
				</section>
			</Fragment>
		);
	}
}

export default Answer;
