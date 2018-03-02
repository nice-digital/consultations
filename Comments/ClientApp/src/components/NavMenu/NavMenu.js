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
						<NavLink to={"/counter"}>Counter</NavLink>
					</li>
					<li className="stacked-nav__list-item">
						<NavLink to={"/fetchdata"}>Fetch Data</NavLink>
					</li>
					<li className="stacked-nav__list-item">
						<NavLink to={"/basic-form"}>Basic Form</NavLink>
					</li>
					<li className="stacked-nav__list-item">
						<NavLink to={"/document"}>Document</NavLink>
					</li>
				</ul>
			</nav>
		);
	}
}
