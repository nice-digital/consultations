// @flow

import React, { Component } from "react";
import Moment from "react-moment";
import { Helmet } from "react-helmet";
import { StickyContainer, Sticky } from "react-sticky";
import { withRouter } from "react-router";

import { PhaseBanner } from "./../PhaseBanner/PhaseBanner";
import { BreadCrumbs } from "./../Breadcrumbs/Breadcrumbs";
import { StackedNav } from "./../StackedNav/StackedNav";
import { HashLinkTop } from "../../helpers/component-helpers";
// import { CommentPanel } from "./../CommentPanel/CommentPanel";
import { load } from "./../../data/loader";

// import preload from "../../data/pre-loader";

type PropsType = {
	staticContext?: any,
	match: any,
	location: any,
};
type StateType = {
	loading: boolean,
	documentsData: any, // the list of other documents in this consultation
	chapterData: any, // the current chapter's details - markup and sections,
	consultationData: any, // the top level info - title etc
};
type DataType = any;
type DocumentsType = any;

export class Document extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			chapterData: null,
			documentsData: null,
			consultationData: null,
			loading: true,
		};

		// const preloaded = preload(this.props.staticContext, "sample", this.props.match.params);

		// const preloadConsultation = preload(this.props.staticContext, "consultation", this.props.match.params);

		// if (preloaded) {
		// 	this.state = { document: preloaded, loading: false };
		// }
	}

	// TODO: separate this into a utility
	gatherData = async () => {
		const documentsData =
			await load("documents", undefined, {consultationId: this.props.match.params.consultationId})
				.then(response => response.data)
				.catch();

		const chapterData =
			await load("chapter", undefined, {
				consultationId: this.props.match.params.consultationId,
				documentId: this.props.match.params.documentId,
				chapterSlug: this.props.match.params.chapterSlug
			})
				.then(response => response.data)
				.catch();

		const consultationData =
			await load("consultation", undefined, {
				consultationId: this.props.match.params.consultationId
			})
				.then(response => response.data)
				.catch();

		return {
			consultationData,
			documentsData,
			chapterData
		};
	};

	componentDidMount() {

		if (!this.state.documentsData && !this.state.chapterData) {

			this.gatherData()
				.then( data =>{
					this.setState({
						...data,
						loading: false,
					});
				})
				.catch();
		}
	}

	componentDidUpdate(prevProps: PropsType){
		const oldRoute = prevProps.location.pathname;
		const newRoute = this.props.location.pathname;
		if (oldRoute !== newRoute) {
			this.gatherData()
				.then( data =>{
					this.setState({
						...data,
						loading: false,
					});
				})
				.catch();
		}
	}

	renderDocumentHtml = (data: DataType) => {
		return { __html: data };
	};

	getSupportingDocumentLinks = (documents: DocumentsType) => {
		if (!documents) return null;
		const isValidDocument = d => d.title && d.documentId;
		const isCurrentDocument = documentId => documentId === parseInt(this.props.match.params.documentId, 0);
		const documentToLinkObject = d => ({
			label: d.title,
			url: `/${this.props.match.params.consultationId}/${d.documentId}/${d.chapters[0].slug}`,
			current: isCurrentDocument(d.documentId),
		});

		const filteredDocuments = documents.filter(isValidDocument).map(documentToLinkObject);

		return {
			title: "Documents in this Consultation",
			links: filteredDocuments,
		};
	};

	getDocumentChapterLinks = (documentId: string) => {
		if (!documentId) return null;

		const isCurrentDocument = d => d.documentId === parseInt(documentId, 0);

		const isCurrentChapter = slug => slug === this.props.match.params.chapterSlug;

		const createChapterLink = chapter => {
			return {
				label: chapter.title,
				url: `/${this.props.match.params.consultationId}/${this.props.match.params.documentId}/${chapter.slug}`,
				current: isCurrentChapter(chapter.slug),
			};
		};

		const documents = this.state.documentsData;

		const currentDocument = documents.filter(isCurrentDocument);

		return {
			title: "Chapters in this document",
			links: currentDocument[0].chapters.map(createChapterLink),
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
				label: "In Consultation",
				url: "#"
			},
			{
				label: "Document title",
				url: "#"
			}
		];
	};

	getCurrentDocumentTitle = (documents: Object, documentId: string) => {
		const matchCurrentDocument = d => d.documentId === parseInt(documentId, 0);
		const currentDocumentDetails = documents.filter(matchCurrentDocument)[0];
		return currentDocumentDetails.title;
	};

	render() {

		if (this.state.loading) return <h1>Loading...</h1>;

		const { title, reference, endDate } = this.state.consultationData;
		const { documentsData } = this.state;
		const { sections, content } = this.state.chapterData;

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
								<h2>{this.getCurrentDocumentTitle(documentsData, this.props.match.params.documentId)}</h2>
								<p className="page-header__lead">
									[{reference}] Open until{" "}
									<Moment format="D MMMM YYYY" date={endDate}/>
								</p>
							</div>
							<StickyContainer className="grid">
								<div data-g="12 md:3">
									<StackedNav links={this.getDocumentChapterLinks(this.props.match.params.documentId)}/>
									<StackedNav links={this.getSupportingDocumentLinks(documentsData)}/>
								</div>
								<div data-g="12 md:6">
									<div className="document-comment-container">
										<div dangerouslySetInnerHTML={this.renderDocumentHtml(content)}/>
									</div>
								</div>
								<div data-g="12 md:3">
									<Sticky>
										{({ style }) =>

											<div style={style}>

												{ sections.length ?
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
															{
																sections.map((item, index) => {
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
																})
															}
														</ol>
													</nav>
													: null }

											</div>
										}

									</Sticky>
								</div>
							</StickyContainer>
						</div>
					</div>
				</div>
			</div>
		);
	}
}

export default withRouter(Document);
