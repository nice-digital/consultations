// @flow

import React from "react";

type LinksType = {
	label: string,
	url: string
};

type PropsType = {
	links: Array<LinksType>
};

export const BreadCrumbs = (props: PropsType) => {
	return (
		<nav aria-label="Breadcrumbs">
			<p className="visually-hidden">You are here:</p>
			<ol
				className="breadcrumbs"
				aria-labelledby="breadcrumb-label"
				itemScope
				itemType="http://schema.org/BreadcrumbList"
			>
				{props.links.map((segment, index) => {
					const { label, url } = segment;
					return (
						<li
							key={`${label}${index}`}
							className="breadcrumbs__crumb"
							itemProp="itemListElement"
							itemScope
							itemType="http://schema.org/ListItem"
						>
							<a href={url} itemProp="name">
								{label}
							</a>
							<meta itemProp="position" content={index} />
						</li>
					);
				})}
			</ol>
		</nav>
	);
};
