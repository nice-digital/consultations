// @flow

import React from "react";
import { NavHashLink } from "react-router-hash-link";
import { pullFocusById } from "./accessibility-helpers";

type HashLinkTopType = {
	label: string,
	to: string,
	behavior: string,
	block: string,
	currentNavItem: null | string
};

export function HashLinkTop(props: HashLinkTopType) {
	const { labael, to, behavior, block, currentNavItem } = props;
	return (
		<NavHashLink
			aria-current={currentNavItem === to ? "location" : "false"}
			role="menuitem"
			to={to}
			scroll={el => {
				el.scrollIntoView({
					behavior,
					block
				});
				const element = el.attributes.id.value;
				pullFocusById(element);
			}}
		>
			{label}
		</NavHashLink>
	);
}
