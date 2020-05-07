// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import Helmet from "react-helmet";
import Cookies from "js-cookie";
//import stringifyObject from "stringify-object";

import { queryStringToObject, canUseDOM } from "../../helpers/utils";
import { UserContext } from "../../context/UserContext";
import { LoginBanner } from "../LoginBanner/LoginBanner";
import { Header } from "../Header/Header";
import { Breadcrumbs } from "../Breadcrumbs/Breadcrumbs";
import { ConsultationItem } from "../ConsultationItem/ConsultationItem";
import preload from "../../data/pre-loader";
import { FilterPanel } from "../FilterPanel/FilterPanel";
import { load } from "../../data/loader";
import { withHistory } from "../HistoryContext/HistoryContext";
import TextFilterWithHistory from "../TextFilter/TextFilter";
import { DownloadResultsInfo } from "../DownloadResultsInfo/DownloadResultsInfo";
import { Pagination } from "../Pagination/Pagination";

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
	isAuthorised: boolean,
	error: {
		hasError: boolean,
		message: string | null,
	},
	indevReturnPath: string,
	search: string,
	keywordToFilterBy: string,
	pageNumber: number,
	itemsPerPage: number
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

		let preloadedData;
		if (this.props.staticContext && this.props.staticContext.preload) {
			preloadedData = this.props.staticContext.preload.data;
		}

		const querystring = this.props.location.search;

		const isAuthorised = ((preloadedData && preloadedData.isAuthorised) || (canUseDOM() && window.__PRELOADED__ && window.__PRELOADED__["isAuthorised"]));

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
			isAuthorised: isAuthorised,
			error: {
				hasError: false,
				message: null,
			},
			indevReturnPath: "",
			search: this.props.location.search,
			keywordToFilterBy: null,
			pageNumber: 1,
			itemsPerPage: 25,
		};

		if (isAuthorised){

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
					path: this.props.basename + this.props.location.pathname + this.props.location.search,
					consultationListData: preloadedConsultations,
					loading: false,
					isAuthorised: isAuthorised,
					hasInitialData: true,
					error: {
						hasError: false,
						message: null,
					},
					indevReturnPath: preloadedConsultations.indevBasePath,
					search: this.props.location.search,
					keywordToFilterBy: null,
					pageNumber: 1,
					itemsPerPage: 25,
				};
			}
		}
	}

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
					hasInitialData: true,
					loading: false,
					indevReturnPath: response.data.indevBasePath,
					pageNumber: 1
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
				const inTenMinutes = new Date(new Date().getTime() + 10 * 60 * 1000);
				Cookies.set("documentReferrer", documentReferrer, {
					expires: inTenMinutes,
				});
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
		if (optionId === "Keyword"){
			this.setState({keywordToFilterBy: ""});
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
			if (!filters.length){
				filters = [];
			}

			filters.unshift({
				groupTitle: "Keyword",
				optionLabel: this.state.keywordToFilterBy,
				groupId: "Keyword",
				optionId: "Keyword",
			});
		}
		return filters;
	}

	getPaginateStartAndFinishPosition = (consultationsCount, pageNumber, itemsPerPage) => {
		let paginationPositions = {};

		paginationPositions.start = (pageNumber - 1) * itemsPerPage;
		paginationPositions.finish = (paginationPositions.start + itemsPerPage) <= consultationsCount ? (paginationPositions.start + itemsPerPage) : consultationsCount;

		return paginationPositions;
	}

	changeAmount = (e) => {
		let itemsPerPage = e.target.value;

		if (itemsPerPage === "all") {
			itemsPerPage = 5000003;
		}

		itemsPerPage = parseInt(itemsPerPage, 10);

		this.setState({ itemsPerPage });
		//this.setState({ pageNumber: 1 });
	}

	changePage = (e) => {
		e.preventDefault();

		let pageNumber = e.target.getAttribute("data-pager");

		if (pageNumber === "previous") {
			pageNumber = this.state.pageNumber - 1;
		}
		if (pageNumber === "next") {
			pageNumber = this.state.pageNumber + 1;
		}

		pageNumber = parseInt(pageNumber, 10);

		this.setState({ pageNumber });
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
			isAuthorised,
			pageNumber,
			itemsPerPage,
		} = this.state;

		const {
			optionFilters,
			textFilter,
		} = consultationListData;

		const consultationsToShow = this.state.consultationListData.consultations.filter(consultation => consultation.show);

		const paginationPositions = this.getPaginateStartAndFinishPosition(consultationsToShow.length, pageNumber, itemsPerPage);

		const consultationsPaginated = consultationsToShow.slice(paginationPositions.start, paginationPositions.finish);

		if (isAuthorised && loading) return <h1>Loading...</h1>;

		if (isAuthorised && !hasInitialData) return null;

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
									<div className="grid mt--d">
										<div data-g="12 md:3">
											<h2 className="h5 mt--0">Filter</h2>
											{textFilter &&
												<TextFilterWithHistory
													onKeywordUpdated={this.keywordToFilterByUpdated}
													keyword={this.state.keywordToFilterBy}
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
												paginationPositions={paginationPositions}
												appliedFilters={this.getAppliedFilters()}
												path={this.state.path}
												isLoading={this.state.loading}
												onRemoveFilter={this.removeFilter}
											/>

											{consultationsToShow.length > 0 ? (
												<ul className="list--unstyled">
													{consultationsPaginated.map((item, idx) =>
														<ConsultationItem key={idx}
															basename={this.props.basename}
															{...item}
														/>
													)}
												</ul>
											) : (
												<p>No consultations found matching supplied filters.</p>
											)}
											<Pagination
												onChangePage={this.changePage}
												onChangeAmount={this.changeAmount}
												itemsPerPage={itemsPerPage}
												consultationCount={consultationsToShow.length}
												currentPage={pageNumber}
											/>
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
