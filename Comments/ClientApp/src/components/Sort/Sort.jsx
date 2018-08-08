// @flow

import React, { Fragment } from "react";

import SortLink from "./../SortLink/SortLink";

type PropsType = {
	sortOrder: string,
	path: string
};

export const Sort = (props: PropsType) => (
	<Fragment>
		<span aria-hidden="true">Sort by: </span>
		<SortLink text="Document order" sortOrder="DocumentAsc" currentSortOrder={props.sortOrder} path={props.path} />
		<span aria-label="or"> | </span>		
		<SortLink text="Date" sortOrder="DateDesc" currentSortOrder={props.sortOrder} path={props.path} />
	</Fragment>
);

export default Sort;
