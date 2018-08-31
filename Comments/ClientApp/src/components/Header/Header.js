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
	consultationState: {
		endDate: string,
		consultationIsOpen: boolean,
		consultationHasNotStartedYet: boolean,
	},
}

export const Header = (props: PropsType) => {
	const title = props.title;
	const subtitle1 = props.subtitle1;
	const subtitle2 = props.subtitle2;
	const endDate = props.consultationState.endDate;
	const isOpen = props.consultationState.consultationIsOpen;
	const notStartedYet = props.consultationState.consultationHasNotStartedYet;

	let startOrEnd = "ended";
	if (notStartedYet) {
		startOrEnd = "starts";
	}

	return (
		<Fragment>
			<h1 className="page-header__heading mt--0">{title}</h1>
			<p className="page-header__lead mb--d">
				{isOpen ?
					<Fragment>
						Open until{" "}<Moment format="D MMMM YYYY" date={endDate} />
					</Fragment>
					:
					<Fragment>
						The consultation {startOrEnd} on <Moment format="D MMMM YYYY" date={endDate} /> at <Moment format="HH:mm" date={endDate} />
					</Fragment>
				}
			</p>
			{subtitle1 &&
			<p>{subtitle1}</p>
			}
			{subtitle2 &&
			<p>{subtitle2}</p>
			}
		</ Fragment>
	);
};
