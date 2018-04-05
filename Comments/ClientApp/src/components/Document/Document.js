// @flow
import React, { Component } from "react";
import Moment from "react-moment";
import { Helmet } from "react-helmet";
import { StickyContainer, Sticky } from "react-sticky";
import { withRouter } from "react-router";
import Scrollspy from "react-scrollspy";

import { load } from "./../../data/loader";
import { PhaseBanner } from "./../PhaseBanner/PhaseBanner";
import { BreadCrumbs } from "./../Breadcrumbs/Breadcrumbs";
import { StackedNav } from "./../StackedNav/StackedNav";
import { HashLinkTop } from "../../helpers/component-helpers";
// import { CommentPanel } from "./../CommentPanel/CommentPanel";

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
	currentInPageNavItem: null | string
};
type DataType = any;
type DocumentsType = Array<Object>;

export class Document extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			chapterData: null,
			documentsData: null,
			consultationData: null,
			loading: true,
			currentInPageNavItem: null
		};

		// const preloaded = preload(this.props.staticContext, "sample", this.props.match.params);

		// const preloadConsultation = preload(this.props.staticContext, "consultation", this.props.match.params);

		// if (preloaded) {
		// 	this.state = { document: preloaded, loading: false };
		// }
	}

	// TODO: separate this into a utility
	gatherData = async () => {
		const { consultationId, documentId, chapterSlug } = this.props.match.params;
		const documentsData =
			await load("documents", undefined, {
				consultationId
			}).then(response => response.data).catch(err => { throw new Error("arrrggghhhhh 1 " + err); });

		const chapterData =
			await load("chapter", undefined, {
				consultationId,
				documentId,
				chapterSlug
			}).then(response => response.data).catch(err => { throw new Error("arrrggghhhhh 2 " + err); });

		const consultationData =
			await load("consultation", undefined, {
				consultationId
			}).then(response => response.data).catch(err => { throw new Error("arrrggghhhhh 3 " + err); });

		return { consultationData, documentsData, chapterData };
	};

	componentDidMount() {
		if (!this.haveAllData()) {
			this.gatherData()
				.then( data =>{
					this.setState({
						...data,
						loading: false,
					});
				})
				.catch(err => { throw new Error("gatherData in componentDidMount failed " + err);});
		}
	}

	componentDidUpdate(prevProps: PropsType){
		const oldRoute = prevProps.location.pathname;
		const newRoute = this.props.location.pathname;
		if (oldRoute !== newRoute) {
			this.setState({
				loading: true
			});
			this.gatherData()
				.then( data =>{
					this.setState({
						...data,
						loading: false,
					});
				})
				.catch(err => { throw new Error("gatherData in componentDidUpdate failed " + err);});
		}
	}

	renderDocumentHtml = (data: DataType) => {
		return { __html: data };
	};

	getSupportingDocumentLinks = (documents: DocumentsType, currentDocumentFromRoute: number, currentConsultationFromRoute: number) => {
		if (!documents) return null;
		const isValidDocument = d => d.title && d.documentId;
		const isCurrentDocument = documentId => documentId === currentDocumentFromRoute;
		const documentToLinkObject = d => ({
			label: d.title,
			url: `/${currentConsultationFromRoute}/${d.documentId}/${d.chapters[0].slug}`,
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

	generateScrollspy = (sections: Array<Object>) => {
		return sections.map(section => section.slug);
	};

	inPageNav = (e: HTMLElement) => {
		if (!e) return null;
		const currentInPageNavItem = e.getAttribute("id");
		this.setState({ currentInPageNavItem });
	};

	haveAllData = () => this.state.consultationData && this.state.documentsData && this.state.chapterData;

	render() {
		if (!this.haveAllData()) return <h1>Loading...</h1>;

		const { title, reference, endDate } = this.state.consultationData;
		const { documentsData } = this.state;
		const { sections, content } = this.state.chapterData;
		const consultationId = parseInt(this.props.match.params.consultationId, 0);
		const documentId = parseInt(this.props.match.params.documentId, 0);
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
								<h2>{this.getCurrentDocumentTitle(documentsData, documentId)}</h2>
							</div>
							<StickyContainer className="grid">
								<div data-g="12 md:3">
									<StackedNav links={this.getDocumentChapterLinks(documentId)}/>
									<StackedNav links={this.getSupportingDocumentLinks(documentsData, documentId, consultationId)}/>
								</div>
								<div data-g="12 md:6">
									<div className={`document-comment-container ${this.state.loading ? "loading" : "loaded"}`}>
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
														<Scrollspy
															componentTag="ol"
															items={this.generateScrollspy(sections)}
															currentClassName="is-current"
															className="in-page-nav__list"
															role="menubar"
															onUpdate={(e)=> { this.inPageNav(e); }}
														>
															{sections.map((item, index) => {
																const props = {
																	label: item.title,
																	to: item.slug,
																	behavior: "smooth",
																	block: "start"
																};
																return (
																	<li role="presentation" className="in-page-nav__item" key={index}>
																		<HashLinkTop {...props} currentNavItem={this.state.currentInPageNavItem}/>
																	</li>
																);
															})}
														</Scrollspy>
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
