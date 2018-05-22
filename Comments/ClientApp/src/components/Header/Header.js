// @flow

import React, { Fragment } from "react";
import Moment from "react-moment";

type PropsType = {
	title: string,
	reference: string,
	endDate: any,
	match: any,
	onNewCommentClick: Function
}

export const Header = (props: PropsType) => {
	const title = props.title;
	const reference = props.reference;
	const endDate = props.endDate;

	return (
		<Fragment>
			<p className="mb--0">
				Consultation |{" "}
				<button
					className="buttonAsLink"
					tabIndex={0}
					onClick={e => {
						e.preventDefault();
						props.onNewCommentClick({
							sourceURI: props.match.url,
							commentText: "",
							commentOn: "Consultation",
							quote: title
						});
					}}
				>
					Comment on whole consultation
				</button>&nbsp;&nbsp;
			</p> 

			<h1 className="page-header__heading mt--0">{title}</h1>
			<p className="page-header__lead">
				[{reference}] Open until{" "}
				<Moment format="D MMMM YYYY" date={endDate} />
			</p>
		</ Fragment>
	);
};