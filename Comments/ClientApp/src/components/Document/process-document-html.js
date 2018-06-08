import ReactHtmlParser from "react-html-parser";
import { nodeIsChapter, nodeIsInternalLink, nodeIsSection } from "./html-transforms/types";
import processChapterSection from "./html-transforms/chapter-section";
import processInternalLink from "./html-transforms/internal-link";

// onNewCommentClick passed through from <Document />
export const processDocumentHtml = (incomingHtml, onNewCommentClick, sourceURI, addButtons) => {

	function addCommentingButtons(node){
		if (nodeIsChapter(node) || nodeIsSection(node)) {
			return processChapterSection(node, incomingHtml, onNewCommentClick, sourceURI);
		}
	}

	function transformHtml(node) {
		if (addButtons) {

		}
		if (nodeIsInternalLink(node)) {
			return processInternalLink(node);
		}
	}

	return ReactHtmlParser(incomingHtml, {
		transform: transformHtml
	});
};
