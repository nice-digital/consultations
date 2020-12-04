// @flow

import React from "react";
//import Cookies from "js-cookie";
import { withRouter } from "react-router";

import { load } from "../data/loader";
import preload from "../data/pre-loader";

export const UserContext = React.createContext({
	updateContext: () => {}
});

type PropsType = {
	location: any,
	children: any,
	staticContext: any,
	match: any,
};

type StateType = {
	isAuthorised: boolean,
	isOrganisationCommenter: boolean,
	displayName: string,
	signInURL: string,
	registerURL: string,
	organisationName: string,
	initialDataLoaded: boolean,
};

export class UserProvider extends React.Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			isAuthorised: false,
			isOrganisationCommenter: false,
			displayName: "",
			signInURL: "",
			registerURL: "",
			updateContext: this.updateContext,
			organisationName: null,
			initialDataLoaded: false,
		};

		const isServerSideRender = (this.props.staticContext && this.props.staticContext.preload);
		const preloadSource = isServerSideRender ? this.props.staticContext.preload.data : window.__PRELOADED__; // TODO: extract this preloaded line out to (or near) the preload endpoint method.

		//const userSessionParameters = this.getUserSessionParameters();
		const consultationId = this.getConsultationId();

		console.log("cookie is:" + JSON.stringify(preloadSource?.cookies));


		var cookieString = preloadSource.cookies || ""; //this function only ever executes client-side. 

		var sessionCookieExistsForThisConsultation =  (cookieString.indexOf(`ConsultationSession-${consultationId}`) !== -1);
 

		let isOrganisationCommenter = false;
		let organisationName = null;
		let isAuthorised = preloadSource ? preloadSource.isAuthorised : false;

		console.log("before the if statement: " + sessionCookieExistsForThisConsultation);


		if (sessionCookieExistsForThisConsultation){
			//console.log("User session parameters 2: " + JSON.stringify(userSessionParameters));
			const preloadedUserSessionData = preload(
					this.props.staticContext,
					"checkorganisationusersession",
					[],
					{
						consultationId: consultationId,
					},
					preloadSource
				);
			
			if (preloadedUserSessionData){
				isOrganisationCommenter = preloadedUserSessionData.valid;
				isAuthorised = isAuthorised || preloadedUserSessionData.valid ; //you could be authorised by idam or by organisation session cookie.
				organisationName = preloadedUserSessionData.organisationName;
			}
		};

		if (preloadSource){
			this.state = {
				isAuthorised,
				isOrganisationCommenter: isOrganisationCommenter,
				displayName: preloadSource.displayName,
				signInURL: preloadSource.signInURL,
				registerURL: preloadSource.registerURL,
				updateContext: this.updateContext,
				organisationName: organisationName,
				initialDataLoaded: true,
			};
			if (this.props.staticContext) {
				this.props.staticContext.analyticsGlobals.isSignedIn = preloadSource.isAuthorised;
			}
		}
	}

	setStateForValidSessionCookie = (organisationName) => {
		this.setState({
			isAuthorised: true,
			isOrganisationCommenter: true,
			organisationName,
		});
	}


	loadUser = (returnURL) => {
		console.log("load user");
		this.checkSessionId()
		.then(data => {
			console.log("load user:" + JSON.stringify(data.validityAndOrganisationName));
			if (data.validityAndOrganisationName?.valid === true) {
				this.setStateForValidSessionCookie(data.validityAndOrganisationName.organisationName);
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

		//const userSessionParameters = this.getUserSessionParameters();
		const consultationId = this.getConsultationId();

		var cookieString = window.__PRELOADED__.cookies || ""; //this function only ever executes client-side. 

		var sessionCookieExistsForThisConsultation =  (cookieString.indexOf(`ConsultationSession-${consultationId}`) !== -1);

		if (!sessionCookieExistsForThisConsultation)
			return await {validityAndOrganisationName: {valid: false}};

		const validityAndOrganisationName = load(
			"checkorganisationusersession",
			undefined,
			[],
			{
				consultationId: consultationId,
			})
			.then(response => response.data)
			.catch(err => {
				console.log(JSON.stringify(err));
			});
		return {
			validityAndOrganisationName: await validityAndOrganisationName,
		};
	}

	// getUserSessionParameters = () => {
	// 	const consultationId = this.getConsultationId();
	// 	const sessionId = Cookies.get(`ConsultationSession-${consultationId}`);
	// 	console.log("Session id: " + sessionId);
	// 	return {consultationId, sessionId};
	// }


	//a child component can call this method to update the context in case a session cookie has been set.
	updateContext = () => {
		this.checkSessionId()
		.then(data => {
			if (data.validityAndOrganisationName.valid === true) {
				this.setStateForValidSessionCookie(data.validityAndOrganisationName.organisationName);
			}
		});
	}

	// fire when route changes

	componentDidMount() {
		console.log("CDM");
		if(!this.state.initialDataLoaded){

			console.log("CDM: initial data not loaded");
			this.loadUser(this.props.location.pathname); //this is currently only needed as the sign in url isn't right on SSR. TODO: fix SSR.
		}
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
