import React from "react";

export const QuestionControls = (props) => {

	const {
		isFirst,
		isLast,
		question,
		unsavedChanges,
		totalQuestionQty,
		moveQuestion,
		deleteQuestion,
	} = props;

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
			<button
				className="btn mr--0 mb--0 ml--0"
				onClick={e => deleteQuestion(e, question)}>
				Delete
			</button>
			{totalQuestionQty > 1 && question.questionText && question.questionText.length > 0 && !unsavedChanges &&
			<div className="right">
				{!isFirst &&
				<button
					className="btn btn--inverse mb--0 mr--0"
					onClick={e => moveQuestion(e, question, "up")}>
					Move Up
				</button>
				}
				{!isLast &&
				<button
					className="btn btn--inverse ml--c mr--0 ml--0 mb--0"
					onClick={e => moveQuestion(e, question, "down")}>
					Move Down
				</button>
				}
			</div>
			}
		</div>
	);
};
