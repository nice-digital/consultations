import React from "react";

export const UserContext = React.createContext();

class UserProvider extends React.Component {
	constructor(props) {
		super(props);

		this.state = {
			isAuthorised: true,
			userName: "Bob"
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
