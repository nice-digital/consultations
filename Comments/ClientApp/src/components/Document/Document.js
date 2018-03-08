// @flow

import React, { Component } from "react";
import axios from "axios";
import Moment from "react-moment";
import { Helmet } from "react-helmet";
import Breadcrumbs from "./../Breadcrumbs/Breadcrumbs";
import { StackedNav } from "./../StackedNav/StackedNav";
import { HashLinkTop } from "./../../helpers/component_helpers";
import CommentPanel from "../CommentPanel/CommentPanel";

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
		// todo: separate this into the shared loader
		axios("/sample.json").then((response: ResponseType) => {
			this.setState({
				document: response.data
			});
		});
	}

	renderDocumentHtml = () => {
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
				<nav className="in-page-nav" aria-labelledby="inpagenav-title">
					<h2 id="inpagenav-title" className="in-page-nav__title">
						On this page
					</h2>
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
				</nav>
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

	// todo: will these need to be manually extracted from the consultation.documents array?
	renderThisDocumentChapterLinks = () => {
		if (this.state.document) {
			const links = {
				root: {
					label: "Chapters in this document",
					url: "#"
				},
				links: [
					{
						label: "this is a sample label",
						url: "#"
					}
				]
			};
			return <StackedNav links={links} />;
		}
	};

	render() {
		if (!this.state.document) {
			return null;
		}

		// todo: where are the breadcrumbs going to come from?
		const breadcrumbs = [
			{ label: "Home", url: "/document" },
			{ label: "NICE Guidance", url: "#" },
			{ label: "In Consulation", url: "#" },
			{ label: "Document title", url: "#" }
		];

		const { title, endDate, reference } = this.state.document.consultation;

		return (
			<div>
				<Helmet>
					<title>Comment on Document</title>
				</Helmet>
				<CommentPanel />
				<Breadcrumbs segments={breadcrumbs} />
				<div className="page-header">
					<h1 className="page-header__heading">{title}</h1>
					<p className="page-header__lead">
						[{reference}] Open until{" "}
						<Moment format="DD-MM-YYYY" date={endDate} />
					</p>
				</div>
				<div className="grid">
					<div data-g="12 md:3">
						{this.renderThisDocumentChapterLinks()}
						{this.renderSupportingDocumentLinks()}
					</div>
					<div data-g="12 md:6">
						<div className="document-comment-container">
							<div dangerouslySetInnerHTML={this.renderDocumentHtml()} />
						</div>
					</div>
					<div data-g="12 md:3">{this.renderInPageNav()}</div>
				</div>
			</div>
		);
	}
}

export default Document;
