// @flow
import React, { Component } from "react";
import { withRouter } from "react-router-dom";

import { UserContext } from "../../context/UserContext";
import { tagManager } from "../../helpers/tag-manager";

import  LoginBannerWithRouter from "../LoginBanner/LoginBanner";

type PropsType = {
	questionsTabIsOpen: boolean,
}

type StateType = {
	respondingAsOrg: boolean | null,
	respondingAsOrgType: string | null,
}

export class LoginPanel extends Component<PropsType, StateType> {

	constructor(props: PropsType) {
		super(props);

		this.state = {
			respondingAsOrg: null,
			respondingAsOrgType: null,
		};
	}

	componentDidMount() {}

	fieldsChangeHandler = (e) => {
		const fieldName = e.target.name;
		let fieldValue = e.target.value;

		const tagManagerLabel = {
			sole: "organisation login sole representative",
			lead: "organisation login as lead",
			code: "organisation login with code",
			false: "individual login",
			true: null,
		}[fieldValue];

		if (fieldValue === "true" || fieldValue === "false") {
			fieldValue = (fieldValue === "true");
		}

		this.setState(() => {
			if (tagManagerLabel) {
				tagManager({
					event: "generic",
					category: "Consultation comments page",
					action: "Clicked",
					label: tagManagerLabel,
				});
			}

			return { [fieldName]: fieldValue };
		});
	};

	goBack = () => {
		const { respondingAsOrgType } = this.state;

		// back to second question
		let newState = { respondingAsOrgType: null };

		// back to first question
		if (respondingAsOrgType === null || respondingAsOrgType === "code") {
			newState.respondingAsOrg = null;
		}

		this.setState(newState);
	};

	render() {
		const { respondingAsOrg, respondingAsOrgType } = this.state;
		const questionsTabIsOpen = this.props.questionsTabIsOpen;
		const organisationalCommentingFeature = this.context.organisationalCommentingFeature;

		const showFirstScreen = (respondingAsOrg === null);
		const showSecondScreen = (respondingAsOrg === true && (respondingAsOrgType === null || respondingAsOrgType === "code"));
		const showLogin = (respondingAsOrg === false || respondingAsOrgType !== null || !organisationalCommentingFeature);
		const showBackLink = (!showFirstScreen);

		const loginBannerProps = {
			signInButton: false,
			signInText: "to access the consultation",
			currentURL: this.props.match.url,
			signInURL: this.context.signInURL,
			registerURL: this.context.registerURL,
			allowOrganisationCodeLogin: false,
			title: "Make or review comments as an individual",
			isInCommentsPanel: true,
		};

		if (respondingAsOrgType === "lead" || respondingAsOrgType === "sole") {
			loginBannerProps.title = "Make or review comments on behalf of your organisation";
		}

		if (respondingAsOrgType === "code") {
			loginBannerProps.allowOrganisationCodeLogin = true;
			loginBannerProps.codeLoginOnly = true;
			loginBannerProps.title = null;
		}

		if (!organisationalCommentingFeature) {
			loginBannerProps.title = null;
			loginBannerProps.signInText = null;
			loginBannerProps.signInButton = true;
		}

		return (
			<>
				{organisationalCommentingFeature &&
					<>
						{showFirstScreen &&
							<LoginSelectionOrg
								currentlySelected={respondingAsOrg}
								onChangeOption={this.fieldsChangeHandler}
							/>
						}

						{showSecondScreen &&
							<LoginSelectionCode
								currentlySelected={respondingAsOrgType}
								onChangeOption={this.fieldsChangeHandler}
							/>
						}
					</>
				}

				{showLogin &&
					<LoginBannerWithRouter {...loginBannerProps} />
				}

				{showBackLink &&
					<div className="container">
						<button className="login-panel__back" id="loginPanelBackButton" onClick={this.goBack}>&lt; Back</button>
					</div>
				}

				{questionsTabIsOpen &&
					<hr/>
				}
			</>
		);

	}
}

const LoginSelectionOrg = (props) => {
	const { currentlySelected, onChangeOption } = props;

	const radios = [
		{
			variant: "organisation",
			label: "As part of an organisation",
			value: true,
		},
		{
			variant: "individual",
			label: "As an individual",
			value: false,
		},
	];

	return (
		<div className="container" id="loginPanelScreen1">
			<h3>How are you taking part in this consultation?</h3>
			<p>You will not be able to change how you comment later.</p>
			<LoginSelectionRadio
				name="respondingAsOrg"
				radios={radios}
				currentlySelected={currentlySelected}
				onChangeOption={onChangeOption}
			/>
		</div>
	);
};

const LoginSelectionCode = (props) => {
	const { currentlySelected, onChangeOption } = props;

	const radios = [
		{
			variant: "code",
			label: "Yes, I have a code provided by my organisation",
			value: "code",
		},
		{
			variant: "lead",
			label: "Yes, I am the commenting lead for my organisation",
			value: "lead",
		},
		{
			variant: "sole",
			label: "No, I am the sole representative for my organisation",
			value: "sole",
		},
	];

	return (
		<div className="container" id="loginPanelScreen2">
			<h3>Do you have a code from your organisation?</h3>
			<LoginSelectionRadio
				name="respondingAsOrgType"
				radios={radios}
				currentlySelected={currentlySelected}
				onChangeOption={onChangeOption}
			/>
		</div>
	);
};

const LoginSelectionRadio = (props) => {
	const { name, radios, currentlySelected, onChangeOption } = props;

	return (
		<div role="radiogroup">
			{radios.map((radio, index) => (
				<div className="form__group form__group--radio form__group--inline" key={index}>
					<input
						className="form__radio"
						id={`${name}--${radio.variant}`}
						type="radio"
						name={name}
						checked={currentlySelected === radio.value}
						onChange={onChangeOption}
						value={radio.value}
					/>
					<label
						className="form__label form__label--radio"
						htmlFor={`${name}--${radio.variant}`}>
						{radio.label}
					</label>
				</div>
			))}
		</div>
	);
};

export default withRouter(LoginPanel);

LoginPanel.contextType = UserContext;
