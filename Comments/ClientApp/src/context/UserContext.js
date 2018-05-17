import React from "react";
import {load} from "../data/loader";
import { withRouter } from "react-router";

export const UserContext = React.createContext();

class UserProvider extends React.Component {
	constructor(props) {
		super(props);

		// todo: get authorisation here?
		this.state = {
			isAuthorised: false,
			displayName: "Bob Bobbson",
			signInURL: "",
			registerURL: ""
		};
	}

	getAuth() {
		load("comments", undefined, [], { sourceURI: this.props.location.pathname }).then(
			res => {
				this.setState({
					isAuthorised: res.data.isAuthorised,
					signInURL: res.data.signInURL
				});
			}
		);
	}

	componentDidUpdate(prevProps){
		const oldRoute = prevProps.location.pathname;
		const newRoute = this.props.location.pathname;
		if (oldRoute !== newRoute) {
			this.getAuth();
		}
	}

	componentDidMount() {
		this.getAuth();
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
