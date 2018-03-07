// @flow

import React from "react";
import { Link } from "react-router-dom";

type LinkType = {
	label: string,
	url: string
};

type PropsType = {
	links: {
		root: LinkType,
		links: Array<LinkType>
	}
};

export const StackedNav = (props: PropsType) => {
	const { root, links } = props.links;
	return (
		<nav className="stacked-nav" aria-label="{root.label}">
			<RootLink {...root} />
			<ul className="stacked-nav__list">
				{links.map(item => <ListLink key={item.label} {...item} />)}
			</ul>
		</nav>
	);
};

export const RootLink = (props: LinkType) => {
	const { label, url } = props;
	return (
		<h2 className="stacked-nav__root">
			<Link to={url}>{label}</Link>
		</h2>
	);
};

export const ListLink = (props: LinkType) => {
	const { label, url } = props;
	return (
		<li key={label} className="stacked-nav__list-item">
			<Link to={url}>{label}</Link>
		</li>
	);
};

export default StackedNav;
