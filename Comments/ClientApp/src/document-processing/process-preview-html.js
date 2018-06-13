import ReactHtmlParser from "react-html-parser";
import { nodeIsInternalLink } from "./transforms/types";
import processInternalLink from "./transforms/internal-link";

// onNewCommentClick passed through from <Document />
export const processPreviewHtml = (incomingHtml, onNewCommentClick, sourceURI) => {

	function transformHtml(node) {
		if (nodeIsInternalLink(node)) {
			return processInternalLink(node);
		}
	}

	return ReactHtmlParser(incomingHtml, {
		transform: transformHtml
	});
};
