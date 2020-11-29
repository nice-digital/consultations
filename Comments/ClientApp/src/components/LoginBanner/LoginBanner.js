// @flow
import React, { Component, Fragment } from "react";
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
}

type OrganisationCode = {
	organisationAuthorisationId: number,
	organisationId: number,
	organisationName: string,
	collationCode: string,
}

type StateType = {
	organisationCode: string,
	hasError: bool,
	errorMessage: string,
	showAuthorisationOrganisation: bool,
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
				this.setState({hasError: false});
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

	CreateOrganisationUserSession = async () => {
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
		this.CreateOrganisationUserSession().then(data => {
			console.log("data is:" + JSON.stringify(data));

			var expirationDate = new Date(data.session.expirationDateTicks);

			Cookies.set(`ConsultationSession-${consultationId}`, data.session.sessionId, {expires: expirationDate}); //TODO: add to cookie policy

			//now, set state to show logged in. 
			this.setState({
				showAuthorisationOrganisation: false,
			})
			updateContextFunction();
		})
		.catch(err => {
			this.setState({
				hasError: true,
				errorMessage: "Unable to confirm",
			})
		});			
	}


	render(){
		const limitWidthOfButton = !this.props.signInButton; //the sign-in button isn't shown when we're trying to save space.

		return (
			<div className="panel panel-white mt--0 mb--0 sign-in-banner" data-qa-sel="sign-in-banner">
				<div className="container">
					<div className="LoginBanner">
						{this.props.allowOrganisationCodeLogin &&
							<Fragment>
								<p>To comment as part of an organisation, please enter your organisation code:</p>
								<div className={this.state.hasError ? "input input--error" : "input"}>
									<DebounceInput
										minLength={8}
										debounceTimeout={400}
										type="text"
										onChange={e => this.handleOrganisationCodeChange(e.target.value)}
										className={limitWidthOfButton ? "limitWidth input__input" : "input__input"}
										data-qa-sel="OrganisationCodeLogin"
										value={this.state.userEnteredCollationCode}
										element={Input}
										error={this.state.hasError}
										errorMessage={this.state.errorMessage}
										label="Organisation code"
									/>											
								</div>
								{this.state.showAuthorisationOrganisation && 
									<Fragment>
										<p>Confirm organisation name
											<p><strong>{this.state.authorisationOrganisationFound.organisationName}</strong></p>
										</p>							
										<UserContext.Consumer>
											{({ contextValue: ContextType, updateContext }) => (
												<button className="btn btn--cta" onClick={() => this.handleConfirmClick(updateContext)}  title={"Confirm your organisation is " + this.state.authorisationOrganisationFound.organisationName}>Confirm</button>
											)}
										</UserContext.Consumer>
									</Fragment>
								}
							</Fragment>
						}
						{this.props.allowOrganisationCodeLogin && this.props.signInButton && 
							<Fragment>
								<p>If you don't have an organisation code, sign in to your NICE account.</p>
								<p>
									<a className="btn" href={this.props.signInURL} title="Sign in to your NICE account">Sign in</a>
								</p>
							</Fragment>
						}
						{this.props.allowOrganisationCodeLogin && !this.props.signInButton && 
							<p>If you don't have an organisation code, <a href={this.props.signInURL} title="Sign in to your NICE account">sign in to your NICE account.</a></p>
						}
						{!this.props.allowOrganisationCodeLogin && 
							<Fragment>
								<a href={this.props.signInURL} title="Sign in to your NICE account">Sign in to your NICE account</a> {this.props.signInText || "to comment on this consultation"}.{" "}								
								{this.props.signInButton && 
									<p>
										<a className="btn" href={this.props.signInURL} title="Sign in to your NICE account">Sign in</a>
									</p>
								}
							</Fragment>
						}
						Don't have an account?{" "}
						<a href={this.props.registerURL} title="Register for a NICE account">
							Register
						</a>
					</div>
				</div>
			</div>
		);
	}
}

export default withRouter(LoginBanner); 