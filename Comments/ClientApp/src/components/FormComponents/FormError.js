import React from "react";

export const FormError = (props) => {
	return (
		<div className="panel">
			<h1>{props.title}</h1>
			<p>{props.secondary}</p>
		</div>
	);
};
