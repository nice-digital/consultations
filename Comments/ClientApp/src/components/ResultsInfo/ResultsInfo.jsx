// @flow

import React, { Fragment } from "react";

import Sort from "./../Sort/Sort";
import AppliedFilter from "./../AppliedFilter/AppliedFilter";

type PropsType = {
	count: number,
	sortOrder: string,
	appliedFilters: TopicListAppliedFilterType[],
	path: string,
	isLoading: boolean
};

export const ResultsInfo = (props: PropsType) => (
	<div className="results-info">
		<h2 className="results-info__count" aria-live="assertive" id="results-info-count">
			{props.isLoading ? (
				<span aria-busy="true">Loading…</span>
			) : (
				<Fragment>Showing {props.count} response{props.count === 1 ? "" : "s"}</Fragment>
			)}
		</h2>
		<p className="results-info__sort hide-print">
			<Sort sortOrder={props.sortOrder} path={props.path} />
		</p>
		{props.appliedFilters.length > 0 &&
			<ul className="results-info__filters hide-print">
				{props.appliedFilters.map(filter => <AppliedFilter key={`${filter.groupId}:${filter.optionId}`} appliedFilter={filter} path={props.path} />)}
			</ul>
		}
	</div>
);

export default ResultsInfo;
