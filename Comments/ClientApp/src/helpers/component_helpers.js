// @flow

import React from "react";
import { HashLink } from "react-router-hash-link";

export const HashLinkTop = (
	label: string,
	destination: string,
	behavior: string = "instant",
	block: string = "start") => {
	return (
		<HashLink
			to={destination}
			scroll={el => el.scrollIntoView({ behavior, block })}>
			{label}
		</HashLink>
	);
};
