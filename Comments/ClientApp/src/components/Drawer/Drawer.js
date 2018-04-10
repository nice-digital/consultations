// @flow

import React, {Component, Fragment} from "react";
import { mobileWidth } from "../../constants";

import iconDockExtended from "./icon-dock-extended.svg";
import iconDockSmall from "./icon-dock-small.svg";

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
	isMobile = () => {
		if  (window) {
			return window.innerWidth <= mobileWidth;
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
				this.setState({ drawerExpandedWidth: false });
				break;
			case "toggleWidth--wide":
				this.setState({ drawerExpandedWidth: true });
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
						{ !this.state.drawerMobile ?
							<Fragment>
								{/*<button className="Drawer__toggleWidth Drawer__toggleWidth--narrow" name="toggleWidth" onClick={()=>this.handleClick("toggleWidth--narrow")}>
									<span className="icon icon--dock-right-small" aria-hidden="true" />
									<img src={iconDockSmall} alt="Icon Dock Small" />
								</button>
								<button className="Drawer__toggleWidth Drawer__toggleWidth--expanded" name="toggleWidth" onClick={()=>this.handleClick("toggleWidth--expanded")}>
									<span className="icon icon--dock-right-large" aria-hidden="true" />
									<img src={iconDockExtended} alt="Icon Dock Extended"/>
								</button>*/}
								<img style={this.state.drawerExpandedWidth ? {"opacity": "1"} : {"opacity": "0.3"} }
									src={iconDockSmall} alt="Icon Dock Small" width="40" onClick={()=>this.handleClick("toggleWidth--narrow")}/>
								<img style={this.state.drawerExpandedWidth ? {"opacity": "0.3"} : {"opacity": "1"} }
									src={iconDockExtended} alt="Icon Dock Extended" width="40" onClick={()=>this.handleClick("toggleWidth--wide")}/>
							</Fragment>
							: null }
					</div>
				</div>
			</div>
		);
	}
}

export default Drawer;
