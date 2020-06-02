// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import Helmet from "react-helmet";
import Cookies from "js-cookie";
//import stringifyObject from "stringify-object";

import { appendQueryParameter, removeQueryParameter, stripMultipleQueries, queryStringToObject, objectToQueryString, canUseDOM } from "../../helpers/utils";
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
		contributionFilter: Array,
		teamFilter: Array,
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

		const isAuthorised = ((preloadedData && preloadedData.isAuthorised) || (canUseDOM() && window.__PRELOADED__ && window.__PRELOADED__["isAuthorised"])),
			isAdminUser = ((preloadedData && preloadedData.isAdminUser) || (canUseDOM() && window.__PRELOADED__ && window.__PRELOADED__["isAdminUser"])),
			isTeamUser = ((preloadedData && preloadedData.isTeamUser) || (canUseDOM() && window.__PRELOADED__ && window.__PRELOADED__["isTeamUser"]));

		const querystring = this.props.location.search;

		let querystringObject = queryStringToObject(querystring);

		// add team to querystring on initial load - if user is part of a team
		if ((isTeamUser) && (!("team" in querystringObject))) {
			querystringObject.Team = "MyTeam";
		}

		// add contribution to querystring on initial load - if user is not part of a team
		if ((!(isTeamUser)) && (!("contribution" in querystringObject))) {
			querystringObject.Contribution = "HasContributed";
		}

		const pageNumber = "page" in querystringObject ? parseInt(querystringObject.page, 10) : 1;

		let itemsPerPage = "amount" in querystringObject ? querystringObject.amount : 25;

		itemsPerPage = !isNaN(itemsPerPage) ? parseInt(itemsPerPage, 10) : itemsPerPage;

		this.state = {
			searchTerm: "",
			path: "",
			consultationListData: {
				consultations: [], //if you don't set this to an empty array, the filter line below will fail in the first SSR render.
				optionFilters: [],
				textFilter: {},
				contributionFilter: [],
				teamFilter: null,
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
			pageNumber: pageNumber,
			itemsPerPage: itemsPerPage,
		};

		if (isAuthorised) {
			const preloadedConsultations = preload(
				this.props.staticContext,
				"consultationList",
				[],
				Object.assign({ relativeURL: this.props.match.url }, querystringObject),
				preloadedData
			);

			if (preloadedConsultations) {
				this.state = {
					searchTerm: "",
					path: this.props.basename + this.props.location.pathname + objectToQueryString({ ...querystringObject }),
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
					pageNumber: pageNumber,
					itemsPerPage: itemsPerPage,
				};
			}
		}
	}

	loadDataAndUpdateState = () => {
		const querystring = this.props.history.location.search;
		let querystringObject = queryStringToObject(querystring);

		const path = this.props.basename + this.props.location.pathname + this.props.history.location.search;
		this.setState({
			path,
			search: this.props.history.location.search,
		});

		load("consultationList", undefined, [], Object.assign({relativeURL: this.props.match.url}, querystringObject))
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

	unlisten = () => { };

	componentWillUnmount() {
		this.unlisten();
	}

	componentDidMount() {
		if (!this.state.hasInitialData) {
			this.loadDataAndUpdateState();
		}
		this.unlisten = this.props.history.listen(() => {
			let path = this.props.basename + this.props.location.pathname + this.props.history.location.search,
				paginationQueries = ["page", "amount"];

			path = stripMultipleQueries(path, paginationQueries);

			const statePath = stripMultipleQueries(this.state.path, paginationQueries);

			if (!path || path !== statePath) {
				this.loadDataAndUpdateState();
			}
		});

		let indevReturnPath = this.state.consultationListData.indevBasePath;
		if (typeof (document) !== "undefined") {
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
				if (cookieReferrer != null) {
					indevReturnPath = cookieReferrer;
				}
			}
		}

		this.setState({ indevReturnPath: indevReturnPath });
	}

	keywordToFilterByUpdated = (keywordToFilterBy) => {
		this.setState({ keywordToFilterBy });
	}

	removeFilter = (optionId) => {
		if (optionId === "Keyword") {
			this.setState({ keywordToFilterBy: "" });
		}
	}

	generateFilters = (filters, mapOptions) => {
		return filters
			.map(mapOptions)
			.reduce((arr, group) => arr.concat(group), []);
	}

	getAppliedFilters(): AppliedFilterType[] {
		const {
			optionFilters,
			contributionFilter,
		} = this.state.consultationListData;

		let teamFilter = [];

		const mapOptions =
			(group: ReviewFilterGroupType) => group.options
				.filter(opt => opt.isSelected)
				.map(opt => ({
					groupTitle: group.title,
					optionLabel: opt.label,
					groupId: group.id,
					optionId: opt.id,
				}));

		if (this.state.consultationListData.teamFilter) {
			teamFilter = this.state.consultationListData.teamFilter;
		}

		let filters = contributionFilter.concat(teamFilter, optionFilters);

		filters = this.generateFilters(filters, mapOptions);

		if (this.state.keywordToFilterBy) {
			if (!filters.length) {
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
		let paginationPositions = {
			start: 0,
			finish: consultationsCount,
		};

		if (!isNaN(itemsPerPage)) {
			paginationPositions.start = (pageNumber - 1) * itemsPerPage;
			paginationPositions.finish = (paginationPositions.start + itemsPerPage) <= consultationsCount ? (paginationPositions.start + itemsPerPage) : consultationsCount;
		}

		return paginationPositions;
	}

	changeAmount = (e) => {
		let itemsPerPage = e.target.value,
			pastPageRange = false,
			pageNumber = this.state.pageNumber,
			path = stripMultipleQueries(this.state.path, ["amount", "page"]);

		if (!isNaN(itemsPerPage)) {
			itemsPerPage = parseInt(itemsPerPage, 10);
			pastPageRange = (this.state.pageNumber * itemsPerPage) > (this.state.consultationListData.consultations.filter(consultation => consultation.show).length);
		}

		if ((pastPageRange) || (itemsPerPage === "all")) {
			pageNumber = 1;
		}

		path = appendQueryParameter(path, "amount", itemsPerPage.toString());
		path = appendQueryParameter(path, "page", pageNumber);

		this.setState({ itemsPerPage, path, pageNumber }, () => {
			this.props.history.push(path);
		});
	}

	changePage = (e) => {
		e.preventDefault();

		let pageNumber = e.target.getAttribute("data-pager"),
			path = removeQueryParameter(this.state.path, "page");

		if (pageNumber === "previous") {
			pageNumber = this.state.pageNumber - 1;
		}
		if (pageNumber === "next") {
			pageNumber = this.state.pageNumber + 1;
		}

		pageNumber = parseInt(pageNumber, 10);
		path = appendQueryParameter(path, "page", pageNumber);

		this.setState({ pageNumber, path }, () => {
			this.props.history.push(path);
		});
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
			contributionFilter,
			teamFilter,
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
									<Breadcrumbs links={BackToIndevLink} />
									<Header title="Download Responses" />
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
											<FilterPanel filters={contributionFilter} path={path} />
											{teamFilter &&
												<FilterPanel filters={teamFilter} path={path} />
											}
											<FilterPanel filters={optionFilters} path={path} />
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
											) : (<p>No consultations found matching supplied filters.</p>)}
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
