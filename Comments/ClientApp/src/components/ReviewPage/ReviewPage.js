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

export class ReviewPage extends Component {
	constructor() {
		super();

		this.state = {
			documentsList: [],
			documentFilter: 1
		};
	}

	getData() {
		load("documents", undefined, [], { consultationId: 1 })
			.then(response => {
				this.setState({
					documentsList: response.data,
					documentFilter: this.getCurrentSourceURI()
				});
			})
			.catch(err => {
				throw new Error("documentsData " + err);
			});
	}

	generateDocumentList = (documentsList) =>{
		const documentLinks = documentsList.map(
			(consultationDocument) => {
				return {
					label: consultationDocument.title,
					url: `${this.props.location.pathname}?sourceURI=${consultationDocument.sourceURI}`,
					current: this.getCurrentSourceURI() === consultationDocument.sourceURI
				};
			}
		);

		return {
			title: "View comments by document",
			links: documentLinks
		};
	};

	componentDidMount(){
		this.getData();
	}

	componentDidUpdate(prevProps){
		const oldDocId = queryStringToObject(prevProps.location.search).documentId;
		const newDocId = queryStringToObject(this.props.location.search).documentId;

		if (oldDocId !== newDocId) {
			this.setState({
				documentFilter: newDocId
			});
		}
	}

	getCurrentSourceURI = () => {
		const queryParams = queryStringToObject(this.props.location.search);
		return queryParams.sourceURI;
	};

	getBreadcrumbs = () => {
		const { consultationId } = this.props.match.params;
		const consultationsFirstDocument = this.state.documentsList[0].documentId;
		const firstDocumentChapterSlug = this.state.documentsList[0].chapters[0].slug;
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
										title="Unstable angina and NSTEMI: early management"
										reference="GID-TA10232"
										endDate="31 May 2018"
										onNewCommentClick={this.onNewCommentClick()}
										url="1/1/Introduction"
									/>
									<span>Review your comments</span>
									<StickyContainer className="grid">
										<div data-g="12 md:3">
											<StackedNav links={this.generateDocumentList(this.state.documentsList)}	/>
										</div>
										<div data-g="12 md:6">
											<CommentListWithRouter isReviewPage={true} />
										</div>
										<div data-g="12 md:3">right</div>
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
