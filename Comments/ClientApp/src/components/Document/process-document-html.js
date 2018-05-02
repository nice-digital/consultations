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
			node.attribs["data-heading-type"] === "section"
		) {
			return addSectionButton(node);
		} else if (
			node.name === "a" &&
			node.attribs &&
			node.attribs["data-heading-type"] === "chapter"
		) {
			return addChapterButton(node);
		}

	}

	function addSectionButton(node) {
		const isTypeText = child => child.type === "text";

		const childrenThatHaveTypeOfText = node.children.filter(isTypeText);

		const sectionName = childrenThatHaveTypeOfText[0].data;

		const elementId = node.attribs.id;

		return (
			<Fragment key={0}>
				<button
					className="document-comment-container__commentButton"
					tabIndex={0}
					onClick={e => {
						e.preventDefault();
						onNewCommentClick({
							placeholder: `Comment on ${sectionName}`,
							sourceURI: sourceURI,
							commentText: "",
							commentOn: "Section",
							htmlElementID: elementId
						});
					}}
				>
					<span className="icon icon--comment" aria-hidden="true" />
					<span className="visually-hidden">
						Comment on section: {sectionName}
					</span>
				</button>
				{convertNodeToElement(node)}
			</Fragment>
		);
	}

	function addChapterButton(node) {
		const isTypeText = child => child.type === "text";

		const childrenThatHaveTypeOfText = node.children.filter(isTypeText);

		const chapterName = childrenThatHaveTypeOfText[0].data;

		const elementId = node.attribs.id;

		return (
			<Fragment key={0}>
				<button
					className="document-comment-container__commentButton"
					tabIndex={0}
					onClick={e => {
						e.preventDefault();
						onNewCommentClick({
							placeholder: `Comment on chapter: ${chapterName}`,
							sourceURI: sourceURI,
							commentText: "",
							commentOn: "Chapter",
							htmlElementID: elementId
						});
					}}
				>
					<span className="icon icon--comment" aria-hidden="true" />
					<span className="visually-hidden">
						Comment on chapter: {chapterName}
					</span>
				</button>
				{convertNodeToElement(node)}
			</Fragment>
		);
	}

	return ReactHtmlParser(incomingHtml, {
		transform: addButtons
	});
};
