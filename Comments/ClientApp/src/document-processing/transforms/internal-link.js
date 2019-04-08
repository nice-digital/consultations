import React from "react";
import { NavHashLink } from "react-router-hash-link";
import { pullFocusByQuerySelector } from "../../helpers/accessibility-helpers";

export default function processInternalLink(node) {
	const props = node.attribs;
	props.tabIndex = "0";
	props.className = props.class;
	props.to = props.href;
	props.key = props.href;
	props["data-identified-as"] = "internal-link";
	delete props.class;
	const label = node.children[0]["data"] || props.href;
	return (
		<NavHashLink
			{...props}
			scroll={el => {
				el.scrollIntoView({behavior: "smooth", block: "start"});
				const element = el.attributes.id.value;
				pullFocusByQuerySelector(`#${element}`);
			}}>
			{label}
		</NavHashLink>
	);
}
