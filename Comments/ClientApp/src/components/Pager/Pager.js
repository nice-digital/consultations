// @flow

import React, { Fragment } from "react";

type PagerProps = {
	active: boolean,
	label: string,
	type: String,
	onChangePage: Function
};

export const Pager = (props: PagerProps) => {
	const {
		active,
		label,
		type,
		onChangePage,
	} = props;


	const listClasses = `pagination__pager ${type !== "normal" ? type : ""} ${active ? "active" : ""}`.trimRight();

	let ariaLabel = type === "normal" ? `Go to page ${label}` : `Go to ${type} page`;

	ariaLabel = active ? `Current page, page ${label}` : ariaLabel;

	return (
		<Fragment>
			{type === "last" &&
				<li className="pagination__truncate">
					<span>&hellip;</span>
				</li>
			}

			<li className={listClasses} key={label}>
				<a onClick={onChangePage} data-pager={label} href={`#${label}`} aria-label={ariaLabel}>{label}</a>
			</li>

			{type === "first" &&
				<li className="pagination__truncate">
					<span>&hellip;</span>
				</li>
			}
		</Fragment>
	);
};
