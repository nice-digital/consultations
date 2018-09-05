// @flow

import React, { Component, Fragment } from "react";
import ReactDOM from "react-dom";

type PropsType = {
	html: string,
	newCommentClickFunc: func,
	matchUrl: string,
	allowComment: boolean
};

type StateType = {
};

export class Chapter extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
		};
	}

	componentDidMount() {
		const element = ReactDOM.findDOMNode(this);
		console.log("in chapter");
		console.log(element);
		//alert(element);
		//TODO: add buttons here.
	}

	render() {
		return (<div dangerouslySetInnerHTML={{__html: this.props.html}} />);
	}
}

export default Chapter;
