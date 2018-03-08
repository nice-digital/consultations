// @flow

import React from "react";
import { Link } from "react-router-dom";

type LinkType = {
	label: string,
	url: string,
	current?: boolean
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
			<h2 className="stacked-nav__root">
				<Link to={root.url} aria-current={root.current ? true : false}>
					{root.label}
				</Link>
			</h2>
			<ul className="stacked-nav__list">
				{links.map(item => (
					<li key={item.label} className="stacked-nav__list-item">
						<Link to={item.url} aria-current={item.current ? true : false}>
							{item.label}
						</Link>
					</li>
				))}
			</ul>
		</nav>
	);
};

export default StackedNav;
