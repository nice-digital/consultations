import React, { Component, Fragment } from "react";
import CommentListWithRouter from "../CommentList/CommentList";
import { withRouter } from "react-router";
import { load } from "./../../data/loader";
import { Header } from "../Header/Header";
import { PhaseBanner } from "./../PhaseBanner/PhaseBanner";
import { projectInformation } from "../../constants";
import { BreadCrumbs } from "./../Breadcrumbs/Breadcrumbs";
import { StickyContainer, Sticky } from "react-sticky";
import { StackedNav } from "./../StackedNav/StackedNav";
import { queryStringToObject } from "../../helpers/utils";
import { UserContext } from "../../context/UserContext";
import { pullFocusById } from "../../helpers/accessibility-helpers";

type DocumentType = {
	title: string,
	sourceURI: string,
	convertedDocument: boolean
};

type PropsType = {
	location: {
		pathname: string,
		search: string
	}
};

export class ReviewPage extends Component<PropsType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			documentsList: [],
			consultationData: null,
			isSubmitted: false
		};
	}

	gatherData = async () => {
		const consultationId = this.props.match.params.consultationId;

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
			documentsList: await documentsData,
			consultationData: await consultationData
		};
	};

	generateDocumentList = (documentsList: Array<DocumentType>) =>{
		let documentLinks = documentsList.filter(docs => docs.convertedDocument)
			.map(
				(consultationDocument) => {
					return {
						label: consultationDocument.title,
						url: `${this.props.location.pathname}?sourceURI=${encodeURIComponent(consultationDocument.sourceURI)}`,
						current: this.getCurrentSourceURI() === consultationDocument.sourceURI,
						isReactRoute: true
					};
				}
			);

		return {
			title: "View comments by document",
			links: documentLinks
		};
	};

	componentDidMount() {
		// if (!this.state.hasInitialData) {
		this.gatherData()
			.then(data => {
				this.setState({
					...data
				});
			})
			.catch(err => {
				throw new Error("gatherData in componentDidMount failed " + err);
			});
		// }
	}

	componentDidUpdate(prevProps: PropsType) {
		const oldQueryString = prevProps.location.search;
		const newQueryString = this.props.location.search;
		if (oldQueryString === newQueryString) return;
		pullFocusById("comments-column");
	}

	getCurrentSourceURI = () => {
		const queryParams = queryStringToObject(this.props.location.search);
		return queryParams.sourceURI;
	};

	getBreadcrumbs = () => {
		const { consultationId } = this.props.match.params;
		const firstCommentableDocument = this.state.documentsList.filter(doc => doc.convertedDocument)[0]; //todo: this whole function needs to get it's content from the feed.
		const consultationsFirstDocument = firstCommentableDocument.documentId;
		const firstDocumentChapterSlug = firstCommentableDocument.chapters[0].slug;
		return [
			{
				label: "All Consultations",
				url: "#"
			},
			{
				label: "Consultation",
				url: "https://alpha.nice.org.uk/guidance/indevelopment/gid-ng10107/consultation/html-content"
			},
			{
				label: "Documents",
				url: `/${consultationId}/${consultationsFirstDocument}/${firstDocumentChapterSlug}`
			}
		];
	};

	submitConsultation = () => {
		this.setState({
			isSubmitted: true
		});
	};

	render() {
		if (this.state.documentsList.length === 0) return <h1>Loading...</h1>;
		const { title, reference, endDate } = this.state.consultationData;
		return (
			<Fragment>
				<div className="container">
					<div className="grid">
						<div data-g="12">
							<PhaseBanner
								phase={projectInformation.phase}
								name={projectInformation.name}
								repo={projectInformation.repo}
							/>
							<BreadCrumbs links={this.getBreadcrumbs()} />
							<main role="main">
								<div className="page-header">
									<Header
										title={title}
										reference={reference}
										endDate={endDate}/>
									<h2 className="mt--0">Comments for review</h2>
									<StickyContainer className="grid">
										<div data-g="12 md:6 md:push:3">
											<h3 className="mt--0" id="comments-column">{this.state.isSubmitted ? "Comments submitted" : "Comments"}</h3>
											<CommentListWithRouter
												isReviewPage={true}
												isVisible={true}
												isSubmitted={this.state.isSubmitted}/>
										</div>
										<div data-g="12 md:3 md:pull:6">
											<Sticky disableHardwareAcceleration>
												{({ style }) => (
													<div style={style}>
														<StackedNav links={
															{
																title: "All comments in this consultation",
																links: [
																	{
																		label: title,
																		url: this.props.location.pathname,
																		current: this.getCurrentSourceURI() == null,
																		isReactRoute: true
																	}
																]
															}
														}/>
														<StackedNav
															links={this.generateDocumentList(this.state.documentsList)}/>
													</div>
												)}
											</Sticky>
										</div>
										<div data-g="12 md:3">
											<Sticky disableHardwareAcceleration>
												{({ style }) => (
													<div style={style}>
														<UserContext.Consumer>
															{contextValue => {
																if (contextValue.isAuthorised) {
																	return (
																		<Fragment>
																			<h3 className="mt--0">Ready to submit?</h3>
																			<button
																				disabled={this.state.isSubmitted}
																				className="btn btn--cta"
																				onClick={this.submitConsultation}
																			>
																				{this.state.isSubmitted ? "Comments submitted": "Submit your comments"}
																			</button>
																			<button
																				className="btn btn--secondary">
																				Download all comments
																			</button>
																		</Fragment>
																	);
																}
															}}
														</UserContext.Consumer>
													</div>
												)}
											</Sticky>
										</div>
									</StickyContainer>
								</div>
							</ main>
						</div>
					</div>
				</div>
			</Fragment>
		);
	}
}

export default withRouter(ReviewPage);
