// @flow
import React from "react";

export const TextAnswer = (props) => {
	return (
		<div className="form__group form__group--textarea mb--b">
			<label
				className="form__label visually-hidden"
				htmlFor={props.unique}>
				{props.questionText}
			</label>
			<textarea
				data-hj-whitelist
				data-qa-sel="Comment-text-area"
				disabled={props.readOnly}
				id={props.unique}
				className="form__input form__input--textarea"
				onInput={props.textareaChangeHandler}
				defaultValue={props.answerText}/>
		</div>
	);
};
