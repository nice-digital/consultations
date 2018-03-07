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
	}

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
