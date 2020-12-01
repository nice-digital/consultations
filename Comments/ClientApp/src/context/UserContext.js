// @flow

import React from "react";
import Cookies from "js-cookie";

import { load } from "../data/loader";
import { withRouter } from "react-router";

export const UserContext = React.createContext({
	updateContext: () => {}
});

type PropsType = {
	location: any,
	children: any,
	staticContext: any,
	match: any
};

type StateType = {
	isAuthorised: boolean,
	displayName: string,
	signInURL: string,
	registerURL: string
};

export class UserProvider extends React.Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			isAuthorised: false,
			displayName: "",
			signInURL: "",
			registerURL: "",
			updateContext: this.updateContext,
		};

		const isServerSideRender = (this.props.staticContext && this.props.staticContext.preload);
		const preloadSource = isServerSideRender ? this.props.staticContext.preload.data : window.__PRELOADED__; // TODO: extract this preloaded line out to (or near) the preload endpoint method.

		if (preloadSource){
			this.state = {
				isAuthorised: preloadSource.isAuthorised,
				displayName: preloadSource.displayName,
				signInURL: preloadSource.signInURL,
				registerURL: preloadSource.registerURL,
				updateContext: this.updateContext,
			};
			if (this.props.staticContext) {
				this.props.staticContext.analyticsGlobals.isSignedIn = preloadSource.isAuthorised;
			}
		}
	}

	setStateForValidSessionCookie = () => {
		this.setState({
			isAuthorised: true,
		});
	}


	loadUser = (returnURL) => {
		this.checkSessionId()
		.then(data => {
			if (data.validityAndOrganisationName.valid === true) {
				this.setStateForValidSessionCookie();
			}
			else{
				load("user", undefined, [], { returnURL, cachebust: new Date().getTime() })
				.then(
					res => {
						const signInURL = res.data.signInURL;
						this.setState({
							isAuthorised: res.data.isAuthorised,
							displayName: res.data.displayName,
							signInURL: signInURL,
							registerURL: res.data.registerURL,
						});
						//update signin links in global nav here. because SSR isn't rendering them right on the server.
						var signInLinks = document.getElementById("global-nav-header").querySelectorAll("a[href*='account/login']");
						for (var i=0; i < signInLinks.length; i++) {
							signInLinks[i].setAttribute("href", signInURL);
						}
					}
				);

			}
		});
	}

	//unfortunately the context is above the routes, so this.props.match is always null, so we can't pull the consultation id out of there. hence we're falling back to regex.
	getConsultationId = () =>{
		const regex = /^\/(\d+)\/\d+\/[a-z-A-Z0-9]+/;
		const pathname = this.props.location.pathname;

		if (!pathname)
			return;

		const matches = regex.exec(pathname);
		if (!matches || matches.length !== 2)
			return;

		return matches[1];
	}

	checkSessionId = async () => {

		const consultationId = this.getConsultationId();
		if (!consultationId)
			return await {validityAndOrganisationName: {valid: false}};

		const sessionId = Cookies.get(`ConsultationSession-${consultationId}`);
		if (!sessionId)
			return await {validityAndOrganisationName: {valid: false}};

		const validityAndOrganisationName = load(
			"checkorganisationusersession",
			undefined,
			[],
			{
				consultationId: consultationId,
				sessionId: sessionId,
			})
			.then(response => response.data)
			.catch(err => {
				console.log(JSON.stringify(err));
			});
		return {
			validityAndOrganisationName: await validityAndOrganisationName,
		};
	}


	//a child component can call this method to update the context in case a session cookie has been set.
	updateContext = () => {
		this.checkSessionId()
		.then(data => {
			if (data.validityAndOrganisationName.valid === true) {
				this.setStateForValidSessionCookie();
			}
		});
	}

	// fire when route changes

	componentDidMount() {
		this.loadUser(this.props.location.pathname); //this is currently only needed as the sign in url isn't right on SSR. TODO: fix SSR.
	}

	render() {
		return (
			<UserContext.Provider value={this.state}>
				{this.props.children}
			</UserContext.Provider>
		);
	}
}

export default withRouter(UserProvider);
