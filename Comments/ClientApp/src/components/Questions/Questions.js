// @flow
import React, {Component, Fragment} from "react";
import {withRouter} from "react-router-dom";
import Helmet from "react-helmet";

import {LoginBanner} from "../LoginBanner/LoginBanner";
import {Breadcrumbs} from "../Breadcrumbs/Breadcrumbs";
import {NestedStackedNav} from "../NestedStackedNav/NestedStackedNav";
import {UserContext} from "../../context/UserContext";
import preload from "../../data/pre-loader";
import {load} from "../../data/loader";
import {TextQuestion} from "../QuestionTypes/TextQuestion/TextQuestion";

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

		let preloadedConsultation, preloadedDocuments, preloadedQuestions;

		let { consultationId, documentId } = this.props.match.params;
		documentId = documentId !== "0" ? documentId : "";

		let preloadedData = {};
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data; //this is data from Configure => SupplyData in Startup.cs. the main thing it contains for this call is the cookie for the current user.
		}

		preloadedConsultation = preload(
			this.props.staticContext,
			"consultation",
			[],
			{
				consultationId,
				isReview: false,
			},
			preloadedData,
		);

		preloadedDocuments = preload(
			this.props.staticContext,
			"documents",
			[],
			{consultationId},
			preloadedData,
		);

		preloadedQuestions = preload(
			this.props.staticContext,
			"questions",
			[],
			{consultationId, documentId},
			preloadedData,
		);

		if (preloadedConsultation && preloadedDocuments && preloadedQuestions) {
			this.state = {
				consultationData: preloadedConsultation,
				documentsData: preloadedDocuments,
				questionsData: preloadedQuestions,
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
		let {consultationId, documentId} = this.props.match.params;

		documentId = documentId !== "0" ? documentId : "";

		const consultationData = load("consultation", undefined, [], {
			consultationId,
			isReview: false,
		})
			.then(response => response.data)
			.catch(err => {
				this.setState({
					error: {
						hasError: true,
						message: "consultationData " + err,
					},
				});
			});

		const documentsData = load("documents", undefined, [], {consultationId})
			.then(response => response.data)
			.catch(err => {
				this.setState({
					error: {
						hasError: true,
						message: "documentsData " + err,
					},
				});
			});

		const questionsData = load("questions", undefined, [], {consultationId, documentId})
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
			consultationData: await consultationData,
			documentsData: await documentsData,
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

	componentDidUpdate(prevProps){
		const oldRoute = prevProps.location.pathname;
		const newRoute = this.props.location.pathname;

		if (oldRoute === newRoute) return;

		this.gatherData()
			.then(data => {
				this.setState({...data});
			});
	}

	numberOfQuestions = (questionsData, documentId) => {
		return questionsData
			.map(question => {
				if (question.documentId === documentId) {
					return 1;
				}
				return 0;
			})
			.reduce((total, current) => {
				return total + current;
			}, 0);
	};

	createConsultationNavigation = (consultationData, documentsData, questionsData, currentConsultationId, currentDocumentId) => {
		const supportsQuestions = document => document.supportsQuestions;

		const isCurrentRoute = (consultationId, documentId) => {
			return (consultationId === parseInt(currentConsultationId, 10))
				&& (documentId === parseInt(currentDocumentId, 10));
		};

		const documentsList = documentsData
			.filter(supportsQuestions)
			.map(consultationDocument => {
				return {
					title: consultationDocument.title,
					to: `/admin/questions/${consultationDocument.consultationId}/${consultationDocument.documentId}`,
					marker: 1,
					current: isCurrentRoute(
						consultationDocument.consultationId,
						consultationDocument.documentId),
				};
			},
			);

		return [
			{
				title: consultationData.title,
				to: `/admin/questions/${consultationData.consultationId}/0`,
				marker: 1,
				current: isCurrentRoute(consultationData.consultationId, 0),
				children: documentsList,
			},
		];
	};

	render() {
		if (!this.state.hasInitialData) return null;

		const fakeLinks = [
			{
				label: "Back to In Development",
				url: "/",
				localRoute: false,
			},
			{
				label: "Add questions",
				url: this.props.match.url,
				localRoute: true,
			},
		];

		const {consultationData, documentsData, questionsData} = this.state;

		const currentDocumentId = this.props.match.params.documentId;
		const currentConsultationId = this.props.match.params.consultationId;

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
									<Breadcrumbs links={fakeLinks}/>
									<h1 className="h3">{consultationData.title}</h1>
									<div className="grid">
										<div data-g="12 md:6">
											<NestedStackedNav
												navigationStructure={
													this.createConsultationNavigation(
														consultationData,
														documentsData,
														questionsData,
														currentConsultationId,
														currentDocumentId,
													)
												}/>
										</div>
										<div data-g="12 md:6">
											<div>
												<ul>
													{questionsData.map(question => (
														<TextQuestion key={question.questionId} question={question}/>
													))}
												</ul>
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
