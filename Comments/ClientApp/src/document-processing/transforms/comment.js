import React from "react";

export default function processComment(node) {
	return (
		<p>{node.text}</p>
	);
}
