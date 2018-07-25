// @flow

import React from "react";

export default function processComment(node: Object) {
	const errorText = node.data;
	const errorArray = errorText.split(":");
	const errorStatus = errorArray[0]; // "[E] - Error"
	const errorInitial = errorStatus.match(/[A-Z]/)[0]; // "E"
	const errorMessage = errorArray[1].trim(); // "Inline graphic objects aren't supported by the converter"

	const toggleDetail = e => {
		const relatedDetails = e.target.nextElementSibling;
		relatedDetails.classList.toggle("active");
	};

	return (
		<div key={node} className={`ConversionError ConversionError--${errorInitial}`}
			 onClick={(e) => toggleDetail(e)}>
			<button
				className={`ConversionError__Button ConversionError__Button--${errorInitial}`}>
				{errorInitial}
			</button>
			<ul className={`ConversionError__details ConversionError__details--${errorInitial}`}>
				<li className="ml--d">{errorMessage}</li>
			</ul>
		</div>
	);
}
