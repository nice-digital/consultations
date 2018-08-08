// @flow

import React, { Fragment } from "react";
import Moment from "react-moment";

type PropsType = {
	title: string,
	reference: string,
	endDate?: any,
	match?: any,
	subtitle1: string,
	subtitle2: string,
}

export const Header = (props: PropsType) => {
	const title = props.title;
	const subtitle1 = props.subtitle1;
	const subtitle2 = props.subtitle2;
	const reference = props.reference;
	const endDate = props.consultationState.endDate;
	const isOpen = props.consultationState.consultationIsOpen;
	const notStartedYet = props.consultationState.consultationHasNotStartedYet;
	const ended = props.consultationState.consultationHasEnded;

	let startOrEnd = "ended";
	let colourTag = "tag tag--consultation";

	if (notStartedYet) {
		startOrEnd = "starts";
		colourTag = "tag tag--beta";
	} else if (ended) {
		colourTag = "tag";
	}

	return (
		<Fragment>
			<h1 className="page-header__heading mt--0">{title}</h1>
			{subtitle1 &&
				<p className="mt--0 mb--0">{subtitle1}</p>
			}
			{subtitle2 &&
				<p className="mt--0 mb--0">{subtitle2}</p>
			}
			<p className="page-header__lead">
				<span>[{reference}] </span>
				{isOpen ? (
					<span>
						<span>Open until&nbsp;</span>
						<span className={colourTag}><Moment format="D MMMM YYYY" date={endDate} /></span>
					</span>
				):(
					<span className={colourTag}>The consultation {startOrEnd} on <Moment format="D MMMM YYYY" date={endDate} /> at <Moment format="HH:mm" date={endDate} /></span>
				)}
			</p>
		</ Fragment>
	);
};
