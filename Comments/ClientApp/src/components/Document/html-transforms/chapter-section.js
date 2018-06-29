import React, { Fragment } from "react";
import { convertNodeToElement } from "react-html-parser";
import { nodeIsTypeText } from "./types";

export default function processChapterSection(node, incomingHtml, onNewCommentClick, sourceURI) {
	const elementType = node.attribs["data-heading-type"].toLowerCase();
	const elementName = node.children.filter(nodeIsTypeText)[0].data;
	const elementId = node.attribs.id;

	return (
		<Fragment key={0}>
			<button
				data-qa-sel="in-text-comment-button"
				className="document-comment-container__commentButton"
				tabIndex={0}
				onClick={e => {
					e.preventDefault();
					onNewCommentClick({
						sourceURI: sourceURI,
						commentText: "",
						commentOn: elementType,
						htmlElementID: elementType === "section" ? elementId : "",
						quote: elementName
					});
				}}
			>
				<span className="icon icon--comment" aria-hidden="true" />
				<span className="visually-hidden">Comment on {elementType}: {elementName}</span>
			</button>
			{convertNodeToElement(node)}
		</Fragment>
	);
}
