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
									: "icon--chevron-left"
								}`}
							aria-hidden="true"
						/>
					</button>
					<button id="js-reading-mode-toggle"
						title="Reading mode" className="Drawer__toggleOpen"
							style={{transform: "translateY(3px)"}}
							aria-haspopup="true">
						<svg style={{ transform: "translate(-3px, 4px)"	}} xmlns="http://www.w3.org/2000/svg" width="30" height="30" viewBox="0 0 24 24"><path d="M15 12h-10v1h10v-1zm-4 2h-6v1h6v-1zm4-6h-10v1h10v-1zm0 2h-10v1h10v-1zm0-6h-10v1h10v-1zm0 2h-10v1h10v-1zm7.44 10.277c.183-2.314-.433-2.54-3.288-5.322.171 1.223.528 3.397.911 5.001.089.382-.416.621-.586.215-.204-.495-.535-2.602-.82-4.72-.154-1.134-1.661-.995-1.657.177.005 1.822.003 3.341 0 6.041-.003 2.303 1.046 2.348 1.819 4.931.132.444.246.927.339 1.399l3.842-1.339c-1.339-2.621-.693-4.689-.56-6.383zm-6.428 1.723h-13.012v-16h14v7.894c.646-.342 1.348-.274 1.877.101l.123-.018v-8.477c0-.828-.672-1.5-1.5-1.5h-15c-.828 0-1.5.671-1.5 1.5v17c0 .829.672 1.5 1.5 1.5h13.974c-.245-.515-.425-1.124-.462-2z"/></svg>
					</button>
				</div>
				{/* #sidebar-panel necessary here for pulling keyboard focus */}
				<div id="sidebar-panel" className="Drawer__main">
					<h1 id="commenting-panel" className="p mt--c mb--e">Comments Panel</h1>
					{/*wrappedComponentRef exposes the underlying, unwrapped component*/}
					<CommentListWithRouter
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
