// @flow
import React, { Component } from "react";
import { QuestionControls } from "../QuestionControls";

type StateType = {
	question: QuestionType;
	unsavedChanges: boolean;
}

type PropsType = {
	question: QuestionType;
	saveQuestion: Function;
	deleteQuestion: Function;
	updateUnsavedIds: Function;
	moveQuestion: Function;
	readOnly: boolean;
}

export class OpenQuestion extends Component<PropsType, StateType> {

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

	static getDerivedStateFromProps(nextProps: any, prevState: any) {
		const prevTimestamp = prevState.question.lastModifiedDate;
		const nextTimestamp = nextProps.question.lastModifiedDate;
		const hasQuestionBeenUpdated = () => prevTimestamp !== nextTimestamp;
		if (hasQuestionBeenUpdated()) {
			return {
				question: nextProps.question,
				unsavedChanges: false,
				readOnly: nextProps.readOnly,
			};
		}
		return null;
	}

	componentDidUpdate(prevProps: PropsType) {
		const nextTimestamp = this.props.question.lastModifiedDate;
		const prevTimestamp = prevProps.question.lastModifiedDate;
		const hasQuestionBeenUpdated = () => prevTimestamp !== nextTimestamp;
		if (hasQuestionBeenUpdated()) {
			this.props.updateUnsavedIds(`${this.props.question.questionId}q`, false);
		}
	}

	textareaChangeHandler = (e: SyntheticInputEvent<any>) => {
		const question = this.state.question;
		question.questionText = e.target.value;
		this.setState(
			() => {
				this.props.updateUnsavedIds(`${question.questionId}q`, true);
				return {
					question,
					unsavedChanges: true,
				};
			},
		);
	};

	render() {
		if (!this.state.question) {
			return null;
		}

		const {counter, isLast, isFirst, readOnly, totalQuestionQty} = this.props;

		const {question, unsavedChanges} = this.state;

		return (
			<li className="CommentBox mb--e">
				<h1 className="CommentBox__title CommentBox__title--legend">Question {counter} - Open Question</h1>
				<section role="form">
					<form onSubmit={e => this.props.saveQuestion(e, question)} className="mb--0">
						<div className="form__group form__group--textarea mb--b">
							<label className="form__label visually-hidden" htmlFor={question.questionId}>Set question</label>
							{unsavedChanges &&
							<p className="CommentBox__validationMessage mt--0" role="alert">You have unsaved changes</p>
							}
							<textarea
								disabled={readOnly}
								data-hj-whitelist
								id={question.questionId}
								className="form__input form__input--textarea"
								onChange={this.textareaChangeHandler}
								tabIndex={0}
								value={question.questionText}/>
						</div>
						{!readOnly &&
						<QuestionControls
							isFirst={isFirst}
							isLast={isLast}
							question={question}
							unsavedChanges={unsavedChanges}
							totalQuestionQty={totalQuestionQty}
							moveQuestion={this.props.moveQuestion}
							deleteQuestion={this.props.deleteQuestion}
						/>}
					</form>
				</section>
			</li>
		);
	}
}
