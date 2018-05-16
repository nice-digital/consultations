import React from "react";

export const UserContext = React.createContext();

class UserProvider extends React.Component {
	constructor(props) {
		super(props);

		// todo: get authorisation here?
		this.state = {
			isAuthorised: false,
			userName: "Bob",
			signInLink: "",
			registerLink: ""
		};
	}

	render() {
		return (
			<UserContext.Provider value={this.state}>
				{this.props.children}
			</UserContext.Provider>
		);
	}
}

export default UserProvider;
