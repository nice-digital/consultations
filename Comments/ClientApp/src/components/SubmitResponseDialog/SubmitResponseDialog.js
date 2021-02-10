import React, { PureComponent, Fragment } from "react";
import { pullFocusByQuerySelector } from "../../helpers/accessibility-helpers";
import { SubmitResponseFeedback } from "../SubmitResponseFeedback/SubmitResponseFeedback";
import { Input } from "@nice-digital/nds-input";

export class SubmitResponseDialog extends PureComponent {

	state = {
		feedbackVisible: false,
	};

	mandatoryQuestionsAreValid = () => {
		let organisationIsValid = false;
		if ((this.props.respondingAsOrganisation === "yes" && this.props.organisationName.length > 0) || this.props.respondingAsOrganisation === "no") {
			organisationIsValid = true;
		}

		let tobaccoIsValid = false;
		if ((this.props.hasTobaccoLinks === "yes" && this.props.tobaccoDisclosure.length > 0) || this.props.hasTobaccoLinks === "no") {
			tobaccoIsValid = true;
		}

		let organisationExpressionOfInterestIsValid = false;
		if (this.props.respondingAsOrganisation === "no" || !this.props.showExpressionOfInterestSubmissionQuestion ||
			this.props.organisationExpressionOfInterest === "yes"  || this.props.organisationExpressionOfInterest === "no"){
			organisationExpressionOfInterestIsValid = true;
		}

		return organisationIsValid && tobaccoIsValid && organisationExpressionOfInterestIsValid;
	};

	emailIsValid = () => {
		const emailRegex = /\S+@\S+\.\S+/;
		const emailAddress = this.props.emailAddress;

		let emailIsValid = false;

		if ((emailAddress.length > 0) && (emailRegex.test(emailAddress.toLowerCase()))) {
			emailIsValid = true;
		}

		return emailIsValid;
	}

	submitConsultation = () => {
		if (this.props.validToSubmit && this.mandatoryQuestionsAreValid() && this.props.unsavedIds.length === 0) {
			this.props.submitConsultation();
		}
		else {
			this.setState({
				feedbackVisible: true,
			});
			pullFocusByQuerySelector("#SubmitResponseFeedback");
		}
	};

	handleSubmitToLeadClick = () => {
		if (this.props.validToSubmit && this.emailIsValid() && this.props.unsavedIds.length === 0) {
			this.props.submitToLead();
		} else {
			this.setState({
				feedbackVisible: true,
			});
			pullFocusByQuerySelector("#SubmitResponseFeedback");
		}
	};

	render() {
		const {
			isAuthorised,
			submittedDate,
			fieldsChangeHandler,
			organisationName,
			tobaccoDisclosure,
			respondingAsOrganisation,
			hasTobaccoLinks,
			showExpressionOfInterestSubmissionQuestion,
			organisationExpressionOfInterest,
			isOrganisationCommenter,
		} = this.props;

		return (
			<>
				{isOrganisationCommenter ? (
					<div className="panel">
						<h2>You are about to send your final response to {organisationName}</h2>
						<p>Enter your email address so that your organisation can contact you about your comments if they have any questions.</p>
						<Input
							label="Email address"
							name="emailAddress"
							type="email"
							placeholder="eg: your.name@example.com..."
							onChange={fieldsChangeHandler}
						/>
						<p>Once you have sent your response you will not be able to edit it or add any more comments.</p>
						{this.state.feedbackVisible &&
							<SubmitResponseFeedback
								{...this.props}
								unsavedIdsQty={this.props.unsavedIds.length}
							/>
						}
						<button onClick={this.handleSubmitToLeadClick} className="btn btn--cta">Send your response to your organisation</button>
					</div>
				) : (
					<div className="panel">
						<p className="lead">You are about to submit your final response to NICE.</p>

						<p>You must answer the following questions before you submit.</p>

						<p><strong>Are you responding on behalf of an organisation?</strong></p>

						<div role="radiogroup" aria-label="Are you responding on behalf of an organisation?">
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
									data-qa-sel="respond-yes-responding-as-org"
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
						<Fragment>
							<div className="form__group form__group--text">
								<label htmlFor="organisationName" className="form__label">
									<strong>Enter the name of your organisation</strong>
								</label>
								<input data-qa-sel="organisation-name" data-hj-whitelist id="organisationName" name="organisationName" value={organisationName}
									className="form__input" type="text" onChange={fieldsChangeHandler}/>
							</div>

							{showExpressionOfInterestSubmissionQuestion &&
								<Fragment>
									<p>
										<strong>Would your organisation like to express an interest in formally supporting this quality standard?</strong><br/>
										<a href="/standards-and-indicators/get-involved/support-a-quality-standard" target="_new">More information</a>
									</p>
									<div role="radiogroup" aria-label="Would your organisation like to express an interest in formally supporting this quality standard?">
										<div className="form__group form__group--radio form__group--inline">
											<input
												className="form__radio"
												id="organisationExpressionOfInterest--true"
												type="radio"
												name="organisationExpressionOfInterest"
												checked={organisationExpressionOfInterest === "yes"}
												onChange={fieldsChangeHandler}
												value={"yes"}
											/>
											<label
												data-qa-sel="express-interest-yes"
												className="form__label form__label--radio"
												htmlFor="organisationExpressionOfInterest--true">
												Yes
											</label>
										</div>

										<div className="form__group form__group--radio form__group--inline">
											<input
												className="form__radio"
												id="organisationExpressionOfInterest--false"
												type="radio"
												name="organisationExpressionOfInterest"
												checked={organisationExpressionOfInterest === "no"}
												onChange={fieldsChangeHandler}
												value={"no"}
											/>
											<label
												data-qa-sel="express-interest-no"
												className="form__label form__label--radio"
												htmlFor="organisationExpressionOfInterest--false">
												No
											</label>
										</div>
									</div>
								</Fragment>
							}
						</Fragment>
						}

						<p><strong>Do you or the organisation you represent have any links with the tobacco industry?</strong></p>
						<p className="mb--0">This includes:</p>
						<ul className="mt--0">
							<li>current or past links</li>
							<li>direct or indirect links</li>
							<li>receiving funding from the tobacco industry.</li>
						</ul>

						<div role="radiogroup"
							aria-label="Do you or the organisation you represent have any links with the tobacco industry?">

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
									value={"no"}
								/>
								<label
									data-qa-sel="respond-no-has-tobac-links"
									className="form__label form__label--radio"
									htmlFor="hasTobaccoLinks--false">
									No
								</label>
							</div>
						</div>

						{hasTobaccoLinks === "yes" &&
							<div data-qa-sel="respond-yes-has-tobac-links" className="form__group form__group--textarea">
								<label htmlFor="tobaccoDisclosure" className="form__label">
									<strong>Please provide details</strong>
								</label>
								<textarea
									data-hj-whitelist
									id="tobaccoDisclosure"
									name="tobaccoDisclosure"
									value={tobaccoDisclosure}
									className="form__input form__input--textarea"
									onChange={fieldsChangeHandler}/>
							</div>
						}

						{isAuthorised &&
						<Fragment>
							<p><strong>Now submit your response to NICE.</strong></p>
							<p>After submission you won't be able to edit your comments further or add any extra comments.</p>
							{this.state.feedbackVisible &&
							<SubmitResponseFeedback
								{...this.props}
								unsavedIdsQty={this.props.unsavedIds.length}
							/>
							}
							<button
								className="btn btn--cta"
								data-qa-sel="submit-comment-button"
								onClick={this.submitConsultation}>
								{submittedDate ? "Responses submitted" : "Submit my response"}
							</button>
						</Fragment>
						}

					</div>
				)}
			</>
		);
	}
}
