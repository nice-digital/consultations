// @flow

import React from "react";
import { load } from "../data/loader";
import { withRouter } from "react-router";

export const UserContext = React.createContext();

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
		};

		const isServerSideRender = (this.props.staticContext && this.props.staticContext.preload);
		const preloadSource = isServerSideRender ? this.props.staticContext.preload.data : window.__PRELOADED__; // TODO: extract this preloaded line out to (or near) the preload endpoint method.

		if (preloadSource){
			this.state = {
				isAuthorised: preloadSource.isAuthorised,
				displayName: preloadSource.displayName,
				signInURL: preloadSource.signInURL,
				registerURL: preloadSource.registerURL,
			};
			if (this.props.staticContext) {
				this.props.staticContext.analyticsGlobals.isSignedIn = preloadSource.isAuthorised;
			}
		}
	}

	loadUser = (returnURL) => {
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
	};

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
