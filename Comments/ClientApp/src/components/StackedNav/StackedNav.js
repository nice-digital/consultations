// @flow

import React from "react";

type RootType = {
	label: string,
	url: string
};

type PropType = {
	root: RootType,
	links: [
		{
			label: string,
			url: string
		}
	]
};

function StackedNav(props: PropType) {
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

function rootLabel(root: RootType) {
	if (root.label) {
		return <h2 className="stacked-nav__root">{rootLink(root)}</h2>;
	}
	return null;
}

function rootLink(root: RootType) {
	if (root.url) {
		return <a href={root.url}>{root.label}</a>;
	} else {
		return root.label;
	}
}

export default StackedNav;
