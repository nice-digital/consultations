// @flow

import React, { Component } from "react";
import Cookies from "js-cookie";
import { tagManager } from "../../helpers/tag-manager";

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
		if(window.CookieControl.preferenceCookies){
			Cookies.set("TutorialVisible", isPanelVisible);
		}
	};

	getCookie = () => {
		const cookieValue = Cookies.get("TutorialVisible");
		return cookieValue === "true" || cookieValue === undefined;
	};

	handleClick = (isPanelVisible: boolean) => {
		this.setCookie(!isPanelVisible);
		this.setState({isPanelVisible: !isPanelVisible},
			() => {
				tagManager({
					event: "generic",
					category: "Consultation comments page",
					action: "Clicked",
					label: `${this.state.isPanelVisible ? "Show" : "Hide"} how to comment panel button`,
				});
			});
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
					{isPanelVisible &&
					<div>
						<h1 className="pt--c">How to comment</h1>
						<div className="grid">
							<div data-g="12 sm:4">
								<p className="mt--0">Use the icon next to chapters, subsections and recommendations to comment on
									them</p>
								<img className="Tutorial__buttonImage" src="images/tutorial-1.png"
										 alt="Mouse pointer hovering over commenting icon"/>
							</div>
							<div data-g="12 sm:4">
								<p className="mt--0">You can also highlight a selection of text using the cursor to make a comment on it</p>
								<img className="Tutorial__buttonImage" src="images/tutorial-2.gif"
										 alt="Animation showing an example of highlighting text and clicking comment"/>
							</div>
							<div data-g="12 sm:4">
								<p className="mt--0">Review your comments or answers before submitting your response</p>
								<img className="Tutorial__buttonImage" src="images/tutorial-3.png"
										 alt="Indication of how to get to the review page"/>
							</div>
						</div>
					</div>
					}
				</div>
			</aside>
		);
	}
}
