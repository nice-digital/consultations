// @flow

import React, { Component } from "react";
import Moment from "react-moment";
import { Helmet } from "react-helmet";
import { StickyContainer, Sticky } from "react-sticky";

import { PhaseBanner } from "./../PhaseBanner/PhaseBanner";
import { BreadCrumbs } from "./../Breadcrumbs/Breadcrumbs";
import { StackedNav } from "./../StackedNav/StackedNav";
import { HashLinkTop } from "../../helpers/component-helpers";
import { CommentPanel } from "./../CommentPanel/CommentPanel";
import { load } from "./../../data/loader";

type PropsType = {};

type StateType = {
	document: ?{
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

type DataType = any;

type DocumentsType = any;

type ChaptersType = any;

class Document extends Component<PropsType, StateType> {
	constructor() {
		super();

		this.state = {
			document: null
		};

		load("sample.json")
			.then(response  => {
				this.setState({
					document: response.data
				});
			})
		// TODO: explore why this is logging in testing
			.catch(err => console.log("💔 Problem with load"));
	}

	renderDocumentHtml = (data: DataType) => {
		return { __html: data };
	};

	getSupportingDocumentLinks = (documents: DocumentsType) => {
		const isValidDocument = d => d.title && d.documentId;
		const documentToLinkObject = d => ({
			label: d.title,
			url: `/1/${d.documentId}/${d.chapters[0].slug}`
		});
		return {
			root: {
				label: "Additional documents to comment on",
				url: "#"
			},
			links: documents.filter(isValidDocument).map(documentToLinkObject)
		};
	};

	getDocumentChapterLinks = (chapters: ChaptersType) => {
		if (chapters) throw new Error("Need to add chapters to getDocumentChapterLinks");
		return {
			root: {
				label: "Chapters in this document",
				url: "#",
				current: true
			},
			links: [
				{
					label: "this is a sample label",
					url: "#",
					current: true
				}
			]
		};
	};

	getBreadcrumbs = () => {
		return [
			{
				label: "Home",
				url: "/document"
			},
			{
				label: "NICE Guidance",
				url: "#"
			},
			{
				label: "In Consulation",
				url: "#"
			},
			{
				label: "Document title",
				url: "#"
			}
		];
	};

	render() {
		if (!this.state.document) return <h1>Loading...</h1>;

		const { title, endDate, reference, documents } = this.state.document.consultation;

		const { sections, content } = this.state.document.chapterHTML;

		return (
			<div>
				<Helmet>
					<title>Comment on Document</title>
				</Helmet>
				<div className="container">
					<div className="grid">
						<div data-g="12">
							<PhaseBanner/>
							<BreadCrumbs links={this.getBreadcrumbs()}/>
							<div className="page-header">
								<h1 className="page-header__heading">{title}</h1>
								<p className="page-header__lead">
									[{reference}] Open until{" "}
									<Moment format="D MMMM YYYY" date={endDate}/>
								</p>
							</div>
							<StickyContainer className="grid">
								<div data-g="12 md:3">
									<StackedNav links={this.getDocumentChapterLinks()}/>
									<StackedNav links={this.getSupportingDocumentLinks(documents)}/>
								</div>
								<div data-g="12 md:6">
									<div className="document-comment-container">
										<div dangerouslySetInnerHTML={this.renderDocumentHtml(content)}/>
									</div>
								</div>
								<div data-g="12 md:3">
									<Sticky>
										{({ style }) => <CommentPanel style={style}/>}
									</Sticky>
									<nav
										className="in-page-nav"
										aria-labelledby="inpagenav-title"
									>
										<h2 id="inpagenav-title" className="in-page-nav__title">
											On this page
										</h2>
										<ol
											className="in-page-nav__list"
											aria-hidden="false"
											role="menubar"
										>
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
								</div>
							</StickyContainer>
						</div>
					</div>
				</div>
			</div>
		);
	}
}

export default Document;
