import React, { Component } from "react";

export class QuestionTemplates extends Component {
	state = {
		questions: this.props.questions,
		filteredQuestions: this.props.questions,
	};

	filterQuestions = (e) => {
		const filterQuery = e.target.value;
		let filteredQuestions = this.state.questions;
		filteredQuestions = filteredQuestions.filter(question => {
			return question.questionText.indexOf(
				filterQuery,
			) !== -1;
		});
		this.setState({
			filteredQuestions,
		});
	};

	render() {
		const {textQuestionTypeId, currentConsultationId, currentDocumentId, newQuestion} = this.props;
		return (
			<div className="card">
				<h3>Previously set questions</h3>
				<TemplateFilter
					filterQuestions={this.filterQuestions}
				/>
				{this.state.filteredQuestions.length === 0
				&& <p><i>No matches</i></p>
				}
				<ul className="list--unstyled">
					{this.state.filteredQuestions.map(item =>
						(<TemplateItem
							{...item}
							key={item._id}
							currentConsultationId={currentConsultationId}
							currentDocumentId={currentDocumentId}
							newQuestion={newQuestion}/>))}
				</ul>
			</div>
		);
	}
}

// todo: lowercase() the filter terms and questions!

const TemplateItem = (props) => {
	const {
		questionType,
		questionText,
		newQuestion,
		currentDocumentId,
		currentConsultationId,
		questionTypeId } = props;
	return (
		<li>
			<div className="card">
				<div className="card__body">
					<div className="grid">
						<div data-g="8">{questionText}</div>
						<div data-g="2">{questionType.type === "YesNo" ? "Yes / No Question" : "Open Question"}</div>
						<div data-g="2">
							<button
								onClick={e => {
									if (currentDocumentId === "consultation") {
										newQuestion(e, currentConsultationId, null, questionTypeId, questionText);
									} else {
										newQuestion(e, currentConsultationId, parseInt(currentDocumentId, 10), questionTypeId, questionText);
									}
								}}
								className="btn btn-small right mr--0">Insert
							</button>
						</div>
					</div>
				</div>
			</div>
		</li>
	);
};

const TemplateFilter = (props) => {
	return (
		<div className="form__group mb--b">
			<label
				className="form__label"
				htmlFor="filter">
				Search by question
			</label>
			<input
				className="form__input"
				onChange={props.filterQuestions}
				id="filter"
				tabIndex={0}/>
		</div>
	);
};

