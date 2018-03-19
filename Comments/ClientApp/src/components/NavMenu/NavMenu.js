// @flow

import React from "react";
import { NavLink } from "react-router-dom";

export default function NavMenu() {
	return (
		<nav className="stacked-nav">
			<h2 className="stacked-nav__root">
				<NavLink to={"/"}>Home</NavLink>
			</h2>
			<ul className="stacked-nav__list">
				<li className="stacked-nav__list-item">
					<NavLink to={"/weather-forecast"}>Weather</NavLink>
				</li>
				<li className="stacked-nav__list-item">
					<NavLink to={"/1/1/introduction"}>Document View</NavLink>
				</li>
			</ul>
		</nav>
	);
}
