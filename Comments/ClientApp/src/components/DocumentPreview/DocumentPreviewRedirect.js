// @flow

import React, { Component } from "react";
import { withRouter, Redirect } from "react-router";
import { load } from "../../data/loader";

type StateType = {
	redirectUrl: string | null,
	error: {
		hasError: boolean,
		message: string
	}
};

type PropsType = {
	match: Object
};

export class DocumentPreviewRedirect extends Component<PropsType, StateType> {

	constructor(){
		super();
		this.state = {
			redirectUrl: null,
			error: {
				hasError: false,
				message: null,
			},
		};
	}

	componentDidMount(){
		const { consultationId, documentId, reference } = this.props.match.params;
		this.createRedirectUrl(consultationId, documentId, reference);
	}

	createRedirectUrl = (consultationId: number, documentId: number, reference: string) => {
		load("previewdraftdocuments", undefined, [], { consultationId, documentId, reference })
			.then(response => {
				const documents = response.data;
				//	get current document data
				const currentDocument = documents.filter(d => d.documentId === parseInt(documentId, 0))[0];
				// get the slug of the first chapter
				const firstChapterSlug = currentDocument.chapters[0].slug;
				// go to the route with the chapter slug in it
				this.setState({
					redirectUrl: `${this.props.match.url}/chapter/${firstChapterSlug}`,
				});
			})
			.catch(err => {
				//throw new Error("documentsData " + err);
				this.setState({
					error: {
						hasError: true,
						message: "documentsData " + err,
					},
				});
			});
	};

	render() {
		if (this.state.error.hasError) { throw new Error(this.state.error.message); }
		if (!this.state.redirectUrl) return null;
		return <Redirect to={this.state.redirectUrl} />;
	}
}

export default withRouter(DocumentPreviewRedirect);
