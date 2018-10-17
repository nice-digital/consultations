// @flow

import React, { Fragment } from "react";
import { LiveMessage } from "react-aria-live";

import AppliedFilter from "./../AppliedFilter/AppliedFilter";

type PropsType = {
	consultationCount: number,
	appliedFilters: AppliedFilterType[],
	path: string,
	isLoading: boolean
};

const ShouldShowFilter = (appliedFilters: AppliedFilterType[], filterText: string) => {
	const typeFilters = appliedFilters.filter(f => f.groupId === "Type");
	if (typeFilters.length === 1){
		//return !!typeFilters.find(f => f.optionId === filterText); //can't use find in IE.
		return (typeFilters.filter(f => f.optionId === filterText).length > 0);
	}
	return true;
};

const GetShowingText = (props: PropsType) => {

	//const appliedFilters = props.appliedFilters

	return `Showing ${props.consultationCount} consultation${props.consultationCount === 1 ? "": "s"}`;
};

export const DownloadResultsInfo = (props: PropsType) => (
	<div className="results-info">
		<h2 className="results-info__count" id="results-info-count">
			{(props.showCommentsCount || props.showQuestionsCount) &&
				props.isLoading ? (
					<span aria-busy="true">Loading…</span>
				) : (
					<Fragment>
						<span>{GetShowingText(props)}</span>
						<LiveMessage message={GetShowingText(props)} aria-live="polite" />
					</Fragment>
				)
			}
		</h2>
		{/* <p className="results-info__sort hide-print">
			<Sort sortOrder={props.sortOrder} path={props.path} />
		</p> */}
		{props.appliedFilters.length > 0 &&
			<ul className="results-info__filters hide-print">
				{props.appliedFilters.map(filter => <AppliedFilter key={`${filter.groupId}:${filter.optionId}`} appliedFilter={filter} path={props.path} />)}
			</ul>
		}
	</div>
);

export default DownloadResultsInfo;
