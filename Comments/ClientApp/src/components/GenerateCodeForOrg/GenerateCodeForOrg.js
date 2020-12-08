// @flow

import React, { Component } from "react";
import { load } from "../../data/loader";

import { Button } from "@nice-digital/nds-button";
import { Tag } from "@nice-digital/nds-tag";
import { CopyToClipboard } from 'react-copy-to-clipboard';

type GenerateCodeForOrgProps = {
	organisationAuthorisationId: number,
	organisationId: number,
	organisationName: string,
	canGenerateCollationCode: boolean,
	collationCode: string,
	consultationId: number
};

type GenerateCodeForOrgState = {
	generatedCollationCode: string,
	showGenerateButton: boolean,
	hasBeenCopied: boolean,
	isLoading: boolean,
	error: {
		hasError: boolean,
		message: string | null,
	}
};

export class GenerateCodeForOrg extends Component<GenerateCodeForOrgProps, GenerateCodeForOrgState> {
	constructor(props: GenerateCodeForOrgProps) {
		super(props);

		this.state = {
			generatedCollationCode: props.collationCode,
			showGenerateButton: props.canGenerateCollationCode,
			hasBeenCopied: false,
			isLoading: false,
			error: {
				hasError: false,
				message: null,
			}
		};
	};

	generateCode = () => {
		this.setState({
				isLoading: true
			}, async () => {
				await load("organisation", undefined, [], {
						organisationId: this.props.organisationId,
						consultationId: this.props.consultationId
					}, "POST", {}, true)
					.then(response => {
						this.setState({
							generatedCollationCode: response.data.collationCode,
							showGenerateButton: response.data.canGenerateCollationCode,
							isLoading: false
						})
					})
					.catch(err => {
						this.setState({
							error: {
								hasError: true,
								message: "generateCodeForOrg error  " + err,
							},
							isLoading: false
						});
					});
			}
		);
	};

	showCopiedLabel = () => {
		this.setState({ hasBeenCopied: true });

		setTimeout( () => {
			this.setState({ hasBeenCopied: false });
		}, 2000);
	};

	render() {

		const {
			organisationName,
		} = this.props;

		const {
			generatedCollationCode,
			showGenerateButton,
			hasBeenCopied,
			isLoading: isButtonDisabled,
		} = this.state;

		return (
			<div className="organisation-codes__row">
				<div className="organisation-codes__name">
					<p>{organisationName}</p>
				</div>
				<div className="organisation-codes__generate pl--b">
					{showGenerateButton && (
						<Button onClick={this.generateCode} aria-live="polite" disabled={isButtonDisabled}>{isButtonDisabled ? "Loading" : "Generate"}</Button>
					)}

					<span>{generatedCollationCode}</span>

					{!showGenerateButton && (
						<CopyToClipboard text={generatedCollationCode} onCopy={this.showCopiedLabel}>
          					<Tag role="button" tabIndex="0" className="tag tag--copied" aria-live="polite">{hasBeenCopied ? "Copied" : "Copy code"}</Tag>
        				</CopyToClipboard>
					)}
				</div>
			</div>
		);
	};
};
