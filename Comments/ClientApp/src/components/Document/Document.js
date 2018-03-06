// @flow

import React, { Component } from "react";
import { Helmet } from "react-helmet";
import Breadcrumbs from "./../Breadcrumbs/Breadcrumbs";
import StackedNav from "./../StackedNav/StackedNav";
import { HashLinkTop } from "./../component_helpers";
import axios from "axios";

type PropsType = {};

type StateType = {
	document: any
};

class Document extends Component<PropsType, StateType> {
	constructor() {
		super();

		this.state = {
			document: null
		};
	}

	componentDidMount() {
		axios("http://127.0.0.1:1234/sample.json").then(response => {
			console.log(response);
			this.setState({
				document: response.data
			});
		});
	}

	renderHTML = () => {
		if (this.state.document) {
			return { __html: this.state.document.chapterHTML.content };
		} else {
			return null;
		}
	};

	renderInPageNav = () => {
		if (this.state.document) {
			const { sections } = this.state.document.chapterHTML;
			return (
				<ul>
					{sections.map(item => {
						return <li key={item.title}>{HashLinkTop(item.title, `#${item.slug}`, true)}</li>;
					})}
				</ul>
			);
		}
	};

	render() {
		const breadcrumbs = [
			{ label: "Home", url: "/document" },
			{ label: "NICE Guidance", url: null },
			{ label: "In Consulation", url: null },
			{ label: "Document title", url: null }
		];

		const chapterLinks = {
			root: { label: "Chapters in this document", url: "#" },
			links: [
				{ label: "Key priorities for implementation", url: null },
				{ label: "Recommendations", url: null },
				{
					label:
						"Intravenous fluid therapy in children and young people in hospital",
					url: null
				},
				{ label: "Context", url: null },
				{ label: "Recommendations for research", url: null }
			]
		};

		const additionalDocuments = {
			root: { label: "Additional documents to comment on", url: "#" },
			links: [
				{
					label:
						"Intravenous fluid therapy in children and young people in hospital - Short Guideline",
					url: null
				}
			]
		};

		return (
			<div>
				<Helmet>
					<title>Comment on Document</title>
				</Helmet>
				<Breadcrumbs segments={breadcrumbs} />
				<div className="page-header">
					<h1 className="page-header__heading">
						Intravenous fluid therapy in children and young people in hospital :
						Consultation
					</h1>
					<p className="page-header__lead">
						In development [NG29] Expected publication date TBC
					</p>
				</div>
				<div className="grid">
					<div data-g="12 md:3">
						<StackedNav links={chapterLinks} />
						<StackedNav links={additionalDocuments} />
					</div>
					<div data-g="12 md:6">
						<div className="document-comment-container">
							<div dangerouslySetInnerHTML={this.renderHTML()} />
						</div>
					</div>
					<div data-g="12 md:3">{this.renderInPageNav()}</div>
				</div>
			</div>
		);
	}
}

export default Document;
