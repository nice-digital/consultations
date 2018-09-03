// @flow

import ReactHtmlParser from "react-html-parser";
import { nodeIsChapter, nodeIsInternalLink, nodeIsSection, nodeIsSubsection } from "./transforms/types";
import { processChapterSectionSubsection } from "./transforms/chapter-section-subsection";
import processInternalLink from "./transforms/internal-link";

// onNewCommentClick passed through from <Document />
export const processDocumentHtml = (incomingHtml: string, onNewCommentClick: Function, sourceURI: string, allowComments: boolean) => {

	function transformHtml(node) {
		if (nodeIsChapter(node) || nodeIsSection(node) || nodeIsSubsection(node)) {
			return processChapterSectionSubsection(node, onNewCommentClick, sourceURI, allowComments);
		}

		if (nodeIsInternalLink(node)) {
			return processInternalLink(node);
		}
	}

	return ReactHtmlParser(incomingHtml, {
		transform: transformHtml,
	});
};
