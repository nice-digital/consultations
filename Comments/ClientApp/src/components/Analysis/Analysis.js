// @flow

import React, { Component, Fragment } from "react";
import { Prompt, withRouter } from "react-router-dom";
import Helmet from "react-helmet";
import { LoginBanner } from "../LoginBanner/LoginBanner";
import { StackedNav } from "../StackedNav/StackedNav";
import { UserContext } from "../../context/UserContext";
import { load } from "../../data/loader";

type PropsType = {
	staticContext: ContextType;
	match: any;
	location: any;
};

type StateType = {
	error: ErrorType;
	loading: boolean;
	hasInitialData: boolean;
};

export class Analysis extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			editingAllowed: true,
			questionsData: {},
			loading: true,
			hasInitialData: false,
			unsavedIds: [],
			error: {
				hasError: false,
				message: null,
			},
		};
	}

	gatherData = async () => {
		//TODO: gather data!
		
		// const questionsData = load(
		// 	"questions",
		// 	undefined,
		// 	[],
		// 	{consultationId: this.props.match.params.consultationId, draft: this.props.draftProject, reference: this.props.match.params.reference}
		// )
		// 	.then(response => response.data)
		// 	.catch(err => {
		// 		this.setState({
		// 			error: {
		// 				hasError: true,
		// 				message: "questionsData " + err,
		// 			},
		// 		});
		// 	});
		// return {
		// 	questionsData: await questionsData,
		// };
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
									<h1 className="h3">title goes here</h1>
									<div className="grid">
										<div data-g="12 md:6">
											<p>remove this?</p>
										</div>
										<div data-g="12 md:6">
											<div>
												main content here
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

export default withRouter(Analysis);
