// @flow

import React from "react";
import { load } from "../data/loader";
import { withRouter } from "react-router";
//import preload from "../data/pre-loader";
//import stringifyObject from "stringify-object";

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
			registerURL: ""
		};

		const isServerSideRender = (this.props.staticContext && this.props.staticContext.preload);		
		const preloadSource = isServerSideRender ? this.props.staticContext.preload.data : window.__PRELOADED__; //TODO: extract this preloaded line out to (or near) the preload endpoint method.

		if (preloadSource){
			this.state = {
				isAuthorised: preloadSource.isAuthorised,
				displayName: preloadSource.displayName,
				signInURL: preloadSource.signInURL,
				registerURL: preloadSource.registerURL
			};
		} 
	}

	loadUser = () => {
		load("user", undefined, [], { returnURL: this.props.location.pathname })
			.then(
				res => {
					this.setState({
						isAuthorised: res.data.isAuthorised,
						displayName: res.data.displayName,
						signInURL: res.data.signInURL,
						registerURL: res.data.registerURL
					});
				}
			);
	}

	componentDidUpdate(prevProps: PropsType) {
		const oldRoute = prevProps.location.pathname;
		const newRoute = this.props.location.pathname;
		if (oldRoute !== newRoute) {
			this.loadUser();
		}
	}

	componentDidMount() {
		if (!this.state.isAuthorised) {  //this shouldn't be needed..
			this.loadUser();
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
