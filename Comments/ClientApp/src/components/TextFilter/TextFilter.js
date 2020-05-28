import React, { Component } from "react";
import { DebounceInput } from "react-debounce-input";
import queryString from "query-string";
import { withHistory } from "../HistoryContext/HistoryContext";
import { appendQueryParameter, removeQueryParameter, removeQuerystring } from "../../helpers/utils";

export class TextFilter extends Component {

	constructor(props) {
		super(props);

		this.state = {
			keyword: "",
		};
	}

	componentDidMount() {
		const keyword = queryString.parse(this.props.search).Keyword;
		this.setState({
			keyword,
		}, () => {
			this.props.onKeywordUpdated(keyword);
		});
	}

	removeKeyword = () => {
		this.handleKeywordChange("");
	};

	componentDidUpdate(prevProps){
		if (prevProps.keyword !== "" && this.props.keyword === ""){
			this.removeKeyword();
		}
	}

	getHref = (keyword) => {
		const pathWithoutQuerystring = removeQuerystring(this.props.path);
		const querystringWithRemovedKeyword = removeQueryParameter(this.props.search, "Keyword");

		if (keyword.length <= 0){
			return pathWithoutQuerystring + querystringWithRemovedKeyword;
		}

		const querystringWithKeywordAdded = appendQueryParameter(querystringWithRemovedKeyword, "Keyword", keyword);
		return pathWithoutQuerystring + querystringWithKeywordAdded;
	};

	handleKeywordChange = (keyword) => {
		this.setState({
			keyword,
		}, () => {
			this.props.history.push(this.getHref(keyword)); //TODO: fix the history!!!
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
						data-qa-sel="FilerByTitleOrGID"
						id="textFilter"
						value={keyword}
					/>
				</div>
			</div>
		);
	}
}

export default withHistory(TextFilter);
