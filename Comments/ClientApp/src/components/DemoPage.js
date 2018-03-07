// @flow

import React, { Component } from "react";
import NavMenu from "./NavMenu/NavMenu";

export default class Layout extends Component {
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
