// @flow

import React, { Component } from "react";

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
		const cookie = this.getCookie();
		const isPanelVisible = cookie !== false;
		this.setState({isPanelVisible});		
	}

	setCookie = (isPanelVisible: boolean) => {
		window.document.cookie = "isPanelVisible = " + isPanelVisible.toString();
	};

	getCookie = () => {
		const cookie = window.document.cookie;
		const cookieData = cookie.match(/isPanelVisible=(true|false);/);
		if (cookieData !== null)
		{
			const cookieValue = cookieData[0].match(/(true|false)/);
			return cookieValue[0] === "true";
		}
		return null;
	};

	handleClick = (isPanelVisible: boolean) => {
		this.setCookie(!isPanelVisible);
		this.setState({isPanelVisible: !isPanelVisible});
	};

	render() {
		const isPanelVisible = this.state.isPanelVisible;
		return (
			<div className="Tutorial mt--0 pt--b pb--b">
				<div className="container">
					<button 
						className="buttonAsLink"
						onClick={() => this.handleClick(isPanelVisible)}>
						{isPanelVisible ? "Hide how to comment" : "Show how to comment"}
					</button>
					{isPanelVisible ?
						<div>
							<h1 className="h4 pt--c">How to comment</h1>
							<div className="grid">
								<div data-g="6 lg:5">
									<p className="mt--0"><strong>Use the icon next to chapters, subsections and recommendations to comment on them</strong></p>
								</div>
								<div data-g="6 lg:push:1">
									<p className="mt--0 mb--0"><strong>Highlight a selection of text using the cursor to make a comment on it</strong></p>
									<img src="images/text-selection.gif" alt="Animation showing an example of highlighting text and clicking comment" />
								</div>
							</div>
						</div>
						: null }
				</div>
			</div>			
		);
	}
}