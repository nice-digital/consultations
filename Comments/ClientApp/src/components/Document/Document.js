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
import { LoginBanner } from "./../LoginBanner/LoginBanner";
import { UserContext } from "../../context/UserContext";
import { Selection } from "../Selection/Selection";
// import stringifyObject from "stringify-object";

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

	getDocumentLinks = (
		getCommentable: boolean,
		title: string,
		documents: DocumentsType,
		currentDocumentFromRoute: number,
		currentConsultationFromRoute: number
	) => {
		if (!documents) return null;

		const isCurrentDocument = documentId => documentId === currentDocumentFromRoute;
		const isCommentable = d => d.supportsComments;
		const isSupporting = d => !d.supportsComments;

		const documentToLinkObject = d => {
			const label = d.title || "Download Document";
			// If it's a commentable document, get the link of the first chapter in the document, else use the href and provide a download link
			let url: string = "";

			if (d.supportsComments) {
				url = d.supportsComments ? `/${currentConsultationFromRoute}/${d.documentId}/${d.chapters[0].slug}` : d.href;
			} else {
				url = d.href || "#";
			}

			const current = isCurrentDocument(d.documentId);

			return {
				label,
				url,
				current
			};
		};

		let filteredDocuments: DocumentsType = [];

		if (getCommentable) {
			filteredDocuments = documents
				.filter(isCommentable)
				.map(documentToLinkObject);
		} else {
			filteredDocuments = documents
				.filter(isSupporting)
				.map(documentToLinkObject);
		}

		return {
			title,
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
					this.props.match.params.documentId}/${chapter.slug}`,
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
				label: "All Consultations",
				url: "#"
			},
			{
				label: "Consultation",
				url: "https://alpha.nice.org.uk/guidance/indevelopment/gid-ng10103/consultation/html-content"
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
				<UserContext.Consumer>
					{contextValue => !contextValue.isAuthorised ?
						<LoginBanner signInButton={false} currentURL={this.props.match.url}
									 signInURL={contextValue.signInURL} registerURL={contextValue.registerURL}/> : null}
				</UserContext.Consumer>
				<div className="container">
					<div className="grid">
						<div data-g="12">
							<PhaseBanner
								phase={projectInformation.phase}
								name={projectInformation.name}
								repo={projectInformation.repo}
							/>
							<BreadCrumbs links={this.getBreadcrumbs()}/>
							<main role="main">
								<div className="page-header">
									<p className="mb--0">
										Consultation |{" "}
										<button
											className="buttonAsLink"
											tabIndex={0}
											onClick={e => {
												e.preventDefault();
												this.props.onNewCommentClick({

													sourceURI: this.props.match.url,
													commentText: "",
													commentOn: "Consultation",
													quote: title
												});
											}}
										>
											Comment on whole consultation
										</button>
										&nbsp;&nbsp;
									</p>
									<h1 className="page-header__heading mt--0">{title}</h1>
									<p className="page-header__lead">
										[{reference}] Open until{" "}
										<Moment format="D MMMM YYYY" date={endDate}/>
									</p>
									<p className="mb--0">
										Document |{" "}
										<button
											className="buttonAsLink"
											tabIndex={0}
											onClick={e => {
												e.preventDefault();
												this.props.onNewCommentClick({

													sourceURI: this.props.match.url,
													commentText: "",
													commentOn: "Document",
													quote: this.getCurrentDocumentTitle(
														documentsData,
														documentId
													)
												});
											}}
										>
											Comment on this document
										</button>
									</p>
									<h2 className="mt--0">
										{this.getCurrentDocumentTitle(documentsData, documentId)}
									</h2>
								</div>
								<StickyContainer className="grid">
									{/* .navColumn only present for reading mode demo */}
									<div data-g="12 md:3" className="navigationColumn">
										<StackedNav
											links={this.getDocumentChapterLinks(documentId)}
										/>
										<StackedNav
											links={this.getDocumentLinks(
												true,
												"Other commentable documents in this consultation",
												documentsData,
												documentId,
												consultationId
											)}
										/>
										<StackedNav
											links={this.getDocumentLinks(
												false,
												"Supporting documents",
												documentsData,
												documentId,
												consultationId
											)}
										/>
									</div>
									<div data-g="12 md:6" className="documentColumn">
										<div
											className={`document-comment-container ${
												this.state.loading ? "loading" : ""}`}
										>
											<Selection newCommentFunc={this.props.onNewCommentClick}
													   sourceURI={this.props.match.url}>
												{processDocumentHtml(
													content,
													this.props.onNewCommentClick,
													this.props.match.url
												)}
											</Selection>
										</div>
									</div>
									<div data-g="12 md:3" className="inPageNavColumn">
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
																		to: `#${item.slug}`,
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
							</main>
						</div>
					</div>
				</div>
			</Fragment>
		);
	}
}

export default withRouter(Document);
