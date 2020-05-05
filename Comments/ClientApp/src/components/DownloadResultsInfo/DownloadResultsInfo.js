// @flow

import React, { Component, Fragment } from "react";
import { LiveMessage } from "react-aria-live";

import AppliedFilter from "./../AppliedFilter/AppliedFilter";

type PropsType = {
	consultationCount: number,
	appliedFilters: AppliedFilterType[],
	path: string,
	isLoading: boolean,
	onRemoveFilter: Function,
	paginationPositions: {}
};

export class DownloadResultsInfo extends Component<PropsType, StateType> {

	shouldShowFilter = (appliedFilters: AppliedFilterType[], filterText: string) => {
		const typeFilters = appliedFilters.filter(f => f.groupId === "Type");
		if (typeFilters.length === 1){
			//return !!typeFilters.find(f => f.optionId === filterText); //can't use find in IE.
			return (typeFilters.filter(f => f.optionId === filterText).length > 0);
		}
		return true;
	};

	getShowingText = (props: PropsType) => {
		//TODO: "Showing Open consultations" etc.
		//const appliedFilters = props.appliedFilters
		const paginationExtract = props.consultationCount > 0 ? `${props.paginationPositions.start + 1} to ${props.paginationPositions.finish} of`: "";

		return `Showing ${paginationExtract} ${props.consultationCount} consultation${props.consultationCount === 1 ? "": "s"}`;
	};

	onRemoveFilter = (optionId) => {
		if (typeof(this.props.onRemoveFilter) === "function"){
			this.props.onRemoveFilter(optionId);
		} else{
			console.log("problem in dri");
		}
	}

	render(){
		return (
			<div className="results-info">
				<h2 className="results-info__count" id="results-info-count">
					{(this.props.showCommentsCount || this.props.showQuestionsCount) &&
						this.props.isLoading ? (
							<span aria-busy="true">Loading…</span>
						) : (
							<Fragment>
								<span>{this.getShowingText(this.props)}</span>
								<LiveMessage message={this.getShowingText(this.props)} aria-live="polite" />
							</Fragment>
						)
					}
				</h2>
				{this.props.appliedFilters.length > 0 &&
					<ul className="results-info__filters hide-print">
						{this.props.appliedFilters.map(filter =>
							<AppliedFilter key={`${filter.groupId}:${filter.optionId}`}
								appliedFilter={filter}
								path={this.props.path}
								onRemoveFilter={this.onRemoveFilter} />)}
					</ul>
				}
			</div>
		);
	}
}

export default DownloadResultsInfo;
