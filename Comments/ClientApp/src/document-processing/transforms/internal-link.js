import React from "react";
import { NavHashLink } from "react-router-hash-link";
import { pullFocusById } from "../../helpers/accessibility-helpers";

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
			scroll={el => {
				el.scrollIntoView({behavior: "smooth", block: "start"});
				const element = el.attributes.id.value;
				pullFocusById(element);
			}}>
			{label}
		</NavHashLink>
	);
}
