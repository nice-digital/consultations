import React from "react";
import { NavHashLink } from "react-router-hash-link";

export default function processInternalLink(node) {
	const props = node.attribs;
	props.className = props.class;
	props.to = props.href;
	props.key = props.href;
	delete props.class;
	const label = node.children[0]["data"] || props.href;
	return (
		<NavHashLink
			{...props}
			scroll={el => el.scrollIntoView({behavior: "smooth", block: "start"})}>
			{label}
		</NavHashLink>
	);
}
