// @flow

import React from "react";
import { Link } from "react-router-dom";
import { isExternalLink } from "../../helpers/utils";

type LinkType = {
	label: string,
	url: string,
	current?: boolean
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
					<li key={`${item.label}${index}`} className="stacked-nav__list-item">
						{/* Return a standard <a> if the link starts with 'http' */}
						{isExternalLink(item.url) ?
							<a href={item.url} target="_blank" rel="noopener">{item.label}</a> :
							<Link to={item.url} aria-current={item.current ? "page" : "false"}>
								{item.label}
							</Link>
						}
					</li>
				))}
			</ul>
		</nav>
	);
};

export default StackedNav;
