// @flow

import React, {Component} from "react";
import { mobileWidth } from "../../constants";

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

	// todo: this only works on component initialsation at the moment
	isMobile = () => window.innerWidth <= mobileWidth;

	drawerClassnames = () => {
		const width = this.state.drawerExpandedWidth ? "Drawer--wide" : "Drawer--narrow";
		const open = this.state.drawerOpen ? "Drawer--open" : "Drawer--closed";
		const mobile = this.state.drawerMobile ? "Drawer--mobile" : "Drawer--desktop";
		return `Drawer ${width} ${open} ${mobile}`;
	};


	handleClick = (event) => {
		switch (event) {
			case "toggleWidth":
				this.setState(prevState => ({ drawerExpandedWidth: !prevState.drawerExpandedWidth }));
				break;
			case "toggleOpen":
				this.setState(prevState => ({ drawerOpen: !prevState.drawerOpen }));
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
					<div className="Drawer__mainControls">
						{ !this.isMobile() ?
							<button className="Drawer__toggleWidth" name="toggleWidth" onClick={()=>this.handleClick("toggleWidth")}>
								<span className={`icon ${this.state.drawerExpandedWidth ? "icon--minus" : "icon--plus"}`} aria-hidden="true" />
							</button>
							: null }
					</div>
				</div>
			</div>
		);
	}
}

export default Drawer;
