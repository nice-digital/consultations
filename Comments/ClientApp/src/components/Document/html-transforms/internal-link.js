import React, { Fragment } from "react";
import { convertNodeToElement } from "react-html-parser";
import { nodeIsTypeText } from "./types";

export default function processInternalLink(node) {
	// todo: carry on here
	return convertNodeToElement(node);
}
