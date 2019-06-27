// @flow
import React, {Fragment} from "react";

export const YesNoAnswer = (props) => {
	return (
		<Fragment>
			<div role="radiogroup" aria-label={props.questionText}>
				<div className="form__group form__group--radio form__group--inline">
					<input
						disabled={props.readOnly}
						className="form__radio"
						id={`${props.unique}--radio--yes`}
						type="radio"
						name={`${props.unique}--radio`}
						checked={props.answerBoolean === true}
						onChange={props.yesNoChangeHandler}
						value={true}
					/>
					<label
						className="form__label form__label--radio"
						htmlFor={`${props.unique}--radio--yes`}>
						Yes
					</label>
				</div>

				<div className="form__group form__group--radio form__group--inline">
					<input
						disabled={props.readOnly}
						className="form__radio"
						id={`${props.unique}--radio--no`}
						type="radio"
						name={`${props.unique}--radio`}
						checked={props.answerBoolean === false}
						onChange={props.yesNoChangeHandler}
						value={false}
					/>
					<label
						className="form__label form__label--radio"
						htmlFor={`${props.unique}--radio--no`}>
						No
					</label>
				</div>

			</div>

			<div className="form__group form__group--textarea mb--b">
				<label
					className="form__label mb-d"
					htmlFor={`${props.unique}--text`}>
					Do you want to add further information?
				</label>
				<textarea
					data-hj-whitelist
					data-qa-sel="Comment-text-area"
					disabled={props.readOnly}
					id={`${props.unique}--text`}
					className="form__input form__input--textarea"
					onInput={props.textareaChangeHandler}
					defaultValue={props.answerText}/>
			</div>

		</Fragment>
	);
};

// unique={unique}
// questionText={questionText}
// readOnly={readOnly}
// textareaChangeHandler={this.textareaChangeHandler}
// yesNoChangeHandler={this.yesNoChangeHandler}
// answerText={answerText}
// answerBoolean={answerBoolean}
