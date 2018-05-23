import React, { Fragment } from "react";
import ReactHtmlParser, { convertNodeToElement } from "react-html-parser";

function nodeIsChapter(node){
	return node.name === "a" && node.attribs && node.attribs["data-heading-type"] === "chapter";
}

function nodeIsSection(node) {
	return node.name === "a" && node.attribs && node.attribs["data-heading-type"] === "section";
}

function nodeIsNumberedParagraph(node) {
	return (node.name === "p") && (node.attribs) && (node.attribs["class"]) === "numbered-paragraph";
}

function nodeIsTypeText(node) {
	return node.type === "text";
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
		const isNumberedParagraph = nodeIsNumberedParagraph(node);
		if (
			isChapter || isSection || isNumberedParagraph
		) {

			let elementType = "";
			if (isChapter || isSection) {
				elementType = node.attribs["data-heading-type"].toLowerCase();
			} else if (isNumberedParagraph) {
				elementType = node.attribs["class"].toLowerCase();
			}

			const elementName = node.children.filter(nodeIsTypeText)[0].data;
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
								htmlElementID: elementType === "section" ? elementId : "",
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
