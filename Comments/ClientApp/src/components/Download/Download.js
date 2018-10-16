// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import Helmet from "react-helmet";
import Cookies from "js-cookie";
import stringifyObject from "stringify-object";

import { queryStringToObject } from "../../helpers/utils";
import { UserContext } from "../../context/UserContext";
import { LoginBanner } from "../LoginBanner/LoginBanner";
import { Header } from "../Header/Header";
import { Breadcrumbs } from "../Breadcrumbs/Breadcrumbs";
import { ConsultationItem } from "./ConsultationItem/ConsultationItem";
import preload from "../../data/pre-loader";
import { FilterPanel } from "../FilterPanel/FilterPanel";
import { load } from "../../data/loader";
import { withHistory } from "../HistoryContext/HistoryContext";
import TextFilterWithHistory from "../TextFilter/TextFilter";

type StateType = {
	path: string;
	searchTerm: string;
	consultationListData: {
		consultations: Array<ConsultationListRow>,
		optionFilters: Array<OptionFilterGroup>,
		textFilter: TextFilterGroup,
		indevBasePath: string,
	};
	hasInitialData: boolean;
	loading: boolean;
	error: {
		hasError: boolean;
		message: string | null;
	};
	indevReturnPath: string;
}

type PropsType = {
	staticContext: any;
	match: {
		params: any;
		url: string;
	};
	basename: string;
	location: {
		pathname: string;
	},
	history: HistoryType,
}

export class Download extends Component<PropsType, StateType> {

	constructor(props: PropsType) {
		super(props);

		this.state = {
			searchTerm: "",
			path: "",
			consultationListData: {},
			hasInitialData: false,
			loading: true,
			error: {
				hasError: false,
				message: null,
			},
			indevReturnPath: "",
		};

		let preloadedData;
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data;
		}

		const querystring = this.props.location.search;

		const preloadedConsultations = preload(
			this.props.staticContext,
			"consultationList",
			[],
			Object.assign({relativeURL: this.props.match.url}, queryStringToObject(querystring)), //queryStringToObject(querystring),
			preloadedData
		);

		if (preloadedConsultations) {

			this.state = {
				searchTerm: "",
				path: this.props.basename + this.props.location.pathname,
				consultationListData: preloadedConsultations,
				loading: false,
				hasInitialData: true,
				error: {
					hasError: false,
					message: null,
				},
				indevReturnPath: "",
			};
		}
	}

	loadDataAndUpdateState = () => {
		const querystring = this.props.history.location.search;
		const path = this.props.basename + this.props.location.pathname + this.props.history.location.search;
		this.setState({
			path,
		});
		load("consultationList", undefined, [], Object.assign({relativeURL: this.props.match.url}, queryStringToObject(querystring)))
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
	};

	unlisten = () => {};

	componentWillUnmount(){
		this.unlisten();
	}

	componentDidMount() {
		if (!this.state.hasInitialData) {
			this.loadDataAndUpdateState();
		}
		this.unlisten = this.props.history.listen(() => {
			console.log("filter changed");
			const path = this.props.basename + this.props.location.pathname + this.props.history.location.search;
			if (!this.state.path || path !== this.state.path) {
				this.loadDataAndUpdateState();
			}
		});

		let indevReturnPath = this.state.consultationListData.indevBasePath;
		if (typeof(document) !== "undefined"){
			const documentReferrer = document.referrer;
			if (documentReferrer.toLowerCase().indexOf("indev") !== -1) {
				indevReturnPath = documentReferrer;
				Cookies.set("documentReferrer", documentReferrer);
			}
			else {
				const cookieReferrer = Cookies.get("documentReferrer");
				if (cookieReferrer != null){
					indevReturnPath = cookieReferrer;
				}
			}
		} 
		this.setState({indevReturnPath: indevReturnPath});		
	}

	render() {
		const BackToIndevLink = [
			{
				label: "Back to InDev",
				url: this.state.indevReturnPath,
				localRoute: false,
			},
		];

		const {
			path,
			loading,
			hasInitialData,
			consultationListData,
		} = this.state;

		const {
			optionFilters,
			textFilter,
		} = consultationListData;

		console.log(`SSRconsultations: ${stringifyObject(this.state.consultationListData.consultations)}`);
		const consultationsToShow = typeof(this.state.consultationListData.consultations) === "undefined" ? [] : this.state.consultationListData.consultations.filter(consultation => consultation.show);

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
									<Breadcrumbs links={BackToIndevLink}/>
									<Header title="Download Responses"/>
									<div className="grid">
										<div data-g="12 md:3">
											<h2 className="h5">Filter</h2>
											{textFilter && <TextFilterWithHistory search={this.props.location.search}
																														 path={this.props.basename + this.props.location.pathname} {...textFilter}/>}
											<FilterPanel filters={optionFilters} path={path}/>
										</div>
										<div data-g="12 md:9">
											<h2 className="h5">All consultations</h2>
											<ul className="list--unstyled">
												{consultationsToShow.map((item, idx) =>
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

export default withRouter(withHistory(Download));
