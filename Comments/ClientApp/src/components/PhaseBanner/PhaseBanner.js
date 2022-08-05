// @flow

import React, { PureComponent } from "react";

type PropsType = {
	phase: string,
	name: string,
}

export class PhaseBanner extends PureComponent<PropsType> {
	render() {
		const {phase, name} = this.props;
		return (
			<div className="container phase-banner-container">
				<div className="grid">
					<div data-g="12">
						<aside className="phase-banner mt--b">
							<span className="phase-banner__tag">
								<span className="tag tag--impact tag--beta">{phase}</span>
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
