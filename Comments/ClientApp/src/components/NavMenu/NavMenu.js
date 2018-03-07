// @flow

import React, { Component } from "react";
import { NavLink } from "react-router-dom";

export default class NavMenu extends Component {
	render() {
		return (
			<nav className="stacked-nav">
				<h2 className="stacked-nav__root">
					<NavLink to={"/"}>Home</NavLink>
				</h2>
				<ul className="stacked-nav__list">
					<li className="stacked-nav__list-item">
						<NavLink to={"/fetchdata"}>Fetch Data</NavLink>
					</li>
					<li className="stacked-nav__list-item">
						<NavLink to={"/1/1/introduction"}>Document View</NavLink>
					</li>
				</ul>
			</nav>
		);
	}
}
