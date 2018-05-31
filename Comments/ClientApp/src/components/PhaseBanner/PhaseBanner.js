// @flow

import React from "react";
import * as Bowser from "bowser";

type PropsType = {
	phase: string,
	name: string,
	repo: string
}

export const PhaseBanner = (props: PropsType) => {
	const { phase, name, repo } = props;
	return (
		<aside className="phase-banner mt--b">
			<span className="phase-banner__tag" role="button" onClick={()=>alert(`${Bowser.name} ${Bowser.version} on ${Bowser.osname} ${Bowser.osversion}`)}>
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
