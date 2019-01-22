// @flow

import React from "react";
import { NavHashLink } from "react-router-hash-link";
import { pullFocusByQuerySelector } from "./accessibility-helpers";

type HashLinkTopType = {
	label: string,
	to: string,
	behavior: string,
	block: string
};

export function HashLinkTop(props: HashLinkTopType) {
	const { label, to, behavior, block } = props;
	return (
		<NavHashLink
			role="menuitem"
			to={to}
			scroll={el => {
				el.scrollIntoView({
					behavior,
					block,
				});
				const element = el.attributes.id.value;
				pullFocusByQuerySelector(`#${element}`);
			}}
		>
			{label}
		</NavHashLink>
	);
}

