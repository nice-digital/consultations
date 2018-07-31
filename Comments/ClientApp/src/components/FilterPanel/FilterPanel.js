// @flow

import React, { Component } from "react";

import FilterGroup from "../FilterGroup/FilterGroup";

type PropsType = {
	filters: TopicListFilterGroupType[],
	path: string
};

type StateType = {
	isExpanded: boolean,
	canUseDOM: boolean
};

export class FilterPanel extends Component<PropsType, StateType> {

	constructor(props: PropsType) {
		super(props);

		this.state = {
			isExpanded: true,
			canUseDOM: false
		};

		this.handleClick = this.handleClick.bind(this);
	}

	componentDidMount() {
		this.setState({ canUseDOM: true });
	}

	handleClick: () => void;
	handleClick() {
		this.setState(prevState => ({
			isExpanded: !prevState.isExpanded
		  }));
	}

	getFilterGroupsToDisplay(): TopicListFilterGroupType[] {
		return this.props.filters
			.filter(filterGroup => filterGroup.options.some(opt => opt.isSelected || opt.filteredResultCount > 0));
	}

	render() {
		return <div className="filter-panel">
			{this.state.canUseDOM ? (
				<button type="button"
					className="filter-panel__heading"
					aria-expanded={this.state.isExpanded}
					aria-controls="filter-panel-body"
					onClick={this.handleClick}>
					Filter
				</button>
			) : (
				<h2 className="filter-panel__heading">Filter</h2>
			)}
			<div id="filter-panel-body"
				className="filter-panel__body"
				aria-hidden={!this.state.isExpanded}>

				{this.getFilterGroupsToDisplay().map(filterGroup =>
					<FilterGroup key={filterGroup.id}
						path={this.props.path}
						filterGroup={filterGroup} />
				)}

				{!this.state.canUseDOM &&
					<button type="submit"
						className="btn filter-panel__submit">
						Apply filters
					</button>
				}
			</div>
		</div>;
	}
}

export default FilterPanel;
