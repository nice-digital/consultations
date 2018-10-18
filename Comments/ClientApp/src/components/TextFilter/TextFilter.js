import React, { Component } from "react";
import { DebounceInput } from "react-debounce-input";
import queryString from "query-string";
//import stringifyObject from "stringify-object";
import { withRouter } from "react-router-dom";

import { withHistory } from "../HistoryContext/HistoryContext";
import { appendQueryParameter, removeQueryParameter } from "../../helpers/utils";

export class TextFilter extends Component {

	constructor(props) {
		super(props);

		this.state = {
			keyword: "",
		};
	}

	componentDidMount() {
		//console.log(this.props);
		const keyword = queryString.parse(this.props.search).Keyword;
		this.setState({
			keyword,
		}, () => {
			this.props.onKeywordUpdated(keyword);
		});
	}

	removeKeyword = () => {
		alert('remove keyword hit');
		this.handleKeywordChange("");
	}

	// UNSAFE_componentWillReceiveProps(nextProps) {
	// 	console.log(`nextProps: ${stringifyObject(nextProps)}`);
	// 	// this.setState({
	// 	// 	isSelected: nextProps.option.isSelected
	// 	// });
	// }

	getHref = (keyword) => {
		if (keyword.length <= 0){
			return this.props.path;
		}
		const querystringWithRemovedKeyword = removeQueryParameter(this.props.search, "Keyword");
		const querystringWithKeywordAdded = appendQueryParameter(querystringWithRemovedKeyword, "Keyword", keyword);
		return this.props.path + querystringWithKeywordAdded;
	};

	handleKeywordChange = (keyword) => {
		this.setState({
			keyword,
		}, () => {
			//this.props.history.push(this.getHref(keyword));
			this.props.onKeywordUpdated(keyword);
		});
	};

	render() {
		const {keyword} = this.state;

		const title = this.props.title || "Filter by title or GID reference";

		return (
			<div className="panel">
				<div className="form__group form__group--text">
					<label htmlFor="textFilter" className="form__label">{title}</label>
					<DebounceInput
						minLength={3}
						debounceTimeout={400}
						type="text"
						onChange={e => this.handleKeywordChange(e.target.value)}
						className="form__input form__input--text"
						id="textFilter"
						value={keyword}
					/>
				</div>
			</div>
		);
	}
}

export default TextFilter;
//export default withHistory(TextFilter);
//export default withRouter(withHistory(TextFilter));
