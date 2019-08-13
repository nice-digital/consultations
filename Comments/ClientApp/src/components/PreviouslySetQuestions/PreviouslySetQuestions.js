import React, { Component } from "react";
import { QuestionsFilter } from "./QuestionsFilter";
import { SingleQuestion } from "./SingleQuestion";

export class PreviouslySetQuestions extends Component {
	state = {
		filter: {
			string: "",
			directorate: true,
		},
	};

	handleFilter = (event, filterByDirectorate) => {
		if (filterByDirectorate === null) {
			this.setState({
				filter: {
					string: event.target.value,
					directorate: this.state.filter.directorate,
				},
			});
		} else {
			this.setState({
				filter: {
					string: this.state.filter.string,
					directorate: filterByDirectorate,
				},
			});
		}
	};

	getFilteredQuestions = (questions, searchString, filterByDirectorate, currentUserRoles) => {
		return questions.filter(question => {
			if (filterByDirectorate) {
				return (
					(question.questionText.toLowerCase().indexOf(searchString.toLowerCase()) !== -1)
					&&
					(currentUserRoles.find(currentRole => question.createdByRoles.includes(currentRole)))
				);
			} else {
				return question.questionText.toLowerCase().indexOf(searchString.toLowerCase()) !== -1;
			}
		});
	};

	render() {
		const {string, directorate} = this.state.filter;
		const {currentConsultationId, currentDocumentId, newQuestion, currentUserRoles, questions} = this.props;
		const filteredQuestions = this.getFilteredQuestions(questions, string, directorate, currentUserRoles);
		return (
			<div className="card">
				<h3>Previously set questions</h3>
				<QuestionsFilter
					handleFilter={this.handleFilter}
					filter={this.state.filter}/>
				{filteredQuestions.length === 0 ? <p><i>No matches</i></p> :
					<ul className="list--unstyled">
						{filteredQuestions.map(item =>
							(<SingleQuestion
								{...item}
								key={item.questionId}
								currentConsultationId={currentConsultationId}
								currentDocumentId={currentDocumentId}
								newQuestion={newQuestion}/>))}
					</ul>
				}
			</div>
		);
	}
}
