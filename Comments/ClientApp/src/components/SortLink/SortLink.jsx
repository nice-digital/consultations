// @flow

import React, { Component } from "react";

import { removeQueryParameter, appendQueryParameter } from "./../../helpers/utils";
import { withHistory } from "./../HistoryContext/HistoryContext";

type PropsType = {
	history: HistoryType,
	currentSortOrder: string,
	sortOrder: string,
	text: string,
	path: string
};

const SortQueryParamKey: string = "Sort";

export class SortLink extends Component<PropsType> {

	constructor(props: PropsType) {
		super(props);

		this.handleClick = this.handleClick.bind(this);
	}

	//handleClick: () => void;
	handleClick(e: DOMEvent) {
		e.preventDefault();

		this.props.history.push(this.getHref());
	}

	getHref() {
		let path = removeQueryParameter(this.props.path, SortQueryParamKey);
		return appendQueryParameter(path, SortQueryParamKey, this.props.sortOrder);
	}

	render() {
		if(this.props.sortOrder == this.props.currentSortOrder) {
			return <span>{this.props.text}</span>;
		}
		else {
			return <a href={this.getHref()} className="gtm-topic-list-sort-link" onClick={this.handleClick}>
				<span className="visually-hidden">Sort by</span> {this.props.text}
			</a>;
		}
	}
}

export default withHistory(SortLink);
