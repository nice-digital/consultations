import React from "react";

function Breadcrumbs(props) {
	return (
		<nav aria-label="Breadcrumbs">
			<p className="visually-hidden">You are here:</p>
			<ol
				className="breadcrumbs"
				aria-labelledby="breadcrumb-label"
				itemScope
				itemType="http://schema.org/BreadcrumbList"
			>

				{props.segments.map((segment, index) => {
					return (
						<li
							key={`${segment.label}${index}`}
							className="breadcrumbs__crumb"
							itemProp="itemListElement"
							itemScope
							itemType="http://schema.org/ListItem">
							<a href={segment.url} itemProp="name">
								{segment.label}
							</a>
							<meta itemProp="position" content={index} />
						</li>
					);
				})}
			</ol>
		</nav>
	);
}

export default Breadcrumbs;
