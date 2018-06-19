// @flow

import React, { Component } from "react";
import { withRouter, Redirect } from "react-router";
import { load } from "../../data/loader";

type StateType = {
	redirectUrl: string | null
};

type PropsType = {
	match: Object
};

export class DocumentPreviewRedirect extends Component<PropsType, StateType> {

	constructor(){
		super();
		this.state = {
			redirectUrl: null
		};
	}

	componentDidMount(){
		const { consultationId, documentId } = this.props.match.params;
		this.createRedirectUrl(consultationId, documentId);
	}

	createRedirectUrl = (consultationId: number, documentId: number) => {
		load("documents", undefined, [], { consultationId })
			.then(response => {
				const documents = response.data;
				//	get current document data
				const currentDocument = documents.filter(d => d.documentId === parseInt(documentId, 0))[0];
				// get the slug of the first chapter
				const firstChapterSlug = currentDocument.chapters[0].slug;
				// go to the route with the chapter slug in it
				this.setState({
					redirectUrl: `${this.props.match.url}/chapter/${firstChapterSlug}`
				});
			})
			.catch(err => {
				throw new Error("documentsData " + err);
			});
	};

	render() {
		if (!this.state.redirectUrl) return null;
		return <Redirect to={this.state.redirectUrl} />;
	}
}

export default withRouter(DocumentPreviewRedirect);
