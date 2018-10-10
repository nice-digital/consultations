// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import Helmet from "react-helmet";
import { UserContext } from "../../context/UserContext";
import { LoginBanner } from "../LoginBanner/LoginBanner";
import { Header } from "../Header/Header";
import { Breadcrumbs } from "../Breadcrumbs/Breadcrumbs";
import { ConsultationItem } from "./ConsultationItem/ConsultationItem";
import preload from "../../data/pre-loader";
import { FilterPanel } from "../FilterPanel/FilterPanel";
import TestFilters from "./TestFilters.json";
import { load } from "../../data/loader";

type StateType = {
	path: string;
	consultationListData: {
		consultations: [],
		optionFilters: [],
		textFilters: [],
	}; // todo: return to this
	hasInitialData: boolean;
	loading: boolean;
	error: {
		hasError: boolean;
		message: string | null;
	};
}

type PropsType = {
	staticContext: any;
	match: {
		params: Object;
		url: string;
	};
	basename: string;
	location: {
		pathname: string;
	}
}

class Download extends Component<PropsType, StateType> {

	constructor(props) {
		super(props);

		this.state = {
			path: "",
			consultationListData: [],
			hasInitialData: false,
			loading: true,
			error: {
				hasError: false,
				message: null,
			},
		};

		let preloadedData, preloadedConsultations;
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data;
		}

		preloadedConsultations = preload(
			this.props.staticContext,
			"consultationList",
			[],
			{},
			preloadedData
		);

		if (preloadedConsultations) {
			this.state = {
				path: this.props.basename + this.props.location.pathname,
				consultationListData: preloadedConsultations,
				loading: false,
				hasInitialData: true,
				error: {
					hasError: false,
					message: null,
				},
			};
		}

	}

	componentDidMount() {
		if (!this.state.hasInitialData) {
			load("consultationsList")
				.then(response => {
					this.setState({
						consultationListData: response.data,
					});
				})
				.catch(err => {
					this.setState({
						error: {
							hasError: true,
							message: "consultationsList error  " + err,
						},
					});
				});
		}
	}

	render() {

		const fakeLinks = [
			{
				label: "Back to InDev",
				url: "#",
				localRoute: false,
			},
		];

		const {path, loading, hasInitialData, consultationListData} = this.state;

		const consultations = consultationListData.consultations;

		if (!hasInitialData) return null;

		if (loading) return <h1>Loading...</h1>;

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
							<title>Download Responses</title>
						</Helmet>
						<div className="container">
							<div className="grid">
								<div data-g="12">
									<Breadcrumbs links={fakeLinks}/>
									<Header title="Download Responses"/>
									<div className="grid">
										<div data-g="12 md:3">
											<div className="mt--d">
												<h2 className="h5">Filter</h2>
												<FilterPanel
													filters={TestFilters.filters}
													path={path}/>
											</div>
										</div>
										<div data-g="12 md:9">
											<h2 className="h5">All consultations</h2>
											<ul className="list--unstyled">
												{consultations.map((item, idx) =>
													<ConsultationItem key={idx} {...item} />
												)}
											</ul>
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

export default withRouter(Download);
