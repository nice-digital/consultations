import React, { Fragment } from "react";
import ReactHtmlParser, { convertNodeToElement } from "react-html-parser";

// onNewCommentClick passed through from <Document />
export const processDocumentHtml = (
	incomingHtml,
	onNewCommentClick,
	sourceURI
) => {
	function addButtons(node) {
		if (
			node.name === "a" &&
			node.attribs &&
			(node.attribs["data-heading-type"] === "chapter" || node.attribs["data-heading-type"] === "section")
		) {
			const isTypeText = child => child.type === "text";
			const elementType = node.attribs["data-heading-type"].toLowerCase();
			const elementName = node.children.filter(isTypeText)[0].data;
			const elementId = node.attribs.id;

			return (
				<Fragment key={0}>
					<button
						className="document-comment-container__commentButton"
						tabIndex={0}
						onClick={e => {
							e.preventDefault();
							onNewCommentClick({
								sourceURI: sourceURI,
								commentText: "",
								commentOn: elementType,
								htmlElementID: elementType === "section" ? elementType : "",
								quote: elementName
							});
						}}
					>
						<span className="icon icon--comment" aria-hidden="true" />
						<span className="visually-hidden">
							Comment on {elementType}: {elementName}
						</span>
					</button>
					{convertNodeToElement(node)}
				</Fragment>
			);
		}
	}

	return ReactHtmlParser(incomingHtml, {
		transform: addButtons
	});
};
