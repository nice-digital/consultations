// @flow

import React from "react";

type LinkType = {|
	label: string,
	url: string
|};

type PropsType = {|
	root: LinkType,
	links: Array<LinkType>
|};

function StackedNav(props: PropsType) {
	const { root, links } = props.links;
	return (
		<nav className="stacked-nav" aria-label="{root.label}">
			{rootLabel(root)}
			<ul className="stacked-nav__list">
				{links.map((item, index) => {
					return (
						<li key={`${item.url}${index}`} className="stacked-nav__list-item">
							<a href={item.url}>{item.label}</a>
						</li>
					);
				})}
			</ul>
		</nav>
	);
}

function rootLabel(root: LinkType) {
	if (root.label) {
		return <h2 className="stacked-nav__root">{rootLink(root)}</h2>;
	}
	return null;
}

function rootLink(root: LinkType) {
	if (root.url) {
		return <a href={root.url}>{root.label}</a>;
	} else {
		return root.label;
	}
}

export default StackedNav;
