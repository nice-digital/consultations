// @flow
import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import { DebounceInput } from "react-debounce-input";
import queryString from "query-string";
import Cookies from "js-cookie";
import { UserContext } from "../../context/UserContext";
import { load } from "../../data/loader";

type PropsType = {
	signInURL: string,
	registerURL: string,
	signInButton: boolean,
	signInText?: string,
	match: PropTypes.object.isRequired,
    location: PropTypes.object.isRequired,
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
	showOrganisationLoginSection: bool,
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
			showOrganisationLoginSection: true,
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
			this.checkOrganisationCode(userEnteredCollationCode);
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
		const sessionId = load(
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
			sessionId: await sessionId,
		};
	}

	CreateOrganisationUserSession = async () => {
		const session = this.gatherDataForCreateOrganisationUserSession()
			.then(data => {
				if (data.session != null) { //todo: the shape of this has changed. it's now probably an object with sessionId in it plus a datetime.
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
			Cookies.set(`ConsultationSession-${consultationId}`, data.sessionId); //TODO: add to cookie policy + expiration time of end date + 28 days !!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			//now, set state to show logged in. 
			this.setState({
				showOrganisationLoginSection: false,
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

		return (
			<div className="panel panel--inverse mt--0 mb--0 sign-in-banner"
					 data-qa-sel="sign-in-banner">
				<div className="container">
					<div className="grid">
						<div data-g="12">
							<div className="LoginBanner">
								{this.state.showOrganisationLoginSection &&
									<Fragment>
										<p>If you would like to comment on this consultation as part of an organisation, please enter your organisation code here:</p>
										<label>
											Organisation code<br/>
											{this.state.hasError && 
												<div>{this.state.errorMessage}</div>
											}
											<DebounceInput
												minLength={6}
												debounceTimeout={400}
												type="text"
												onChange={e => this.handleOrganisationCodeChange(e.target.value)}
												className="form__input form__input--text limitWidth"
												data-qa-sel="OrganisationCodeLogin"
												id="collationCode"
												value={this.state.userEnteredCollationCode}
											/>
										</label>
										<br/><br/>
										{this.state.showAuthorisationOrganisation && 
											<Fragment>
												<label>Confirm organisation name<br/>
													<strong>{this.state.authorisationOrganisationFound.organisationName}</strong>
												</label>
												<br/>												
												<UserContext.Consumer>
													{({ contextValue: ContextType, updateContext }) => (
														<button className="btn btn--cta" onClick={() => this.handleConfirmClick(updateContext)}  title={"Confirm your organisation is " + this.state.authorisationOrganisationFound.organisationName}>Confirm</button>
													)}
												</UserContext.Consumer>
												<br/>
											</Fragment>
										}	
									</Fragment>							
								}
								<a href={this.props.signInURL} title="Sign in to your NICE account">
									Sign in to your NICE account</a> {this.props.signInText || "to comment on this consultation"}.{" "}
								<br/>
								Don't have an account?{" "}
								<a href={this.props.registerURL} title="Register for a NICE account">
									Register
								</a>
							</div>
							{this.props.signInButton &&
								<p>
									<a className="btn btn--inverse" href={this.props.signInURL} title="Sign in to your NICE account">Sign in</a>
								</p>
							}
						</div>
					</div>
				</div>
			</div>
		);
	}
}

export default withRouter(LoginBanner); 