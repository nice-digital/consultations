// @flow
import React, { Component } from "react";
import { withRouter } from "react-router-dom";
import { DebounceInput } from "react-debounce-input";
import queryString from "query-string";
import Cookies from "js-cookie";
import { UserContext } from "../../context/UserContext";
import { load } from "../../data/loader";
import { Input } from "@nice-digital/nds-input";

type PropsType = {
	signInURL: string,
	registerURL: string,
	signInButton: boolean,
	signInText?: string,
	match: PropTypes.object.isRequired,
	location: PropTypes.object.isRequired,
	allowOrganisationCodeLogin: boolean,
	orgFieldName: string, //the organisation text input needs a unique name, however the login banner will be on the page twice. so passing in a unique name
	codeLoginOnly: boolean,
	title: string,
	isInCommentsPanel: boolean,
}

type OrganisationCode = {
	organisationAuthorisationId: number,
	organisationId: number,
	organisationName: string,
	collationCode: string,
}

type StateType = {
	userEnteredCollationCode: string,
	hasError: boolean,
	errorMessage: string,
	showAuthorisationOrganisation: boolean,
	authorisationOrganisationFound: OrganisationCode,
}

export class LoginBanner extends Component<PropsType, StateType> {

	constructor(props: PropsType) {
		super(props);

		this.state = {
			userEnteredCollationCode: "",
			hasError: false,
			errorMessage: "",
			showAuthorisationOrganisation: false,
			authorisationOrganisationFound: null,
		};
	}

	componentDidMount() {
		const userEnteredCollationCode = queryString.parse(this.props.location.search).code;
		if (userEnteredCollationCode){
			this.setState({
				userEnteredCollationCode,
			}, () => {
				this.checkOrganisationCode();
			});
		}
	}

	handleOrganisationCodeChange = (userEnteredCollationCode) => {
		this.setState({
			userEnteredCollationCode,
		}, () => {
			if (userEnteredCollationCode){ //if there's a blank value, don't bother hitting the server.
				this.checkOrganisationCode(userEnteredCollationCode);
			}else{
				this.setState({
					hasError: false,
					showAuthorisationOrganisation: false,
				});
			}
		});
	};

	gatherDataForCheckOrganisationCode = async () => {
		const organisationCode = load(
			"organisation",
			undefined,
			[],
			{
				collationCode: this.state.userEnteredCollationCode,
				consultationId: this.props.match.params.consultationId,
			})
			.then(response => response.data)
			.catch(err => {
				this.setState({
					hasError: true,
					errorMessage: err.response.data.errorException.Message,
					showAuthorisationOrganisation: false,
				});
			});
		return {
			organisationCode: await organisationCode,
		};
	}

	checkOrganisationCode = () => {
		this.gatherDataForCheckOrganisationCode()
			.then(data => {
				if (data.organisationCode != null) {
					this.setState({
						hasError: false,
						errorMessage: "",
						showAuthorisationOrganisation: true,
						authorisationOrganisationFound: data.organisationCode,
					});
				}
			})
			.catch(err => {
				throw new Error("checkOrganisationCode failed " + err);
			});
	}

	gatherDataForCreateOrganisationUserSession = async () => {
		const session = load(
			"organisationsession",
			undefined,
			[],
			{
				collationCode: this.state.userEnteredCollationCode,
				organisationAuthorisationId: this.state.authorisationOrganisationFound.organisationAuthorisationId,
			},
			"POST")
			.then(response => response.data)
			.catch(err => {
				this.setState({
					hasError: true,
					errorMessage: err.response.data.errorException.Message,
					showAuthorisationOrganisation: false,
				});
			});
		return {
			session: await session,
		};
	}

	createOrganisationUserSession = async () => {
		const session = this.gatherDataForCreateOrganisationUserSession()
			.then(data => {
				if (data.session != null) {
					this.setState({
						hasError: false,
						errorMessage: "",
					});
					return data.session;
				}
			})
			.catch(err => {
				throw new Error("checkOrganisationCode failed " + err);
			});
		return {
			session: await session,
		};
	}

	handleConfirmClick = (updateContextFunction) => {
		const consultationId = this.props.match.params.consultationId;
		this.createOrganisationUserSession().then(data => {
			console.log("data is:" + JSON.stringify(data));

			var expirationDate = new Date(data.session.expirationDateTicks);

			Cookies.set(`ConsultationSession-${consultationId}`, data.session.sessionId, {expires: expirationDate});

			//now, set state to show logged in.
			this.setState({
				showAuthorisationOrganisation: false,
			});
			updateContextFunction();
		})
			.catch(() => {
				this.setState({
					hasError: true,
					errorMessage: "Unable to confirm",
				});
			});
	}

	render(){
		const limitWidthOfButton = !this.props.signInButton; //the sign-in button isn't shown when we're trying to save space.

		const codeLoginOnly = this.props.codeLoginOnly ?? false;
		const title = this.props.title ?? "";
		const isInCommentsPanel = this.props.isInCommentsPanel ?? false;

		return (
			<div className={`${!isInCommentsPanel ? "panel panel-white" : ""} mt--0 mb--0 sign-in-banner`} id="loginBanner" data-qa-sel="sign-in-banner">
				<div className="container">
					<div className="LoginBanner" role="form">
						{title !== "" &&
							<h3>{title}</h3>
						}
						{this.props.allowOrganisationCodeLogin &&
							<>
								<div className={this.state.hasError ? "input input--error" : "input"}>
									<DebounceInput
										minLength={5}
										debounceTimeout={400}
										type="text"
										onChange={e => this.handleOrganisationCodeChange(e.target.value)}
										className={limitWidthOfButton ? "limitWidth input__input" : "input__input"}
										data-qa-sel="OrganisationCodeLogin"
										value={this.state.userEnteredCollationCode}
										element={Input}
										error={this.state.hasError}
										errorMessage={this.state.errorMessage}
										label="Enter your organisation code"
										name={"orgCode-" + this.props.orgFieldName}
									/>
								</div>
								{this.state.showAuthorisationOrganisation &&
									<>
										<p>Confirm organisation name</p>
										<p><strong>{this.state.authorisationOrganisationFound.organisationName}</strong></p>
										<UserContext.Consumer>
											{/* eslint-disable-next-line*/}
											{({ contextValue: ContextType, updateContext }) => (
												<div>
													<button className="btn btn--cta" onClick={() => this.handleConfirmClick(updateContext)}  title={"Confirm your organisation is " + this.state.authorisationOrganisationFound.organisationName}>Confirm</button>
												</div>
											)}
										</UserContext.Consumer>
									</>
								}
							</>
						}
						{!codeLoginOnly &&
							<>
								{this.props.allowOrganisationCodeLogin && this.props.signInButton &&
									<>
										<p>If you don't have an organisation code, sign in to your NICE account.</p>
										<p>
											<a className="btn" href={this.props.signInURL} title="Sign in to your NICE account">Sign in</a>
										</p>
									</>
								}
								{this.props.allowOrganisationCodeLogin && !this.props.signInButton &&
									<>If you don't have an organisation code, <a href={this.props.signInURL} title="Sign in to your NICE account">sign in to your NICE account.</a>&nbsp;&nbsp;</>
								}
							</>
						}
						{!this.props.allowOrganisationCodeLogin &&
							<>
								<p className={`${!isInCommentsPanel ? "display--inline" : ""}`}><a href={this.props.signInURL} title="Sign in to your NICE account">Sign in to your NICE account</a> {this.props.signInText ?? "to comment on this consultation"}.{" "}</p>
								{this.props.signInButton &&
									<p>
										<a className="btn" href={this.props.signInURL} title="Sign in to your NICE account">Sign in</a>
									</p>
								}
							</>
						}
						{!codeLoginOnly &&
							<p className={`${!isInCommentsPanel ? "display--inline" : ""}`}>
								<a href={this.props.registerURL} title="Register for a NICE account">
									Register
								</a>
								{" "}
								for a NICE account if you don't already have one.
							</p>
						}
					</div>
				</div>
			</div>
		);
	}
}

export default withRouter(LoginBanner);
LoginBanner.contextType = UserContext;
