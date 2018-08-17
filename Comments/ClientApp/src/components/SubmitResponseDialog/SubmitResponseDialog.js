import React, {Component, Fragment} from "react";

export class SubmitResponseDialog extends Component {

	constructor(){
		super();
		this.state = {
			showOrganisationInput: false,
			showTobaccoInput: false,
			mandatoryQuestionsAreValid: false,
		};
	}

	render(){
		const {
			isAuthorised,
			userHasSubmitted,
			validToSubmit,
			submitConsultation,
			inputChangeHandler,
			organisationName,
			tobaccoDisclosure,
			respondingAsOrganisation,
			hasTobaccoLinks } = this.props;
		const {
			showOrganisationInput,
			showTobaccoInput,
			mandatoryQuestionsAreValid } = this.state;
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
												value="true"/>
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
												value="false"/>
											<label
												className="form__label form__label--radio"
												htmlFor="respondingAsOrganisation--false">
												No
											</label>
										</div>

									</div>

									{showOrganisationInput &&
										<div className="form__group form__group--text">
											<label htmlFor="organisationName" className="form__label">
												Enter the name of your organisation
											</label>
											<input id="organisationName" name="organisationName" value={organisationName}
														 className="form__input" type="text" onChange={inputChangeHandler}/>
										</div>
									}

									<p>Do you or the organisation you represent have any links with the tobacco industry?</p>
									<p>This includes:</p>
									<ul>
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
												value="true"/>
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
												value="false"/>
											<label
												className="form__label form__label--radio"
												htmlFor="hasTobaccoLinks--false">
												No
											</label>
										</div>

									</div>

									{showTobaccoInput &&
										<div className="form__group form__group--textarea">
											<label htmlFor="tobaccoDisclosure" className="form__label">
												Please provide details
											</label>
											<textarea id="tobaccoDisclosure" name="tobaccoDisclosure" value={tobaccoDisclosure} className="form__input form__input--textarea" onChange={inputChangeHandler}/>
										</div>
									}

									<p>Ready to submit?</p>

									{isAuthorised &&
									<Fragment>
										<h3 className="mt--0">Ready to submit?</h3>
										<button
											disabled={!validToSubmit && mandatoryQuestionsAreValid}
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
		)
	}
};
