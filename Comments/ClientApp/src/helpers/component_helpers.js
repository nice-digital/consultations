// @flow

import React from "react";
import { NavHashLink } from "react-router-hash-link";

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
			to={`#${to}`}
			scroll={el => el.scrollIntoView({behavior, block})}
		>
			{label}
		</NavHashLink>
	);
}
