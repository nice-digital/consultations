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

		let preloadedData = {};

		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data;


			//console.log(`preloadedData: ${stringifyObject(preloadedData)}`);

			this.state.isAuthorised = preloadedData.isAuthorised;
			this.state.displayName = preloadedData.displayName;
			this.state.signInURL = preloadedData.signInURL;
			this.state.registerURL = preloadedData.registerURL;
		}



		//console.log('about to call user with url:' + this.props.match.url);
		//const preloaded = preload(
		//	this.props.staticContext,
		//	"user",
		//	[],
		//	{
		//		returnURL: this.props.match.url
		//	},
		//	preloadedData
		//);
		//console.log('in constructor');
		//console.log('preloaded' + stringifyObject(preloaded));
		//if (preloaded) {
		//	console.log('setting state in context');
		//	this.state = {
		//		isAuthorised: preloadedData.isAuthorised,
		//		displayName: preloadedData.displayName,
		//		signInURL: preloadedData.signInURL,
		//		registerURL: preloadedData.signInURL
		//	};
		//}

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
		this.loadUser();
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
