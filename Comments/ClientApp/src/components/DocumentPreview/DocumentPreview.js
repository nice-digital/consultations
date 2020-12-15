// @flow

import React, { Component, Fragment } from "react";
import { Helmet } from "react-helmet";
import { withRouter } from "react-router";

import preload from "../../data/pre-loader";
import { load } from "./../../data/loader";
import BreadCrumbsWithRouter from "./../Breadcrumbs/Breadcrumbs";
import { StackedNav } from "./../StackedNav/StackedNav";
import { processPreviewHtml } from "../../document-processing/process-preview-html";
import LoginBannerWithRouter from "./../LoginBanner/LoginBanner";
import { UserContext } from "../../context/UserContext";
import { DocumentPreviewErrorOverview } from "./DocumentPreviewErrorOverview";
import { pullFocusByQuerySelector } from "../../helpers/accessibility-helpers";
import { Header } from "../Header/Header";
import { canUseDOM } from "../../helpers/utils";

type PropsType = {
	staticContext?: any,
	match: any,
	location: any,
	onNewCommentClick: Function,
	preview: boolean
};

type StateType = {
	loading: boolean,
	documentsData: any, // the list of other documents in this consultation
	chapterData: any, // the current chapter's details - markup and sections,
	consultationData: any, // the top level info - title etc
	currentInPageNavItem: null | string,
	hasInitialData: boolean,
	onboarded: boolean,
	currentChapterDetails?: {
		title: string,
		slug: string
	},
	allowComments: boolean,
	error: {
		hasError: boolean,
		message: string | null,
	},
	isAuthorised: boolean,
};

export class DocumentPreview extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			chapterData: null,
			documentsData: [],
			consultationData: null,
			loading: true,
			hasInitialData: false,
			currentInPageNavItem: null,
			onboarded: false,
			allowComments: true,
			children: null,
			error: {
				hasError: false,
				message: null,
			},
			isAuthorised: false,
		};

		if (this.props) {
			let preloadedData = {};
			if (this.props.staticContext && this.props.staticContext.preload) {
				preloadedData = this.props.staticContext.preload.data; //this is data from Configure => SupplyData in Startup.cs. the main thing it contains for this call is the cookie for the current user.
			}

			let preloadedChapter, preloadedDocuments, preloadedConsultation;

			const isAuthorised = ((preloadedData && preloadedData.isAuthorised) || (canUseDOM() && window.__PRELOADED__ && window.__PRELOADED__["isAuthorised"]));

			if (isAuthorised){

				this.state = {
					chapterData: null,
					documentsData: [],
					consultationData: null,
					loading: true,
					hasInitialData: false,
					currentInPageNavItem: null,
					onboarded: false,
					allowComments: true,
					children: null,
					error: {
						hasError: false,
						message: null,
					},
					isAuthorised: true,
				};

				preloadedChapter = preload(
					this.props.staticContext,
					"previewchapter",
					[],
					{ ...this.props.match.params },
					preloadedData,
				);
				preloadedDocuments = preload(
					this.props.staticContext,
					"previewdraftdocuments",
					[],
					{ ...this.props.match.params },
					preloadedData,
				);

				preloadedConsultation = preload(
					this.props.staticContext,
					"draftconsultation",
					[],
					{ ...this.props.match.params },
					preloadedData,
				);

				if (preloadedChapter && preloadedDocuments && preloadedConsultation) {
					const allowComments = preloadedConsultation.supportsComments &&
						preloadedConsultation.consultationState.consultationIsOpen &&
						!preloadedConsultation.consultationState.userHasSubmitted;
					this.state = {
						chapterData: preloadedChapter,
						documentsData: preloadedDocuments,
						consultationData: preloadedConsultation,
						loading: false,
						hasInitialData: true,
						currentInPageNavItem: null,
						onboarded: false,
						allowComments: allowComments,
						error: {
							hasError: false,
							message: null,
						},
						isAuthorised: true,
					};
				}
			}
		}
	}

	gatherData = async () => {
		const { consultationId, documentId, chapterSlug, reference } = this.props.match.params;
		let chapterData;
		let documentsData;

		chapterData = load("previewchapter", undefined, [], {
			consultationId,
			documentId,
			chapterSlug,
			reference,
		})
			.then(response => response.data)
			.catch(err => {
				//throw new Error("previewChapterData " + err);
				this.setState({
					error: {
						hasError: true,
						message: "previewChapterData " + err,
					},
				});
			});

		documentsData = load("previewdraftdocuments", undefined, [], {
			consultationId,
			documentId,
			reference,
		})
			.then(response => response.data)
			.catch(err => {
				//throw new Error("previewdraftdocumentsData " + err);
				this.setState({
					error: {
						hasError: true,
						message: "previewdraftdocumentsData " + err,
					},
				});
			});

		const consultationData = load("draftconsultation", undefined, [], {
			consultationId,
			documentId,
			reference,
		})
			.then(response => response.data)
			.catch(err => {
				//throw new Error("consultationData " + err);
				this.setState({
					error: {
						hasError: true,
						message: "consultationData " + err,
					},
				});
			});

		return {
			chapterData: await chapterData,
			documentsData: await documentsData,
			consultationData: await consultationData,
		};
	};

	componentDidMount() {
		if (!this.state.hasInitialData && this.state.isAuthorised) {
			this.gatherData()
				.then(data => {
					const allowComments = data.consultationData.supportsComments &&
						data.consultationData.consultationState.consultationIsOpen &&
						!data.consultationData.consultationState.userHasSubmitted;

					this.setState({
						...data,
						loading: false,
						hasInitialData: true,
						allowComments : allowComments,
					});
					this.addChapterDetailsToSections(this.state.chapterData);
				})
				.catch(err => {
					console.log("cdm error was: " + JSON.stringify(err));
					//throw new Error("gatherData in componentDidMount failed " + err);
					this.setState({
						error: {
							hasError: true,
							message: "gatherData in componentDidMount failed  " + err,
						},
					});
				});
		}
	}

	componentDidUpdate(prevProps: PropsType) {
		const oldRoute = prevProps.location.pathname;
		const newRoute = this.props.location.pathname;
		if (oldRoute === newRoute) return;

		this.setState({
			loading: true,
		});
		if (this.state.isAuthorised){
			this.gatherData()
				.then(data => {

					this.setState({
						...data,
						loading: false,
					});
					this.addChapterDetailsToSections(this.state.chapterData);
					// once we've loaded, pull focus to the document container
					pullFocusByQuerySelector(".document-comment-container");
				})
				.catch(err => {
					console.log("cdu error was: " + JSON.stringify(err));
					//throw new Error("gatherData in componentDidUpdate failed " + err);
					this.setState({
						error: {
							hasError: true,
							message: "gatherData in componentDidUpdate failed  " + err,
						},
					});
				});
		}
	}

	getPreviewDocumentChapterLinks = (
		documentId: number,
		chapterSlug: string,
		documents: any,
		title: string,
		reference: string,
		consultationId: number | string,
	) => {
		if (!documentId) return null;
		const isCurrentDocument = d => d.documentId === parseInt(documentId, 0);
		const isCurrentChapter = slug => slug === chapterSlug;
		const createChapterLink = chapter => {
			return {
				label: chapter.title,
				url: `/preview/${reference}/consultation/${consultationId}/document/${documentId}/chapter/${chapter.slug}`,
				current: isCurrentChapter(chapter.slug),
				isReactRoute: true,
			};
		};
		const currentDocument = documents.filter(isCurrentDocument);
		return {
			title,
			links: currentDocument[0].chapters.map(createChapterLink),
		};
	};

	addChapterDetailsToSections = (chapterData: Object) => {
		const { title, slug } = this.state.chapterData;
		const chapterDetails = { title, slug };
		chapterData.sections.unshift(chapterDetails);
		this.setState({ chapterData });
	};

	render() {
		if (this.state.error.hasError) { throw new Error(this.state.error.message); }
		if (!this.state.hasInitialData && this.state.isAuthorised) return <h1>Loading...</h1>;
		const { title, reference } = this.state.consultationData || {};
		const { content } = this.state.chapterData || {};
		const documentId = parseInt(this.props.match.params.documentId, 0);
		return (
			<Fragment>
				<Helmet>
					<title>{title}</title>
				</Helmet>
				<UserContext.Consumer>
					{(contextValue: any) => !contextValue.isAuthorised ?
						<LoginBannerWithRouter signInButton={false}
									 currentURL={this.props.match.url}
									 signInURL={contextValue.signInURL}
									 registerURL={contextValue.registerURL}
									 signInText="to view the document preview"
									 allowOrganisationCodeLogin={false} />
						: 
						<div className="container">
							<div className="grid">
								<div data-g="12">
									{this.state.consultationData && this.state.consultationData.breadcrumbs &&
										<BreadCrumbsWithRouter links={this.state.consultationData.breadcrumbs}/>
									}
									<main role="main">
										<Header
											title={title}
											reference={reference}
											consultationState={this.state.consultationData.consultationState}/>
										<div className="grid">
											<div data-g="12 md:3">
												<StackedNav links={this.getPreviewDocumentChapterLinks(
													documentId,
													this.props.match.params.chapterSlug,
													this.state.documentsData,
													"Chapters in this document",
													reference,
													this.props.match.params.consultationId,

												)} />
											</div>
											<div data-g="12 md:9" className="documentColumn">
												<div
													className={`document-comment-container ${this.state.loading ? "loading" : ""}`}>
													<DocumentPreviewErrorOverview content={content} />
													{processPreviewHtml(content)}
												</div>
											</div>
										</div>
									</main>
								</div>
							</div>
						</div>						
					}
				</UserContext.Consumer>
				
			</Fragment>
		);
	}
}

export default withRouter(DocumentPreview);
