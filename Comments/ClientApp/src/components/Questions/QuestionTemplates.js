import React from "react";
import fakeTemplateData from "./fake-template-data2.json";

export class QuestionTemplates extends React.Component {
	state = {
		questions: fakeTemplateData.questions,
		filteredQuestions: fakeTemplateData.questions,
	};

	filterQuestions = (e) => {
		const filterQuery = e.target.value;
		let filteredQuestions = this.state.questions;
		filteredQuestions = filteredQuestions.filter(question => {
			return question.question.indexOf(
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
							textQuestionTypeId={textQuestionTypeId}
							currentConsultationId={currentConsultationId}
							currentDocumentId={currentDocumentId}
							newQuestion={newQuestion}/>))}
				</ul>
			</div>
		);
	}
}

const TemplateItem = (props) => {
	const {questionText, newQuestion, currentDocumentId,currentConsultationId, textQuestionTypeId } = props;
	return (
		<li>
			<div className="card">
				<div className="card__body">
					<div className="grid">
						<div data-g="9">{questionText}</div>
						<div data-g="3">
							<button
								onClick={e => {
									if (currentDocumentId === "consultation") {
										newQuestion(e, currentConsultationId, null, textQuestionTypeId, questionText);
									} else {
										newQuestion(e, currentConsultationId, parseInt(currentDocumentId, 10), textQuestionTypeId, questionText);
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

