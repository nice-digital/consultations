import ReactHtmlParser from "react-html-parser";
import { nodeIsChapter, nodeIsInternalLink, nodeIsSection, nodeIsSubsection } from "./html-transforms/types";
import { processChapterSectionSubsection } from "./html-transforms/chapter-section-subsection";
import processInternalLink from "./html-transforms/internal-link";

// onNewCommentClick passed through from <Document />
export const processDocumentHtml = (incomingHtml, onNewCommentClick, sourceURI, allowComments) => {
	function transformHtml(node) {
		if (nodeIsChapter(node) || nodeIsSection(node) || nodeIsSubsection(node)) {
			return processChapterSectionSubsection(node, incomingHtml, onNewCommentClick, sourceURI, allowComments);
		}
		if (nodeIsInternalLink(node)) {
			return processInternalLink(node);
		}
	}

	return ReactHtmlParser(incomingHtml, {
		transform: transformHtml
	});
};
