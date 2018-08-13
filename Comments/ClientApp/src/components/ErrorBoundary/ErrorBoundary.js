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
				message: this.state.error.message,
				stack: this.state.error.stack
			};

			return (
				<main role="main">
					<div className="container">
						<div className="panel page-header">
							<h1 className="heading mt--c">Something's gone wrong</h1>
							<p className="lead">We'll look into it right away. Please try again in a few minutes. And if it's still not fixed, <a href="/get-involved/contact-us">contact us</a>.</p>
							<p><a href="/guidance/inconsultation">Back to consultations</a></p>
							<div className="hide">
								{this.state.error.message}
								{this.state.error.stack}
							</div>
						</div>
					</div>
				</main>
			);
		}
		return this.props.children;
	}
}

export default ErrorBoundary
