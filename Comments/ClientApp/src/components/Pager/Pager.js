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

	return (
		<Fragment>
			{type === "last" &&
				<li className="pagination__truncate">
					<span>...</span>
				</li>
			}

			<li className={`pagination__pager ${active ? "active" : ""}`} key={label}>
				<a onClick={onChangePage} data-pager={label} href={`#${label}`}>{label}</a>
			</li>

			{type === "first" &&
				<li className="pagination__truncate">
					<span>...</span>
				</li>
			}
		</Fragment>
	);
};
