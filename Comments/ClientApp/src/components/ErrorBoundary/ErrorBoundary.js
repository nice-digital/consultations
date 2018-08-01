// @flow

import React, { Component } from "react";
import Error from "./../Error/Error";

type PropsType = any;

type StateType = {
	hasError: boolean,
	error: {
		message: string,
		stack: string
	},
	info: {
		componentStack: string
	}
}

export class ErrorBoundary extends Component<PropsType, StateType> {
	constructor(props) {
		super(props);
		this.state = {
			hasError: false,
			error: {
				message: "",
				stack: ""
			},
			info: {
				componentStack: ""
			}
		};
	}

	componentDidCatch(error, info) {
		// Display fallback UI
		this.setState({
			hasError: true,
			error,
			info
		});
	}

	render() {
		if (this.state.hasError) {
			const error = {
				name: "Error",
				message: this.state.error.message,
				stack: this.state.error.stack
			};
			// You can render any custom fallback UI
			return <Error error={error}/>;
		}
		return this.props.children;
	}
}

export default ErrorBoundary
