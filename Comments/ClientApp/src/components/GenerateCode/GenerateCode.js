// @flow

import React, { Component } from "react";
import { GenerateCodeForOrg } from "../GenerateCodeForOrg/GenerateCodeForOrg";

import { Button } from "@nice-digital/nds-button";

type GenerateCodeProps = {
	organisationCodes: Array,
	consultationId: number
};

type GenerateCodeState = {
	showCodeGenerators: boolean
}

export class GenerateCode extends Component<GenerateCodeProps, GenerateCodeState> {
	constructor(props: GenerateCodeProps) {
		super(props);

		this.state = { showCodeGenerators: false };
	};

	handleCodeButtonClick = () => {
		this.setState({ showCodeGenerators: !this.state.showCodeGenerators });
	};

	render() {
		const {
			organisationCodes,
			consultationId
		} = this.props;

		const {
			showCodeGenerators
		} = this.state;

		return (
			<div>
				<Button variant="inverse" onClick={this.handleCodeButtonClick} aria-expanded={showCodeGenerators} aria-controls={`organisation-codes-${consultationId}`}>
					Share with organisation <span className="arrow-icon"></span>
				</Button>

				{showCodeGenerators && (
					<div className="panel organisation-codes mv--b" id={`organisation-codes-${consultationId}`}>
						<h2 className="h5">Generate code</h2>
						<p>Select an organisation</p>
						{organisationCodes.map((item, index) =>
							<GenerateCodeForOrg key={index} {...item} consultationId={consultationId} />
						)}
					</div>
				)}
			</div>
		);
	};
};