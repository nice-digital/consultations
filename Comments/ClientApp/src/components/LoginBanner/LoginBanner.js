// @flow
import React, { Component } from "react";
import { withRouter } from "react-router-dom";
import { DebounceInput } from "react-debounce-input";
import queryString from "query-string";
//import { withHistory } from "../HistoryContext/HistoryContext";
import { appendQueryParameter, removeQueryParameter, removeQuerystring } from "../../helpers/utils";

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

type StateType = {
	organisationCode: string
}

export class LoginBanner extends Component<PropsType, StateType> {

	constructor(props: PropsType) {
		super(props);

		this.state = {
			organisationCode: "",
		};
	}

	componentDidMount() {
		const organisationCode = queryString.parse(this.props.location.search).organisationCode;
		console.log(this.props.location.search);

		if (organisationCode){
			this.setState({
				organisationCode,
			}, () => {
				this.checkOrganisationCode(organisationCode);
			});
		}
	}

	removeOrganisationCode = () => {
		this.handleOrganisationCodeChange("");
	};

	componentDidUpdate(prevProps){
		if (prevProps.organisationCode !== "" && this.props.organisationCode === ""){
			this.removeOrganisationCode();
		}
	}

	getHref = (organisationCode) => {
		const pathWithoutQuerystring = removeQuerystring(this.props.path);
		const querystringWithRemovedKeyword = removeQueryParameter(this.props.search, "OrganisationCode");

		if (organisationCode.length <= 0){
			return pathWithoutQuerystring + querystringWithRemovedKeyword;
		}

		const querystringWithKeywordAdded = appendQueryParameter(querystringWithRemovedKeyword, "OrganisationCode", organisationCode);
		return pathWithoutQuerystring + querystringWithKeywordAdded;
	};

	handleOrganisationCodeChange = (organisationCode) => {
		this.setState({
			organisationCode,
		}, () => {
			//this.props.history.push(this.getHref(organisationCode)); //TODO: fix the history!!!
			this.checkOrganisationCode(organisationCode);
		});
	};

	checkOrganisationCode = () => {
		console.log("check organisation code" + this.state.organisationCode);
	}


	render(){
		const {organisationCode} = this.state;
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
										id="organisationCode"
										value={organisationCode}
									/>
								</label>
								<br/><br/>
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

export default withRouter(LoginBanner); //withHistory(LoginBanner);