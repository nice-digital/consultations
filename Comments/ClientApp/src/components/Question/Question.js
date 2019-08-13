// @flow

import React, { Component, Fragment } from "react";
import ReactMarkdown from "react-markdown";
import { Answer } from "../Answer/Answer";

type PropsType = {
	staticContext?: any,
	saveAnswerHandler: Function,
	deleteAnswerHandler: Function,
	question: QuestionType,
	readOnly: boolean,
	isUnsaved: boolean,
	documentTitle?: string,
	showAnswer: boolean,
};

type StateType = {
	questionId: number
};

export class Question extends Component<PropsType, StateType> {

	isTextSelection = (question) => question.commentOn && question.commentOn.toLowerCase() === "selection" && question.quote;

	render() {
		if (!this.props.question) return null;
		const { documentTitle } = this.props;
		const { commentOn, quote } = this.props.question;
		let answers = this.props.question.answers;
		if (answers === null || answers.length < 1){
			answers = [{
				answerId: -1,
				questionId: this.props.question.questionId,
			}];
		}

		return (
			<li className={this.props.isUnsaved ? "CommentBox CommentBox--unsavedChanges" : "CommentBox"}>
				<Fragment>
					{documentTitle &&
						<h3 className="CommentBox__title mt--0 mb--0">{documentTitle}</h3>
					}
					<h4 data-qa-sel="comment-box-title" className="CommentBox__title mt--0 mb--0">
						Question on <span className="text-lowercase">{commentOn}</span>
					</h4>
				</Fragment>

				{this.isTextSelection(this.props.question) &&
					<Fragment>
						<h3 className="CommentBox__title mt--0 mb--0">{documentTitle}</h3>
						<h4 data-qa-sel="comment-box-title" className="CommentBox__title mt--0 mb--0">
							Question on: <span className="text-lowercase">{commentOn}</span>
						</h4>
						<div className="CommentBox__quote mb--d">{quote}</div>
					</Fragment>
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

				{this.props.showAnswer &&
					answers.map((answer) => {
						return (
							<Answer
								questionText={this.props.question.questionText}
								questionType={this.props.question.questionType}
								updateUnsavedIds={this.props.updateUnsavedIds}
								questionId={this.props.question.questionId}
								readOnly={this.props.readOnly}
								isVisible={this.props.isVisible}
								key={answer.answerId}
								unique={`Answer${answer.answerId}`}
								answer={answer}
								saveAnswerHandler={this.props.saveAnswerHandler}
								deleteAnswerHandler={this.props.deleteAnswerHandler}
							/>
						);
					})
				}
			</li>
		);
	}
}
