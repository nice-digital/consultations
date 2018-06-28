import React, { Fragment } from "react";
import ReactHtmlParser, { convertNodeToElement } from "react-html-parser";

function nodeIsChapter(node){
	return node.name === "a" && node.attribs && node.attribs["data-heading-type"] === "chapter";
}

function nodeIsSection(node) {
	return node.name === "a" && node.attribs && node.attribs["data-heading-type"] === "section";
}

function nodeIsSubsection(node) {
	return node.name === "p" && node.attribs && node.attribs["data-heading-type"] === "numbered-paragraph";
}

function nodeIsTypeText(node) {
	return node.type === "text";
}

function nodeIsSpanTag(node) {
	return node.name === "span";
}

// onNewCommentClick passed through from <Document />
export const processDocumentHtml = (
	incomingHtml,
	onNewCommentClick,
	sourceURI
) => {
	function addButtons(node) {
		const isChapter = nodeIsChapter(node);
		const isSection = nodeIsSection(node);
		const isSubsection = nodeIsSubsection(node);
		if (
			isChapter || isSection || isSubsection
		) {
			const elementType = node.attribs["data-heading-type"].toLowerCase();

			let elementName = node.children.filter(nodeIsTypeText)[0].data;

			if (isSubsection) {
				elementName = node.children.filter(nodeIsSpanTag)[0].children.filter(nodeIsTypeText)[0].data;
			}

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
								htmlElementID: (elementType === "section" || elementType === "numbered-paragraph") ? elementId : "",
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
