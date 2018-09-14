// @flow

import React, {PureComponent} from "react";
import * as Bowser from "bowser";

type PropsType = {
	phase: string,
	name: string,
	repo: string
}

const getBrowserInfo = () => {
	if (typeof window !== "undefined") {
		return `${Bowser.name} ${Bowser.version} on ${Bowser.osname} ${Bowser.osversion}`;
	}
	return "";
};

export class PhaseBanner extends PureComponent<PropsType> {
	render() {
		const {phase, name, repo} = this.props;
		return (
			<aside className="phase-banner mt--b">
				{/* eslint-disable-next-line*/}
				<span className="phase-banner__tag" role="button" onClick={() => alert(getBrowserInfo())}>
					<span className="tag tag--impact tag--agile">{phase}</span>
				</span>
				<span className="phase-banner__label">
					{name} is in development which means that some features may not work fully.{" "}
					<a
						href={repo}
						rel="noopener noreferrer"
						target="_blank"
					>Report an issue</a>
				</span>
			</aside>
		);
	}
}
