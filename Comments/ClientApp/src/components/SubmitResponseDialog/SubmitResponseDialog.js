import React, {Fragment} from "react";

export const SubmitResponseDialog = props => {
	const {
		isAuthorised,
		userHasSubmitted,
		validToSubmit,
		submitConsultation,
		inputChangeHandler,
		respondingAsOrganisation, //TODO: use this field
		organisationName,
		hasTobaccoLinks, //TODO: use this field
		tobaccoDisclosure,
	} = props;

	return (
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

								<fieldset className="form__fieldset">
									<legend className="form__legend">Please answer the below questions before submitting</legend>
									<div className="form__group form__group--text">
										<label htmlFor="organisationName" className="form__label">
											Organisation
										</label>
										<input id="organisationName" name="organisationName" value={organisationName} className="form__input" type="text" onChange={inputChangeHandler}/>
										<div className="form__hint form__hint--inverse">If you are commenting on behalf of an organisation, please enter the organisation name</div>
									</div>
									<div className="form__group form__group--textarea">
										<label htmlFor="tobaccoDisclosure" className="form__label">
											Disclose tobacco industry links
										</label>
										<textarea id="tobaccoDisclosure" name="tobaccoDisclosure" value={tobaccoDisclosure} className="form__input form__input--textarea" onChange={inputChangeHandler}/>

										<div className="form__hint form__hint--inverse">Please disclose whether you or the organisation who's behalf on which you are commenting has any past or current, direct or indirect links to, or receives funding from, the tobacco industry.</div>
									</div>
								</fieldset>

								{isAuthorised &&
								<Fragment>
									<h3 className="mt--0">Ready to submit?</h3>
									<button
										disabled={!validToSubmit}
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
