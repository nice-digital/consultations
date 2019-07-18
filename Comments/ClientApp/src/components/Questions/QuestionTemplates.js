import React, { Component } from "react";
import { DebounceInput } from "react-debounce-input";

export class QuestionTemplates extends Component {
	state = {
		questions: this.props.questions,
		filteredQuestions: this.props.questions,
	};

	filterQuestions = (e) => {
		const filterQuery = e.target.value.toLowerCase();
		let filteredQuestions = this.state.questions;
		filteredQuestions = filteredQuestions.filter(question => {
			return question.questionText.toLowerCase().indexOf(
				filterQuery,
			) !== -1;
		});
		this.setState({
			filteredQuestions,
		});
	};

	render() {
		const {currentConsultationId, currentDocumentId, newQuestion} = this.props;
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

const TemplateItem = (props) => {
	const {
		questionText,
		newQuestion,
		currentDocumentId,
		currentConsultationId,
		questionTypeId,
		questionType,
	} = props;
	const getQuestionType = (type) => {
		return type === "Text" ? "Open Question" : "Yes/No Question";
	};
	return (
		<li>
			<div className="card">
				<div className="grid">
					<div data-g="9">
						<div className="card__header">
							<p className="card__heading">
								{questionText}
							</p>
						</div>
						<dl className="card__metadata">
							<div className="card__metadatum">
								<dt>
									<span className="card__tag tag tag--consultation tag--flush">
										{getQuestionType(questionType.type)}
									</span>
								</dt>
								<dd className="visually-hidden">{questionType.type}</dd>
							</div>
						</dl>
					</div>
					<div data-g="3">
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
		</li>
	);
};

const TemplateFilter = props => {
	return (
		<div className="panel mb--d">
			<div className="form__group form__group--text">
				<label htmlFor="textFilter" className="form__label">Filter by question text</label>
				<input
					className="form__input"
					onChange={props.filterQuestions}
					id="filter"
					tabIndex={0}/>
			</div>
		</div>
	);
};
