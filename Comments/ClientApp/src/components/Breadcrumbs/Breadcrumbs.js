// @flow

import React from "react";
import { Link } from "react-router-dom";
import { isHttpLink } from "../../helpers/utils";

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
							{(isHttpLink(url) || url.indexOf("/") === 0) ?
								<a href={url}>{label}</a>
								:
								<Link to={url} itemProp="name">
									{label}
								</Link>
							}
							<meta itemProp="position" content={index} />
						</li>
					);
				})}
			</ol>
		</nav>
	);
};
