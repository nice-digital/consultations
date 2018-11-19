import React, { PureComponent } from "react";
import { Link } from "react-router-dom";

export class NestedStackedNav extends PureComponent {

	renderNavRow = (item) => {
		if (item.children) {
			return (
				<ListItem key={item.to}>
					<ListLink item={item}/>
					<ListWrapper>
						{item.children.map((this.renderNavRow))}
					</ListWrapper>
				</ListItem>
			);
		} else {
			return (
				<ListItem key={item.to}>
					<ListLink item={item}/>
				</ListItem>
			);
		}
	};

	render() {
		const navigationStructure = this.props.navigationStructure;
		if (!navigationStructure) {
			return null;
		}
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
	<Link aria-current={props.item.current ? "page" : null} to={props.item.to}>
		<span data-g="11">{props.item.title}</span>
		{props.item.marker &&
			<span className="text-right" data-g="1">({props.item.marker})</span>
		}
	</Link>
);

const ListWrapper = (props) => (
	<ul className="stacked-nav__list">{props.children}</ul>
);

const ListItem = (props) => (
	<li className="grid grid--gutterless stacked-nav__list-item">{props.children}</li>
);
