// @flow

import React, { Component, Fragment } from "react";
import { mobileWidth } from "../../constants";
import CommentListWithRouter from "../CommentList/CommentList";
import { pullFocusById } from "../../helpers/accessibility-helpers";

type PropsType = {
	commentList: Function,
	match: {
		params: {
			consultationId: number
		}
	}
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

	// This isn't called in this file - this is the method called from DocumentView
	newComment(comment: Object) {
		this.setState({
			drawerOpen: true
		});
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
				pullFocusById("#js-drawer-toggleopen");
				break;
			default:
				return;
		}
	};

	render() {
		return (
			<section aria-label="Commenting panel"
					 className={this.drawerClassnames()}>
				<div className="Drawer__controls">
					<button
						tabIndex={0}
						data-qa-sel="open-commenting-panel"
						id="js-drawer-toggleopen"
						className="Drawer__control Drawer__control--comments"
						onClick={() => this.handleClick("toggleOpen")}
						aria-controls="sidebar-panel"
						aria-haspopup="true"
						aria-label={this.state.drawerOpen ?
							"Close the commenting panel" :
							"Open the commenting panel"}>
						{this.state.drawerMobile ?
							<span
								className={`icon ${
									this.state.drawerOpen
										? "icon--chevron-right"
										: "icon--chevron-left"}`}
								aria-hidden="true"
								data-qa-sel="close-commenting-panel"/>
							:
							this.state.drawerOpen ?
								<Fragment>Close comments</Fragment> :
								<Fragment>Open comments</Fragment>}
					</button>
				</div>
				<div aria-hidden={!this.state.drawerOpen}
					 data-qa-sel="comment-panel"
					 id="sidebar-panel"
					 className="Drawer__main">
					{/*wrappedComponentRef exposes the underlying, unwrapped component*/}
					<CommentListWithRouter
						isReviewPage={false}
						isVisible={this.state.drawerOpen}
						// $FlowIgnore | this.commentList is bound to this below
						wrappedComponentRef={component => (this.commentList = component)}/>
				</div>
			</section>
		);
	}
}

export default Drawer;
