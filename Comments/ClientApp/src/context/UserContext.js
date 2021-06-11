// @flow

import React from "react";
import Cookies from "js-cookie";
import { withRouter } from "react-router";

import { load } from "../data/loader";
import preload from "../data/pre-loader";

export const UserContext = React.createContext({
	updateContext: () => {},
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
	isLead: boolean,
	displayName: string,
	signInURL: string,
	registerURL: string,
	organisationName: string,
	initialDataLoaded: boolean,
	organisationalCommentingFeature: boolean,
};

export class UserProvider extends React.Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			isAuthorised: false,
			isOrganisationCommenter: false,
			isLead: false,
			displayName: "",
			signInURL: "",
			registerURL: "",
			updateContext: this.updateContext,
			organisationName: null,
			initialDataLoaded: false,
			organisationalCommentingFeature: false,
		};

		const isServerSideRender = (this.props.staticContext && this.props.staticContext.preload);
		const preloadSource = isServerSideRender ? this.props.staticContext.preload.data : window.__PRELOADED__; // TODO: extract this preloaded line out to (or near) the preload endpoint method.

		const userSessionParameters = this.getUserSessionParameters(preloadSource);

		let isOrganisationCommenter = false;
		let organisationName = null;
		let isAuthorised = preloadSource ? preloadSource.isAuthorised : false;

		if (userSessionParameters.sessionCookieExistsForThisConsultation){
			const preloadedUserSessionData = preload(
				this.props.staticContext,
				"checkorganisationusersession",
				[],
				{
					consultationId: userSessionParameters.consultationId,
				},
				preloadSource,
			);

			if (preloadedUserSessionData){
				isOrganisationCommenter = preloadedUserSessionData.valid;
				isAuthorised = isAuthorised || preloadedUserSessionData.valid ; //you could be authorised by idam or by organisation session cookie.
				organisationName = preloadedUserSessionData.organisationName;
			}
		}

		if (preloadSource){
			this.state = {
				isAuthorised,
				isOrganisationCommenter: isOrganisationCommenter,
				isLead: preloadSource.isLead,
				displayName: preloadSource.displayName,
				signInURL: preloadSource.signInURL,
				registerURL: preloadSource.registerURL,
				updateContext: this.updateContext,
				organisationName: organisationName,
				initialDataLoaded: true,
				organisationalCommentingFeature: preloadSource.organisationalCommentingFeature,
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
		load("user", undefined, [], { returnURL, cachebust: new Date().getTime() })
			.then(
				res => {
					const signInURL = res.data.signInURL;
					this.setState({
						isAuthorised: res.data.isAuthenticatedByAccounts,
						displayName: res.data.displayName,
						signInURL: signInURL,
						registerURL: res.data.registerURL,
						isLead: (res.data.organisationsAssignedAsLead && res.data.organisationsAssignedAsLead.length > 0),
					});
					//update signin links in global nav here. because SSR isn't rendering them right on the server.
					var signInLinks = document.getElementById("global-nav-header").querySelectorAll("a[href*='account/login']");
					for (var i=0; i < signInLinks.length; i++) {
						signInLinks[i].setAttribute("href", signInURL);
					}
				},
			)
			.then(
				this.checkSessionId()
					.then(data => {
						if (data.validityAndOrganisationName?.valid === true) {
							this.setStateForValidSessionCookie(data.validityAndOrganisationName.organisationName);
						}
					}),
			);
	}

	//unfortunately the context is above the routes, so this.props.match is always null, so we can't pull the consultation id out of there. hence we're falling back to regex.
	getConsultationId = () =>{
		const regex = /^\/(\d+)\/(review|submitted|\d+\/[a-z-A-Z0-9]+)/;
		const pathname = this.props.location.pathname;

		if (!pathname)
			return;

		const matches = regex.exec(pathname);
		if (!matches || matches.length !== 3)
			return;

		return matches[1];
	}

	checkSessionId = async () => {
		const userSessionParameters = this.getUserSessionParameters(window.__PRELOADED__);

		if (!userSessionParameters.sessionCookieExistsForThisConsultation)
			return await {
				validityAndOrganisationName: {valid: false},
				userSessionParameters: userSessionParameters,
			};

		const validityAndOrganisationName = load(
			"checkorganisationusersession",
			undefined,
			[],
			{
				consultationId: userSessionParameters.consultationId,
			})
			.then(response => response.data)
			.catch(err => {
				console.log(JSON.stringify(err));
				return {valid: false};
			});
		return {
			validityAndOrganisationName: await validityAndOrganisationName,
			userSessionParameters: userSessionParameters,
		};
	}

	getUserSessionParameters = (preloadSource) => {
		const consultationId = this.getConsultationId();
		const cookieName = `ConsultationSession-${consultationId}`;
		const cookieString = preloadSource?.cookies || "";
		const sessionCookieExistsForThisConsultation =  (Cookies.get(cookieName) || cookieString.indexOf(cookieName) !== -1);
		return {consultationId, sessionCookieExistsForThisConsultation};
	}

	//a child component can call this method to update the context in case a session cookie has been set.
	updateContext = () => {
		this.checkSessionId()
			.then(data => {
				if (data.validityAndOrganisationName.valid === true) {
					this.setStateForValidSessionCookie(data.validityAndOrganisationName.organisationName);
				}
			});
	}

	loadContext = () => {
		this.loadUser(this.props.location.pathname);
	}

	componentDidMount() {
		this.loadContext(); //this is needed here as the sign in url isn't right on SSR. TODO: fix SSR.
	}

	componentDidUpdate(prevProps) {
		if (this.props.location !== prevProps.location) { //fired when the route changes, as you might be authenticated for some routes, but not others (e.g. org commenting cookie)
			this.loadContext();
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
