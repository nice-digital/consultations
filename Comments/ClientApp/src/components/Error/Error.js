// @flow

import React from "react";

type ErrorType = {
	name: string,
	message: string,
	stack: string
};

type PropsType = {
	error: ErrorType
};

export const Error = (props: PropsType) => {
	const { name, message, stack } = props.error;

	return (
		<main role="main">
			<div className="container">
				<div className="panel page-header">
					<h1 className="heading mt--c">Something's gone wrong</h1>
					<p className="lead">We'll look into it right away. Please try again in a few minutes. And if it's still not fixed, <a href="/get-involved/contact-us">contact us</a>.</p>
					<p><a href="~/guidance/inconsultation">Back to consultations</a></p>
					<div className="hide">
						{message}
						{stack}
					</div>
				</div>
			</div>
		</main>
	);
}
	

export default Error;
