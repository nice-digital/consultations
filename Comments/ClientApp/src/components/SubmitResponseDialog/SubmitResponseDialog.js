import React, { Component } from "react";
import { pullFocusByQuerySelector } from "../../helpers/accessibility-helpers";
import { SubmitResponseFeedback } from "../SubmitResponseFeedback/SubmitResponseFeedback";
import { Input } from "@nice-digital/nds-input";
import { Alert } from "@nice-digital/nds-alert";

export class SubmitResponseDialog extends Component {

	state = {
		feedbackVisible: false,
		showSubmitWarning: false,
	};

	emailRef = React.createRef();

	mandatoryQuestionsAreValid = () => {
		let organisationIsValid = false;
		if ((this.props.respondingAsOrganisation && this.props.organisationName.length > 0) || (this.props.respondingAsOrganisation !== null && !this.props.respondingAsOrganisation)) {
			organisationIsValid = true;
		}

		let tobaccoIsValid = false;
		if ((this.props.hasTobaccoLinks && this.props.tobaccoDisclosure.length > 0) || (this.props.hasTobaccoLinks !== null && !this.props.hasTobaccoLinks)) {
			tobaccoIsValid = true;
		}

		let organisationExpressionOfInterestIsValid = false;
		if (!this.props.respondingAsOrganisation || !this.props.showExpressionOfInterestSubmissionQuestion ||
			this.props.organisationExpressionOfInterest  || (this.props.organisationExpressionOfInterest !== null && !this.props.organisationExpressionOfInterest)){
			organisationExpressionOfInterestIsValid = true;
		}

		return organisationIsValid && tobaccoIsValid && organisationExpressionOfInterestIsValid;
	};

	submitConsultation = () => {
		if (this.state.showSubmitWarning) {
			const tooManyAnswersFilter = (question) => question.answers.length > 1;
			const tooManyAnswersToAQuestion = this.props.questions.some(tooManyAnswersFilter);
			if (this.props.validToSubmit && this.mandatoryQuestionsAreValid() && this.props.unsavedIds.length === 0 && !tooManyAnswersToAQuestion) {
				this.props.submitConsultation();
			}
			else {
				this.setState({
					feedbackVisible: true,
				});
				pullFocusByQuerySelector("#SubmitResponseFeedback");
			}
		} else {
			this.setState({
				showSubmitWarning: true,
			});
		}
	};

	handleSubmitToLeadClick = () => {
		if (this.props.validToSubmit && this.emailRef.current.validity.valid && this.props.unsavedIds.length === 0) {
			this.props.submitToLead(this.props.organisationName);
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
			isLead,
		} = this.props;

		return (
			<>
				{isOrganisationCommenter && !isLead ? (
					<div className="panel">
						<h2>You are about to send your final response to {organisationName}</h2>
						<p>Enter your email address so that your organisation can contact you about your comments if they have any questions.</p>
						<Input
							label="Email address"
							name="emailAddress"
							type="email"
							required
							placeholder="eg: your.name@example.com..."
							onChange={fieldsChangeHandler}
							inputRef={this.emailRef}
						/>
						<p>Once you have sent your response you will not be able to edit it or add any more comments.</p>
						{this.state.feedbackVisible &&
							<SubmitResponseFeedback
								{...this.props}
								unsavedIdsQty={this.props.unsavedIds.length}
								emailIsEmpty={this.emailRef.current.validity.valueMissing}
								emailIsWrongFormat={this.emailRef.current.validity.typeMismatch}
							/>
						}
						<button onClick={this.handleSubmitToLeadClick} className="btn btn--cta">Send your response to your organisation</button>
					</div>
				) : (
					<div className="panel">
						<p className="lead">You are about to submit your final response to NICE.</p>

						{isLead &&
							<>
								<p>You are responding on behalf of <strong>{organisationName}</strong></p>
							</>
						}

						<p>You must answer the following {(!isLead || showExpressionOfInterestSubmissionQuestion) ? "questions" : "question"} before you submit.</p>

						{!isLead &&
						<>

							<div role="radiogroup" aria-labelledby="responding_as_organisation">

								<p id="responding_as_organisation"><strong>Are you responding on behalf of an organisation?</strong></p>

								<div className="form__group form__group--radio form__group--inline">
									<input
										className="form__radio"
										id="respondingAsOrganisation--true"
										type="radio"
										name="respondingAsOrganisation"
										checked={respondingAsOrganisation}
										onChange={fieldsChangeHandler}
										value={true}
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
										checked={!respondingAsOrganisation && respondingAsOrganisation != null}
										onChange={fieldsChangeHandler}
										value={false}
									/>
									<label
										data-qa-sel="respond-no-responding-as-org"
										className="form__label form__label--radio"
										htmlFor="respondingAsOrganisation--false">
										No
									</label>
								</div>

							</div>
						</>
						}

						{(!isLead && respondingAsOrganisation) &&
							<>
								<div className="form__group form__group--text">
									<label htmlFor="organisationName" className="form__label">
										<strong>Enter the name of your organisation</strong>
									</label>
									<input data-qa-sel="organisation-name" data-hj-whitelist id="organisationName" name="organisationName" value={organisationName}
										className="form__input" type="text" onChange={fieldsChangeHandler}/>
								</div>
							</>
						}

						{respondingAsOrganisation && showExpressionOfInterestSubmissionQuestion &&
							<>

								<div role="radiogroup" aria-labelledby="organisation_express_interest">
									<p id="organisation_express_interest">
										<strong>Would your organisation like to express an interest in formally supporting this quality standard?</strong><br/>
										<a href="/standards-and-indicators/get-involved/support-a-quality-standard" target="_new">More information</a>
									</p>
									<div className="form__group form__group--radio form__group--inline">
										<input
											className="form__radio"
											id="organisationExpressionOfInterest--true"
											type="radio"
											name="organisationExpressionOfInterest"
											checked={organisationExpressionOfInterest}
											onChange={fieldsChangeHandler}
											value={true}
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
											checked={organisationExpressionOfInterest !== null && !organisationExpressionOfInterest}
											onChange={fieldsChangeHandler}
											value={false}
										/>
										<label
											data-qa-sel="express-interest-no"
											className="form__label form__label--radio"
											htmlFor="organisationExpressionOfInterest--false">
											No
										</label>
									</div>
								</div>
							</>
						}

						<div role="radiogroup" aria-labelledby="tobacco_links">
							<p id="tobacco_links"><strong>Do you or the organisation you represent have any links with the tobacco industry?</strong></p>
							<p className="mb--0">This includes:</p>
							<ul className="mt--0">
								<li>current or past links</li>
								<li>direct or indirect links</li>
								<li>receiving funding from the tobacco industry.</li>
							</ul>

							<div className="form__group form__group--radio form__group--inline">
								<input
									className="form__radio"
									id="hasTobaccoLinks--true"
									type="radio"
									name="hasTobaccoLinks"
									value={true}
									checked={hasTobaccoLinks}
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
									checked={hasTobaccoLinks !== null && !hasTobaccoLinks}
									value={false}
								/>
								<label
									data-qa-sel="respond-no-has-tobac-links"
									className="form__label form__label--radio"
									htmlFor="hasTobaccoLinks--false">
									No
								</label>
							</div>
						</div>

						{hasTobaccoLinks &&
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
						<>
							<p><strong>Now submit your response to NICE.</strong></p>
							<p>After submission you won't be able to edit your comments further or add any extra comments.</p>
							{this.state.showSubmitWarning &&
								<Alert type="caution" role="alert">
									<p>I understand that once I have submitted my response, I will not be able to edit my comments or provide additional information.</p>
								</Alert>
							}
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
								{submittedDate ? "Responses submitted" : this.state.showSubmitWarning ? "Yes submit my response" : "Submit my response"}
							</button>
							{this.state.showSubmitWarning &&
								<button
									className="btn"
									data-qa-sel="cancel-comment-button"
									onClick={() => {
										this.setState({ showSubmitWarning: false, })
									}}
								>
									Cancel
								</button>
							}
						</>
						}

					</div>
				)}
			</>
		);
	}
}
