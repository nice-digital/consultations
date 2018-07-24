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
		
		const footerData = load("footer")
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
					this.setState({
						loading: false,
						hasInitialData: true
					});
				})
				.catch(err => {
					throw new Error("gatherData in componentDidMount failed " + err);
				});
		}
	}

	render() {
		if (!this.state.hasInitialData) return <h1>Loading...</h1>;

		return (
			<Fragment>
				{this.state.footerHTML}
			</Fragment>
		);
	}
}

export default Footer;
