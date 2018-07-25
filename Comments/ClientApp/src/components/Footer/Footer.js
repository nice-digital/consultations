// @flow

import React, { Component, Fragment } from "react";
import preload from "../../data/pre-loader";
import { load } from "./../../data/loader";

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
			loading: true,
			hasInitialData: false,
			footerHTML: ""
		};

		// if (this.props) {
		// 	const preloadedFooterHTML = preload(
		// 		this.props.staticContext,
		// 		"footer",
		// 	);

		// 	if (preloadedFooterHTML) {
		// 		this.state = {
		// 			loading: false,
		// 			hasInitialData: true,
		// 			footerHTML: preloadedFooterHTML
		// 		};
		// 	}
		// }
	}

	gatherData = async () => {
		
		const footerData = load("footer", "")
			.then(response => response.data)
			.catch(err => {
				throw new Error("chapterData " + err);
			});
		
		return {
			footerData: await footerData,
		};
	};

	componentDidMount() {
		if (!this.state.hasInitialData) {
			this.gatherData()
				.then(data => {
					//annoyingly the footer html contains script! so we've got some nasty code here to run it.
					let footerHTML = data.footerData;
					const extractedScript = /<script( type="text\/javascript")?>([\S\s]+)<\/script>/gi.exec(footerHTML);
					if (extractedScript !== null){
						footerHTML = footerHTML.replace(extractedScript[0], "");
					}
					this.setState({
						loading: false,
						hasInitialData: true,
						footerHTML
					});
					if (extractedScript !== null && window){
						window.eval(extractedScript[extractedScript.length - 1]); //sigh.
					}
				})
				.catch(err => {
					throw new Error("gatherData in componentDidMount failed " + err);
				});
		}
	}

	render() {
		if (!this.state.hasInitialData) return null; // <h1>Loading...</h1>;

		return (
			<div dangerouslySetInnerHTML={{
				__html: this.state.footerHTML
			}} />
		);
	}
}

export default Footer;
