// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import Helmet from "react-helmet";

import { LoginBanner } from "../LoginBanner/LoginBanner";
import { NestedStackedNav } from "../NestedStackedNav/NestedStackedNav";
import { UserContext } from "../../context/UserContext";
import preload from "../../data/pre-loader";
import { load } from "../../data/loader";
import { TextQuestion } from "../QuestionTypes/TextQuestion/TextQuestion";
import { saveQuestionHandler, deleteQuestionHandler } from "../../helpers/editing-and-deleting";

type PropsType = any; // todo

type StateType = any; // todo

export class Questions extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			consultationData: null,
			documentsData: null,
			questionsData: null,
			loading: true,
			hasInitialData: false,
			error: {
				hasError: false,
				message: null,
			},
		};

		let preloadedQuestionsData;

		let preloadedData = {};
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data; //this is data from Configure => SupplyData in Startup.cs. the main thing it contains for this call is the cookie for the current user.
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

	componentDidUpdate(prevProps: PropsType) {
		const oldRoute = prevProps.location.pathname;
		const newRoute = this.props.location.pathname;
		if (oldRoute === newRoute) return;
		//	this is where we need to filter what's displayed
	}

	// numberOfQuestions = (questionsData, documentId) => {
	// 	return questionsData
	// 		.map(question => {
	// 			if (question.documentId === documentId) {
	// 				return 1;
	// 			}
	// 			return 0;
	// 		})
	// 		.reduce((total, current) => {
	// 			return total + current;
	// 		}, 0);
	// };

	getQuestionsToDisplay = (currentDocumentId: number, questionsData: Object) => {
		if (!currentDocumentId || !questionsData) return {};
		currentDocumentId = parseInt(currentDocumentId, 10);
		const isCurrentDocument = item => item.documentId === currentDocumentId;
		if (currentDocumentId === -1) return questionsData.consultationQuestions; // documentId of -1 represents consultation level questions
		return questionsData.documents.filter(isCurrentDocument)[0].documentQuestions;
	};

	createConsultationNavigation = (questionsData: Object, currentConsultationId: string, currentDocumentId: string) => {
		const supportsQuestions = document => document.supportsQuestions;
		const isCurrentRoute = (documentId) => documentId === parseInt(currentDocumentId, 10);
		const documentsList = questionsData.documents
			.filter(supportsQuestions)
			.map(consultationDocument => {
				return {
					title: consultationDocument.title,
					to: `/admin/questions/${currentConsultationId}/${consultationDocument.documentId}`,
					marker: "nope",
					current: isCurrentRoute(consultationDocument.documentId),
				};
			});
		return [
			{
				title: questionsData.consultationTitle,
				to: `/admin/questions/${currentConsultationId}/-1`,
				marker: "nope",
				current: isCurrentRoute(-1), // -1 is going to represent the consultation level questions (didn't realise there could be a documentId of 0!
				children: documentsList,
			},
		];
	};

	saveQuestion = (event: Event, question: QuestionType) => {
		saveQuestionHandler(event, question, this);
	};

	deleteQuestion = (event: Event, questionId: number) => {
		deleteQuestionHandler(event, questionId, this);
	};

	render() {
		if (!this.state.hasInitialData) return null;

		const {questionsData} = this.state;
		const currentDocumentId = this.props.match.params.documentId;
		const currentConsultationId = this.props.match.params.consultationId;
		const questionsToDisplay = this.getQuestionsToDisplay(currentDocumentId, questionsData);

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
												this.createConsultationNavigation(questionsData, currentConsultationId, currentDocumentId)
											}/>
										</div>
										<div data-g="12 md:6">
											<div>
												{currentDocumentId ?
													<Fragment>
														<button className="btn btn--cta">Add Question</button>
														{questionsToDisplay.length ?
															<ul className="list--unstyled">
																{questionsToDisplay.map(question => (
																	<TextQuestion
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

