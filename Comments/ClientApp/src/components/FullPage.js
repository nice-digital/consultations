// @flow

import React, { Component } from "react";

export default class Layout extends Component {
	render() {
		return (
			<div className="container">
				<div className="grid">
					<div data-g="12">
						<p className="phase-banner">
							<span className="phase-banner__tag">
								<span className="tag tag--impact tag--alpha">Alpha</span>
							</span>
							<span className="phase-banner__label">
								NICE Consulations is in development. This means it is not
								feature complete and there may be issues. Find any?{" "}
								<a
									href="https://github.com/nhsevidence/consultations"
									rel="noopener noreferrer"
									target="_blank"
								>
									Please, let us know!
								</a>
							</span>
						</p>
					</div>
					<div data-g="12">{this.props.children}</div>
				</div>
			</div>
		);
	}
}
