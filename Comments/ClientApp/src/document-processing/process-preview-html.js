// @flow

import React from "react";
import ReactHtmlParser from "react-html-parser";
import { nodeIsInternalLink, nodeIsComment } from "./transforms/types";
import processInternalLink from "./transforms/internal-link";
import processComment from "./transforms/comment";
import { filterDeep } from "lodash-deeper";
import uniqBy from "lodash/uniqBy";

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
		transform: transformHtml,
	});
};

export const generateErrorList = (incomingHtml: string) => {
	function filterOutComments(nodes) {
		const filteredNodes = filterDeep(nodes, (item) => {
			if (item && item.type) {
				return item.type === "comment";
			}
		});
		return uniqBy(filteredNodes, "data");
	}

	function transformHtml(node){
		return processError(node);
	}

	return ReactHtmlParser(incomingHtml, {
		preprocessNodes: filterOutComments,
		transform: transformHtml,
	});

};

function processError(node){
	return (
		<li>{node.data}</li>
	);
}
