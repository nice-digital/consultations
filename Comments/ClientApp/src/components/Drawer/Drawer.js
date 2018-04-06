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
		}
	};

	render() {
		return (
			<div className={this.drawerClassnames()}>
				<div className="Drawer__controls">
					<button className="Drawer__toggleOpen" onClick={()=>this.handleClick("toggleOpen")}>&laquo;</button>
				</div>
				<div className="Drawer__main">
					<button name="toggleWidth" onClick={()=>this.handleClick("toggleWidth")}>Toggle Width</button>
					<h1>Drawer</h1>
					<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Aspernatur, at aut commodi dolor dolorum ea
						eius ex fugiat ipsa iure, libero maxime molestiae obcaecati quidem repellat vero voluptates? Ipsam,
						libero.</p>
					<textarea name="monkey" id="monkey" defaultValue="I have an insightful comment I wish to add"/>
					<button className="btn">Submit</button>
				</div>
			</div>
		);
	}
}

export default Drawer;
