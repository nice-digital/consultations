// @flow

import React from "react";
import uuid from "uuid/v4";

export default function processComment(node: Object) {
	const uniqueClass = `ConversionClass-${uuid()}`;
	const errorText = node.data;
	const errorArray = errorText.split(":");
	const errorStatus = errorArray[0]; // "[E] - Error"
	const errorInitial = errorStatus.match(/[A-Z]/)[0]; // "E"
	const errorMessage = errorArray[1].trim(); // "Inline graphic objects aren't supported by the converter"

	// need to count the number of successive error messages?

	const toggleDetail = (uniqueId: string) => {
		const detail: any = document.querySelector(`ul.${uniqueId}`);
		detail.classList.toggle("visible");
	};

	return (
		<div key={uniqueClass} role="button" className={`ConversionError ConversionError--${errorInitial}`}
			 onClick={() => toggleDetail(uniqueClass)}>
			<button
				className={`visible ConversionError__Button ConversionError__Button--${errorInitial} ${uniqueClass}`}>
				1
			</button>
			<ul
				className={`ConversionError__details ConversionError__details--${errorInitial} ${uniqueClass}`}
			>
				<li className="ml--d">{errorMessage}</li>
			</ul>
		</div>
	);
}
