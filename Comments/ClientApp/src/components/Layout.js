import React, { Component } from "react";
import { NavMenu } from "./NavMenu";

export class Layout extends Component {
	displayName = Layout.name;

	render() {
		return (
			<div className="container">
				<div className="grid">
					<div data-g="12 sm:3">
						<NavMenu />
					</div>
					<div data-g="12 sm:9">{this.props.children}</div>
				</div>
			</div>
		);
	}
}
