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
	return (
		//YSOD error page for developers
		<div className="container">
			<div className="alert">
				<h2 className="page-header__heading mt--0">
					<span className="icon icon--warning" aria-hidden="true"/> {props.error.name}
				</h2>
				<p className="page-header__lead">{props.error.message}</p>
				<p>{props.error.stack}</p>
			</div>
		</div>
	);
};

export default Error;
