// @flow
import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import { DebounceInput } from "react-debounce-input";
import queryString from "query-string";
import Cookies from "js-cookie";
import { appendQueryParameter, removeQueryParameter, removeQuerystring } from "../../helpers/utils";
import { load } from "../../data/loader";

type PropsType = {
	signInURL: string,
	registerURL: string,
	signInButton: boolean,
	signInText?: string,
	match: PropTypes.object.isRequired,
    location: PropTypes.object.isRequired,
	history: PropTypes.object.isRequired,
	key: string
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
				this.checkOrganisationCode(userEnteredCollationCode);
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

	gatherData = async () => {
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
					errorMessage: err, //err is a whole object, not just a message. todo: fix.
					showAuthorisationOrganisation: false,					
				});
			});
		return {
			organisationCode: await organisationCode,
		};
	}

	checkOrganisationCode = () => {
		this.gatherData()
			.then(data => {
				if (data.organisationCode != null) {
					this.setState({
						hasError: false,
						errorMessage: "",
						showAuthorisationOrganisation: true,
						authorisationOrganisationFound: data.organisationCode,
					});
				} else{
					console.log("in the else, data is:");
					console.log(JSON.stringify(data));
					this.setState({
						hasError: true,
						errorMessage: "",
						showAuthorisationOrganisation: false,
						authorisationOrganisationFound: null,
					});
				}
			})
			.catch(err => {
				throw new Error("gatherData in checkOrganisationCode failed " + err);
			});		
	}

	handleConfirmClick = () => {
		console.log("confirm was clicked. set cookie here.");		
		
		var session = "todo: a guid"; //todo: call the server here to get a session id.

		Cookies.set("OrganisationUser", session);
	}


	render(){
		const {userEnteredCollationCode} = this.state;
		const { match, location, history } = this.props

		return (
			<div className="panel panel--inverse mt--0 mb--0 sign-in-banner"
					 data-qa-sel="sign-in-banner">
				<div className="container">
					<div className="grid">
						<div data-g="12">
							<div className="LoginBanner">
								<p>If you would like to comment on this consultation as part of an organisation, please enter your organisation code here:</p>
								<label>
									Organisation code 
									<DebounceInput
										minLength={6}
										debounceTimeout={400}
										type="text"
										onChange={e => this.handleOrganisationCodeChange(e.target.value)}
										className="form__input form__input--text"
										data-qa-sel="OrganisationCodeLogin"
										id="collationCode"
										value={userEnteredCollationCode}
									/>
								</label>
								<br/><br/>
								{this.state.showAuthorisationOrganisation && 
									<Fragment>
										<label>Confirm organisation name
											<strong>{this.state.authorisationOrganisationFound.organisationName}</strong>
										</label>
										<br/>
										<button className="btn btn--cta" onClick={() => this.handleConfirmClick()}  title={"Confirm your organisation is " + this.state.authorisationOrganisationFound.organisationName}>Confirm</button>
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