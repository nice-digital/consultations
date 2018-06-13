// @flow

import ReactHtmlParser from "react-html-parser";
import { nodeIsChapter, nodeIsInternalLink, nodeIsSection } from "./transforms/types";
import processChapterSection from "./transforms/chapter-section";
import processInternalLink from "./transforms/internal-link";

// onNewCommentClick passed through from <Document />
export const processDocumentHtml = (incomingHtml: string, onNewCommentClick: Function, sourceURI: string) => {

	function transformHtml(node) {
		if (nodeIsChapter(node) || nodeIsSection(node)) {
			return processChapterSection(node, incomingHtml, onNewCommentClick, sourceURI);
		}

		if (nodeIsInternalLink(node)) {
			return processInternalLink(node);
		}
	}

	return ReactHtmlParser(incomingHtml, {
		transform: transformHtml
	});
};
