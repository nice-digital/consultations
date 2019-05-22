// @flow

import React, { PureComponent } from "react";
import * as Bowser from "bowser";

type PropsType = {
	phase: string,
	name: string,
}

const getBrowserInfo = () => {

	const environmentName = process.env.NODE_ENV;

	if (typeof window !== "undefined") {
		return `${Bowser.name} ${Bowser.version} on ${Bowser.osname} ${Bowser.osversion} running as ${environmentName}`;
	}
	return environmentName;
};

const phaseBannerClicked = () => {
	if (window.location.hostname.includes("alpha")) {
		alert(getBrowserInfo());
	}
};

export class PhaseBanner extends PureComponent<PropsType> {
	render() {
		const {phase, name} = this.props;
		return (
			<div className="container phase-banner-container">
				<div className="grid">
					<div data-g="12">
						<aside className="phase-banner mt--b">
							{/* eslint-disable-next-line*/}
							<span className="phase-banner__tag" role="button" onClick={() => phaseBannerClicked()}>
								<span className="tag tag--impact tag--agile">{phase}</span>
							</span>
							<span className="phase-banner__label">
								{name} is in development, which means that some features may not work fully.{" "}
								<a
									href="/get-involved/contact-us"
									rel="noopener noreferrer"
									target="_blank"
								>Report an issue</a>
							</span>
						</aside>
					</div>
				</div>
			</div>
		);
	}
}
