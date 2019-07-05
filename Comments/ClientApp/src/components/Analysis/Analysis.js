// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import Helmet from "react-helmet";

import { LoginBanner } from "../LoginBanner/LoginBanner";
import { UserContext } from "../../context/UserContext";
import { load } from "../../data/loader";
import { CommentBox } from "../CommentBox/CommentBox";
import { Question } from "../Question/Question";
import { KeyPhrases } from "../KeyPhrases/KeyPhrases";

type PropsType = {
	staticContext: ContextType;
	match: any;
	location: any;
};

type StateType = {
	error: ErrorType;
	loading: boolean;
	hasInitialData: boolean;
	comments: Array<CommentWithAnalysisType>, 
	questions: Array<QuestionWithAnalysisType>,
	allSentiments: Array<string>,
	allKeyPhrases: Array<KeyPhrase>,
	consultationTitle: string,
};

export class Analysis extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			editingAllowed: true,
			loading: true,
			hasInitialData: false,
			unsavedIds: [],
			error: {
				hasError: false,
				message: null,
			},
			questions: {},
			comments: {},
			consultationTitle: "",
		};
	}

	gatherData = async () => {
		const data = load(
			"analysis",
			undefined,
			[],
			{consultationId: this.props.match.params.consultationId}
		)
			.then(response => response.data)
			.catch(err => {
				this.setState({
					error: {
						hasError: true,
						message: "analysisData " + err,
					},
				});
			});
		return await data;
	};

	componentDidMount() {
		//console.log("cdm");
		if (!this.state.hasInitialData) {
			//console.log("calling gather data");
			this.gatherData()
				.then(data => {
					// console.log("data gathered:");
					// console.log(data);
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
		this.setState({
			loading: true,
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
	
	render() {
		if (!this.state.hasInitialData && this.state.loading) return <h1>Loading...</h1>;
		//const {questionsData, unsavedIds} = this.state;
		// If there's no documentId, set currentDocumentId to null
		const currentDocumentId =
			this.props.match.params.documentId === undefined ? null : this.props.match.params.documentId;
		const currentConsultationId = this.props.match.params.consultationId;

		return (
			<UserContext.Consumer>
				{(contextValue: any) => !contextValue.isAuthorised ?
					<LoginBanner
						signInButton={false}
						currentURL={this.props.match.url}
						signInURL={contextValue.signInURL}
						registerURL={contextValue.registerURL}
						signInText="to analyse a consultation"
					/>
					:
					<Fragment>
						<Helmet>
							<title>Consultation Analysis</title>
						</Helmet>
						<div className="container">
							<div className="grid">
								<div data-g="12">
									<h1 className="h3">{this.state.consultationTitle}</h1>
									<h3>Sentiments</h3>									
									{this.state.allSentiments.map((sentiment, index) => {
										return (
											<img key={index} className="sentiment header" src={`images/${sentiment}.png`}/>
										);
									})}
									<h3>Key phrases</h3>
									<KeyPhrases keyPhrases={this.state.allKeyPhrases} />
									<h3>Questions</h3>
									{this.state.questions.map((question) => {
										return (
											<ul className="AnalysisList CommentList list--unstyled mt--0 analysis-item" key={question.questionId}>
												<Question
													isUnsaved={false}
													updateUnsavedIds={this.updateUnsavedIds}
													readOnly={true}
													key={question.questionId}
													unique={`Comment${question.questionId}`}
													question={question}
													saveAnswerHandler={null}
													deleteAnswerHandler={null}
													showAnalysis={true}
												/>		
												{question.analysed ? 
													<Fragment>
														{question.answers.map((answer) => {
															return (
																<Fragment>
																	<li className="analysis-item sentiment" key={`analysis-${answer.answerId}1`}>
																		<div className="sentiments">
																			<img className="sentiment-overall" src={`images/${answer.sentiment}.png`}/>
																			<p>Individual scores: <br/>
																				<img className="sentiment" src={`images/positive.png`}/> Positive: {(answer.sentimentScorePositive * 100).toFixed(2)}% <br/>
																				<img className="sentiment" src={`images/negative.png`}/> Negative: {(answer.sentimentScoreNegative * 100).toFixed(2)}% <br/>
																				<img className="sentiment" src={`images/neutral.png`}/> Neutral: {(answer.sentimentScoreNeutral  * 100).toFixed(2)}% <br/>
																				<img className="sentiment" src={`images/mixed.png`}/> Mixed: {(answer.sentimentScoreMixed  * 100).toFixed(2)}% 
																			</p>
																		</div>
																	</li>
																	<li className="analysis-item keyphrases" key={`analysis-${answer.answerId}2`}>
																		<KeyPhrases keyPhrases={answer.keyPhrases} />
																	</li>
																</Fragment>
															);
														})}
													</Fragment>
													:
													<li className="analysis-item">This question has not been analysed</li>
												}
											</ul>
										);
									})}
									<h3>Comments</h3>
									{this.state.comments.length === 0 ? <p>No comments</p> :
										<Fragment>
											{this.state.comments.map((comment) => {
												return (
													<ul className="AnalysisList CommentList list--unstyled mt--0 analysis-item" key={`analysis-${comment.commentId}`}>
														<CommentBox
															updateUnsavedIds={this.updateUnsavedIds}
															readOnly={true}
															key={comment.commentId}
															unique={`Comment${comment.commentId}`}
															comment={comment}
															saveHandler={null}
															deleteHandler={null}
														/>
														{comment.analysed ? 
															<Fragment>
																<li className="analysis-item sentiment">
																	<div className="sentiments">
																		<img className="sentiment-overall" src={`images/${comment.sentiment}.png`}/>
																		<p>Individual scores: <br/>
																			<img className="sentiment" src={`images/positive.png`}/> Positive: {(comment.sentimentScorePositive * 100).toFixed(2)}% <br/>
																			<img className="sentiment" src={`images/negative.png`}/> Negative: {(comment.sentimentScoreNegative * 100).toFixed(2)}% <br/>
																			<img className="sentiment" src={`images/neutral.png`}/> Neutral: {(comment.sentimentScoreNeutral  * 100).toFixed(2)}% <br/>
																			<img className="sentiment" src={`images/mixed.png`}/> Mixed: {(comment.sentimentScoreMixed  * 100).toFixed(2)}% 
																		</p>
																	</div>
																</li>
																<li className="analysis-item keyphrases">
																	<KeyPhrases keyPhrases={comment.keyPhrases} />
																</li>
															</Fragment>
															:
															<li className="analysis-item">This comment has not been analysed</li>
														}
													</ul>
												);
											})}
										</Fragment>
									}
									{/* <div className="grid">
										<div data-g="12">
											main content here
										</div>
									</div> */}
								</div>
							</div>
						</div>
					</Fragment>
				}
			</UserContext.Consumer>
		);
	}
}

export default withRouter(Analysis);
