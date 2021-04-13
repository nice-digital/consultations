// @flow
import React, { Component } from "react";
import { withRouter } from "react-router-dom";

import { UserContext } from "../../context/UserContext";
import  LoginBannerWithRouter from "../LoginBanner/LoginBanner";

type PropsType = {
	enableOrganisationalCommentingFeature: boolean,
	consultationStatus: string,
}

type StateType = {
	organisationalResponder: boolean | null,
	responderCode: string | null,
}

export class LoginPanel extends Component<PropsType, StateType> {

	constructor(props: PropsType) {
		super(props);

		this.state = {
			organisationalResponder: null,
			responderCode: null,
		};
	}

	componentDidMount() {}

	fieldsChangeHandler = (e) => {
		let fieldValue = e.target.value;

		if (fieldValue === "true" || fieldValue === "false") {
			fieldValue = (fieldValue === "true");
		}

		this.setState({	[e.target.name]: fieldValue });
	};

	goBack = () => {
		const { responderCode } = this.state;

		// back to second question
		let newState = { responderCode: null };

		// back to first question
		if (responderCode === null || responderCode === "code") {
			newState.organisationalResponder = null;
		}

		this.setState(newState);
	};

	render() {
		const { organisationalResponder, responderCode } = this.state;
		const { enableOrganisationalCommentingFeature } = this.props;

		const showFirstScreen = (organisationalResponder === null);
		const showSecondScreen = (organisationalResponder === true && (responderCode === null || responderCode === "code"));
		const showLogin = (organisationalResponder === false || responderCode !== null || !enableOrganisationalCommentingFeature);
		const showBackLink = (!showFirstScreen);

		const loginBannerProps = {
			signInButton: true,
			signInText: "to comment on this consultation as an individual",
			currentURL: this.props.match.url,
			signInURL: this.context.signInURL,
			registerURL: this.context.registerURL,
			allowOrganisationCodeLogin: false,
			title: "Comment as an individual",
			showBorder: false,
		};

		if (responderCode === "lead" || responderCode === "sole") {
			loginBannerProps.title = "Comment on behalf of my organisation";
			loginBannerProps.signInText = responderCode === "lead" ? "to comment on this consultation as a Commenting lead" : null;
		}

		if (responderCode === "code") {
			loginBannerProps.allowOrganisationCodeLogin = true;
			loginBannerProps.codeLoginOnly = true;
			loginBannerProps.title = null;
		}

		if (!enableOrganisationalCommentingFeature) {
			loginBannerProps.title = null;
			loginBannerProps.signInText = null;
		}

		return (
			<>
				{enableOrganisationalCommentingFeature &&
					<>
						{showFirstScreen &&
							<LoginSelectionOrg
								currentlySelected={organisationalResponder}
								onChangeOption={this.fieldsChangeHandler}
							/>
						}

						{showSecondScreen &&
							<LoginSelectionCode
								currentlySelected={responderCode}
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
						<span role="button" className="login-panel__back" tabIndex={0} onMouseDown={this.goBack}>Back</span>
					</div>
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
		<div className="container">
			<h3>Are you commenting as an individual or as part of an organisation?</h3>
			<p>You will not be able to change how you comment later.</p>
			<LoginSelectionRadio
				name="organisationalResponder"
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
		<div className="container">
			<h3>Do you have a code from your organisation?</h3>
			<LoginSelectionRadio
				name="responderCode"
				radios={radios}
				currentlySelected={currentlySelected}
				onChangeOption={onChangeOption}
			/>
		</div>
	);
};

// type stuff
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
