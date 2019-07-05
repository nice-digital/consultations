// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import Helmet from "react-helmet";
import { LoginBanner } from "../LoginBanner/LoginBanner";
import { UserContext } from "../../context/UserContext";
import { load } from "../../data/loader";
import { CommentBox } from "../CommentBox/CommentBox";
import { Question } from "../Question/Question";

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
		console.log("cdm");
		if (!this.state.hasInitialData) {
			console.log("calling gather data");
			this.gatherData()
				.then(data => {

					console.log("data gathered:");
					console.log(data);

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

	// componentDidUpdate(prevProps: PropsType) {
	// 	const oldRoute = prevProps.location.pathname;
	// 	const newRoute = this.props.location.pathname;
	// 	if (oldRoute === newRoute) return;
	// 	this.setState({
	// 		loading: true,
	// 	});
	// 	this.gatherData()
	// 		.then(data => {
	// 			this.setState({
	// 				...data,
	// 				loading: false,
	// 				unsavedIds: [],
	// 			});
	// 		})
	// 		.catch(err => {
	// 			this.setState({
	// 				error: {
	// 					hasError: true,
	// 					message: "gatherData in componentDidMount failed " + err,
	// 				},
	// 			});
	// 		});
	// }
	
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
									<h2>Combined analysis for all comments and answers</h2>
									<h3>Sentiments</h3>									
									{this.state.allSentiments.map((sentiment) => {
										return (
											<img className="sentiment" src={`images/${sentiment}.png`}/>
										);
									})}
									<h3>Key phrases</h3>
									{this.state.allKeyPhrases.map((keyPhrase) => {
										return (
											<span> {keyPhrase.text} </span>
										);
									})}
									<h2>Individual comment and answer analysis</h2>

									<h3>Questions</h3>
									{this.state.questions.map((question) => {
										return (
											<Question
												isUnsaved={false}
												updateUnsavedIds={this.updateUnsavedIds}
												readOnly={!this.state.allowComments}
												key={question.questionId}
												unique={`Comment${question.questionId}`}
												question={question}
												saveAnswerHandler={this.saveAnswerHandler}
												deleteAnswerHandler={this.deleteAnswerHandler}
											/>
										);
									})}
									<h3>Comments</h3>
									{this.state.comments.length === 0 ? <p>No comments</p> :
										<ul className="CommentList list--unstyled mt--0">
											{this.state.comments.map((comment) => {
												return (
													<CommentBox
														updateUnsavedIds={this.updateUnsavedIds}
														readOnly={!this.state.allowComments}
														key={comment.commentId}
														unique={`Comment${comment.commentId}`}
														comment={comment}
														saveHandler={this.saveCommentHandler}
														deleteHandler={this.deleteCommentHandler}
													/>
												);
											})}
										</ul>
									}
									<div className="grid">
										<div data-g="12">
											main content here
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

export default withRouter(Analysis);
