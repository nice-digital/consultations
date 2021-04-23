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
			<aside className="tutorial mt--0 pt--d pb--d">
				<div className="container">
					<button
						className="buttonAsLink"
						onClick={() => this.handleClick(isPanelVisible)}>
						{isPanelVisible ? "Hide how to comment" : "Show how to comment"}
					</button>
					{isPanelVisible &&
						<div className="grid">
							<div data-g="12 sm:7">
								<span className="h1 pt--c">How to comment</span>
								<p>Please view our 5-minute video on how to use the online commenting system.</p>
								<p>See how to:</p>
								<ul>
									<li>Create comments and respond to questions</li>
									<li>Add, edit and delete comments</li>
									<li>Create comments specific to sections of the document</li>
									<li>Review all your feedback before submitting your response</li>
								</ul>
							</div>
							<div data-g="12 sm:5">
								<div className="tutorial__video">
									<iframe src="https://www.youtube.com/embed/DILcZkxVPCY?rel=0" title="YouTube video player" frameBorder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowFullScreen></iframe>
								</div>
							</div>
						</div>
					}
				</div>
			</aside>
		);
	}
}
