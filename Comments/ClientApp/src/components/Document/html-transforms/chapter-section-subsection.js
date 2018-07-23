import React, { Fragment } from "react";
import { convertNodeToElement } from "react-html-parser";
import { nodeIsTypeText, nodeIsSubsection, nodeIsSpanTag } from "./types";

export const processChapterSectionSubsection = (node, incomingHtml, onNewCommentClick, sourceURI, allowComments) => {
	let commentOn = node.attribs["data-heading-type"].toLowerCase();
	let quote = node.children.filter(nodeIsTypeText)[0].data;

	if (nodeIsSubsection(node)) {
		quote = node.children.filter(nodeIsSpanTag)[0].children.filter(nodeIsTypeText)[0].data;
		commentOn = "subsection";
	}

	const htmlElementID = (commentOn === "section" || commentOn === "subsection") ? node.attribs.id : "";

	return (
		<Fragment key={0}>
			{allowComments &&
				<button
					data-qa-sel="in-text-comment-button"
					className="document-comment-container__commentButton"
					tabIndex={0}
					onClick={e => {
						e.preventDefault();
						onNewCommentClick({
							sourceURI,
							commentText: "",
							commentOn,
							htmlElementID,
							quote
						});
					}}
				>
					<span className="icon icon--comment" aria-hidden="true" />
					<span className="visually-hidden">Comment on {commentOn}: {quote}</span>
				</button>
			}
			{convertNodeToElement(node)}
		</Fragment>
	);
};
