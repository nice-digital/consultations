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
//import stringifyObject from "stringify-object";


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

type StateType = {
	documentsList: Array<any>,
	consultationData: any,
	userHasSubmitted: boolean,
	validToSubmit: false,
	viewSubmittedComments: boolean
};

export class ReviewPage extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			documentsList: [],
			consultationData: null,
			userHasSubmitted: false,
			viewSubmittedComments: false,
			validToSubmit: false
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

				//console.log(`data: ${stringifyObject(data.consultationData.consultationState.supportsSubmission)}`);
				this.setState({
					...data,
					userHasSubmitted: data.consultationData.consultationState.userHasSubmitted,
					validToSubmit: data.consultationData.consultationState.supportsSubmission
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
		this.commentList.submitComments();
	};

	submittedHandler = () => {
		console.log('submitted handler in reviewpage');
		this.setState({
			userHasSubmitted: true,
			validToSubmit: false,
			viewSubmittedComments: false
		});
	}

	//this validation handler code is going to have to get a bit more advance when questions are introduced, as it'll be possible
	//to answer a question on the review page and the submit button should then enable - if the consultation is open + hasn't already been submitted + all the mandatory questions are answered.
	//(plus there's the whole unsaved changes to deal with. what happens there?)
	validationHander = (validToSubmit) => {
		this.setState({
			validToSubmit: validToSubmit
		});
	}

	viewSubmittedCommentsHandler = () => {
		console.log('viewSubmittedCommentsHandler in reviewpage');
		this.setState({
			viewSubmittedComments: true
		});
	}

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
										consultationState={this.state.consultationData.consultationState}/>
									<h2 className="mt--0">{this.state.userHasSubmitted ? "Comments submitted" : "Comments for review"}</h2>

									{(this.state.userHasSubmitted && !this.state.viewSubmittedComments) ?

										<div className="hero">
											<div className="hero__container">
												<div className="hero__body">
													<div className="hero__copy">
														{/* <h1 class="hero__title">Hero title</h1> */}
														<p className="hero__intro" data-qa-sel="submitted-text">Thank you, your comments have been submitted</p>
														<div className="hero__actions">
															<button className="btn" data-qa-sel="review-submitted-comments" onClick={this.viewSubmittedCommentsHandler}>Review all submitted comments</button>
															{/* <a onClick={this.state.viewSubmittedComments = true}>Review all submitted comments</a> */}
														</div>
													</div>
												</div>
											</div>
										</div>
										:
										<StickyContainer className="grid">
											<div data-g="12 md:6 md:push:3">
												<div className="tabs" data-tabs>
													<ul className="tabs__list" role="tablist">
														<li className={`tabs__tab ${this.props.viewComments ? "" : "tabs__tab--active"}`} role="presentation">
															<button className="tabs__tab-btn" type="button" role="tab">
																Questions
															</button>
														</li>
														<li className={`tabs__tab ${this.props.viewComments ? "tabs__tab--active" : ""}`} role="presentation">
															<button className="tabs__tab-btn" type="button" role="tab">
																Comments
															</button>
														</li>
														<li className="tabs__tab" role="presentation">
															<button className="tabs__tab-btn" type="button" role="tab">
																Submit
															</button>
														</li>
													</ul>
													<div className="tabs__content">
														<div className="tabs__pane" role="tabpanel">
															<h3 className="mt--0" id="comments-column">Questions</h3>
															<CommentListWithRouter
																isReviewPage={true}
																isVisible={true}
																isSubmitted={this.state.userHasSubmitted}
																wrappedComponentRef={component => (this.commentList = component)}
																submittedHandler={this.submittedHandler}
																validationHander={this.validationHander}
																viewComments={false}/>

														</div>
														<div className="tabs__pane" role="tabpanel">
															<h3 className="mt--0" id="comments-column">Comments</h3>
															<CommentListWithRouter
																isReviewPage={true}
																isVisible={true}
																isSubmitted={this.state.userHasSubmitted}
																wrappedComponentRef={component => (this.commentList = component)}
																submittedHandler={this.submittedHandler}
																validationHander={this.validationHander}
																viewComments={true}/>
														</div>
														<div className="tabs__pane" role="tabpanel">
															<div className="hero">
																<div className="hero__container">
																	<div className="hero__body">
																		<div className="hero__copy">
																			{/* <h1 className="hero__title">Hero title</h1> */}
																			<p className="hero__intro">You are about to submit your final response to NICE</p>
																			<p>After submission you won't be able to:</p>
																			<ul>
																				<li>edit your comments further</li>
																				<li>add any extra comments.</li>
																			</ul>
																			<p>Do you want to continue?</p>
																			{/* <div className="hero__actions">
																				<a className="btn btn--cta">Yes, submit my response</a>
																				<a className="btn" target="_blank" rel="noopener external">No, keep editing</a>
																			</div> */}
																			<UserContext.Consumer>
																				{contextValue => {
																					if (contextValue.isAuthorised) {
																						return (
																							<Fragment>
																								<h3 className="mt--0">Ready to submit?</h3>
																								<button
																									disabled={!this.state.validToSubmit}
																									className="btn btn--cta"
																									data-qa-sel="submit-comment-button"
																									onClick={this.submitConsultation}
																								>
																								{this.state.userHasSubmitted ? "Comments submitted": "Submit your comments"}
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
																	</div>
																</div>
															</div>
														</div>
													</div>
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
											</div>
										</StickyContainer>
									}
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
