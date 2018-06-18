// @flow

import ReactHtmlParser from "react-html-parser";
import { nodeIsInternalLink, nodeIsComment } from "./transforms/types";
import processInternalLink from "./transforms/internal-link";
import processComment from "./transforms/comment";

export const processPreviewHtml = (incomingHtml: string) => {

	function transformHtml(node) {
		if (nodeIsInternalLink(node)) {
			return processInternalLink(node);
		}

		if (nodeIsComment(node)) {
			return processComment(node);
		}
	}

	return ReactHtmlParser(incomingHtml, {
		transform: transformHtml
	});
};
