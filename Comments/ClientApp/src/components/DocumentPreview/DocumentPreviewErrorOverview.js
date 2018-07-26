import React from "react";
import { generateErrorList } from "../../document-processing/process-preview-html";

export const DocumentPreviewErrorOverview = (props) => {
	const errorContent = generateErrorList(props.content);
	const qty = errorContent.length;
	if (qty) {
		return (
			<div className="panel panel--inverse">
				<p><b>There {qty > 1 ? `are ${qty} types of error` : "is 1 type of error"} in this chapter.</b></p>
				<ul>
					{errorContent}
				</ul>
			</div>
		);
	}
	return null;
};
