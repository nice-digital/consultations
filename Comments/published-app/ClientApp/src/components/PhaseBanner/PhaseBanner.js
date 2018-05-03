// @flow

import React from "react";

export const PhaseBanner = () => {
	return (
		<p className="phase-banner">
			<span className="phase-banner__tag">
				<span className="tag tag--impact tag--beta">Beta</span>
			</span>
			<span className="phase-banner__label">
				NICE Consulations is in development. This means it is not feature
				complete and there may be issues. Find any?{" "}
				<a
					href="https://github.com/nhsevidence/consultations"
					rel="noopener noreferrer"
					target="_blank"
				>
					Please, let us know!
				</a>
			</span>
		</p>
	);
};
