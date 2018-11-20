// @flow
import React, {Component, Fragment} from "react";

export class TextQuestion extends Component {

	constructor() {
		super();
		this.state = {
			question: {},
		};
	}

	componentDidMount() {
		this.setState({
			question: this.props.question,
		});
	}

	textareaChangeHandler = (e: SyntheticEvent) => {
		const question = this.state.question;
		question.questionText = e.target.value;
		//const unsavedChanges = !(answer.answerId === -1 && answer.answerText.length === 0);
		//this.props.updateUnsavedIds(`${answer.questionId}q`, unsavedChanges);
		this.setState({
			question
			//unsavedChanges,
		});
	};

	render() {
		const { questionText } = this.state.question;

		return (
			<li>
				<textarea
					data-hj-whitelist
					data-qa-sel="Question-text-area"
					className="form__input form__input--textarea"
					onInput={this.textareaChangeHandler}
					value={questionText}/>
				<button>Save Question</button>
				<button>Delete</button>
			</li>
		);
	};
}
