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

type StateType = {
	path: string;
	consultationsData: any; // todo: return to this
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
			consultationsData: [],
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
			"consultations",
			[],
			{...this.props.match.params},
			preloadedData
		);

		if (preloadedConsultations) {
			this.state = {
				path: this.props.basename + this.props.location.pathname,
				consultationsData: preloadedConsultations,
				loading: false,
				hasInitialData: true,
				error: {
					hasError: false,
					message: null,
				},
			};
		}

	}

	render() {

		const fakeLinks = [
			{
				label: "Not",
				url: "/",
				localRoute: true,
			},
			{
				label: "Real",
				url: "/",
				localRoute: true,
			},
			{
				label: "Breadcrumbs",
				url: "/",
				localRoute: true,
			},
		];

		const { path, loading, hasInitialData, consultationsData } = this.state;

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
												<FilterPanel
													filters={TestFilters.filters}
													path={path}/>
											</div>
										</div>
										<div data-g="12 md:9">
											<h2 className="h4">All consultations</h2>
											<ul className="list--unstyled">
												{consultationsData.map((item, idx) =>
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
