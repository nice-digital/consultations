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
import { objectToQueryString } from "../../helpers/utils";
import queryString from "query-string";

export class ReviewPage extends Component {
	constructor() {
		super();

		this.state = {
			documentsList: []
		};
	}

	// type LinkType = {
	// 	label: string,
	// 	url: string,
	// 	current?: boolean
	// };
	
	// type PropsType = {
	// 	links: ?{
	// 		title: string,
	// 		links: Array<LinkType>
	// 	}
	// };

	getData() {
		load("documents", undefined, [], { consultationId: 1 })
			.then(response => {
				this.setState({
					documentsList: response.data
				});
			})
			.catch(err => {
				throw new Error("documentsData " + err);
			});
	}

	generateDocumentList = (documentsList) =>{
		let queryParams = queryString.parse(this.props.location.search);
		const documentLinks = documentsList.map(
			(document) => {
				return {
					label: document.title,
					url: `${this.props.location.pathname}?documentId=${document.documentId}`,
					current: queryParams.documentId == document.documentId ? true : false
				};
			}
		);

		return {
			title: "All comments in this consultation:",
			links: documentLinks
		};
	}

	componentDidMount(){
		this.getData();
	}

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

	onNewCommentClick = () => {
		return null;
	}

	render() {
		if (!this.state.documentsList) return <h1>Loading...</h1>;

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
									<span>Comments for review</span>

									<StickyContainer className="grid">
										<div data-g="12 md:3">
											<StackedNav links={this.generateDocumentList(this.state.documentsList)}	/>
										</div>
										<div data-g="12 md:6">
											<CommentListWithRouter isReviewPage={true}
												wrappedComponentRef={component => (this.commentList = component)}
											/>
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
