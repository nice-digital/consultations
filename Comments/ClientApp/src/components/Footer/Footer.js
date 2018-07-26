// @flow

import React, { Component, Fragment } from "react";
import { withRouter } from "react-router-dom";

import preload from "../../data/pre-loader";
import { load } from "./../../data/loader";

//import stringifyObject from "stringify-object";

type PropsType = {
	staticContext?: any,
	match: any,
};

type StateType = {
	loading: boolean,
	hasInitialData: boolean,
	footerHTML: string,
};

export class Footer extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);

		this.state = {
			hasInitialData: false,
			footerHTML: ""
		};

		if (this.props) {
			const preloadedFooterHTML = preload(
				this.props.staticContext,
				"footer",
				[],
				{},
				{},
				false,
			);
			if (preloadedFooterHTML) {
				this.setFooter(preloadedFooterHTML);
			}
		}
	}

	gatherData = async () => {		
		const footerData = load("footer")
			.then(response => response.data)
			.catch(err => {
				throw new Error("chapterData " + err);
			});
		
		return {
			footerData: await footerData,
		};
	};

	setFooter = (footerHTML: string) => {
		//annoyingly the footer html contains script! so we've got some nasty code here to run it.
		// const extractedScript = /<script( type="text\/javascript")?>([\S\s]+)<\/script>/gi.exec(footerHTML);
		// if (extractedScript !== null){
		// 	footerHTML = footerHTML.replace(extractedScript[0], "");
		// }
		this.setState({
			hasInitialData: true,
			footerHTML
		});
		// if (extractedScript !== null && window){
		// 	window.eval(extractedScript[extractedScript.length - 1]); //sigh.
		// }
	}

	componentDidMount() {
		if (!this.state.hasInitialData) {
			this.gatherData()
				.then(data => {
					this.setFooter(data.footerData);
				})
				.catch(err => {
					console.error("Footer failed to load");
					//throw new Error("gatherData in componentDidMount failed " + err);
				});
		}
	}

	render() {
		if (!this.state.hasInitialData) return null;

		return (
			<div dangerouslySetInnerHTML={{
				__html: this.state.footerHTML
			}} />
		);
	}
}

export default withRouter(Footer);