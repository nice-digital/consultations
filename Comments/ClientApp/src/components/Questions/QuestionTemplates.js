import React from "react";
import fakeTemplateData from "./fake-template-data";

export class QuestionTemplates extends React.Component {
	state = {
		questions: fakeTemplateData,
		filteredQuestions: fakeTemplateData,
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
							textQuestionTypeId={this.props.textQuestionTypeId}
							currentConsultationId={this.props.currentConsultationId}
							currentDocumentId={this.props.currentDocumentId}
							newQuestion={this.props.newQuestion}/>))}
				</ul>
			</div>
		);
	}
}

const TemplateItem = (props) => {
	const question = props.question;
	return (
		<li>
			<div className="card">
				<div className="card__body">
					<div className="grid">
						<div data-g="9">{question}</div>
						<div data-g="3">
							<button
								onClick={e => {
									if (props.currentDocumentId === "consultation") {
										props.newQuestion(e, props.currentConsultationId, null, props.textQuestionTypeId, question);
									} else {
										props.newQuestion(e, props.currentConsultationId, parseInt(props.currentDocumentId, 10), props.textQuestionTypeId, question);
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
		<div className="form__group form__group--textarea mb--b">
			<label
				className="form__label" htmlFor="filter">
				Filter questions by question text
			</label>
			<input className="form__input"
						 onChange={props.filterQuestions}
						 id="filter"
						 tabIndex={0}/>
		</div>
	);
};

