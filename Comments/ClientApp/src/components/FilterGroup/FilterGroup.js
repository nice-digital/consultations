// @flow

import React, { Component } from "react";

import { FilterOption } from "../FilterOption/FilterOption";

type PropsType = {
	path: string,
	filterGroup: ReviewFilterGroupType
};

type StateType = {
	isExpanded: boolean,
	canUseDOM: boolean
};

export class FilterGroup extends Component<PropsType, StateType> {

	constructor(props: PropsType) {
		super(props);

		this.state = {
			isExpanded: true,
			canUseDOM: false,
		};

		this.handleTitleClick = this.handleTitleClick.bind(this);
	}

	componentDidMount() {
		this.setState({
			canUseDOM: true,
			//isExpanded: this.getSelectedCount() > 0
		});
	}

	handleTitleClick() {
		this.setState(prevState => ({
			isExpanded: !prevState.isExpanded,
		  }));
	}

	getSelectedCount() {
		return this.props.filterGroup.options.filter(option => option.isSelected).length;
	}

	getOptionsToRender() {
		return this.props.filterGroup.options.filter(opt => opt.unfilteredResultCount > 0);
	}

	render() {
		// Text for showing the number of options selected
		const selectedCount = this.getSelectedCount();
		let numSelected = null;
		if(selectedCount > 0) {
			numSelected = <div className="filter-group__count">{selectedCount} selected</div>;
		}

		return <div className="filter-group">
			{this.state.canUseDOM ? (
				<button type="button"
					aria-expanded={this.state.isExpanded}
					aria-controls={`group-${this.props.filterGroup.id}`}
					className={`filter-group__heading gtm-topic-list-filter-group-${ this.state.isExpanded ? "collapse" : "expand" }`}
					onClick={this.handleTitleClick}>
					<div id={`group-title-${this.props.filterGroup.id}`}>
						{this.props.filterGroup.title}
					</div>
					{numSelected}
				</button>
			) : (
				<h3 className="filter-group__heading">
					<div id={`group-title-${this.props.filterGroup.id}`}>
						{this.props.filterGroup.title}
					</div>
					{numSelected}
				</h3>
			)}

			<div role="group"
				id={`group-${this.props.filterGroup.id}`}
				aria-hidden={!this.state.isExpanded}
				aria-labelledby={`group-title-${this.props.filterGroup.id}`}
				className="filter-group__options">
				{this.getOptionsToRender().map(opt =>
					<FilterOption key={opt.id}
						groupId={this.props.filterGroup.id}
						groupName={this.props.filterGroup.title}
						path={this.props.path}
						option={opt} />,
				)}
			</div>
		</div>;
	}
}

export default FilterGroup;
