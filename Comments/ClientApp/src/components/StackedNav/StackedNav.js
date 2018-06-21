// @flow

import React from "react";
import { Link } from "react-router-dom";

type LinkType = {
	label: string,
	url: string,
	current?: boolean,
	isReactRoute: boolean
};

type PropsType = {
	links: ?{
		title: string,
		links: Array<LinkType>
	}
};

export const StackedNav = (props: PropsType) => {
	if (!props.links) return null;
	const { title, links } = props.links;
	return (
		<nav className="stacked-nav" aria-label={title}>
			<h2 className="stacked-nav__root">{title}</h2>
			<ul className="stacked-nav__list">
				{links.map((item, index) => (
					<li key={`${item.label}${index}`} data-qa-sel="nav-list-item" className="stacked-nav__list-item">
						{item.isReactRoute ?
							<Link to={item.url} aria-current={item.current ? "page" : "false"}>
								{item.label}
							</Link>
							:
							<a href={item.url} target="_blank" rel="noopener">{item.label}</a>
						}
					</li>
				))}
			</ul>
		</nav>
	);
};

export default StackedNav;
