// @flow

import React, { Component } from "react";
import { load } from "../../data/loader";

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
				stack: "",
			},
			info: {
				componentStack: "",
			},
		};
	}

	componentDidCatch(error: any, info: any) {
		// Display fallback UI
		this.setState({
			hasError: true,
			error,
			info,
		},
		() => {
			this.logError(error);
		});		
	}

	logError = async (error) => {
		const message = `Client-side front-end error logging. running as ${process.env.NODE_ENV} error message: ${error.message} stack: ${error.stack}`;
		load("logging", undefined, [], {logLevel:"Error"}, "POST", {message}, true)
			.then(response => response.data)
			.catch(err => {
				console.error(err);
			});
	};

	render() {
		if (this.state.hasError) {
			const { message, stack } = this.state.error;
			return (
				<main role="main">
					<div className="container">
						<div className="panel page-header">
							<h1 className="heading mt--c">Something's gone wrong</h1>
							<p className="lead">We'll look into it right away. Please try again in a few minutes. And if it's still not fixed, <a href="/get-involved/contact-us">contact us</a>.</p>
							<p><a href="/guidance/inconsultation">Back to consultations</a></p>
							<div className="hide">
								{message}
								{stack}
							</div>
						</div>
					</div>
				</main>
			);
		}
		return this.props.children;
	}
}

export default ErrorBoundary;
