import React, { Fragment } from "react";

export const QuestionControls = (props) => {

	const {
		question,
		unsavedChanges,
		totalQuestionQty,
		moveQuestion,
		deleteQuestion } = props;

	return (
		<div>
			{question.questionText && question.questionText.length > 0 ?
				unsavedChanges ?
					<input
						className="btn ml--0 mb--0"
						type="submit"
						value="Save Question"
					/>
					:
					<span className="ml--0 mb--0 CommentBox__savedIndicator">Saved</span>
				:
				null
			}
			{totalQuestionQty > 1 && question.questionText && question.questionText.length > 0 && !unsavedChanges &&
			<Fragment>
				<button
					className="btn btn--inverse ml--0 mb--0"
					onClick={e => moveQuestion(e, question, "up")}>
					<span className="icon icon--chevron-up" aria-hidden="true"/>
					<span className="visually-hidden">Move Up</span>
				</button>
				<button
					className="btn btn--inverse ml--0 mb--0"
					onClick={e => moveQuestion(e, question, "down")}>
					<span className="icon icon--chevron-down" aria-hidden="true"/>
					<span className="visually-hidden">Move Down</span>
				</button>
			</Fragment>
			}
			<button
				className="btn mr--0 mb--0 pull-right"
				onClick={e => deleteQuestion(e, question)}>
				Delete
			</button>
		</div>
	);
};
