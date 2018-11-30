// @flow

import React, { Component, Fragment } from "react";
import { Prompt, withRouter } from "react-router-dom";
import Helmet from "react-helmet";

import { LoginBanner } from "../LoginBanner/LoginBanner";
import { NestedStackedNav } from "../NestedStackedNav/NestedStackedNav";
import { UserContext } from "../../context/UserContext";
import preload from "../../data/pre-loader";
import { load } from "../../data/loader";
import { TextQuestion } from "../QuestionTypes/TextQuestion/TextQuestion";
import { saveQuestionHandler, deleteQuestionHandler } from "../../helpers/editing-and-deleting";
import { updateUnsavedIds } from "../../helpers/unsaved-comments";

type PropsType = any; // todo - create flow types

type StateType = any; // todo - create flow types

export class Questions extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			questionsData: null,
			loading: true,
			hasInitialData: false,
			unsavedIds: [],
			error: {
				hasError: false,
				message: null,
			},
		};

		let preloadedQuestionsData;

		let preloadedData = {};
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data;
		}

		preloadedQuestionsData = preload(
			this.props.staticContext,
			"questions",
			[],
			{consultationId: this.props.match.params.consultationId},
			preloadedData,
		);

		if (preloadedQuestionsData) {
			this.state = {
				questionsData: preloadedQuestionsData,
				loading: false,
				hasInitialData: true,
				unsavedIds: [],
				error: {
					hasError: false,
					message: null,
				},
			};
		}
	}

	gatherData = async () => {
		const questionsData = load("questions", undefined, [], {consultationId: this.props.match.params.consultationId})
			.then(response => response.data)
			.catch(err => {
				this.setState({
					error: {
						hasError: true,
						message: "questionsData " + err,
					},
				});
			});
		return {
			questionsData: await questionsData,
		};
	};

	componentDidMount() {
		if (!this.state.hasInitialData) {
			this.setState({
				loading: true
			});
			this.gatherData()
				.then(data => {
					this.setState({
						...data,
						loading: false,
						hasInitialData: true,
					});
				})
				.catch(err => {
					this.setState({
						error: {
							hasError: true,
							message: "gatherData in componentDidMount failed " + err,
						},
					});
				});
		}
	}

	componentDidUpdate(prevProps: PropsType, prevState: StateType) {
		const oldRoute = prevProps.location.pathname;
		const newRoute = this.props.location.pathname;
		// console.log(prevState.questionsData.consultationQuestions[0], prevState.questionsData.documents[0].documentQuestions[0]);
		if (oldRoute === newRoute) return;
		this.setState({
			loading: true
		});
		this.gatherData()
			.then(data => {
				this.setState({
					...data,
					loading: false,
					unsavedIds: [],
				});
			})
			.catch(err => {
				this.setState({
					error: {
						hasError: true,
						message: "gatherData in componentDidMount failed " + err,
					},
				});
			});
	}

	getQuestionsToDisplay = (currentDocumentId: string, questionsData: Object) => {
		if (currentDocumentId === null) return;
		const isCurrentDocument = item => item.documentId.toString() === currentDocumentId;
		if (currentDocumentId === "consultation") return questionsData.consultationQuestions;
		return questionsData.documents.filter(isCurrentDocument)[0].documentQuestions;
	};

	createConsultationNavigation = (
		questionsData: Object,
		currentConsultationId: string,
		currentDocumentId: string) => {
		const doesntHaveDocumentIdOfZero = document => document.documentId !== 0; // to be replaced with convertedDocument when available on endpoint
		const isCurrentRoute = (documentId) => documentId.toString() === currentDocumentId;
		const documentsList = questionsData.documents
			.filter(doesntHaveDocumentIdOfZero)
			.map(consultationDocument => {
				return {
					title: consultationDocument.title,
					to: `/admin/questions/${currentConsultationId}/${consultationDocument.documentId}`,
					marker: consultationDocument.documentQuestions.length || null,
					current: isCurrentRoute(consultationDocument.documentId),
				};
			});
		return [
			{
				title: questionsData.consultationTitle,
				to: `/admin/questions/${currentConsultationId}/consultation`,
				marker: questionsData.consultationQuestions.length || null,
				current: isCurrentRoute("consultation"),
				children: documentsList,
			},
		];
	};

	saveQuestion = (event: Event, question: QuestionType) => {
		saveQuestionHandler(event, question, this);
	};

	deleteQuestion = (event: Event, question: QuestionType) => {
		deleteQuestionHandler(event, question, this);
	};

	updateUnsavedIds = (commentId: string, dirty: boolean) => {
		updateUnsavedIds(commentId, dirty, this);
	};

	newQuestion = (e: SyntheticEvent<HTMLElement>, consultationId: string, documentId: number | null, questionTypeId: number) => {
		const newQuestion = {
			questionId: -1,
			questionTypeId,
			documentId,
			questionText: "",
			sourceURI: "",
		};

		const questionsData = this.state.questionsData;
		let currentQuestions;

		if (documentId === null) {
			//	this is a consultation level question
			currentQuestions = questionsData.consultationQuestions;
			newQuestion.sourceURI = `consultations://./consultation/${consultationId}`;
		} else {
			// this is a document level question
			currentQuestions = questionsData.documents.filter(item => item.documentId === documentId)[0].documentQuestions;
			newQuestion.sourceURI = `consultations://./consultation/${consultationId}/document/${documentId}`;
		}

		if (currentQuestions && currentQuestions.length) {
			const existingIds = currentQuestions.map(q => q.questionId);
			const lowestExistingId = Math.min.apply(Math, existingIds);
			newQuestion.questionId = lowestExistingId >= 0 ? -1 : lowestExistingId - 1;
		}
		currentQuestions.unshift(newQuestion);
		this.setState({questionsData});
	};

	render() {
		if (!this.state.hasInitialData) return null;
		const {questionsData, unsavedIds} = this.state;
		let currentDocumentId;
		if (this.props.match.params.documentId === undefined) {
			currentDocumentId = null;
		} else {
			currentDocumentId = this.props.match.params.documentId;
		}
		const currentConsultationId = this.props.match.params.consultationId;
		const questionsToDisplay = this.getQuestionsToDisplay(currentDocumentId, questionsData);

		const textQuestionTypeId = questionsData.questionTypes[0].questionTypeId; // only one Qtype for the time being

		return (
			<UserContext.Consumer>
				{(contextValue: any) => !contextValue.isAuthorised ?
					<LoginBanner
						signInButton={false}
						currentURL={this.props.match.url}
						signInURL={contextValue.signInURL}
						registerURL={contextValue.registerURL}
						signInText="to administer a consultation"
					/>
					:
					<Fragment>
						<Prompt
							when={this.state.unsavedIds.length > 0}
							message={`You have ${unsavedIds.length} unsaved ${unsavedIds.length === 1 ? "change" : "changes"}. Continue without saving?`}
						/>
						<Helmet>
							<title>Set Questions</title>
						</Helmet>
						<div className="container">
							<div className="grid">
								<div data-g="12">
									<h1 className="h3">{questionsData.consultationTitle}</h1>
									<div className="grid">
										<div data-g="12 md:6">
											<NestedStackedNav navigationStructure={
												this.createConsultationNavigation(
													questionsData,
													currentConsultationId,
													currentDocumentId)
											}/>
										</div>
										<div data-g="12 md:6">
											<div>
												{currentDocumentId ?
													<Fragment>
														<button
															className="btn btn--cta"
															onClick={(e) => {
																if (currentDocumentId === "consultation") {
																	this.newQuestion(e, currentConsultationId, null, textQuestionTypeId);
																} else {
																	this.newQuestion(e, currentConsultationId, parseInt(currentDocumentId, 10), textQuestionTypeId);
																}
															}}
														>Add text response question
														</button>
														{questionsToDisplay && questionsToDisplay.length ?
															<ul className="list--unstyled">
																{questionsToDisplay.map(question => (
																	<TextQuestion
																		updateUnsavedIds={this.updateUnsavedIds}
																		key={question.questionId}
																		question={question}
																		saveQuestion={this.saveQuestion}
																		deleteQuestion={this.deleteQuestion}
																	/>
																))}
															</ul> : <p>Click button to add a question.</p>
														}
													</Fragment>
													:
													<p>Choose consulation title or document to add questions.</p>
												}
											</div>
										</div>
									</div>
								</div>
							</div>
						</div>
					</Fragment>
				}
			</UserContext.Consumer>
		);
	}
}

export default withRouter(Questions);

