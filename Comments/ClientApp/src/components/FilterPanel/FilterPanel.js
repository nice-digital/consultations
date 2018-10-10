// @flow

import React, { Component } from "react";

import { FilterGroup } from "../FilterGroup/FilterGroup";

type PropsType = {
	filters: ReviewFilterGroupType[],
	path: string
};

type StateType = {
	canUseDOM: boolean
};

export class FilterPanel extends Component<PropsType, StateType> {

	constructor(props: PropsType) {
		super(props);

		this.state = {
			canUseDOM: false,
		};
	}

	componentDidMount() {
		this.setState({canUseDOM: true});
	}

	getFilterGroupsToDisplay(): ReviewFilterGroupType[] {
		return this.props.filters
			.filter(filterGroup => filterGroup.options.some(opt => opt.isSelected || opt.filteredResultCount > 0));
	}

	render() {
		return (
			<div className="filter-panel">
				<div id="filter-panel-body"
						 className="filter-panel__body">
					{this.getFilterGroupsToDisplay().map(filterGroup =>
						<FilterGroup
							key={filterGroup.id}
							path={this.props.path}
							filterGroup={filterGroup}/>
					)}
					{!this.state.canUseDOM &&
					<button
						type="submit"
						className="btn filter-panel__submit">
						Apply filters
					</button>
					}
				</div>
			</div>
		)
	}
}

export default FilterPanel;
