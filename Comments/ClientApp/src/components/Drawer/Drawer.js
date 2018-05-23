// @flow

import React, { Component } from "react";
import { mobileWidth } from "../../constants";
import CommentListWithRouter from "../CommentList/CommentList";

type PropsType = {
	commentList: Function
};

type StateType = {
	drawerExpandedWidth: boolean,
	drawerOpen: boolean,
	drawerMobile: boolean
};

export class Drawer extends Component<PropsType, StateType> {
	constructor(props: PropsType) {
		super(props);
		this.state = {
			drawerExpandedWidth: false,
			drawerOpen: false,
			drawerMobile: this.isMobile()
		};
	}

	pullFocus = () => {
		window.document.getElementById("js-drawer-toggleopen").focus();
	};

	// This isn't called in this file - this is the method called from DocumentView
	newComment(comment: Object) {
		// make sure the drawer is open once we've clicked a new comment
		this.setState({
			drawerOpen: true
		});
		this.pullFocus();
		// and what we're calling is newComment from inside <CommentList />
		// $FlowIgnore | this is bound by wrappedComponentRef in CommentListWithRouter
		this.commentList.newComment(comment);
	}

	isMobile = () => {
		if (typeof document !== "undefined") {
			return (
				document.getElementsByTagName("body")[0].offsetWidth <= mobileWidth
			);
		}
		return false;
	};

	drawerClassnames = () => {
		const width = this.state.drawerExpandedWidth ? "Drawer--wide" : "";
		const open = this.state.drawerOpen ? "Drawer--open" : "";
		const mobile = this.state.drawerMobile ? "Drawer--mobile" : "";
		return `Drawer ${width} ${open} ${mobile}`;
	};

	handleClick = (event: string) => {
		switch (event) {
			case "toggleWidth--narrow":
				this.setState({ drawerExpandedWidth: false });
				break;
			case "toggleWidth--wide":
				this.setState({ drawerExpandedWidth: true });
				break;
			case "toggleOpen":
				this.setState(prevState => ({ drawerOpen: !prevState.drawerOpen }));
				this.pullFocus();
				break;
			default:
				return;
		}
	};

	render() {
		return (
			<section aria-labelledby="commenting-panel"
					 className={this.drawerClassnames()}
					 aria-expanded={this.state.drawerOpen}
			>
				<div className="Drawer__controls">
					<button
						id="js-drawer-toggleopen"
						className="Drawer__toggleOpen"
						onClick={() => this.handleClick("toggleOpen")}
						aria-controls="sidebar-panel"
						aria-haspopup="true"
					>
						<span className="visually-hidden">
							{this.state.drawerOpen
								? "Close the commenting panel"
								: "Open the commenting panel"}
						</span>
						<span
							className={`icon ${
								this.state.drawerOpen
									? "icon--chevron-right"
									: "icon--chevron-left"}`}
							aria-hidden="true"
						/>
					</button>
					<button id="js-reading-mode-toggle"
						title="Reading mode"
						className="Drawer__toggleOpen"
						style={{ marginTop: "3px" }}
						aria-haspopup="true">
						<img alt="Enable or disable reading view" src="reading-view/icon.png" style={{position: "relative", top: "3px"}}/>
					</button>
				</div>
				{/* #sidebar-panel necessary here for pulling keyboard focus */}
				<div id="sidebar-panel" className="Drawer__main">
					<h1 id="commenting-panel" className="p mt--c mb--e">Comments panel</h1>
					{/*wrappedComponentRef exposes the underlying, unwrapped component*/}
					<CommentListWithRouter isReviewPage={false}
						drawerOpen={this.state.drawerOpen}
						// $FlowIgnore | this.commentList is bound to this below
						wrappedComponentRef={component => (this.commentList = component)}
					/>
				</div>
			</section>
		);
	}
}

export default Drawer;
