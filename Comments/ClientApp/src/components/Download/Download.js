// @flow

import React, { Component, Fragment } from "react";
import { withRouter, Link } from "react-router-dom";
import { Helmet } from "react-helmet";
import Cookies from "js-cookie";
//import stringifyObject from "stringify-object";

import { appendQueryParameter, removeQueryParameter, stripMultipleQueries, queryStringToObject, objectToQueryString, canUseDOM } from "../../helpers/utils";
import { UserContext } from "../../context/UserContext";
import LoginBannerWithRouter from "../LoginBanner/LoginBanner";
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
		hiddenConsultationsFilter: Array,
		indevBasePath: string,
	};
	hasInitialData: boolean,
	loading: boolean,
	isAuthorised: boolean,
	isAdminUser: boolean,
	error: {
		hasError: boolean,
		message: string | null,
	},
	indevReturnPath: string | null,
	search: string,
	keywordToFilterBy: string,
	pageNumber: number,
	itemsPerPage: number,
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
			isTeamUser = ((preloadedData && preloadedData.isTeamUser) || (canUseDOM() && window.__PRELOADED__ && window.__PRELOADED__["isTeamUser"])),
			isStandardUser = !isAdminUser && !isTeamUser;

		let querystringObject = queryStringToObject(this.props.location.search);

		let itemsPerPage = "amount" in querystringObject ? querystringObject.amount : 25;
		itemsPerPage = !isNaN(itemsPerPage) ? parseInt(itemsPerPage, 10) : itemsPerPage;

		const pageNumber = "page" in querystringObject ? parseInt(querystringObject.page, 10) : 1;

		this.state = {
			searchTerm: "",
			path: "",
			consultationListData: {
				consultations: [], //if you don't set this to an empty array, the filter line below will fail in the first SSR render.
				optionFilters: [],
				textFilter: {},
				contributionFilter: [],
				teamFilter: null,
				hiddenConsultationsFilter: [],
				indevBasePath: "",
			},
			hasInitialData: false,
			loading: true,
			isAuthorised: isAuthorised,
			isAdminUser: isAdminUser,
			isTeamUser: isTeamUser,
			isStandardUser: isStandardUser,
			error: {
				hasError: false,
				message: null,
			},
			indevReturnPath: null,
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
				Object.assign({ relativeURL: this.props.match.url }, querystringObject, {initialPageView: !this.state.hasInitialData}),
				preloadedData,
			);

			if (preloadedConsultations) {
				querystringObject = this.setInitialPath(preloadedConsultations, querystringObject);

				this.state = {
					searchTerm: "",
					path: this.props.basename + this.props.location.pathname + objectToQueryString({ ...querystringObject }),
					consultationListData: preloadedConsultations,
					loading: false,
					isAuthorised: isAuthorised,
					isAdminUser: isAdminUser,
					isTeamUser: isTeamUser,
					isStandardUser: isStandardUser,
					hasInitialData: true,
					error: {
						hasError: false,
						message: null,
					},
					indevReturnPath: null,
					search: this.props.location.search,
					keywordToFilterBy: null,
					pageNumber: pageNumber,
					itemsPerPage: itemsPerPage,
				};
			}
		}
	}

	setIndevReturnPath = () => {
		let indevReturnPath = null;

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
		return indevReturnPath;
	};

	loadDataAndUpdateState = () => {
		let querystringObject = queryStringToObject(this.props.history.location.search);

		const indevReturnPath = this.setIndevReturnPath();

		this.setState({
			path: this.props.basename + this.props.location.pathname + this.props.history.location.search,
			search: this.props.history.location.search,
		});

		load("consultationList", undefined, [], Object.assign({ relativeURL: this.props.match.url }, querystringObject, {initialPageView: !this.state.hasInitialData}))
			.then(response => {
				querystringObject = this.setInitialPath(response.data, querystringObject);

				const path = this.props.basename + this.props.location.pathname + objectToQueryString({ ...querystringObject });

				this.setState({
					consultationListData: response.data,
					hasInitialData: true,
					loading: false,
					indevReturnPath: indevReturnPath,
					pageNumber: 1,
					path,
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

	setInitialPath = (data, querystringObject) => {
		if (data.contribution) {
			querystringObject.Contribution = "HasContributed";
		}

		if (data.team) {
			querystringObject.Team = "MyTeam";
		}

		return querystringObject;
	};

	unlisten = () => { };

	componentWillUnmount() {
		this.unlisten();
	}

	componentDidMount() {
		let setIndevReturnPathCalled = false;

		if (!this.state.hasInitialData) {
			this.loadDataAndUpdateState();
			setIndevReturnPathCalled = true;
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

		if (!setIndevReturnPathCalled) {
			const indevReturnPath = this.setIndevReturnPath();
			this.setState({ indevReturnPath });
		}
	}

	keywordToFilterByUpdated = (keywordToFilterBy) => {
		this.setState({ keywordToFilterBy });
	};

	removeFilter = (optionId) => {
		if (optionId === "Keyword") {
			this.setState({ keywordToFilterBy: "" });
		}
	};

	generateFilters = (filters, mapOptions) => {
		return filters
			.map(mapOptions)
			.reduce((arr, group) => arr.concat(group), []);
	};

	getAppliedFilters(): AppliedFilterType[] {
		const {
			optionFilters,
			contributionFilter,
		} = this.state.consultationListData;

		let teamFilter = [];
		let hiddenConsultationsFilter = [];

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

		if (this.state.consultationListData.hiddenConsultationsFilter) {
			hiddenConsultationsFilter = this.state.consultationListData.hiddenConsultationsFilter;
		}

		let filters = contributionFilter.concat(teamFilter, optionFilters, hiddenConsultationsFilter);

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
	};

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
	};

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
	};

	render() {
		let breadcrumbLinkParams = [
			{
				label: "Home",
				url: "/",
				localRoute: false,
			},
			{
				label: this.state.indevReturnPath ? "Back to InDev" : "All consultations",
				url: this.state.indevReturnPath ? this.state.indevReturnPath : "/guidance/inconsultation",
				localRoute: false,
			},
		];

		if (this.state.indevReturnPath) {
			breadcrumbLinkParams.shift();
		}

		const {
			path,
			loading,
			hasInitialData,
			consultationListData,
			isAuthorised,
			isAdminUser,
			pageNumber,
			itemsPerPage,
		} = this.state;

		const {
			optionFilters,
			textFilter,
			contributionFilter,
			teamFilter,
			hiddenConsultationsFilter,
		} = consultationListData;

		const consultationsToShow = this.state.consultationListData.consultations.filter(consultation => consultation.show);

		const paginationPositions = this.getPaginateStartAndFinishPosition(consultationsToShow.length, pageNumber, itemsPerPage);

		const consultationsPaginated = consultationsToShow.slice(paginationPositions.start, paginationPositions.finish);

		if (isAuthorised && loading) return <h1>Loading...</h1>;

		if (isAuthorised && !hasInitialData) return null;

		return (
			<UserContext.Consumer>
				{(contextValue: any) => !contextValue.isAuthorised ?
					<LoginBannerWithRouter
						signInButton={false}
						currentURL={this.props.match.url}
						signInURL={contextValue.signInURL}
						registerURL={contextValue.registerURL}
						signInText="to view a list of consultations"
						allowOrganisationCodeLogin={false}
						orgFieldName="download"
					/>
					:
					<Fragment>
						<Helmet>
							<title>Consultation responses</title>
						</Helmet>
						<div className="container">
							<div className="grid">
								<div data-g="12">
									<Breadcrumbs links={breadcrumbLinkParams} />
									<main>
										<Header title="Consultation responses" />
										<p className="container container-full ml--0">
											<span className="lead">
												Only online consultations responses appear in the results below.
											</span>
											&nbsp;&nbsp;
											<Link to={"/leadinformation"}>
												Request commenting lead permission
											</Link>
										</p>
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
												{!isAdminUser &&
													<FilterPanel filters={contributionFilter} path={path} />
												}
												{teamFilter &&
													<FilterPanel filters={teamFilter} path={path} />
												}
												{hiddenConsultationsFilter &&
													<FilterPanel filters={hiddenConsultationsFilter} path={path} />
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
															/>,
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
									</main>
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
