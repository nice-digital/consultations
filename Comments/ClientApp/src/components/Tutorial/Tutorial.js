// @flow

import React, { Component } from "react";
import Cookies from "js-cookie";

type PropsType = {};

type StateType = {
	isPanelVisible: boolean,
};

export class Tutorial extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			isPanelVisible: false,
		};
	}

	componentDidMount() {
		const isPanelVisible = this.getCookie();
		this.setState({isPanelVisible});		
	}

	setCookie = (isPanelVisible: boolean) => {
		Cookies.set("TutorialVisible", isPanelVisible);
	};

	getCookie = () => {
		const cookieValue = Cookies.get("TutorialVisible");
		return cookieValue === "true" || cookieValue === undefined;
	};

	handleClick = (isPanelVisible: boolean) => {
		this.setCookie(!isPanelVisible);
		this.setState({isPanelVisible: !isPanelVisible});
	};

	render() {
		const isPanelVisible = this.state.isPanelVisible;
		return (
			<aside className="Tutorial mt--0 pt--d pb--d">
				<div className="container">
					<button 
						className="buttonAsLink"
						onClick={() => this.handleClick(isPanelVisible)}>
						{isPanelVisible ? "Hide how to comment" : "Show how to comment"}
					</button>
					{isPanelVisible ?
						<div>
							<h1 className="pt--c">How to comment</h1>
							<div className="grid">
								<div data-g="12 sm:6 lg:4">
									<p className="mt--0">Use the icon next to chapters, subsections and recommendations to comment on them</p>
									<img className="Tutorial__buttonImage" src="images/tutorial_graphic_800.png" alt="Mouse pointer hovering over commenting icon" />
								</div>
								<div data-g="12 sm:6 lg:4">
									<p className="mt--0 mb--0">Highlight a selection of text using the cursor to make a comment on it</p>
									<img src="images/text-selection.gif" alt="Animation showing an example of highlighting text and clicking comment" />
								</div>
							</div>
						</div>
						: null }
				</div>
			</aside>
		);
	}
}
