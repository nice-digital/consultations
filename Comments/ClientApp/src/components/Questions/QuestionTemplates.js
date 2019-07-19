import React, { Component, Fragment } from "react";

// HSCTeam
// CfGTeam
// CHTETeam

export class QuestionTemplates extends Component {
	state = {
		questions: this.props.questions,
		filteredQuestions: this.props.questions,
		onlyMyDirectorate: true,
	};

	filterQuestions = e => {
		let filteredQuestions = this.state.questions;
		console.log("inititally...", filteredQuestions);
		const filterQuery = e.target.value.toLowerCase();
		if (this.state.onlyMyDirectorate) {
			filteredQuestions = filteredQuestions.filter(question => {
				return question.allRoles.some(item => this.props.currentUserRoles.includes(item));
			});
		}
		console.log("after only my directorate", filteredQuestions);
		filteredQuestions = filteredQuestions.filter(question => {
			return question.questionText.toLowerCase().indexOf(
				filterQuery,
			) !== -1;
		});
		console.log("after filtering by text", filteredQuestions);
		this.setState({
			filteredQuestions,
		});
	};

	filterByDirectorate = bool => {
		this.setState(
			{onlyMyDirectorate: bool},
		);
	};

	render() {
		const {currentConsultationId, currentDocumentId, newQuestion} = this.props;
		return (
			<div className="card">
				<h3>Previously set questions</h3>
				<QuestionsFilter
					onlyMyDirectorate={this.state.onlyMyDirectorate}
					filterByDirectorate={this.filterByDirectorate}
					filterQuestions={this.filterQuestions}/>
				{this.state.filteredQuestions.length === 0 ? <p><i>No matches</i></p> :
					<ul className="list--unstyled">
						{this.state.filteredQuestions.map(item =>
							(<TemplateItem
								{...item}
								key={item._id}
								currentConsultationId={currentConsultationId}
								currentDocumentId={currentDocumentId}
								newQuestion={newQuestion}/>))}
					</ul>
				}
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
		allRoles,
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
									<span className="card__tag tag tag--flush">
										{getQuestionType(questionType.type)}
									</span>
								</dt>
								<dd className="visually-hidden">{questionType.type}</dd>
							</div>
							{allRoles.map(role =>
								<Fragment key={role}>
									<div className="card__metadatum">
										<dt>
											<span className="card__tag tag tag--flush">
												{role}
											</span>
										</dt>
										<dd className="visually-hidden">{role}</dd>
									</div>
								</Fragment>,
							)}
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

const QuestionsFilter = props => {
	const {filterQuestions, filterByDirectorate, onlyMyDirectorate} = props;
	return (
		<div className="panel mb--d">
			<div className="form__group form__group--text">
				<label htmlFor="textFilter" className="form__label"><b>Filter by question text</b></label>
				<input
					className="form__input"
					onChange={filterQuestions}
					id="textFilter"
					tabIndex={0}/>
			</div>
			<div role="radiogroup">
				<label htmlFor="filterByRole" className="form__label"><b>Filter by directorate</b></label>
				<div className="form__group form__group--radio form__group--inline">
					<input
						className="form__radio"
						id="filterByRole--mine"
						type="radio"
						name="filterByRole"
						checked={onlyMyDirectorate === true}
						onChange={e => filterByDirectorate(true)}
						value={true}
					/>
					<label
						className="form__label form__label--radio"
						htmlFor="filterByRole--mine">
						My Directorate
					</label>
				</div>
				<div className="form__group form__group--radio form__group--inline">
					<input
						className="form__radio"
						id="filterByRole--all"
						type="radio"
						name="filterByRole"
						checked={onlyMyDirectorate === false}
						onChange={e => filterByDirectorate(false)}
						value={false}
					/>
					<label
						className="form__label form__label--radio"
						htmlFor="filterByRole--all">
						All Directorates
					</label>
				</div>
			</div>
		</div>
	);
};
