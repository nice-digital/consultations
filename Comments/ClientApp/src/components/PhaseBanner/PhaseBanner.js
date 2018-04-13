// @flow

import React from "react";

export const PhaseBanner = () => {
	return (
		<p className="phase-banner">
			<span className="phase-banner__tag">
				<span className="tag tag--impact tag--beta">Beta</span>
			</span>
			<span className="phase-banner__label">
				NICE Consultations is in development. This means it is not feature
				complete and there may be issues.
				<a
					href="https://github.com/nhsevidence/consultations"
					rel="noopener noreferrer"
					target="_blank"
				>
					Visit this page if you wish to report an issue.
				</a>
			</span>
		</p>
	);
};
