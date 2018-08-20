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
		hasTobaccoLinks } = props;

	function mandatoryQuestionsAreValid() {
		let organisationIsValid, tobaccoIsValid = false;
		if ((respondingAsOrganisation === "yes" && organisationName.length > 0) || respondingAsOrganisation === "no") {
			organisationIsValid = true;
		}
		if ((hasTobaccoLinks === "yes" && tobaccoDisclosure.length > 0) || hasTobaccoLinks === "no") {
			tobaccoIsValid = true;
		}
		return organisationIsValid && tobaccoIsValid;
	}

	return(
		<Fragment>
			{userHasSubmitted ?
				<div className="hero">
					<div className="hero__container">
						<div className="hero__body">
							<div className="hero__copy">
								<p className="hero__intro" data-qa-sel="submitted-text">Thank you, your response has been submitted.</p>
							</div>
						</div>
					</div>
				</div>
				:
				<div className="hero">
					<div className="hero__container">
						<div className="hero__body">
							<div className="hero__copy">

								<p className="hero__intro">You are about to submit your final response to NICE</p>
								<p>After submission you won't be able to:</p>
								<ul>
									<li>edit your comments further</li>
									<li>add any extra comments.</li>
								</ul>

								<p>You must answer the following questions before you submit</p>

								<p>Are you responding on behalf of an organisation?</p>

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
										{/* todo: add fieldchangehandlers here */}
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
											className="form__label form__label--radio"
											htmlFor="respondingAsOrganisation--false">
											No
										</label>
									</div>

								</div>

								{respondingAsOrganisation === "yes" &&
								<div className="form__group form__group--text">
									<label htmlFor="organisationName" className="form__label">
										Enter the name of your organisation
									</label>
									<input id="organisationName" name="organisationName" value={organisationName}
												 className="form__input" type="text" onChange={fieldsChangeHandler}/>
								</div>
								}

								<p>Do you or the organisation you represent have any links with the tobacco industry?</p>
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
											className="form__label form__label--radio"
											htmlFor="hasTobaccoLinks--false">
											No
										</label>
									</div>

								</div>

								{hasTobaccoLinks === "yes" &&
								<div className="form__group form__group--textarea">
									<label htmlFor="tobaccoDisclosure" className="form__label">
										Please provide details
									</label>
									<textarea id="tobaccoDisclosure" name="tobaccoDisclosure" value={tobaccoDisclosure} className="form__input form__input--textarea" onChange={fieldsChangeHandler}/>
								</div>
								}

								{isAuthorised &&
								<Fragment>
									<h3 className="mt--0">Ready to submit?</h3>
									<button
										disabled={!validToSubmit || !mandatoryQuestionsAreValid()}
										className="btn btn--cta"
										data-qa-sel="submit-comment-button"
										onClick={submitConsultation}>
										{userHasSubmitted ? "Responses submitted": "Yes, submit my response"}
									</button>
								</Fragment>
								}

							</div>
						</div>
					</div>
				</div>
			}
		</Fragment>
	);
};
