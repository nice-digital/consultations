// @flow
import React, {PureComponent} from "react";

export class TextQuestion extends PureComponent {

	constructor(props) {
		super(props);
	}

	// textareaChangeHandler = (e: SyntheticEvent) => {
	// 	const question = this.state.question;
	// 	question.questionText = e.target.value;
	// 	//const unsavedChanges = !(answer.answerId === -1 && answer.answerText.length === 0);
	// 	//this.props.updateUnsavedIds(`${answer.questionId}q`, unsavedChanges);
	// 	this.setState({
	// 		question
	// 		//unsavedChanges,
	// 	});
	// };

	render() {
		const { questionText, questionId, documentId } = this.props.question;

		return (
			<li>
				<textarea
					data-hj-whitelist
					data-qa-sel="Question-text-area"
					className="form__input form__input--textarea"
					// onInput={this.textareaChangeHandler}
					value={questionText}/>
				<button>Save Question</button>
				<h1>{questionId}</h1>
				<h2>{documentId}</h2>
				<button>Delete</button>
			</li>
		);
	};
}
