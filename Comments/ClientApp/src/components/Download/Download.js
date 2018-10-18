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
import { DownloadResultsInfo } from "../DownloadResultsInfo/DownloadResultsInfo";

type StateType = {
	path: string,
	searchTerm: string,
	consultationListData: {
		consultations: Array<ConsultationListRow>,
		optionFilters: Array<OptionFilterGroup>,
		textFilter: TextFilterGroup,
		indevBasePath: string,
	};
	hasInitialData: boolean,
	loading: boolean,
	error: {
		hasError: boolean,
		message: string | null,
	},
	indevReturnPath: string,
	search: string,
	keywordToFilterBy: string
}

type PropsType = {
	staticContext: any,
	match: {
		params: any,
		url: string,
	};
	basename: string,
	location: {
		pathname: string,
		search: string,
	},
	history: HistoryType,
}

export class Download extends Component<PropsType, StateType> {
	
	constructor(props: PropsType) {
		super(props);
		
		this.johnsTextFilter = React.createRef();

		this.state = {
			searchTerm: "",
			path: "",
			consultationListData: {
				consultations: [], //if you don't set this to an empty array, the filter line below will fail in the first SSR render.
				optionFilters: [],
				textFilter: {},
				indevBasePath: "",
			},
			hasInitialData: false,
			loading: true,
			error: {
				hasError: false,
				message: null,
			},
			indevReturnPath: "",
			search: this.props.location.search,
			keywordToFilterBy: null,
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
			Object.assign({relativeURL: this.props.match.url}, queryStringToObject(querystring)),
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
				search: this.props.location.search,
				keywordToFilterBy: null,
			};
		}
		
	}

	//textFilter = () => {};

	loadDataAndUpdateState = () => {
		const querystring = this.props.history.location.search;
		const path = this.props.basename + this.props.location.pathname + this.props.history.location.search;
		this.setState({
			path,
			search: this.props.history.location.search,
		});
		load("consultationList", undefined, [], Object.assign({relativeURL: this.props.match.url}, queryStringToObject(querystring)))
			.then(response => {
				this.setState({
					consultationListData: response.data,
				});
			})
			.catch(err => { //TODO: maybe this should log?
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

	keywordToFilterByUpdated = (keywordToFilterBy) => {
		this.setState({keywordToFilterBy});
	}

	removeFilter = (optionId) => {
		console.log("todo: figure out if it's a text filter");
		if (optionId === "Keyword"){
			this.textFilter.removeKeyword();
			this.setState({keywordToFilterBy: null}, () => {
				console.log(this.textFilter);
				//this.textFilter.removeKeyword(); //TODO: this should be conditional somewhere.
			});
		}
	}

	getAppliedFilters(): AppliedFilterType[] {
		const mapOptions =
			(group: ReviewFilterGroupType) => group.options
				.filter(opt => opt.isSelected)
				.map(opt => ({
					groupTitle: group.title,
					optionLabel: opt.label,
					groupId: group.id,
					optionId: opt.id,
				}));
				
		let filters = this.state.consultationListData.optionFilters
			.map(mapOptions)
			.reduce((arr, group) => arr.concat(group), []);

		if (this.state.keywordToFilterBy){
			if (filters.length){
				filters = [];
			}

			filters.unshift({
				groupTitle: "Keyword",
				optionLabel: this.state.keywordToFilterBy,
				groupId: "Keyword",
				optionId: "Keyword",
			});
		}
		console.log(`filters:${stringifyObject(filters)}`);
		return filters;
	}

	testMe = () => {
		this.johnsTextFilter.current.removeKeyword();
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

		const consultationsToShow = this.state.consultationListData.consultations.filter(consultation => consultation.show);

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
											<button onClick={this.testMe} >Click me</button>
											{textFilter && 
												<TextFilterWithHistory
													ref={this.johnsTextFilter} 
													onKeywordUpdated={this.keywordToFilterByUpdated} 
													search={this.state.search}
													path={this.state.path}
													{...textFilter}
												/>
											}
											<FilterPanel filters={optionFilters} path={path}/>
										</div>
										<div data-g="12 md:9">
											<DownloadResultsInfo
												consultationCount={consultationsToShow.length}
												appliedFilters={this.getAppliedFilters()}
												path={this.state.path}
												isLoading={this.state.loading}
												onRemoveFilter={this.removeFilter}
											/>
											{consultationsToShow.length > 0 ? (
												<ul className="list--unstyled">
													{consultationsToShow.map((item, idx) =>
														<ConsultationItem key={idx} 
															basename={this.props.basename} 
															{...item} 
														/>
													)}
												</ul>
											) : (
												<p>No consultations found matching supplied filters.</p>
											)}
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
