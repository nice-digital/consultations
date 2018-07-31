// @flow

import React, { Component } from "react";

import { removeQueryParameter, appendQueryParameter } from "./../../../helpers/utils";
import { withHistory } from "./../HistoryContext/HistoryContext";

type PropsType = {
	groupId: string,
	groupName: string,
	path: string,
	option: TopicListFilterOptionType,
	history: HistoryType
};

type StateType = {
	isSelected: boolean
};

export class FilterOption extends Component<PropsType, StateType> {

	constructor(props: PropsType) {
		super(props);

		this.state = {
			isSelected: this.props.option.isSelected
		};

		this.handleCheckboxChange = this.handleCheckboxChange.bind(this);
	}

	UNSAFE_componentWillReceiveProps(nextProps: PropsType) {
		this.setState({
			isSelected: nextProps.option.isSelected
		});
	}

	handleCheckboxChange: () => void;
	handleCheckboxChange() {
		this.setState(prevState => ({
			isSelected: !prevState.isSelected
		  }), () => {
			this.props.history.push(this.getHref());
		  });
	}

	getHref() {
		let path = removeQueryParameter(this.props.path, this.props.groupId, this.props.option.id);

		return this.state.isSelected
			? appendQueryParameter(path, this.props.groupId, this.props.option.id)
			: path;
	}

	render() {
		return (
			<label htmlFor={`filter_${this.props.groupId}_${this.props.option.id}`}
				className="filter-group__option">

				<input id={`filter_${this.props.groupId}_${this.props.option.id}`}
					type="checkbox"
					name={this.props.groupId}
					value={this.props.option.id}
					checked={this.state.isSelected}
					aria-controls="results-info-count"
					className={`gtm-topic-list-filter-${ this.state.isSelected ? "de" : "" }select`}
					title={`${this.props.groupName} - ${this.props.option.label}`}
					onChange={this.handleCheckboxChange} />

				{this.props.option.label}
			</label>
		);
	}
}

export default withHistory(FilterOption);
