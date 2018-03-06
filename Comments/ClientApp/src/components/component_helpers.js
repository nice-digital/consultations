// @flow

import React from "react";
import { HashLink } from "react-router-hash-link";

export const HashLinkTop = (
	label: string,
	destination: string,
	smooth: boolean = true
) => {
	if (smooth) {
		return (
			<HashLink
				to={destination}
				scroll={el => el.scrollIntoView({ behavior: "smooth", block: "start" })}
			>
				{label}
			</HashLink>
		);
	} else {
		return <HashLink to={destination}>{label}</HashLink>;
	}
};
