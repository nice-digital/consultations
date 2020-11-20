// @flow
import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import { DebounceInput } from "react-debounce-input";
import queryString from "query-string";
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
			"checkcollationcode",
			undefined,
			[],
			{
				collationCode: this.state.userEnteredCollationCode,
				consultationId: this.props.match.params.consultationId, 
			})
			.then(response => response.data)
			.catch(err => {
				this.setState({
					error: {
						hasError: true,
						errorMessage: err,
						showAuthorisationOrganisation: false,
					},
				});
			});
		return {
			organisationCode: await organisationCode,
		};
	}

	checkOrganisationCode = () => {
		this.gatherData()
			.then(data => {
				if (data.organisationCode !== null) {
					this.setState({
						hasError: false,
						errorMessage: "",
						showAuthorisationOrganisation: true,
						authorisationOrganisationFound: data.organisationCode,
					});
				} 
			})
			.catch(err => {
				throw new Error("gatherData in checkOrganisationCode failed " + err);
			});		
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
										<a className="btn btn--inverse" href={this.props.signInURL} title={"Confirm your organisation is " + this.state.authorisationOrganisationFound.organisationName}>Confirm</a>
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