import React, { Component, Fragment } from "react";
import CommentListWithRouter from "../CommentList/CommentList";
import { withRouter } from "react-router";
import { load } from "../../data/loader";
import { Header } from "../Header/Header";
import { PhaseBanner } from "../PhaseBanner/PhaseBanner";
import { projectInformation } from "../../constants";
import { BreadCrumbs } from "../Breadcrumbs/Breadcrumbs";
import { StickyContainer, Sticky } from "react-sticky";
import { StackedNav } from "../StackedNav/StackedNav";
import { queryStringToObject } from "../../helpers/utils";
import { UserContext } from "../../context/UserContext";
import { pullFocusById } from "../../helpers/accessibility-helpers";
//import stringifyObject from "stringify-object";

type DocumentType = {
	title: string,
	sourceURI: string,
	convertedDocument: boolean
};

type ConsultationStateType = {
	consultationIsOpen: boolean,
	hasQuestions: boolean,
	consultationHasEnded: boolean,
	hasUserSuppliedAnswers: boolean,
	hasUserSuppliedComments: boolean
};

type ConsultationDataType = {
	consultationState: ConsultationStateType,
	supportsComments: boolean,
	supportsQuestions: boolean
};

type PropsType = {
	location: {
		pathname: string,
		search: string
	}
};

type StateType = {
	documentsList: Array<DocumentType>,
	consultationData: ConsultationDataType,
	userHasSubmitted: boolean,
	validToSubmit: false,
	viewSubmittedComments: boolean
};

export class ReviewListPage extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			documentsList: [],
			consultationData: null,
			userHasSubmitted: false,
			viewSubmittedComments: false,
			validToSubmit: false,
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
			consultationId, isReview: true,
		})
			.then(response => response.data)
			.catch(err => {
				throw new Error("consultationData " + err);
			});

		return {
			documentsList: await documentsData,
			consultationData: await consultationData,
		};
	};

	componentDidMount() {
		// if (!this.state.hasInitialData) {
		this.gatherData()
			.then(data => {

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

	submitConsultation = () => {
		const comments = this.commentList.getComments();
		let answersToSubmit = [];

		if (typeof(this.questionList) !== "undefined"){
			const questions = this.questionList.getQuestions();
			questions.forEach(function(question){
				if (question.answers != null){
					answersToSubmit = answersToSubmit.concat(question.answers);
				}
			});
		}
		let commentsAndAnswers = {comments: comments, answers: answersToSubmit};

		load("submit", undefined, [], {}, "POST", commentsAndAnswers, true)
			.then(() => {
				this.submittedHandler();
			})
			.catch(err => {
				console.log(err);
				if (err.response) alert(err.response.statusText);
			});

	};

	submittedHandler = () => {
		this.setState({
			userHasSubmitted: true,
			validToSubmit: false,
			viewSubmittedComments: false,
		});
	};

	//this validation handler code is going to have to get a bit more advanced when questions are introduced, as it'll be possible
	//to answer a question on the review page and the submit button should then enable - if the consultation is open + hasn't already been submitted + all the mandatory questions are answered.
	//(plus there's the whole unsaved changes to deal with. what happens there?)
	validationHander = () => {
		const comments = this.commentList.getComments();
		let hasAnswers = false;
		if (typeof(this.questionList) !== "undefined"){
			const questions = this.questionList.getQuestions();

			questions.forEach(function(question){
				if (question.answers !== null && question.answers.length > 0){
					hasAnswers = true;
				}
			});
		}
		this.setState({
			validToSubmit: comments.length > 0 || hasAnswers,
		});
	};

	viewSubmittedCommentsHandler = () => {
		this.setState({
			viewSubmittedComments: true,
		});
	};

	render() {
		if (this.state.documentsList.length === 0) return <h1>Loading...</h1>;
		const { title, reference } = this.state.consultationData;	

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
							<BreadCrumbs links={this.state.consultationData.breadcrumbs}/>
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
														<p className="hero__intro" data-qa-sel="submitted-text">Thank you, your comments have been submitted.</p>
														<div className="hero__actions">
															<button className="btn" data-qa-sel="review-submitted-comments" onClick={this.viewSubmittedCommentsHandler}>Review all submitted comments</button>
														</div>
													</div>
												</div>
											</div>
										</div>
										:
										<StickyContainer className="grid">
											<div data-g="12 md:9 md:push:3">

												<CommentListWithRouter
													isReviewPage={true}
													isVisible={true}
													isSubmitted={this.state.userHasSubmitted}
													wrappedComponentRef={component => (this.questionList = component)}
													submittedHandler={this.submittedHandler}
													validationHander={this.validationHander} />
												
												{this.state.userHasSubmitted ?
													<div className="hero">
														<div className="hero__container">
															<div className="hero__body">
																<div className="hero__copy">
																	<p className="hero__intro" data-qa-sel="submitted-text">Thank you, your comments have been submitted.</p>
																	<button className="btn btn--secondary">Download all comments</button>
																</div>
															</div>
														</div>
													</div>
													:
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
																						>{this.state.userHasSubmitted ? "Comments submitted": "Submit your comments"}
																						</button>
																						<button className="btn btn--secondary">Download all comments</button>
																					</Fragment>
																				);
																			}
																		}}
																	</UserContext.Consumer>
																</div>
															</div>
														</div>
													</div>
												}
											</div>
											<div data-g="12 md:3 md:pull:9">
												<Sticky disableHardwareAcceleration>
													{({ style }) => (
														<div style={style}>
															<h3>filter panels here.</h3>
														</div>
													)}
												</Sticky>
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

export default withRouter(ReviewListPage);
