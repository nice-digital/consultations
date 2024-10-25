import React, { Fragment } from "react";
import { convertNodeToElement } from "react-html-parser";
import {
	nodeIsTypeText,
	nodeIsSubsection,
	nodeIsSpanTag,
	nodeIsInternalLink,
	nodeIsParagraphNumber,
	nodeIsNewArticleHeading,
} from "./types";
import processInternalLink from "./internal-link";
import { tagManager } from "../../helpers/tag-manager";

export const processChapterSectionSubsection = (node, onNewCommentClick, sourceURI, allowComments, sectionNumber) => {

	let commentOn = node.attribs["data-heading-type"].toLowerCase();
	let quote =  node.children.filter(nodeIsTypeText)[0].data;

	if (nodeIsSubsection(node)) {
		quote = node.children.filter(nodeIsSpanTag);
		quote = quote.length ? quote : node.children.filter(nodeIsNewArticleHeading);
		quote = quote[0].children.filter(nodeIsTypeText)[0].data;
		commentOn = "subsection";
	}

	const htmlElementID = (commentOn === "section" || commentOn === "subsection") ? node.attribs.id : "";

	//this gets rid of the tooltip added to the anchor in indev. it's just not really needed on text.
	if (typeof(node.attribs) !== "undefined"){
		node.attribs.title = "";
	}

	const CommentButton = (
		<button
			data-gtm="button"
			data-gtm-category="Consultation comments page"
			data-gtm-action="Clicked"
			data-gtm-label={`Comment on ${commentOn || "chapter"}`}
			data-qa-sel="in-text-comment-button"
			data-sectionnumber={sectionNumber}
			title={`Comment on ${commentOn || "chapter"}`}
			className="document-comment-container__commentButton"
			tabIndex={0}
			onClick={e => {
				e.preventDefault();
				tagManager({
					event: "generic",
					category: "Consultation comments page",
					action: "Clicked",
					label: `Comment on ${commentOn || "chapter"}`,
				});
				onNewCommentClick(e, {
					sourceURI,
					commentText: "",
					commentOn,
					htmlElementID,
					quote,
					sectionNumber,
				});
			}}
		>
			<span className="icon icon--comment" aria-hidden="true" />
			<span className="visually-hidden">Comment on {commentOn}: {quote}</span>
		</button>
	);

	const insertCommentButton = nodeIsNewArticleHeading(node);
	let existingElement = convertNodeToElement(node, 0, transform);
	let existingElementChildren = React.Children.toArray(existingElement.props.children);

	// if new html conversion format then insert CommentButton as a child node rather than a sibling
	// nb. new conversion format doesn't have a nested anchor in headings, so node is heading rather than an anchor
	if (allowComments && insertCommentButton) {
		existingElementChildren.unshift(CommentButton);
		existingElement = React.cloneElement(existingElement, null, existingElementChildren);
	}

	return (
		<Fragment key={htmlElementID}>
			{allowComments && !insertCommentButton &&
				CommentButton
			}
			{existingElement}
		</Fragment>
	);
};

const transform = (node) => {
	if (nodeIsInternalLink(node)){
		return processInternalLink(node);
	}
};

//anchor tag with section number in the text of the node.
export const getSectionNumberFromAnchor = (node) => {
	if (node.children != null && node.children[0].data != null){
		const chapterOrSectionText = node.children[0].data;
		const regex = /^([\d.]*)/;
		const matches = chapterOrSectionText.match(regex);
		if (matches !== null && matches[0] !== ""){
			return matches[0];
		}
	}
	return null;
};

//subsection - paragraph element, with section number in a child span with class "paragraph-number"
export const getSectionNumberFromParagraph = (node) => {
	let paragraphNode = node.children.filter(nodeIsParagraphNumber);
	paragraphNode = paragraphNode.length ? paragraphNode : node.children.filter(nodeIsNewArticleHeading);

	if (paragraphNode !== null && paragraphNode[0].children != null){
		return paragraphNode[0].children[0].data.trim();
	}
	return null;
};
