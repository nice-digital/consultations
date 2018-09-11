// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";
import ReactHtmlParser from "react-html-parser";
import { load } from "./../../data/loader";

type PropsType = {
	staticContext?: any,
	match: any,
};

type StateType = {
	hasInitialData: boolean,
	footerHTML: string,
};

export class Footer extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			hasInitialData: false,
			footerHTML: "",
		};
	}

	gatherData = async () => {		
		const footerData = load("footer")
			.then(response => response.data)
			.catch(err => {
				console.error("Footer failed to load", err);
			});
		
		return {
			footerData: await footerData,
		};
	};

	componentDidMount() {
		if (!this.state.hasInitialData) {
			this.gatherData()
				.then(data => {
					this.setState({
						hasInitialData: true,
						footerHTML: data.footerData,
					});
				})
				.catch(err => {
					console.error("Footer failed to load", err);
				});
		}
	}

	//the footer includes a "[year]" string in the copyright text, which needs to be replaced with the current year.
	transformHtml = (node) => {
		if (node.type === "text" && node.data.indexOf("[year]") !== -1){
			const text = node.data.replace("[year]", new Date().getFullYear());
			return (
				<Fragment key="footer-date">{text}</Fragment>
			);
		}
	};

	render() {
		console.log("rendering footer");
		if (!this.state.hasInitialData) return null;

		return (
			<div>
				{ReactHtmlParser(this.state.footerHTML, {transform: this.transformHtml})}
			</div>			
		);
	}
}

export default withRouter(Footer);
