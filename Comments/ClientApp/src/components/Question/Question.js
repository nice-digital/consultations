// @flow

import React, { Component } from "react";
import ReactMarkdown from "react-markdown";
import { Answer } from "../Answer/Answer";
import { UserContext } from "../../context/UserContext";

type PropsType = {
	staticContext?: any,
	saveAnswerHandler: Function,
	deleteAnswerHandler: Function,
	question: QuestionType,
	otherUsersAnswers: Array<AnswerType>,
	readOnly: boolean,
	isUnsaved: boolean,
	documentTitle?: string,
	showAnswer: boolean,
};
export class Question extends Component<PropsType> {

	isTextSelection = (question) => question.commentOn && question.commentOn.toLowerCase() === "selection" && question.quote;

	returnAnswers = (answers, readOnly = this.props.readOnly) => {
		const {
			question,
			updateUnsavedIds,
			saveAnswerHandler,
			deleteAnswerHandler,
		} = this.props;

		const answerComponents = answers.map(answer => (
			<Answer
				questionText={question.questionText}
				questionType={question.questionType}
				updateUnsavedIds={updateUnsavedIds}
				questionId={question.questionId}
				readOnly={readOnly}
				key={answer.answerId}
				unique={`Question${question.questionId}-Answer${answer.answerId}`}
				answer={answer}
				saveAnswerHandler={saveAnswerHandler}
				deleteAnswerHandler={deleteAnswerHandler}
			/>
		));

		return answerComponents;
	}

	render() {
		if (!this.props.question) return null;

		const { documentTitle } = this.props;
		const { commentOn, quote } = this.props.question;

		let otherUsersAnswers = this.props.otherUsersAnswers || [];
		const answers = this.props.question.answers || [];
		let answersToShow = answers.filter(answer => answer.showWhenFiltered);
		if (answersToShow === null || answersToShow.length < 1){
			answersToShow = [{
				answerId: -1,
				questionId: this.props.question.questionId,
				commenterEmail: null,
			}];
		}

		return (
			<li className={this.props.isUnsaved ? "CommentBox CommentBox--unsavedChanges" : "CommentBox"}>
				<>
					{documentTitle &&
						<h3 className="CommentBox__title mt--0 mb--0">{documentTitle}</h3>
					}
					<h4 data-qa-sel="comment-box-title" className="CommentBox__title mt--0 mb--0">
						Question on <span className="text-lowercase">{commentOn}</span>
					</h4>
				</>

				{this.isTextSelection(this.props.question) &&
					<>
						<h3 className="CommentBox__title mt--0 mb--0">{documentTitle}</h3>
						<h4 data-qa-sel="comment-box-title" className="CommentBox__title mt--0 mb--0">
							Question on: <span className="text-lowercase">{commentOn}</span>
						</h4>
						<div className="CommentBox__quote mb--d">{quote}</div>
					</>
				}

				<div className="font-weight-bold markdown mt--d mb--d">
					<ReactMarkdown
						allowedTypes={["root", "text", "list", "listItem"]}
						unwrapDisallowed={true}
						source={this.props.question.questionText}
					/>
				</div>

				{this.props.isUnsaved &&
					<p className="CommentBox__validationMessage">You have unsaved changes</p>
				}
				<UserContext.Consumer>
					{(contextValue: ContextType) => {
						return (
							this.props.showAnswer && (
								<>
									{this.returnAnswers(answersToShow)}
									{!contextValue.isLead &&
										this.returnAnswers(otherUsersAnswers, true)
									}
								</>
							)
						);}}
				</UserContext.Consumer>

			</li>
		);
	}
}
