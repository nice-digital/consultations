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
			isPanelVisible: true,
		};
	}

	handleClick = (isPanelVisible: boolean) => {
		if (isPanelVisible)
		{
			isPanelVisible = false;
		}
		else
		{
			isPanelVisible = true;
		}

		this.setState({
			isPanelVisible: isPanelVisible,
		});
	};

	render() {
		const isPanelVisible = this.state.isPanelVisible;
		return (
			<div className="Tutoral panel mt--0 pt--b pb--b">
				<div className="container">
					<button 
						className="Tutorial"
						onClick={() => this.handleClick(isPanelVisible)}>
						{isPanelVisible ? "Hide commenting, how it works" : "Show commenting, how it works"}
					</button>
					{isPanelVisible ?
						<div>
							<h1 className="Tutorial__title pt--c">Commenting, how it works</h1>
							<div className="grid">
								<div data-g="6">
									<p className="mt--0 pr--e"><strong>Use the icon next to chapters, subsections and recommendations to comment on them</strong></p>
								</div>
								<div data-g="6">
									<p className="mt--0 pl--e"><strong>Highlight a selection of text using the cursor to make a comment on it</strong></p>
								</div>
							</div>
						</div>
						: null }
				</div>
			</div>			
		);
	}
}