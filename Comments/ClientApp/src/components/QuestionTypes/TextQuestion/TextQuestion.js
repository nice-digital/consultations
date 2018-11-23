// @flow
import React, { Component } from "react";

type StateType = {
	question: QuestionType;
	unsavedChanges: boolean;
}

type PropsType = {
	question: QuestionType;
	saveQuestion: Function;
	deleteQuestion: Function;
}

export class TextQuestion extends Component<PropsType, StateType> {

	constructor() {
		super();
		this.state = {
			question: {},
			unsavedChanges: false,
		};
	}

	componentDidMount() {
		this.setState({
			question: this.props.question,
		});
	}

	// componentDidUpdate(prevProps) {
	// 	const nextTimestamp = this.props.question.questionId;
	// 	const prevTimestamp = prevProps.question.questionId;
	// 	const hasQuestionBeenUpdated = () => prevTimestamp !== nextTimestamp;
	// 	if (hasQuestionBeenUpdated()) {
	// 		// this.props.updateUnsavedIds(this.props.question.commentId, false);
	// 	}
	// }
	//
	// static getDerivedStateFromProps(nextProps: any, prevState: any) {
	// 	const prevTimestamp = prevState.question.questionId;
	// 	const nextTimestamp = nextProps.question.questionId;
	// 	const hasQuestionBeenUpdated = () => prevTimestamp !== nextTimestamp;
	// 	if (hasQuestionBeenUpdated()) {
	// 		return {
	// 			question: nextProps.question,
	// 			unsavedChanges: false,
	// 		};
	// 	}
	// 	return null;
	// }


	textareaChangeHandler = (e: SyntheticInputEvent<any>) => {
		const question = this.state.question;
		question.questionText = e.target.value;
		this.setState({
			question,
			unsavedChanges: true,
		});
	};


	render() {
		if (!this.state.question) {
			return null;
		}

		const {question, unsavedChanges} = this.state;

		return (
			<li className="CommentBox">
				<section role="form">
					<form onSubmit={e => this.props.saveQuestion(e, question)} className="mb--0">
						<div className="form__group form__group--textarea mb--b">
							<label className="form__label visually-hidden" htmlFor={question.questionId}>Set question</label>
							{unsavedChanges &&
							<p className="CommentBox__validationMessage mt--0">You have unsaved changes</p>
							}
							<textarea
								data-hj-whitelist
								id={question.questionId}
								className="form__input form__input--textarea"
								onChange={this.textareaChangeHandler}
								tabIndex={0}
								value={question.questionText}/>
						</div>
						{question.questionText && question.questionText.length > 0 ?
							unsavedChanges ?
								<input
									className="btn ml--0 mb--0"
									type="submit"
									value="Save Question"
								/>
								:
								<span className="ml--0 mb--0 CommentBox__savedIndicator">Saved</span>
							:
							null
						}
						<button
							className="btn mr--0 mb--0 right"
							onClick={e => this.props.deleteQuestion(e, question.questionId)}>
							Delete
						</button>
					</form>
				</section>
			</li>
		);
	}
}
