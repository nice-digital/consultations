// @flow

import React, { Component } from "react";
import {mobileWidth} from "../../constants";
import CommentListWithRouter from "../CommentList/CommentList";

type PropsType = {}

type StateType = {}

export class Drawer extends Component<PropsType, StateType> {

	constructor(props) {
		super(props);
		this.state = {
			drawerExpandedWidth: false,
			drawerOpen: false,
			drawerMobile: this.isMobile()
		};
	}

	isMobile = () => {
		if (typeof document !== "undefined") {
			return document.getElementsByTagName("body")[0].offsetWidth <= mobileWidth;
		}
		return false;
	};

	drawerClassnames = () => {
		const width = this.state.drawerExpandedWidth ? "Drawer--wide" : "";
		const open = this.state.drawerOpen ? "Drawer--open" : "";
		const mobile = this.state.drawerMobile ? "Drawer--mobile" : "";
		return `Drawer ${width} ${open} ${mobile}`;
	};

	handleClick = (event) => {
		switch (event) {
			case "toggleWidth--narrow":
				this.setState({drawerExpandedWidth: false});
				break;
			case "toggleWidth--wide":
				this.setState({drawerExpandedWidth: true});
				break;
			case "toggleOpen":
				this.setState(prevState => ({drawerOpen: !prevState.drawerOpen}));
				break;
			default:
				return;
		}
	};

	render() {
		return (
			<div className={this.drawerClassnames()}>
				<div className="Drawer__controls">
					<button className="Drawer__toggleOpen" onClick={() => this.handleClick("toggleOpen")}>
						<span className={`icon ${this.state.drawerOpen ? "icon--chevron-right" : "icon--chevron-left"}`} aria-hidden="true"/>
					</button>
				</div>
				<div className="Drawer__main">
					<CommentListWithRouter />
				</div>
			</div>
		);
	}
}

export default Drawer;
