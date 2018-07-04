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

function isCurrentNavItem(currentNavItem, to){
	if (currentNavItem === to) return "location";
}

export function HashLinkTop(props: HashLinkTopType) {
	const { label, to, behavior, block, currentNavItem } = props;

	if (isCurrentNavItem(currentNavItem, to)) {
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
					pullFocusById(element); // todo: this is upsetting our scroll to destination
				}}
			>
				{label}
			</NavHashLink>
		);
	}

	return (
		<NavHashLink
			role="menuitem"
			to={to}
			scroll={el => {
				el.scrollIntoView({
					behavior,
					block
				});
				const element = el.attributes.id.value;
				pullFocusById(element); // todo: this is upsetting our scroll to destination
			}}
		>
			{label}
		</NavHashLink>
	);


}
