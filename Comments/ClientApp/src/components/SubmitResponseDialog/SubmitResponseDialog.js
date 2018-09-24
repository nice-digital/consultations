import React, { Fragment } from "react";

export const SubmitResponseDialog = (props) => {

	const {
		isAuthorised,
		userHasSubmitted,
		validToSubmit,
		submitConsultation,
		fieldsChangeHandler,
		organisationName,
		tobaccoDisclosure,
		respondingAsOrganisation,
		hasTobaccoLinks,
		unsavedIds,
	} = props;

	function mandatoryQuestionsAreValid() {
		let organisationIsValid = false;
		let tobaccoIsValid = false;
		if (
			(respondingAsOrganisation === "yes" && organisationName.length > 0) || respondingAsOrganisation === "no") {
			organisationIsValid = true;
		}
		if ((hasTobaccoLinks === "yes" && tobaccoDisclosure.length > 0) || hasTobaccoLinks === "no") {
			tobaccoIsValid = true;
		}
		return organisationIsValid && tobaccoIsValid;
	}

	if (userHasSubmitted) return null;

	return(
		<div className="panel">
			<p className="lead">You are about to submit your final response to NICE</p>

			<p>You must answer the following questions before you submit.</p>

			<p><strong>Are you responding on behalf of an organisation?</strong></p>

			<div>

				<div className="form__group form__group--radio form__group--inline">
					<input
						className="form__radio"
						id="respondingAsOrganisation--true"
						type="radio"
						name="respondingAsOrganisation"
						checked={respondingAsOrganisation === "yes"}
						onChange={fieldsChangeHandler}
						value={"yes"}
					/>
					<label
						className="form__label form__label--radio"
						htmlFor="respondingAsOrganisation--true">
								Yes
					</label>
				</div>

				<div className="form__group form__group--radio form__group--inline">
					<input
						className="form__radio"
						id="respondingAsOrganisation--false"
						type="radio"
						name="respondingAsOrganisation"
						checked={respondingAsOrganisation === "no"}
						onChange={fieldsChangeHandler}
						value={"no"}
					/>
					<label
						data-qa-sel="respond-no-responding-as-org"
						className="form__label form__label--radio"
						htmlFor="respondingAsOrganisation--false">
								No
					</label>
				</div>

			</div>

			{respondingAsOrganisation === "yes" &&
					<div className="form__group form__group--text">
						<label htmlFor="organisationName" className="form__label">
							<strong>Enter the name of your organisation</strong>
						</label>
						<input id="organisationName" name="organisationName" value={organisationName}
									 className="form__input" type="text" onChange={fieldsChangeHandler}/>
					</div>
			}

			<p><strong>Do you or the organisation you represent have any links with the tobacco industry?</strong></p>
			<p className="mb--0">This includes:</p>
			<ul className="mt--0">
				<li>current or past links</li>
				<li>direct or indirect links</li>
				<li>receiving funding from the tobacco industry.</li>
			</ul>

			<div>

				<div className="form__group form__group--radio form__group--inline">
					<input
						className="form__radio"
						id="hasTobaccoLinks--true"
						type="radio"
						name="hasTobaccoLinks"
						value={"yes"}
						checked={hasTobaccoLinks === "yes"}
						onChange={fieldsChangeHandler}
					/>
					<label
						className="form__label form__label--radio"
						htmlFor="hasTobaccoLinks--true">
								Yes
					</label>
				</div>

				<div className="form__group form__group--radio form__group--inline">
					<input
						className="form__radio"
						id="hasTobaccoLinks--false"
						type="radio"
						name="hasTobaccoLinks"
						onChange={fieldsChangeHandler}
						checked={hasTobaccoLinks === "no"}
						value={"no"}/>
					<label
						data-qa-sel="respond-no-has-tobac-links"
						className="form__label form__label--radio"
						htmlFor="hasTobaccoLinks--false">
								No
					</label>
				</div>

			</div>

			{hasTobaccoLinks === "yes" &&
					<div className="form__group form__group--textarea">
						<label htmlFor="tobaccoDisclosure" className="form__label">
							<strong>Please provide details</strong>
						</label>
						<textarea id="tobaccoDisclosure" name="tobaccoDisclosure" value={tobaccoDisclosure} className="form__input form__input--textarea" onChange={fieldsChangeHandler}/>
					</div>
			}

			{isAuthorised &&
					<Fragment>
						<p><strong>Now submit your response to NICE</strong></p>
						<p>After submission you won't be able to edit your comments further or add any extra comments.</p>
						<button
							disabled={!validToSubmit || !mandatoryQuestionsAreValid() || unsavedIds.length !== 0}
							className="btn btn--cta"
							data-qa-sel="submit-comment-button"
							onClick={submitConsultation}>
							{userHasSubmitted ? "Responses submitted": "Submit my response"}
						</button>
					</Fragment>
			}
		</div>

	);
};
