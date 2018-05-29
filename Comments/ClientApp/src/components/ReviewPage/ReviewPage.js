import React, { Component, Fragment } from "react";
import CommentListWithRouter from "../CommentList/CommentList";
import { withRouter } from "react-router";
import { load } from "./../../data/loader";
import { Header } from "../Header/Header";
import { PhaseBanner } from "./../PhaseBanner/PhaseBanner";
import { projectInformation } from "../../constants";
import { BreadCrumbs } from "./../Breadcrumbs/Breadcrumbs";
import { StickyContainer } from "react-sticky";
import { StackedNav } from "./../StackedNav/StackedNav";
import { queryStringToObject } from "../../helpers/utils";
import { UserContext } from "../../context/UserContext";

type DocumentType = {
	title: string,
	sourceURI: string,
	supportsComments: boolean
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
			consultationData: null
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
		let documentLinks = documentsList.filter(docs => docs.supportsComments)
			.map(
				(consultationDocument) => {
					return {
						label: consultationDocument.title,
						url: `${this.props.location.pathname}?sourceURI=${consultationDocument.sourceURI}`,
						current: this.getCurrentSourceURI() === consultationDocument.sourceURI
					};
				}
			);

		documentLinks.unshift({
			label: "All document comments",
			url: this.props.location.pathname,
			current: this.getCurrentSourceURI() == null});

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

	getCurrentSourceURI = () => {
		const queryParams = queryStringToObject(this.props.location.search);
		return queryParams.sourceURI;
	};

	getBreadcrumbs = () => {
		const { consultationId } = this.props.match.params;
		const firstCommentableDocument = this.state.documentsList.filter(doc => doc.supportsComments)[0]; //todo: this whole function needs to get it's content from the feed.
		const consultationsFirstDocument = firstCommentableDocument.documentId;
		const firstDocumentChapterSlug = firstCommentableDocument.chapters[0].slug;
		return [
			{
				label: "All Consultations",
				url: "https://alpha.nice.org.uk/guidance/inconsultation" // todo: link to current consultation "consultation list page" on dev?
			},
			{
				label: "Consultation", // todo: consultation overview link to come from API endpoint
				url: "https://alpha.nice.org.uk/guidance/indevelopment/gid-dg10046/consultation/html-content" // todo: demo - hardcoded to jane's overview
			},
			{
				label: "Documents",
				url: `/${consultationId}/${consultationsFirstDocument}/${firstDocumentChapterSlug}`
			}
		];
	};

	onNewCommentClick = () => {
		return null;
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
										endDate={endDate}
										onNewCommentClick={this.onNewCommentClick()}
										url="1/1/Introduction"
									/>
									<h2 className="mt--0">Comments for review</h2>
									<StickyContainer className="grid">
										<div data-g="12 md:3">
											<StackedNav links={this.generateDocumentList(this.state.documentsList)}	/>
										</div>
										<div data-g="12 md:6">
											<h3 className="mt--0">Comments</h3>
											<CommentListWithRouter isReviewPage={true} />
										</div>
										<div data-g="12 md:3">
											<UserContext.Consumer>   
												{ contextValue => {
													if (contextValue.isAuthorised) {
														return (
															<Fragment>
																<h3 className="mt--0">Ready to submit</h3>
																<button
																	className="btn btn--cta">
																	Submit your comments
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
