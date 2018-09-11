// @flow

//NOTE: This has been superseded by Chapter.js. leaving here for now, for reference. 

import ReactHtmlParser from "react-html-parser";
import { nodeIsChapter, nodeIsInternalLink, nodeIsSection, nodeIsSubsection } from "./transforms/types";
import { processChapterSectionSubsection } from "./transforms/chapter-section-subsection";
import processInternalLink from "./transforms/internal-link";

// onNewCommentClick passed through from <Document />
export const processDocumentHtml = (incomingHtml: string, onNewCommentClick: Function, sourceURI: string, allowComments: boolean) => {

	function transformHtml(node) {
		const isSubsection = nodeIsSubsection(node);
		if (nodeIsChapter(node) || nodeIsSection(node) || isSubsection) {
			return processChapterSectionSubsection(node, onNewCommentClick, sourceURI, allowComments, isSubsection);
		}

		if (nodeIsInternalLink(node)) {
			return processInternalLink(node);
		}
	}

	return ReactHtmlParser(incomingHtml, {
		transform: transformHtml,
	});
};
