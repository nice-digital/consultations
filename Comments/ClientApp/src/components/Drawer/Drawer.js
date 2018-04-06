// @flow

import React, {Component} from "react";

type PropsType = {}

type StateType = {}

export class Drawer extends Component<PropsType, StateType> {

	constructor(props) {
		super(props);
		this.state = {
			drawerExpandedWidth: false,
			drawerOpen: true
		};
	}

	drawerClassnames = () => {
		const width = this.state.drawerExpandedWidth ? "Drawer--wide" : "Drawer--narrow";
		const open = this.state.drawerOpen ? "Drawer--open" : "Drawer--closed";
		return `Drawer ${width} ${open}`;
	};

	handleClick = (event) => {
		switch (event) {
			case "toggleWidth":
				this.setState({
					drawerExpandedWidth: !this.state.drawerExpandedWidth
				});
				break;
			case "toggleOpen":
				this.setState({
					drawerOpen: !this.state.drawerOpen
				});
				break;
			default:
				return;
		}
	};

	render() {
		return (
			<div className={this.drawerClassnames()}>
				<div className="Drawer__controls">
					<button className="Drawer__toggleOpen" onClick={()=>this.handleClick("toggleOpen")}>
						<span className={`icon ${this.state.drawerOpen ? "icon--chevron-right" : "icon--chevron-left"}`} aria-hidden="true" />
					</button>
				</div>
				<div className="Drawer__main">
					<button name="toggleWidth" onClick={()=>this.handleClick("toggleWidth")}>
						<span className={`icon ${this.state.drawerExpandedWidth ? "icon--minus" : "icon--plus"}`} aria-hidden="true" />
					</button>
				</div>
			</div>
		);
	}
}

export default Drawer;
