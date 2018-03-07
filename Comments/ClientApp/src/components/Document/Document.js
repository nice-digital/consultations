// @flow

import React, { Component } from "react";
import { Helmet } from "react-helmet";
import Breadcrumbs from "./../Breadcrumbs/Breadcrumbs";
import StackedNav from "./../StackedNav/StackedNav";
import { HashLinkTop } from "./../../helpers/component_helpers";
import axios from "axios";

type PropsType = {};

type StateType = {
	document: null | {
		chapterHTML: {
			content: string,
			sections: [
				{
					slug: string,
					title: string
				}
			]
		},
		consultation: {
			documents: [
				{
					documentId: number,
					title: string,
					chapters: [
						{
							slug: string,
							title: string
						}
					]
				}
			],
			title: string,
			endDate: string,
			reference: string
		}
	}
};

class Document extends Component<PropsType, StateType> {
	constructor() {
		super();

		this.state = {
			document: null
		};
	}

	componentDidMount() {
		type ResponseType = {
			data: Object
		};
		axios("/sample.json").then((response: ResponseType) => {
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
				<ol className="in-page-nav__list" aria-hidden="false" role="menubar">
					{sections.map((item, index) => {
						const props = {
							label: item.title,
							to: item.slug,
							behavior: "smooth",
							block: "start"
						};
						return (
							<li className="in-page-nav__item" key={index}>
								<HashLinkTop {...props} />
							</li>
						);
					})}
				</ol>
			);
		}
	};

	renderSupportingDocumentLinks = () => {
		if (this.state.document) {
			const { documents } = this.state.document.consultation;

			let documentList = [];

			for (const document of documents) {
				if (document.title && document.documentId) {
					documentList.push({
						label: document.title,
						url: `/1/${document.documentId}/${document.chapters[0].slug}`
					});
				}
			}

			const links = {
				root: {
					label: "Additional documents to comment on",
					url: "#"
				},
				links: documentList
			};

			return <StackedNav links={links} />;
		}
	};

	render() {
		const breadcrumbs = [
			{ label: "Home", url: "/document" },
			{ label: "NICE Guidance", url: "#" },
			{ label: "In Consulation", url: "#" },
			{ label: "Document title", url: "#" }
		];

		const chapterLinks = {
			root: { label: "Chapters in this document", url: "#" },
			links: [
				{ label: "Key priorities for implementation", url: "#" },
				{ label: "Recommendations", url: "#" },
				{
					label:
						"Intravenous fluid therapy in children and young people in hospital",
					url: "#"
				},
				{ label: "Context", url: "#" },
				{ label: "Recommendations for research", url: "#" }
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
						{this.renderSupportingDocumentLinks()}
					</div>
					<div data-g="12 md:6">
						<div className="document-comment-container">
							<div dangerouslySetInnerHTML={this.renderHTML()} />
						</div>
					</div>
					<div data-g="12 md:3">
						<nav className="in-page-nav" aria-labelledby="inpagenav-title">
							<h2 id="inpagenav-title" className="in-page-nav__title">
								On this page
							</h2>
							{this.renderInPageNav()}
						</nav>
					</div>
				</div>
			</div>
		);
	}
}

export default Document;
