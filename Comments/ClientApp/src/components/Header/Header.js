// @flow

import React, { Fragment } from "react";
import Moment from "react-moment";

type PropsType = {
	title: string,
	reference: string,
	endDate: any,
	match: any
}

export const Header = (props: PropsType) => {
	const title = props.title;
	const reference = props.reference;
	const endDate = props.consultationState.endDate;
	const isOpen = props.consultationState.consultationIsOpen;
	const notStartedYet = props.consultationState.consultationHasNotStartedYet;

	const textBeforeDate = isOpen ? "Open until" : (notStartedYet ? "Starts on" : "Ended on");
	const colourTag = isOpen ? "tag tag--consultation" : (notStartedYet ? "tag tag--beta" : "tag tag--alpha"); //todo: these colours shouldn't represent phases.

	return (
		<Fragment>
			<h1 className="page-header__heading mt--0">{title}</h1>
			<p className="page-header__lead">
				<span>[{reference}] </span>
				<span>{textBeforeDate}&nbsp;</span>
				<span className={colourTag}><Moment format="D MMMM YYYY" date={endDate} /></span>
			</p>
		</ Fragment>
	);
};

// todo: this is only in one place! do we need it?
