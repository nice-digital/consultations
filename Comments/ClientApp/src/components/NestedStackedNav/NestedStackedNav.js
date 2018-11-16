import React, { PureComponent, Fragment } from "react";
import { Link } from "react-router-dom";

export class NestedStackedNav extends PureComponent {

	constructor(props) {
		super(props);
	}

	renderNavRow = (item) => {
		if (item.children) {
			return (
				<ListItem>
					<ListLink item={item}/>
					<ListWrapper>
						{item.children.map((this.renderNavRow))}
					</ListWrapper>
				</ListItem>
			);
		} else {
			return (
				<ListItem>
					<ListLink item={item}/>
				</ListItem>
			);
		}
	};

	render() {
		const navigationStructure = this.props.navigationStructure;
		return (
			<nav className="stacked-nav NestedStackedNav">
				<ListWrapper>
					{navigationStructure.map(this.renderNavRow)}
				</ListWrapper>
			</nav>
		);
	}

}

const ListLink = (props) => (
	<Link
		aria-current={props.item.active ? "page" : null}
		to={props.item.to}>{props.item.title}</Link>
);

const ListWrapper = (props) => (
	<ul className="stacked-nav__list">{props.children}</ul>
);

const ListItem = (props) => (
	<li className="stacked-nav__list-item">{props.children}</li>
);
