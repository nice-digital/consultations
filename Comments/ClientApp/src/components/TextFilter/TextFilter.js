import React, { Component } from "react";
import { DebounceInput } from "react-debounce-input";
import queryString from "query-string";

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
		console.log(this.props);
		const keyword = queryString.parse(this.props.search).Keyword;
		this.setState({
			keyword,
		});
	}

	getHref = (keyword) => {
		let path = removeQueryParameter(this.props.path, "Keyword");
		return keyword.length ?
			appendQueryParameter(path, "Keyword", keyword)
			:
			path;
	};

	handleKeywordChange = (keyword) => {
		this.setState({
			keyword,
		}, () => {
			this.props.history.push(this.getHref(keyword));
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

export default withHistory(TextFilter);
