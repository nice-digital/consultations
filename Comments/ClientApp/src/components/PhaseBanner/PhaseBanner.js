// @flow

import React from "react";

type PropsType = {
	phase: string,
	name: string,
	repo: string
}

export const PhaseBanner = (props: PropsType) => {
	const { phase, name, repo } = props;
	return (
		<aside className="phase-banner mt--b">
			<span className="phase-banner__tag">
				<span className="tag tag--impact tag--agile">{phase}</span>
			</span>
			<span className="phase-banner__label">
				{name} is in development which means that some features may not work fully.{" "}
				<a
					href={repo}
					rel="noopener noreferrer"
					target="_blank"
				>
					Report an issue
				</a>
			</span>
		</aside>
	);
};
