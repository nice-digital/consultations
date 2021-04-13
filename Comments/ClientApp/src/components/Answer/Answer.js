// @flow

import React, { Component, Fragment } from "react";
import { TextAnswer } from "./../AnswerTypes/TextAnswer/TextAnswer";
import { YesNoAnswer } from "./../AnswerTypes/YesNoAnswer/YesNoAnswer";

type PropsType = {
	staticContext?: any,
	answer: AnswerType,
	readOnly: boolean,
	saveAnswerHandler: Function,
	deleteAnswerHandler: Function,
	unique: string,
	updateUnsavedIds: Function,
};

type StateType = {
	answer: AnswerType,
	unsavedChanges: boolean,
};

export class Answer extends Component<PropsType, StateType> {
	constructor() {
		super();
		this.state = {
			answer: {},
			unsavedChanges: false,
		};
	}

	componentDidMount() {
		this.setState({
			answer: this.props.answer,
		});
	}

	textareaChangeHandler = (e: SyntheticEvent) => {
		const answer = this.state.answer;
		answer.answerText = e.target.value;
		const unsavedChanges = !(answer.answerId === -1 && answer.answerText.length === 0);
		this.props.updateUnsavedIds(`${answer.questionId}q`, unsavedChanges);
		this.setState({
			answer,
			unsavedChanges,
		});
	};

	yesNoChangeHandler = (e: SyntheticEvent) => {
		const answer = this.state.answer;
		answer.answerBoolean = (e.target.value === "true");
		const unsavedChanges = !(answer.answerId === -1 && answer.answerBoolean === undefined);
		this.props.updateUnsavedIds(`${answer.questionId}q`, unsavedChanges);
		this.setState({
			answer,
			unsavedChanges,
		});
	};

	static getDerivedStateFromProps(nextProps: any, prevState: any) {
		const prevTimestamp = prevState.answer.lastModifiedDate;
		const nextTimestamp = nextProps.answer.lastModifiedDate;
		const hasAnswerBeenUpdated = () => prevTimestamp !== nextTimestamp;
		if (hasAnswerBeenUpdated()) {
			return {
				answer: nextProps.answer,
				unsavedChanges: false,
			};
		}
		return null;
	}

	render() {
		if (!this.state.answer) return null;
		const {
			answerBoolean,
			answerText,
			answerId,
			questionId,
			commenterEmail,
		} = this.state.answer;
		const unsavedChanges = this.state.unsavedChanges;
		const answer = this.state.answer;
		const readOnly = this.props.readOnly;
		const questionText = this.props.questionText;
		const unique = this.props.unique;
		return (

			<Fragment>
				<section role="form">
					<form onSubmit={e => this.props.saveAnswerHandler(e, answer, this.props.questionId)} className="mb--0 mt--e">

						{commenterEmail &&
							<p className="CommentBox__commentBy mb--0">Answer by: {commenterEmail}</p>
						}

						{this.props.questionType.type === "YesNo" &&
						<YesNoAnswer
							unique={unique}
							questionText={questionText}
							readOnly={readOnly}
							textareaChangeHandler={this.textareaChangeHandler}
							yesNoChangeHandler={this.yesNoChangeHandler}
							answerText={answerText}
							answerBoolean={answerBoolean}
						/>
						}

						{this.props.questionType.type === "Text" &&
						<TextAnswer
							unique={unique}
							questionText={questionText}
							readOnly={readOnly}
							textareaChangeHandler={this.textareaChangeHandler}
							answerText={answerText}
						/>
						}

						{!readOnly && ((answerText && answerText.length > 0) || (answerBoolean !== undefined)) ?
							unsavedChanges ?
								<input
									data-qa-sel="submit-button"
									className="btn ml--0 mb--0"
									type="submit"
									value="Save answer"
								/>
								:
								<span className="ml--0 mb--0 CommentBox__savedIndicator">Saved</span>
							:
							null
						}

						{!readOnly && answerId > 0 &&
						<button
							data-qa-sel="delete-comment-button"
							className="btn mr--0 mb--0 right"
							onClick={e => this.props.deleteAnswerHandler(e, questionId, answerId)}>
							Delete
						</button>
						}

					</form>
				</section>
			</Fragment>
		);
	}
}
