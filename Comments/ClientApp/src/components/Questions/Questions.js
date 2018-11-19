import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import Helmet from "react-helmet";

import { LoginBanner } from "../LoginBanner/LoginBanner";
import { Breadcrumbs } from "../Breadcrumbs/Breadcrumbs";
import { NestedStackedNav } from "../NestedStackedNav/NestedStackedNav";
import { UserContext } from "../../context/UserContext";
import preload from "../../data/pre-loader";
import { load } from "../../data/loader";

type PropsType = any; // todo

type StateType = any; // todo

export class Questions extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			consultationData: null,
			loading: true,
			hasInitialData: false,
			error: {
				hasError: false,
				message: null,
			},
		};

		let preloadedConsultation, preloadedDocuments;

		let preloadedData = {};
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data; //this is data from Configure => SupplyData in Startup.cs. the main thing it contains for this call is the cookie for the current user.
		}

		preloadedConsultation = preload(
			this.props.staticContext,
			"consultation",
			[],
			{consultationId: this.props.match.params.consultationId, isReview: false},
			preloadedData,
		);

		preloadedDocuments = preload(
			this.props.staticContext,
			"documents",
			[],
			{consultationId: this.props.match.params.consultationId},
			preloadedData,
		);

		if (preloadedConsultation && preloadedDocuments) {
			this.state = {
				consultationData: preloadedConsultation,
				documentsData: preloadedDocuments,
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
		const {consultationId} = this.props.match.params;

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

		return {
			consultationData: await consultationData,
			documentsData: await documentsData,
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

	createConsultationNavigation = (consultationData, documentsData, currentConsultationId, currentDocumentId) => {
		const supportsQuestions = document => document.supportsQuestions;

		const isCurrentRoute = (consultationId, documentId) => {
			return (consultationId === parseInt(currentConsultationId))
				&& (documentId === parseInt(currentDocumentId));
		};

		const documentsList = documentsData
			.filter(supportsQuestions)
			.map(consultationDocument => {
				return {
					title: consultationDocument.title,
					to: `/admin/questions/${consultationDocument.consultationId}/${consultationDocument.documentId}`,
					marker: 2,
					current: isCurrentRoute(
						consultationDocument.consultationId,
						consultationDocument.documentId),
				};
			}
			);

		return [
			{
				title: consultationData.title,
				to: `/admin/questions/${consultationData.consultationId}/0`,
				marker: 3,
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

		const {consultationData, documentsData} = this.state;

		const currentDocumentId = this.props.match.params.documentId;
		const currentConsultationId =this.props.match.params.consultationId;

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
														currentConsultationId,
														currentDocumentId,
													)
												}/>
										</div>
										<div data-g="12 md:6">
											{!currentDocumentId &&
												<p>Choose a consultation or document to add questions</p>
											}
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
