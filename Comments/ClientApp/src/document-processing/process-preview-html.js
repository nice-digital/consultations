// @flow

import ReactHtmlParser from "react-html-parser";
import { nodeIsInternalLink } from "./transforms/types";
import processInternalLink from "./transforms/internal-link";

export const processPreviewHtml = (incomingHtml: string) => {
	function transformHtml(node) {
		if (nodeIsInternalLink(node)) {
			return processInternalLink(node);
		}
	}

	return ReactHtmlParser(incomingHtml, {
		transform: transformHtml
	});
};
