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
		<aside className="phase-banner">
			<span className="phase-banner__tag">
				<span className="tag tag--impact tag--agile">{phase}</span>
			</span>
			<span className="phase-banner__label">
				{name} is in development. This means it is not feature
				complete and there may be issues.{" "}
				<a
					href={repo}
					rel="noopener noreferrer"
					target="_blank"
				>
					Visit this page if you wish to report an issue.
				</a>
			</span>
		</aside>
	);
};
