// @flow
import React, {PureComponent} from "react";

export class TextQuestion extends PureComponent {

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

	textareaChangeHandler = (e: any) => {
		const question = Object.assign({}, this.state.question);
		question.questionText = e.target.value;
		this.setState({
			question,
			unsavedChanges: true
		});
	};

	render() {
		if (!this.state.question) {
			return null
		};

		const { questionText, questionId, documentId } = this.state.question;
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
