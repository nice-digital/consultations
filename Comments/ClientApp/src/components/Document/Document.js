// @flow

import React, { Component, Fragment } from "react";
import Moment from "react-moment";
import { Helmet } from "react-helmet";
import { StickyContainer, Sticky } from "react-sticky";
import { withRouter } from "react-router";
import Scrollspy from "react-scrollspy";

import preload from "../../data/pre-loader";
import { load } from "./../../data/loader";
import { PhaseBanner } from "./../PhaseBanner/PhaseBanner";
import { BreadCrumbs } from "./../Breadcrumbs/Breadcrumbs";
import { StackedNav } from "./../StackedNav/StackedNav";
import { HashLinkTop } from "../../helpers/component-helpers";
import { projectInformation } from "../../constants";
import { processDocumentHtml } from "./process-document-html";
import Selection from "../Selection/Selection";

type PropsType = {
	staticContext?: any,
	match: any,
	location: any,
	onNewCommentClick: Function
};

type StateType = {
	loading: boolean,
	documentsData: any, // the list of other documents in this consultation
	chapterData: any, // the current chapter's details - markup and sections,
	consultationData: any, // the top level info - title etc
	currentInPageNavItem: null | string,
	hasInitialData: boolean
};

type DocumentsType = Array<Object>;

export class Document extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			chapterData: null,
			documentsData: [],
			consultationData: null,
			loading: true,
			hasInitialData: false,
			currentInPageNavItem: null
		};

		if (this.props) {
			const preloadedChapter = preload(
				this.props.staticContext,
				"chapter",
				[],
				{ ...this.props.match.params }
			);
			const preloadedDocuments = preload(
				this.props.staticContext,
				"documents",
				[],
				{ consultationId: this.props.match.params.consultationId }
			);
			const preloadedConsultation = preload(
				this.props.staticContext,
				"consultation",
				[],
				{ consultationId: this.props.match.params.consultationId }
			);

			if (preloadedChapter && preloadedDocuments && preloadedConsultation) {
				this.state = {
					chapterData: preloadedChapter,
					documentsData: preloadedDocuments,
					consultationData: preloadedConsultation,
					loading: false,
					hasInitialData: true,
					currentInPageNavItem: null
				};
			}
		}
	}

	gatherData = async () => {
		const { consultationId, documentId, chapterSlug } = this.props.match.params;

		const chapterData = load("chapter", undefined, [], {
			consultationId,
			documentId,
			chapterSlug
		})
			.then(response => response.data)
			.catch(err => {
				throw new Error("chapterData " + err);
			});

		const documentsData = load("documents", undefined, [], { consultationId })
			.then(response => response.data)
			.catch(err => {
				throw new Error("documentsData " + err);
			});

		const consultationData = load("consultation", undefined, [], {
			consultationId
		})
			.then(response => response.data)
			.catch(err => {
				throw new Error("consultationData " + err);
			});

		return {
			chapterData: await chapterData,
			documentsData: await documentsData,
			consultationData: await consultationData
		};
	};

	componentDidMount() {
		if (!this.state.hasInitialData) {
			this.gatherData()
				.then(data => {
					this.setState({
						...data,
						loading: false,
						hasInitialData: true
					});
				})
				.catch(err => {
					throw new Error("gatherData in componentDidMount failed " + err);
				});
		}
	}

	componentDidUpdate(prevProps: PropsType) {
		const oldRoute = prevProps.location.pathname;
		const newRoute = this.props.location.pathname;
		if (oldRoute !== newRoute) {
			this.setState({
				loading: true
			});
			this.gatherData()
				.then(data => {
					this.setState({
						...data,
						loading: false
					});
				})
				.catch(err => {
					throw new Error("gatherData in componentDidUpdate failed " + err);
				});
		}
	}

	getSupportingDocumentLinks = (
		documents: DocumentsType,
		currentDocumentFromRoute: number,
		currentConsultationFromRoute: number
	) => {
		if (!documents) return null;
		const isValidDocument = d => d.title && d.documentId;
		const isCurrentDocument = documentId =>
			documentId === currentDocumentFromRoute;
		const documentToLinkObject = d => ({
			label: d.title,
			url: `/${currentConsultationFromRoute}/${d.documentId}/${
				d.chapters[0].slug
			}`,
			current: isCurrentDocument(d.documentId)
		});

		const filteredDocuments: DocumentsType = documents
			.filter(isValidDocument)
			.map(documentToLinkObject);

		return {
			title: "Documents in this Consultation",
			links: filteredDocuments
		};
	};

	getDocumentChapterLinks = (documentId: number) => {
		if (!documentId) return null;

		const isCurrentDocument = d => d.documentId === parseInt(documentId, 0);

		const isCurrentChapter = slug =>
			slug === this.props.match.params.chapterSlug;

		const createChapterLink = chapter => {
			return {
				label: chapter.title,
				url: `/${this.props.match.params.consultationId}/${
					this.props.match.params.documentId
				}/${chapter.slug}`,
				current: isCurrentChapter(chapter.slug)
			};
		};

		const documents = this.state.documentsData;

		const currentDocument = documents.filter(isCurrentDocument);

		return {
			title: "Chapters in this document",
			links: currentDocument[0].chapters.map(createChapterLink)
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

	getCurrentDocumentTitle = (documents: Object, documentId: number) => {
		const matchCurrentDocument = d => d.documentId === parseInt(documentId, 0);
		const currentDocumentDetails = documents.filter(matchCurrentDocument)[0];
		return currentDocumentDetails.title;
	};

	generateScrollspy = (sections: Array<Object>): Array<Object> => {
		return sections.map(section => section.slug);
	};

	inPageNav = (e: HTMLElement) => {
		if (!e) return null;
		const currentInPageNavItem = e.getAttribute("id");
		this.setState({ currentInPageNavItem });
	};

	render() {
		if (!this.state.hasInitialData) return <h1>Loading...</h1>;

		const { title, reference, endDate } = this.state.consultationData;
		const { documentsData } = this.state;
		const { sections, content } = this.state.chapterData;
		const consultationId = parseInt(this.props.match.params.consultationId, 0);
		const documentId = parseInt(this.props.match.params.documentId, 0);
		
		return (
			<Fragment>
				<Helmet>
					<title>{title}</title>
				</Helmet>
				<div className="container">
					<div className="grid">
						<div data-g="12">
							<PhaseBanner
								phase={projectInformation.phase}
								name={projectInformation.name}
								repo={projectInformation.repo}
							/>
							<BreadCrumbs links={this.getBreadcrumbs()} />
							<div className="page-header">
								<p className="mb--0">
									Consultation |{" "}
									<a
										tabIndex={0}
										role="button"
										href="#comment-on-whole-consultation"
										onClick={e => {
											e.preventDefault();
											this.props.onNewCommentClick({
												placeholder: "Comment on this whole consultation",
												sourceURI: this.props.match.url,
												commentText: "",
												commentOn: "Consultation"
											});
										}}
									>
										Comment on whole consultation
									</a>
								</p>
								<h1 className="page-header__heading mt--0">{title}</h1>
								<p className="page-header__lead">
									[{reference}] Open until{" "}
									<Moment format="D MMMM YYYY" date={endDate} />
								</p>
								<p className="mb--0">
									Document |{" "}
									<a
										tabIndex={0}
										role="button"
										href="#comment-on-this-document"
										onClick={e => {
											e.preventDefault();
											this.props.onNewCommentClick({
												placeholder: "Comment on this document",
												sourceURI: this.props.match.url,
												commentText: "",
												commentOn: "Document"
											});
										}}
									>
										Comment on this document
									</a>
								</p>
								<h2 className="mt--0">
									{this.getCurrentDocumentTitle(documentsData, documentId)}
								</h2>
							</div>
							<StickyContainer className="grid">
								<div data-g="12 md:3">
									<StackedNav
										links={this.getDocumentChapterLinks(documentId)}
									/>
									<StackedNav
										links={this.getSupportingDocumentLinks(
											documentsData,
											documentId,
											consultationId
										)}
									/>
								</div>
								<div data-g="12 md:6">
										
									<div
										className={`document-comment-container ${
											this.state.loading ? "loading" : ""
										}`}>	
										<Selection newCommentFunc={this.props.onNewCommentClick} sourceURI={this.props.match.url}>
											{processDocumentHtml(
												content,
												this.props.onNewCommentClick,
												this.props.match.url
											)}
										</Selection>
									</div>
								</div>
								<div data-g="12 md:3">
									<Sticky disableHardwareAcceleration>
										{({ style }) => (
											<div style={style}>
												{sections.length ? (
													<nav
														className="in-page-nav"
														aria-labelledby="inpagenav-title"
													>
														<h2
															id="inpagenav-title"
															className="in-page-nav__title"
														>
															On this page
														</h2>
														<Scrollspy
															componentTag="ol"
															items={this.generateScrollspy(sections)}
															currentClassName="is-current"
															className="in-page-nav__list"
															role="menubar"
															onUpdate={e => {
																this.inPageNav(e);
															}}
														>
															{sections.map((item, index) => {
																const props = {
																	label: item.title,
																	to: item.slug,
																	behavior: "smooth",
																	block: "start"
																};
																return (
																	<li
																		role="presentation"
																		className="in-page-nav__item"
																		key={index}
																	>
																		<HashLinkTop
																			{...props}
																			currentNavItem={
																				this.state.currentInPageNavItem
																			}
																		/>
																	</li>
																);
															})}
														</Scrollspy>
													</nav>
												) : null}
											</div>
										)}
									</Sticky>
								</div>
							</StickyContainer>
						</div>
					</div>
				</div>
			</Fragment>
		);
	}
}

export default withRouter(Document);
